using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FullSerializer;
using System;

namespace Submarine.Model {
	public enum RoomType {
		Empty,
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
		TorpedoRoom}

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

		public int OutputCapacity {
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

		// only output is layout is valid and all resources are available
		public int Output { 
			get { 
				if (IsLayoutValid && ResourcesAvailable)
					return OutputCapacity;
				else
					return 0;
			}
		}

		// Some rooms cannot be access be crew (tanks)
		public bool IsAccessable { get; protected set; }



		#region CONSTRUCTOR

		public Room (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) {    
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
			//TilesInRoom.Add (addTile);
			Point coord = new Point (addTile.X, addTile.Y);
			coordinatesOfTilesInRoom.Add (coord);
		}

		public void RemoveTile (Tile removeTile) {
			//TilesInRoom.Remove (removeTile);
			Point coord = new Point (removeTile.X, removeTile.Y);
			coordinatesOfTilesInRoom.Remove (coord);
			// reset Tile (roomId, waalType,...)
			// removeTile.Reset ();
		}

		public void WarnTilesIfRoomLayoutValidationIsChanged (bool oldRoomLayoutValid) {
			if (oldRoomLayoutValid != IsLayoutValid) {
				//Debug.WriteLine ("Validation of room layout has changed, warn title of room");
				WarnTilesOfChange ();
			}				
		}

		private void WarnTilesOfChange () {
			foreach (Point coord in coordinatesOfTilesInRoom) {
				Tile warnTile = inSub.GetTileAt (coord.x, coord.y);
				#if DEBUG
				//TODO: check can be remove
				if (warnTile.RoomID != RoomID)
					UnityEngine.Debug.LogError ("I don't belong here !!");
				#endif
				if (warnTile.TileChangedActions != null)
					warnTile.TileChangedActions (warnTile);
				#if DEBUG
				else //TODO: check can be remove
					UnityEngine.Debug.Log ("No action for " + coord.x + "," + coord.y + " room id " + warnTile.RoomID);
				#endif
			}
		}

		private bool AreAllResourcesAvailable () {
			bool AllAvailable = true; // amuse all resources are available so 1 not available resource will detected in the for each search
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
							resouceAvailable = inSub.GetAllOutputOfUnit (needResource.unit);
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
	}
}