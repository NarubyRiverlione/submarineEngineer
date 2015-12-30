using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FullSerializer;

namespace Submarine.Model {
	public enum RoomType {
		Empty = 0,
		EngineRoom = 1,
		Generator = 2,
		Battery = 3,
		Bridge = 4,
		Gallery = 5,
		
		Cabin = 7,
		Bunks = 8,
		Conn = 9,
		Sonar = 10,
		RadioRoom = 11,
		FuelTank = 12,
		PumpRoom = 13,
		StorageRoom = 14,
		EscapeHatch = 15,
		TorpedoRoom = 16}

	;



	abstract public class Room {
       
		public Sub inSub { get; protected set; }
		// needs reference to sub for layout validation
		public RoomType TypeOfRoom { get; protected set; }

		public List<Point> coordinatesOfTilesInRoom;

		public int Size{ get { return coordinatesOfTilesInRoom.Count (); } }

		public int MinimimValidSize { get; protected set; }

		[fsIgnore]
		public string ValidationText { get; protected set; }

		abstract public bool IsLayoutValid { get; }


		public double CapacityPerTile { get; protected set; }

		public int RoomCapacity {
			get{ return (int)(Size * CapacityPerTile); }
		}
		// current produced or available cargo
		public abstract Units UnitOfCapacity { get; }

		public List<Resource> NeededResources { get; protected set; }

		bool prevResourcesAvailable;
		// previous state of resources needs te be remembered so a change in resource supply can be detected = tile needs redrawing
		public bool ResourcesAvailable {
			get {
				bool newResAv = AllResourcesAreAvailable (); //CurrentResource >= ReqResource;
				if (prevResourcesAvailable != newResAv) {
					prevResourcesAvailable = newResAv;
					WarnTilesOfChange ();
				}	
				return newResAv;
			} 
		}

		public int Output { 
			get { // only output is layout is valid
				if (IsLayoutValid && ResourcesAvailable)
					return RoomCapacity;
				else
					return 0;
			}
		}

		
		public bool IsAccessable { get; protected set; }



		#region CONSTRUCTOR

		public Room (RoomType ofThisRoomType, Sub sub, int minSize, int capPerTile, List<Resource> reqRes) {    // sub: to get info of the sub (dimensions) for extra layout validation of some room types
			TypeOfRoom = ofThisRoomType;
			coordinatesOfTilesInRoom = new List<Point> ();
			IsAccessable = true;
			MinimimValidSize = minSize;
			CapacityPerTile = capPerTile;
			inSub = sub;
			NeededResources = reqRes;

			prevResourcesAvailable = false;

			// don't stop scentese with a '.', maybee a concrete class will add aditional requirements
			ValidationText = "The " + ofThisRoomType + " needs to be at least " + MinimimValidSize + " tiles";
			if (NeededResources != null) {
				foreach (Resource resouce in NeededResources) {
					ValidationText += " and " + resouce.amount + " " + resouce.unit;
				}
				ValidationText += " to be operational";
			}
				
			if (UnitOfCapacity != Units.None)
				ValidationText += ", output will be  " + capPerTile + " " + UnitOfCapacity + " per tile";
		}

		#endregion

		public static Room CreateRoomOfType (RoomType ofThisRoomType, Sub inThisSub) {
			// let factory create the correct concrete class
			return RoomFactory.CreateRoomOfType (ofThisRoomType);
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

		public void WarnTilesInRoomThatLayoutChanged (bool oldRoomLayoutValid) {
			if (oldRoomLayoutValid != IsLayoutValid) {
				//Debug.WriteLine ("Validation of room layout has changed, warn title of room");
				WarnTilesOfChange ();
			}				
		}

		private void WarnTilesOfChange () {
			foreach (Point coord in coordinatesOfTilesInRoom) {
				Tile warnTile = inSub.GetTileAt (coord.x, coord.y);
				if (warnTile.TileChangedActions != null)
					warnTile.TileChangedActions (warnTile);
			}
		}

		private bool AllResourcesAreAvailable () {
			bool AllAvailable = true; // asume all resouces are available so 1 not available resoucre will detected in the for each search
			if (NeededResources != null) {
				foreach (Resource resource in NeededResources) {
					if (inSub != null && resource.unit != Units.None) {
						int resouceAvailable = inSub.GetAllOutputOfUnit (resource.unit);
						if (resouceAvailable < resource.amount)
							AllAvailable = false;
					}
				}
			}
			return AllAvailable;
		}

		public int GetResouceNeeded (Units reqUnit) {
			Resource foundResource = NeededResources.Where (res => res.unit == reqUnit).FirstOrDefault ();
			return foundResource != null ? foundResource.amount : 0;
		}
	}
}