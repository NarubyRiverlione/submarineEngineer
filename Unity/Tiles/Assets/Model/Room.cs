using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FullSerializer;
using System;

namespace Submarine.Model {
	public enum RoomType {
		Empty,
		Remove,
		EngineRoom,
		Generator,
		Battery,
		Bridge,
		Gallery,
		Stairs,
		Cabin,
		Bunks,
		Conn,
		Sonar,
		RadioRoom,
		FuelTank,
		PumpRoom,
		BalastTank,
		StorageRoom,
		EscapeHatch,
		TorpedoRoom,
		Propellor}

	;



	abstract public class Room {
		// don't save point to Sub but set it in Load
		[fsIgnore]
		// needs reference to sub for layout validation
		public Sub inSub { get; set; }

		public int RoomID { get; set; }

		public RoomType TypeOfRoom { get; protected set; }

		public List<Point> coordinatesOfTilesInRoom;

		public int Size{ get { return coordinatesOfTilesInRoom.Count (); } }

		public int MinimimValidSize { get; protected set; }

		public string ValidationText { get; protected set; }

		abstract public bool IsLayoutValid { get; }


		public double CapacityPerTile { get; protected set; }

		int OutputCapacity {
			get{ return (int)(Size * CapacityPerTile); }
		}

		public abstract Units UnitOfOutput { get; }

		// List with all needed resources for a room before room is operational
		public List<Resource> NeededResources { get; protected set; }

		// previous state of resources needs the be remembered so a change in resource supply can be detected = tile needs redrawing
		bool prevResourcesAvailable;
		// check is ALL resources are available, if change in availability is detected warn tile to be redraw via Action Callback
		public bool ResourcesAvailable {
			get {
				bool newResAv = AreAllResourcesAvailable (); 
				if (prevResourcesAvailable != newResAv) {
					prevResourcesAvailable = newResAv;
					if (IsLayoutValid)
						WarnTilesOfChange ();
				}	
				return newResAv;
			} 
		}

		private int _output = 0;
		// only output if layout is valid and all resources are available
		public int Output { 
			get {
				int newOutput = 0;
				if (IsLayoutValid && ResourcesAvailable)
					newOutput = OutputCapacity;
				
				// output has changed -> warn all pieces of each carrier that ... in this room 
				if (newOutput != _output) {
					_output = newOutput;
					List<int> rebuildedCarriers = new List<int> ();

					foreach (Point coord in coordinatesOfTilesInRoom) {
						Tile checkTile = inSub.GetTileAt (coord.x, coord.y);
						foreach (Piece checkPiece in checkTile.PiecesOnTile) {
							int checkCarrier = checkPiece.carrierID;
							if (checkCarrier != 0 && rebuildedCarriers.Contains (checkCarrier) == false) {
								// warn pieces only if there aren't warn't already (carrierID is in rebuilded carriers)
								// and the output of the room is the unit that this carrier needs (else called also if needed room hasn't needed resources)
								if (inSub.ResourceCarriers [checkCarrier].UnitOfContent == UnitOfOutput) {
									//UnityEngine.Debug.Log ("Carrier " + checkCarrier + " should be redraw because output of room " + RoomID + " has changed to " + _output);
									rebuildedCarriers.Add (checkCarrier);
									inSub.ResourceCarriers [checkCarrier].WarnAllPiecesOfCarrier ();
								}
							}
						}
					}
				}
					

				return _output;
			}
		}

		// Some rooms cannot be access be crew (tanks)
		public bool IsAccessable { get; protected set; }

		// lowest coord of tile (will be walkable)
		public List<Point> LowestCoordInRoom { get { return GetLowestCoordOfRoom (); } }

		#region CONSTRUCTOR

		protected Room (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) {    
			TypeOfRoom = ofThisRoomType;
			coordinatesOfTilesInRoom = new List<Point> ();
			IsAccessable = true;	// default: room is accessible, if not change this in construction of concrete class 
			MinimimValidSize = minSize;
			CapacityPerTile = capPerTile;
			inSub = sub;			// sub: to get info of the sub (dimensions) for extra layout validation of some room types
			NeededResources = reqRes;

			prevResourcesAvailable = false;

			SetRoomValidationText ();
		}

		protected void SetRoomValidationText () {
			// don't stop sentence with a '.', maybe a concrete class will add additional requirements

			ValidationText = "The " + TypeOfRoom + " needs to be at least " + MinimimValidSize + " tiles";

			if (NeededResources != null) {
				foreach (Resource resouce in NeededResources) {
					ValidationText += " and minimum " + resouce.amount * MinimimValidSize + " " + resouce.unit;
				}
				ValidationText += " to be operational";
			}

			if (UnitOfOutput != Units.None)
				ValidationText += ", output will be  " + CapacityPerTile + " " + UnitOfOutput + " per tile";
		}

		#endregion


		public static Room CreateRoomOfType (RoomType ofThisRoomType, Sub inThisSub) {
			// let factory create the correct concrete class
			return RoomFactory.CreateRoomOfType (ofThisRoomType, inThisSub);
		}

		public void AddTile (Tile addTile) {
			// remmeber layout validation before adding this tile
			bool oldRoomLayoutValid = IsLayoutValid;   

			Point coord = new Point (addTile.X, addTile.Y);
			coordinatesOfTilesInRoom.Add (coord);

			// update lowest tiles as walkable
			SetLowestTilesInRoomWalkable ();

			// update tiles is layout validation has changed
			if (oldRoomLayoutValid != IsLayoutValid && addTile.RoomID != 0) { 
				// first tile of a new room hasn't a roomID yet, don't call Warn, it will be called when the roomID is set
				Debug.WriteLine ("Validation of room layout has changed, warn title of room");
				WarnTilesOfChange ();
			}
		}

		public void RemoveTile (Tile removeTile) {
			Point coord = new Point (removeTile.X, removeTile.Y);
			coordinatesOfTilesInRoom.Remove (coord);

			// update lowest tiles as walkable
			SetLowestTilesInRoomWalkable ();

			// no need to check if valid layout of room has changed by removing this tile
			// as removing will trigger a complete rebuild of the room
		}

		//		public void WarnTilesIfRoomLayoutValidationIsChanged (bool oldRoomLayoutValid) {
		//			if (oldRoomLayoutValid != IsLayoutValid) {
		//				//Debug.WriteLine ("Validation of room layout has changed, warn title of room");
		//				WarnTilesOfChange ();
		//
		//			}
		//		}

		private void WarnTilesOfChange () {
			foreach (Point coord in coordinatesOfTilesInRoom) {
				Tile warnTile = inSub.GetTileAt (coord.x, coord.y);
				#if DEBUG
				if (warnTile.RoomID != RoomID)
					UnityEngine.Debug.LogError ("I don't belong here !! Should be in room " + warnTile.RoomID + " but are in  " + RoomID);
				#endif

				if (warnTile.TileChangedActions != null)
					warnTile.TileChangedActions (warnTile);

			#if DEBUG
				else
					UnityEngine.Debug.Log ("No action for " + coord.x + "," + coord.y + " room id " + warnTile.RoomID);
				#endif
			}
		}

		private bool AreAllResourcesAvailable () {
			bool AllAvailable = true; // asume all resources are available so 1 not available resource will detected in the for each search
			if (NeededResources != null) {
				foreach (Resource needResource in NeededResources) {
					int resouceAvailable = 0;
					// get available resouces
					if (inSub != null && needResource.unit != Units.None) {
						if (Resource.isCrewType (needResource.unit)) {
							// crew as needed resource, check CrewList
							resouceAvailable = inSub.AmountOfCrewType (needResource.unit);
						}
						else // no crew = normal resource
							if (needResource.unit == Units.food || needResource.unit == Units.tins)
								// no carrier for food: crew needs to go to the gallery
								// no carrier for tins: cook get (invisible?) tins for the storage
								resouceAvailable = inSub.GetAllOutputOfUnit (needResource.unit);
						else
							resouceAvailable = ConnectedResource (needResource.unit);
					}
					// now available is know check it agains needs
					if (resouceAvailable < inSub.GetAllNeededResourcesOfUnit (needResource.unit))		// don't check needs of just this room but similar needs of all rooms (aka 2 bunks need together food)
						AllAvailable = false;
				}
			}
			return AllAvailable;
		}

		public int GetResouceNeeded (Units reqUnit) {
			Resource foundResource = NeededResources.Where (res => res.unit == reqUnit).FirstOrDefault ();
			// resource needed depence on size of room
			return foundResource != null ? (int)Math.Floor (foundResource.amount * Size) : 0;
		}

		private int ConnectedResource (Units type) {
			int resource = 0;
			foreach (Point coord in coordinatesOfTilesInRoom) {
				Tile tile = inSub.GetTileAt (coord.x, coord.y);
				foreach (Piece piece in tile.PiecesOnTile) {
					
					if (piece.Type != PieceType.None && piece.IsConnection) {
						Carrier carrierInRoom = inSub.ResourceCarriers [piece.carrierID];
						if (carrierInRoom.UnitOfContent == type)
							resource += carrierInRoom.Content;
					}
				}
			}
			return resource;
		}

		private List<Point> GetLowestCoordOfRoom () {
			List<Point> lowestPoints = new List<Point> ();

			// get for each X in the room the lowest Y
			List<int> allXinRoom = new List<int> ();

			foreach (Point coord in coordinatesOfTilesInRoom) {
				if (allXinRoom.Contains (coord.x) == false) {
					allXinRoom.Add (coord.x); // add this X as checked
					int lowestY = 99;
					foreach (Point findLowestCoord in coordinatesOfTilesInRoom) {
						if (findLowestCoord.x == coord.x)
							lowestY = findLowestCoord.y < lowestY ? findLowestCoord.y : lowestY;
					}
					lowestPoints.Add (new Point (coord.x, lowestY));
				}
			}
			return lowestPoints;
		}

		public void SetLowestTilesInRoomWalkable () {
			// reset walkable of all tiles
			foreach (Point coord in coordinatesOfTilesInRoom) {
				Tile tile = inSub.GetTileAt (coord.x, coord.y);
				tile.Walkable = false;
			}
			if (IsAccessable == false)
				return; // no access == all tiles of room are not walkable
				
			
			if (TypeOfRoom != RoomType.Stairs) {
				// for normal rooms:  set lowest tiles walkable (if there are no pieces on tile)
				foreach (Point coord in LowestCoordInRoom) {
					Tile tile = inSub.GetTileAt (coord.x, coord.y);
					tile.Walkable = true;
					foreach (Piece piece in tile.PiecesOnTile) {
						if (piece.Type != PieceType.None)
						// found a non-empty tile => no walkable
						tile.Walkable = false;
					}
				}
			}
			else {
				// stairs: all (horizontal and vertical) tiles all walkable
				foreach (Point coord in coordinatesOfTilesInRoom) {
					Tile tile = inSub.GetTileAt (coord.x, coord.y);
					tile.Walkable = true;
				}
			}
		}

	}
}