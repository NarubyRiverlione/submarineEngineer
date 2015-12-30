
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

//using System.Threading.Tasks;
using FullSerializer;


namespace Submarine.Model {
	
	public class Sub {

		public int lengthOfSub { get; private set; }

		public int heightOfSub { get; private set; }


		public int startOfBridgeTower { get; private set; }

		public int lenghtOfBridgeTower { get; private set; }

		public int heightOfBridgeTower { get; private set; }


		public int smallerTailUpper { get; private set; }

		public int smallerTailLower { get; private set; }

		public int smallerTailLenght { get; private set; }


		// tiles inside the sub
		Tile[,] _tile;

		// rooms inside the sub
		[UnityEngine.SerializeField]
		Dictionary<int, Room> rooms;

		// count of rooms
		[fsIgnore]
		public int AmountOfRooms {
			get {
				if (rooms != null)
					return rooms.Count;
				else
					return 0;
			}
		}

		// next roomID
		[UnityEngine.SerializeField]
		// ID 0 = no room assigned to Tile
		int _nextRoomID = 1;

		public Dictionary<string,int> RoomPropertiesInt { get; private set; }

		public Dictionary<RoomType, List<Resource>> RoomPropertiesReqRes { get; private set; }


		#region SAVE / LOAD

		private static readonly fsSerializer _serializer = new fsSerializer ();

		private static string Serialize (Type type, object value) {
			// serialize the data
			fsData data;
			_serializer.TrySerialize (type, value, out data).AssertSuccessWithoutWarnings ();

			// emit the data via JSON
			return fsJsonPrinter.CompressedJson (data);
		}

		private static object Deserialize (Type type, string serializedState) {
			// step 1: parse the JSON data
			fsData data = fsJsonParser.Parse (serializedState);

			// step 2: de serialize the data
			object deserialized = null;
			_serializer.TryDeserialize (data, type, ref deserialized).AssertSuccessWithoutWarnings ();

			return deserialized;
		}

		public void Save (string fileName) {
			// save submarine
			string jsonStringOfSub = Serialize (typeof(Sub), this);

			// save tiles
			List<Tile> allTiles = new List<Tile> ();         // convert 2D array to List
			for (int x = 0; x < lengthOfSub; x++) {
				for (int y = 0; y < heightOfSub; y++) {
					allTiles.Add (GetTileAt (x, y));
				}
			}
			string jsonStringOfAllTitles = Serialize (typeof(List<Tile>), allTiles);

			// save room resources



			File.WriteAllText (fileName, jsonStringOfSub);
			File.WriteAllText (fileName.Substring (0, fileName.Length - 5) + "_Tiles", jsonStringOfAllTitles); // add _Tiles
		}

		// loading function is split into 2 component because first the submarine dimension needs to be read
		// with thes dimensions know all game objects for the tiles can be created (add UpdateTileSprite as changed action)
		// when all game objects are created, read the tiles data and show the correct visual via de TileChangedActions
		public void Load (string fileName) {
			// only load submarine properties, not the tiles
			string jsonStringOfSub = File.ReadAllText (fileName);
			Sub loadedSub = (Sub)Deserialize (typeof(Sub), jsonStringOfSub);

			heightOfSub = loadedSub.heightOfSub;
			lengthOfSub = loadedSub.lengthOfSub;

			heightOfBridgeTower = loadedSub.heightOfBridgeTower;
			lenghtOfBridgeTower = loadedSub.lenghtOfBridgeTower;
			startOfBridgeTower = loadedSub.startOfBridgeTower;

			smallerTailLenght = loadedSub.smallerTailLenght;
			smallerTailLower = loadedSub.smallerTailLower;
			smallerTailUpper = loadedSub.smallerTailUpper;

			_nextRoomID = loadedSub._nextRoomID;

			// set to loaded dictionaries
			RoomPropertiesInt = loadedSub.RoomPropertiesInt;
			RoomPropertiesReqRes = loadedSub.RoomPropertiesReqRes;

			// load all tiles (2D array not supported,,needs conversion)
			_tile = new Tile[lengthOfSub, heightOfSub]; // reset all old tiles

			string jsonStringOfAllTitles = File.ReadAllText (fileName.Substring (0, fileName.Length - 5) + "_Tiles");
			List<Tile> allTiles = (List < Tile >)Deserialize (typeof(List<Tile>), jsonStringOfAllTitles);

			foreach (Tile addTile in allTiles) {
				_tile [addTile.X, addTile.Y] = addTile;
			}

			// tiles are load, now load rooms (with tiles is it)
			rooms = new Dictionary<int, Room> (); // reset rooms
			rooms = loadedSub.rooms; 
		}

		#endregion


		#region CONSTRUCTOR

		public Sub () {
			lengthOfSub = 39;
			heightOfSub = 6;

			lenghtOfBridgeTower = 3;
			heightOfBridgeTower = 2;
			startOfBridgeTower = 22;

			smallerTailUpper = 1;
			smallerTailLower = 1;
			smallerTailLenght = 4;

			// initialize 2D array, still doesn't contain anything
			_tile = new Tile[lengthOfSub, heightOfSub];
			// instantiate rooms
			rooms = new Dictionary<int, Room> ();

			// set room properties 
			SetRoomProperties ();
			// set room resources needs
			SetRoomResoucresNeeds ();

			// set tiles around tail and tower as unavailable for building, create Tower
			CreateSub ();

		}
		// set tiles around tail and tower as unavailable for building, create Tower
		private void CreateSub () {
			// instantiate Tiles
			for (int x = 0; x < lengthOfSub; x++) {
				for (int y = 0; y < heightOfSub; y++) {
					_tile [x, y] = new Tile (x, y);
				}
			}
			// set Tile's outside sub outlines as unavailable
			// upper smaller tail section
			for (int x = 0; x <= smallerTailLenght; x++) {
				for (int y = heightOfSub - heightOfBridgeTower - smallerTailUpper; y < heightOfSub - heightOfBridgeTower; y++) {
					_tile [x, y].canContainRoom = false;
				}
			}
			// lower  smaller tail section 
			for (int x = 0; x <= smallerTailLenght; x++) {
				for (int y = 0; y < smallerTailLower; y++) {
					_tile [x, y].canContainRoom = false;
				}
			}
			// left of Bridge tower
			for (int x = 0; x < startOfBridgeTower; x++) {
				for (int y = heightOfSub - heightOfBridgeTower; y < heightOfSub; y++) {
					_tile [x, y].canContainRoom = false;
				}
			}
			//right of Bridge tower
			for (int x = startOfBridgeTower + lenghtOfBridgeTower; x < lengthOfSub; x++) {
				for (int y = heightOfSub - heightOfBridgeTower; y < heightOfSub; y++) {
					_tile [x, y].canContainRoom = false;
				}
			}
			// add Bridge Tower
			for (int x = startOfBridgeTower; x < startOfBridgeTower + lenghtOfBridgeTower; x++) {
				for (int y = heightOfSub - heightOfBridgeTower; y < heightOfSub; y++) {
					AddTileToRoom (x, y, RoomType.Bridge);
				}
			}
		}
		// set min tile size, capacity per tile and unit of capacity for each Room Type
		private void SetRoomProperties () {
			RoomFactory.inThisSub = this;
			RoomPropertiesInt = new Dictionary<string, int> ();

			RoomPropertiesInt ["EngineRoom_Min"] = 12;
			RoomPropertiesInt ["EngineRoom_CapPerTile"] = 1000;

			RoomPropertiesInt ["Generator_Min"] = 12;
			RoomPropertiesInt ["Generator_CapPerTile"] = 500;
					
			RoomPropertiesInt ["Battery_Min"] = 10;
			RoomPropertiesInt ["Battery_CapPerTile"] = 50;
					
			RoomPropertiesInt ["Bridge_Min"] = 4;
			RoomPropertiesInt ["Bridge_CapPerTile"] = 0;

			RoomPropertiesInt ["Gallery_Min"] = 6;
			RoomPropertiesInt ["Gallery_CapPerTile"] = 5;
					
			RoomPropertiesInt ["Cabin_Min"] = 2;
			RoomPropertiesInt ["Cabin_CapPerTile"] = 1;
					
			RoomPropertiesInt ["Bunks_Min"] = 6;
			RoomPropertiesInt ["Bunks_CapPerTile"] = 2;

			RoomPropertiesInt ["Conn_Min"] = 8;
			RoomPropertiesInt ["Conn_CapPerTile"] = 1;
					
			RoomPropertiesInt ["Sonar_Min"] = 4;
			RoomPropertiesInt ["Sonar_CapPerTile"] = 1;
					
			RoomPropertiesInt ["RadioRoom_Min"] = 4;
			RoomPropertiesInt ["RadioRoom_CapPerTile"] = 1;
					
			RoomPropertiesInt ["FuelTank_Min"] = 9;
			RoomPropertiesInt ["FuelTank_CapPerTile"] = 1000;
					
			RoomPropertiesInt ["PumpRoom_Min"] = 6;
			RoomPropertiesInt ["PumpRoom_CapPerTile"] = 1000;
					
			RoomPropertiesInt ["StorageRoom_Min"] = 4;
			RoomPropertiesInt ["StorageRoom_CapPerTile"] = 100;
				
			RoomPropertiesInt ["EscapeHatch_Min"] = 2;
			RoomPropertiesInt ["EscapeHatch_CapPerTile"] = 0;
					
			RoomPropertiesInt ["TorpedoRoom_Min"] = 20;
			RoomPropertiesInt ["TorpedoRoom_CapPerTile"] = 1;
		}

		private void SetRoomResoucresNeeds () {
			RoomPropertiesReqRes = new Dictionary<RoomType,List<Resource>> ();

			RoomPropertiesReqRes [RoomType.EngineRoom] = new List<Resource> {
				{ new Resource (Units.liters_fuel, 1000) },
				{ new Resource (Units.Engineers, 1) }
			};

			RoomPropertiesReqRes [RoomType.Generator] = new List<Resource> {
				{ new Resource (Units.pks, 2000) },
				{ new Resource (Units.Engineers, 1) }
			};
			RoomPropertiesReqRes [RoomType.Battery] = new List<Resource> { { new Resource (Units.MWs, 1500) } };
			RoomPropertiesReqRes [RoomType.Bridge] = new List<Resource> { { new Resource (Units.Watchstander, 2) } };
			RoomPropertiesReqRes [RoomType.Gallery] = new List<Resource> {
				{ new Resource (Units.tins, 30) },
				{ new Resource (Units.Cook, 1) }
			};
			RoomPropertiesReqRes [RoomType.Cabin] = new List<Resource> { { new Resource (Units.food, 1) } };
			RoomPropertiesReqRes [RoomType.Bunks] = new List<Resource> { { new Resource (Units.food, 6) } };
			RoomPropertiesReqRes [RoomType.Conn] = new List<Resource> {
				{ new Resource (Units.Officers, 2) },
				{ new Resource (Units.Watchstander, 4) }
			};
			RoomPropertiesReqRes [RoomType.Sonar] = new List<Resource> { { new Resource (Units.Sonarman, 1) } };
			RoomPropertiesReqRes [RoomType.RadioRoom] = new List<Resource> { { new Resource (Units.Radioman, 1) } };
			RoomPropertiesReqRes [RoomType.FuelTank] = new List<Resource> ();
			RoomPropertiesReqRes [RoomType.PumpRoom] = new List<Resource> { { new Resource (Units.liters_pump, 100) } };
			RoomPropertiesReqRes [RoomType.StorageRoom] = new List<Resource> ();
			RoomPropertiesReqRes [RoomType.TorpedoRoom] = new List<Resource> { { new Resource (Units.Torpedoman, 4) } };


		}

		#endregion

		//TODO refactor this, too much same code

		#region Design Validation
		private bool ValidationCriteria(RoomType checkRoomType, int minOutput = 0) {
			int roomCount = 0, validCount = 0;
			foreach (var roomPair in rooms) {
				Room room = roomPair.Value;
				if (room.TypeOfRoom == checkRoomType) {
					roomCount++;
					if (room.Output > minOutput)			
						validCount++;
				}
			}
			if (roomCount == 0)
				return false;
			return roomCount == validCount ? true : false;
		}

		public bool ValidateOps () {
				return ValidationCriteria(RoomType.Conn);
		}

		public bool ValidateRadio () {
			return ValidationCriteria (RoomType.RadioRoom);
		}

		public bool ValidateSonar () {
			return ValidationCriteria(RoomType.Sonar );
		}

		public bool ValidateWeapons () {
		// may be later also check missle deck
			return ValidationCriteria(RoomType.TorpedoRoom);
		}

		public bool ValidatePropulsion () {	// Engine & battery must be ok
			bool engineOk = ValidationCriteria(RoomType.EngineRoom);
			bool batteryOk = ValidationCriteria (RoomType.Battery);
			return engineOk && batteryOk;
		}

		#endregion

		#region Rooms


		public int GetAllOutputOfUnit (Units outputUnit) {
			int output = 0;
			foreach (var roomPair in rooms) {
				Room room = roomPair.Value;
				if (room.UnitOfCapacity == outputUnit)
					output += room.Output;
				//TODO: set dedicated CrewTypes for bunks
				if (room.TypeOfRoom == RoomType.Bunks &&
				    (outputUnit == Units.Cook || outputUnit == Units.Watchstander || outputUnit == Units.Engineers || outputUnit == Units.Radioman || outputUnit == Units.Sonarman || outputUnit == Units.Torpedoman)) {
					output += 4;
				}

			}
			return output;
		}

		public int GetAllNeededResourcesOfUnit (Units reqUnit) {
			int resource = 0;
			foreach (var roomPair in rooms) {
				Room room = roomPair.Value;
				if (room.NeededResources != null)
					resource += room.GetResouceNeeded (reqUnit);
			}
			return resource;
		}

		private void AddRoomToSubmarine (Room addThisRoom) {
			rooms.Add (_nextRoomID, addThisRoom);
			_nextRoomID++;
		}

		private void RemoveRoomFromSubmarine (int RoomID) {
			rooms.Remove (RoomID);
		}

		public Room GetRoom (int RoomID) {
			return rooms.ContainsKey (RoomID) ? rooms [RoomID] : null;
		}

		private void MergeRooms (int newRoomID, int oldRoomID) {
			Room oldRoom = GetRoom (oldRoomID);
			if (oldRoom == null) {
				UnityEngine.Debug.LogError ("ERROR: cannot change room because old room (ID:" + oldRoomID + ") doesn't exist");
			}
			else {
				Room newRoom = GetRoom (newRoomID);
				if (newRoom == null) {
					UnityEngine.Debug.LogError ("ERROR: cannot change room because new room (ID:" + newRoomID + ") doesn't exist");
				}
				else {
					// remember is room was (in)valid before adding this tile
					bool newRoomValid = newRoom.IsLayoutValid;	

					foreach (Point coord in oldRoom.coordinatesOfTilesInRoom) {
						Tile oldRoomTile = GetTileAt (coord.x, coord.y);
	
						// change RoomID of each Tile in old room
						oldRoomTile.RoomID = newRoomID;
						// add Tiles to new room (no need to removed them form old room as old room will be destroyed)
						newRoom.AddTile (oldRoomTile);
					}

					// compare new valid layout
					newRoom.WarnTilesInRoomThatLayoutChanged (newRoomValid);  
					// remove old room form sub											 
					RemoveRoomFromSubmarine (oldRoomID);  

				}
			}
		}

		private Room RebuildRoom (int roomID) {
			if (rooms.ContainsKey (roomID) == false) {
				UnityEngine.Debug.LogError ("ERROR: cannot rebuild a room that doesn't exist, roomID:" + roomID);
				return null;
			}
			else {
				RoomType rebuildRoomType = rooms [roomID].TypeOfRoom;

				List<Point> remeberCoordinatesOfTiles = rooms [roomID].coordinatesOfTilesInRoom;

				// remove tiles in the old room (does also reset tile)
				foreach (Point coordTeRemove in remeberCoordinatesOfTiles) {
					Tile rebuildTile = GetTileAt (coordTeRemove.x, coordTeRemove.y);
					//rooms [roomID].RemoveTile (rebuildTile);
					rebuildTile.Reset ();
				}
				// remove the room so it can be rebuild
				RemoveRoomFromSubmarine (roomID);
				// rebuild room
				foreach (Point coordToAdd in remeberCoordinatesOfTiles) {
					Tile rebuildTile = GetTileAt (coordToAdd.x, coordToAdd.y);
					AddTileToRoom (rebuildTile.X, rebuildTile.Y, rebuildRoomType);
				}
				// room is rebuilt, get new roomID
				Point coordToGetRoomID = remeberCoordinatesOfTiles [0];	// get X,Y coordinates so tile can be found in su
				int newRoomID = GetTileAt (coordToGetRoomID.x, coordToGetRoomID.y).RoomID;
				return rooms [newRoomID];
			}
		}

		#endregion

		#region Tiles

		public Tile GetTileAt (int x, int y) {
			if (x > lengthOfSub - 1 || x < 0) {
				//UnityEngine.Debug.LogError ("ERROR: get Tile x (" + x + ")is outside length (" + (lengthOfSub - 1) + ") of submarine");
				return null;
			}
			if (y > heightOfSub - 1 || y < 0) {
				//UnityEngine.Debug.LogError ("ERROR: get Tile x (" + y + ")is outside height (" + (heightOfSub - 1) + ") of submarine");
				return null;
			}

			if (_tile [x, y] == null) {
				throw new Exception ("Tile at " + x + "," + y + " cannot be null");
			}
			return _tile [x, y];
		}

		// add a Tile to a existing room, or start a new room
		public void AddTileToRoom (int x, int y, RoomType buildRoomOfType) {
			Tile newRoomTile = GetTileAt (x, y);
			if (newRoomTile == null) {
				UnityEngine.Debug.LogError ("ERROR: cannot create/expand a room outside the submarine");
			}
			else {
				if (!newRoomTile.canContainRoom) {
					UnityEngine.Debug.LogError ("ERROR: cannot create/expand a room at unavailable Tile (" + x + "," + y + ")");
				}
				else {
					if (newRoomTile.RoomID != 0) {
						//UnityEngine.Debug.Log ("Already in the " + GetRoomTypeOfTile (newRoomTile) + " room (" + newRoomTile.RoomID + "), remove me first");
					}
					else {
						Tile checkTile;
						bool foundSameRoomType;

						// get info of Tile North
						checkTile = GetTileAt (x, y + 1);
						foundSameRoomType = CheckNeigborSameRoomType (x, y, buildRoomOfType, newRoomTile, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 1;		// add wall type for North
							BuildWallsAroundTile (checkTile);	// rebuild wall of neighborer
						}
						// get info of Tile East
						checkTile = GetTileAt (x + 1, y);
						foundSameRoomType = CheckNeigborSameRoomType (x, y, buildRoomOfType, newRoomTile, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 2; // add wall type for East
							BuildWallsAroundTile (checkTile);// rebuild wall of neighborer
						}
						// get info of Tile South
						checkTile = GetTileAt (x, y - 1);
						foundSameRoomType = CheckNeigborSameRoomType (x, y, buildRoomOfType, newRoomTile, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 4; // add wall type for South
							BuildWallsAroundTile (checkTile);// rebuild wall of neighborer
						}
						// get info of Tile West
						checkTile = GetTileAt (x - 1, y);
						foundSameRoomType = CheckNeigborSameRoomType (x, y, buildRoomOfType, newRoomTile, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 8; // add wall type for West
							BuildWallsAroundTile (checkTile);// rebuild wall of neighborer
						}

						//UnityEngine.Debug.Log ("New tile has wall type: " + newRoomTile.WallType);

						if (newRoomTile.RoomID == 0) {
							// if no neighbor Tile is part of same room type then start a new room with this Tile
							Room newRoom = Room.CreateRoomOfType (buildRoomOfType, inThisSub: this);     // create new room of this room type
							AddRoomToSubmarine (newRoom);                   // add new room to submarine
							newRoom.AddTile (newRoomTile);                  // add Tile to room
							newRoomTile.RoomID = _nextRoomID - 1;           // set RoomID in Tile
							//UnityEngine.Debug.Log ("Added tile (" + x + "," + y + "): no neighbor Tile is part of a " + buildRoomOfType + ", so started a new room " + newRoomTile.RoomID + ", wall type is " + newRoomTile.WallType);

						}
					}
				}
			}
		}

		// remove Tile of room (remove room is it's last Tile)
		public void RemoveTileOfRoom (int x, int y) {
			Tile TileToBeRemoved = GetTileAt (x, y);
			if (TileToBeRemoved == null) {
				UnityEngine.Debug.LogError ("ERROR cannot remove Tile that doesn't exists");
			}
			else {
				// remove Tile of room
				int roomID = TileToBeRemoved.RoomID;
				Room removeFromThisRoom = GetRoom (roomID);
				
				if (removeFromThisRoom == null) {
					//UnityEngine.Debug.Log ("Tile doesn't belong to a room");
				}
				else {
					// remember layout validation before removing tile
					bool oldRoomLayoutValid = removeFromThisRoom.IsLayoutValid;             

					// reset tile
					TileToBeRemoved.Reset ();

					// remove tile from room to get size of the room without this tile
					removeFromThisRoom.RemoveTile (TileToBeRemoved);
					// only when there is a room left, this was maybe the last tile in the room
					if (removeFromThisRoom.Size > 0) { 
						// rebuild room to be sure the wall type and layout is still OK
						Room newRoom = RebuildRoom (roomID);    
						removeFromThisRoom = null; 			// set to null be sure it isn't used anymore, the room is rebuild
						roomID = TileToBeRemoved.RoomID; 	// get new roomID

						// compare new valid layout
						newRoom.WarnTilesInRoomThatLayoutChanged (oldRoomLayoutValid);	 
					}
					else {
						// destroy room
						RemoveRoomFromSubmarine (roomID);
					}
				}
			}
		}

		public RoomType GetRoomTypeOfTile (Tile ofThisTile) {
			if (ofThisTile.RoomID == 0) {
				// UnityEngine.Debug.WriteLine("Room at (" + ofThisTile.x + "," + ofThisTile.y + ") is not part of a room");
				return RoomType.Empty;
			}
			else {
				if (!rooms.ContainsKey (ofThisTile.RoomID)) {
					throw new Exception ("Tile (" + ofThisTile.X + "," + ofThisTile.Y + ") " +
					"has roomID " + ofThisTile.RoomID + ", but cannot be found in rooms inside sub ");
				}
				return rooms [ofThisTile.RoomID].TypeOfRoom;
			}
		}

		public bool IsTilePartOfValidRoomLayout (Tile checkTile) {
			if (checkTile.RoomID == 0) {
				//UnityEngine.Debug.Log ("Title isn't part of a room, so it cannot check if it's layout is valid");
				return false;
			}
			else {
				return rooms [checkTile.RoomID].IsLayoutValid;
			}
		}

		public bool IsTilePartOfRoomWithResources (Tile checkTile) {
			if (checkTile.RoomID == 0)
				return false;
			
			if (rooms [checkTile.RoomID].NeededResources != null)
				return rooms [checkTile.RoomID].ResourcesAvailable;
			else {
				return true; // no resources needs for this room = requirments always ok
			}

		}

		private void BuildWallsAroundTile (Tile checkAroundThisTile) {

			checkAroundThisTile.WallType = 0; // reset already found walls

			int x = checkAroundThisTile.X, y = checkAroundThisTile.Y;
			bool foundSameRoomType;
			Tile checkTile;
			RoomType checkRoomType = GetRoomTypeOfTile (checkAroundThisTile);

			// get info of Tile North
			checkTile = GetTileAt (x, y + 1);
			foundSameRoomType = CheckNeigborSameRoomType (x, y, checkRoomType, checkAroundThisTile, checkTile);
			if (foundSameRoomType)
				checkAroundThisTile.WallType += 1;	// add wall type for North
		
			// get info of Tile East
			checkTile = GetTileAt (x + 1, y);
			foundSameRoomType = CheckNeigborSameRoomType (x, y, checkRoomType, checkAroundThisTile, checkTile);
			if (foundSameRoomType)
				checkAroundThisTile.WallType += 2; // add wall type for East

			// get info of Tile South
			checkTile = GetTileAt (x, y - 1);
			foundSameRoomType = CheckNeigborSameRoomType (x, y, checkRoomType, checkAroundThisTile, checkTile);
			if (foundSameRoomType)
				checkAroundThisTile.WallType += 4; // add wall type for South

			// get info of Tile West
			checkTile = GetTileAt (x - 1, y);
			foundSameRoomType = CheckNeigborSameRoomType (x, y, checkRoomType, checkAroundThisTile, checkTile);
			if (foundSameRoomType)
				checkAroundThisTile.WallType += 8; // add wall type for West

			//UnityEngine.Debug.Log ("(re)build walls around (" + x + "," + y + ") wall type is now: " + checkAroundThisTile.WallType);

		}

		// check is neighbor Tile is same room type, add or merge. Return if same room type is found
		private bool CheckNeigborSameRoomType (int x, int y, RoomType wantedRoomType, Tile newRoomTile, Tile checkTile) {
			if (checkTile != null) {
				RoomType roomTypeOfNeighbor = GetRoomTypeOfTile (checkTile);
				// neighbor Tile exists
				if (wantedRoomType == roomTypeOfNeighbor && roomTypeOfNeighbor != RoomType.Empty)
					return true;
			}
			return false; // all other cases return not same room type was found
		}

		private void AddToOrMergeRooms (Tile newRoomTile, Tile neigboreTile) {
			// if neighborer Tile has same room type as this Tile
			if (newRoomTile.RoomID == 0) {
				// Tile is not assigned to a room yet, add it tot neighbor room now
				Room addToThisRoom = rooms [neigboreTile.RoomID];	// room to add this tile too

				//UnityEngine.Debug.Log ("Add Tile (" + newRoomTile.X + "," + newRoomTile.Y + ") to existing room ID:" + neigboreTile.RoomID);
				bool oldRoomLayoutValid = addToThisRoom.IsLayoutValid;      // remember layout validation before removing tile
				newRoomTile.RoomID = neigboreTile.RoomID;                              // store existing RoomID in newRoomTile
				addToThisRoom.AddTile (newRoomTile);                                // add Tile to room
				addToThisRoom.WarnTilesInRoomThatLayoutChanged (oldRoomLayoutValid);	// compare new valid layout 
				
			}
			else {// Tile is already in a room: check if neighborer is in same room
				if (newRoomTile.RoomID != neigboreTile.RoomID) {
					// neighbor Tile is same room type but another room (id) = merge rooms now
					//UnityEngine.Debug.Log ("Tile (" + newRoomTile.X + "," + newRoomTile.Y + ") has RoomID " + newRoomTile.RoomID + " neighbor has RoomID " + neigboreTile.RoomID);
					//UnityEngine.Debug.Log ("Merge rooms now");
					MergeRooms (newRoomTile.RoomID, neigboreTile.RoomID);
	
				}
			}
		}

		#endregion
	}

	
		

}
