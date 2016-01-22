
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

		// next roomID
		[UnityEngine.SerializeField]
		// ID 0 = no room assigned to Tile
		int _nextRoomID = 1;

		// Room properties: capacity / tile, min size (output unit is set in concrete classes)
		public Dictionary<string,float> RoomProperties { get; private set; }

		// Requirements of a room, can be more then 1 = List
		public Dictionary<RoomType, List<Resource>> RoomPropertiesReqRes { get; private set; }


		// List of crew
		public List<Crew> CrewList { get; private set; }

		[UnityEngine.SerializeField]
		int _nextCrewID = 1;

		public int SpacesForOfficers  { get { return GetAllOutputOfUnit (Units.Officers) - AmountOfOfficers; } }

		public int AmountOfOfficers { get { return AmountOfCrewType (Units.Officers); } }

		//		// 1 cook / gallery
		//		public int SpacesForCooks  { get { return AmountOfRoomOftype (RoomType.Gallery) - AmountOfCooks; } }

		//		public int AmountOfCooks  { get { return AmountOfCrewType (Units.Cook); } }

		public int SpacesForEnlisted  { get { return GetAllOutputOfUnit (Units.Enlisted) - AmountOfEnlisted (); } }


		static public int MaxPiecesOnTile = 2;

		public Dictionary<int,Carrier> ResourceCarriers { get; private set; }

		[UnityEngine.SerializeField]
		int _nextCarrierID = 1;

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

			// save tiles, fsserialize doesn't support saving/loading of multi dimension array = convert 2D array to List = save to other file
			List<Tile> allTiles = new List<Tile> ();         
			for (int x = 0; x < lengthOfSub; x++) {
				for (int y = 0; y < heightOfSub; y++) {
					allTiles.Add (GetTileAt (x, y));
				}
			}
			string jsonStringOfAllTitles = Serialize (typeof(List<Tile>), allTiles);

			// save files to disk
			File.WriteAllText (fileName, jsonStringOfSub);
			File.WriteAllText (fileName.Substring (0, fileName.Length - 5) + "_Tiles", jsonStringOfAllTitles); // add _Tiles
		}

		// loading function is split into 2 component because first the submarine dimension needs to be read
		// with these dimensions know all game objects for the tiles can be created (add UpdateTileSprite as changed action)
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
			RoomProperties = loadedSub.RoomProperties;
			RoomPropertiesReqRes = loadedSub.RoomPropertiesReqRes;

			// load all tiles (2D array not supported,needs conversion)
			_tile = new Tile[lengthOfSub, heightOfSub]; // reset all old tiles

			string jsonStringOfAllTitles = File.ReadAllText (fileName.Substring (0, fileName.Length - 5) + "_Tiles");
			List<Tile> allTiles = (List<Tile>)Deserialize (typeof(List<Tile>), jsonStringOfAllTitles);

			foreach (Tile addTile in allTiles) {
				foreach (Piece piece in addTile.PiecesOnTile) {
					piece.inSub = this;
				}
				_tile [addTile.X, addTile.Y] = addTile;
			}

			// tiles are load, now load rooms (with tiles is it)
			rooms = new Dictionary<int, Room> (); // reset rooms
			rooms = loadedSub.rooms; 

			foreach (var roomPair in rooms) {
				//int roomid = roomPair.Key;
				Room room = roomPair.Value;
				room.inSub = this; // set ref. point too this Submarine 
			}

			_nextCrewID = loadedSub._nextCrewID;
			CrewList = loadedSub.CrewList;

			ResourceCarriers = new Dictionary<int, Carrier> ();
			ResourceCarriers = loadedSub.ResourceCarriers;

			_nextCarrierID = loadedSub._nextCarrierID;
			foreach (var carrierPair in ResourceCarriers) {
				foreach (Piece piece in carrierPair.Value.Pieces) {
					piece.inSub = this; // set ref. point too this Submarine
				}
			}

		}

		#endregion


		#region CONSTRUCTOR

		//TODO: should be in a concrete class as it's tight to the submarine outline image, so other classes and outline images can be uses

		// set sub dimensions, room properties, room requirement but don't create rooms or outline here
		// GameObject should be created first so Action updateSprite is attached before a room is created
		public Sub () { 
			
			lengthOfSub = 41;
			heightOfSub = 6;

			lenghtOfBridgeTower = 3;
			heightOfBridgeTower = 2;
			startOfBridgeTower = 24;

			smallerTailUpper = 1;
			smallerTailLower = 1;
			smallerTailLenght = 5;

			// initialize 2D array, still doesn't contain anything
			_tile = new Tile[lengthOfSub, heightOfSub];
			// instantiate Tiles
			for (int x = 0; x < lengthOfSub; x++) {
				for (int y = 0; y < heightOfSub; y++) {
					_tile [x, y] = new Tile (x, y);
				}
			}
			// instantiate rooms
			rooms = new Dictionary<int, Room> ();

			// instantiate CrewList
			CrewList = new List<Crew> ();

			// instantiate ResourceCarriers
			ResourceCarriers = new  Dictionary<int, Carrier> ();

			// set room properties 
			SetRoomProperties ();
			// set room resources needs
			SetRoomResoucresNeeds ();
		}

		// set tiles around tail and tower as unavailable for building, create Tower
		public void SetOutlines () {
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
			// add propellor Tower
			for (int x = 0; x < 1; x++) {
				for (int y = 1; y <= 2; y++) {
					AddTileToRoom (x, y, RoomType.Propellor);
				}
			}
		}

		// set min tile size, capacity per tile and unit of capacity for each Room Type (output unit is set in concrete classes)
		private void SetRoomProperties () {//TODO: read from config file for this submarine outline type
			RoomProperties = new Dictionary<string, float> ();

			RoomProperties ["Propellor_Min"] = 1.0f;
			RoomProperties ["Propellor_CapPerTile"] = 12; // 12

			RoomProperties ["Stairs_Min"] = 1.0f;
			RoomProperties ["Stairs_CapPerTile"] = 0;

			RoomProperties ["Balasttank_Min"] = 6.0f;
			RoomProperties ["Balasttank_CapPerTile"] = 230; // 1380

			RoomProperties ["EngineRoom_Min"] = 18.0f;
			RoomProperties ["EngineRoom_CapPerTile"] = 123; // 1476

			RoomProperties ["Generator_Min"] = 9.0f;
			RoomProperties ["Generator_CapPerTile"] = 478; // 5736
					
			RoomProperties ["Battery_Min"] = 8.0f;
			RoomProperties ["Battery_CapPerTile"] = 5.6f; // 44,8
					
			RoomProperties ["Bridge_Min"] = 4.0f;
			RoomProperties ["Bridge_CapPerTile"] = 0;

			RoomProperties ["Gallery_Min"] = 6.0f;
			RoomProperties ["Gallery_CapPerTile"] = 4; // 24
					
			RoomProperties ["Cabin_Min"] = 2.0f;
			RoomProperties ["Cabin_CapPerTile"] = 1;
					
			RoomProperties ["Bunks_Min"] = 8.0f;
			RoomProperties ["Bunks_CapPerTile"] = 1.5f; // 8

			RoomProperties ["Conn_Min"] = 10.0f;
			RoomProperties ["Conn_CapPerTile"] = 1; // 10
					
			RoomProperties ["Sonar_Min"] = 4.0f;
			RoomProperties ["Sonar_CapPerTile"] = 1; // 4
					
			RoomProperties ["RadioRoom_Min"] = 4.0f;
			RoomProperties ["RadioRoom_CapPerTile"] = 1; // 4
					
			RoomProperties ["FuelTank_Min"] = 9.0f;
			RoomProperties ["FuelTank_CapPerTile"] = 1000; // 9000
					
			RoomProperties ["PumpRoom_Min"] = 8.0f;
			RoomProperties ["PumpRoom_CapPerTile"] = 1000; // 8000
					
			RoomProperties ["StorageRoom_Min"] = 1.0f;
			RoomProperties ["StorageRoom_CapPerTile"] = 27; // 27
				
			RoomProperties ["EscapeHatch_Min"] = 1.0f;
			RoomProperties ["EscapeHatch_CapPerTile"] = 1;
					
			RoomProperties ["TorpedoRoom_Min"] = 12.0f;
			RoomProperties ["TorpedoRoom_CapPerTile"] = 1;
		}

		private void SetRoomResoucresNeeds () {
			RoomPropertiesReqRes = new Dictionary<RoomType,List<Resource>> ();

			RoomPropertiesReqRes [RoomType.Propellor] = new List<Resource> {
				{ new Resource (Units.pks, 670) }
			};
				
			RoomPropertiesReqRes [RoomType.EngineRoom] = new List<Resource> {
				{ new Resource (Units.liters_fuel, 100) },
				{ new Resource (Units.Engineers, 4.0f / RoomProperties ["EngineRoom_Min"]) }
			};

			RoomPropertiesReqRes [RoomType.Generator] = new List<Resource> {
				{ new Resource (Units.pks, 17f / RoomProperties ["Generator_Min"]) },
				{ new Resource (Units.Engineers, 2.0f / RoomProperties ["Generator_Min"]) }
			};
			RoomPropertiesReqRes [RoomType.Battery] = new List<Resource> { { new Resource (Units.kW, 500f / RoomProperties ["Battery_Min"]) } };

			RoomPropertiesReqRes [RoomType.Bridge] = new List<Resource> (); 
			// TODO: not sure if bridge needs crew (underwater) { { new Resource (Units.Watchstanders, 2) } };

			RoomPropertiesReqRes [RoomType.Gallery] = new List<Resource> {
				{ new Resource (Units.tins, 16.0f / RoomProperties ["Gallery_Min"]) },
				//{ new Resource (Units.Cook, 1.0f / RoomProperties ["Gallery_Min"]) },
				// gallery may not need electricity because thats a resource loop:gallery needs generator needs engineers needs food needs gallery
				//	{ new Resource (Units.kW, 10f / RoomProperties ["Gallery_Min"]) }
			};
			RoomPropertiesReqRes [RoomType.Cabin] = new List<Resource> {
				{ new Resource (Units.food, 2.0f / RoomProperties ["Cabin_Min"]) },
				//{ new Resource (Units.kW, 0.2f / RoomProperties ["Cabin_Min"]) }
			};

			RoomPropertiesReqRes [RoomType.Bunks] = new List<Resource> { 
				{ new Resource (Units.food, 8.0f / RoomProperties ["Bunks_Min"]) },
				//{ new Resource (Units.kW, 0.5f / RoomProperties ["Bunks_Min"]) }
			};

			RoomPropertiesReqRes [RoomType.Conn] = new List<Resource> {
				{ new Resource (Units.Officers, 2.0f /	RoomProperties ["Conn_Min"]) },
				{ new Resource (Units.Watchstanders, 4.0f /	RoomProperties ["Conn_Min"]) },
				{ new Resource (Units.kW, 5.0f / RoomProperties ["Conn_Min"]) }
			};

			RoomPropertiesReqRes [RoomType.Sonar] = new List<Resource> {
				{ new Resource (Units.Sonarman, 1.0f / RoomProperties ["Sonar_Min"]) },
				{ new Resource (Units.kW, 200.0f / RoomProperties ["Sonar_Min"]) }
			};

			RoomPropertiesReqRes [RoomType.RadioRoom] = new List<Resource> { 
				{ new Resource (Units.Radioman, 1.0f / RoomProperties ["RadioRoom_Min"]) },
				{ new Resource (Units.kW, 50.0f / RoomProperties ["RadioRoom_Min"]) }
			
			};
	
			RoomPropertiesReqRes [RoomType.PumpRoom] = new List<Resource> { 
				{ new Resource (Units.liters_balast, 1.0f) },
				{ new Resource (Units.kW, 250.0f) }
			};
		
			RoomPropertiesReqRes [RoomType.TorpedoRoom] = new List<Resource> { 
				{ new Resource (Units.Torpedoman, 4.0f / RoomProperties ["TorpedoRoom_Min"]) }
			};
			

			RoomPropertiesReqRes [RoomType.EscapeHatch] = new List<Resource> ();
			RoomPropertiesReqRes [RoomType.Stairs] = new List<Resource> ();
			RoomPropertiesReqRes [RoomType.BalastTank] = new List<Resource> ();
			RoomPropertiesReqRes [RoomType.FuelTank] = new List<Resource> ();
			RoomPropertiesReqRes [RoomType.StorageRoom] = new List<Resource> ();

		}

		#endregion


		#region Design Validation

		// check if all rooms of a RoomType produce enough output
		private bool ValidationCriteria (RoomType checkRoomType, int minOutput = 0) {
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

		public bool ValidateOps () { // Ops = Conn + Radio + Sonar
			bool connOk = ValidationCriteria (RoomType.Conn);
			bool radioOk = ValidationCriteria (RoomType.RadioRoom);
			bool sonarOk = ValidationCriteria (RoomType.Sonar);
			return connOk && radioOk && sonarOk;
		}

		public bool ValidateCrew () {
			bool bunksOk = ValidationCriteria (RoomType.Bunks);
			bool cabinOk = ValidationCriteria (RoomType.Cabin);
			bool escapeOk = ValidationCriteria (RoomType.EscapeHatch);
			return bunksOk && cabinOk && escapeOk;
		}


		public bool ValidateWeapons () {
			//TODO: may be later also check missile deck
			return ValidationCriteria (RoomType.TorpedoRoom);
		}

		public bool ValidatePropulsion () {	// Proppelor & battery must be ok
			// don't check engine but propellor as this is the "end station"
			bool propellorOk = ValidationCriteria (RoomType.Propellor);
			//bool engineOk = ValidationCriteria (RoomType.EngineRoom);
			bool batteryOk = ValidationCriteria (RoomType.Battery);
			return propellorOk && batteryOk;
		}

		#endregion

		#region Rooms

		public int GetAllOutputOfUnit (Units outputUnit) {
			int output = 0;
			foreach (var roomPair in rooms) {
				Room room = roomPair.Value;
				if (room.UnitOfOutput == outputUnit)
					output += room.Output;
			}
			return output;
		}

		public int GetAllNeededResourcesOfUnit (Units reqUnit) {
			int required = 0;
			foreach (var roomPair in rooms) {
				Room room = roomPair.Value;
				if (room.NeededResources != null)
					required += room.GetResouceNeeded (reqUnit);
			}
			return required;
		}

		private int AmountOfRoomOftype (RoomType typeOfRoom) {
			return rooms.Where (r => r.Value.TypeOfRoom == typeOfRoom).Count ();
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
					newRoom.WarnTilesIfRoomLayoutValidationIsChanged (newRoomValid);  
					// remove old room form sub											 
					RemoveRoomFromSubmarine (oldRoomID);  


					// rebuild carriers in this merged room
					List<int> rebuiledCarriers = new List<int> ();
					// check each tile in merged room
					foreach (var coord in newRoom.coordinatesOfTilesInRoom) {
						Tile tileInNewRoom = GetTileAt (coord.x, coord.y);
						// check each piece on tile
						rebuiledCarriers = RebuildCarrierForPieces (tileInNewRoom.PiecesOnTile, rebuiledCarriers);
					}
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
				List<Piece> remeberPieces = new List<Piece> ();
				// remove tiles in the old room (does also reset tile)
				foreach (Point coordTeRemove in remeberCoordinatesOfTiles) {
					Tile rebuildTile = GetTileAt (coordTeRemove.x, coordTeRemove.y);
					foreach (Piece piece in rebuildTile.PiecesOnTile) {
						if (piece.Type != PieceType.None)
							remeberPieces.Add (piece);
					}
					rebuildTile.Reset ();	// also reset all pieces on tile, rebuild carrier when done !
				}

				// remove the room so it can be rebuild
				RemoveRoomFromSubmarine (roomID);

				// rebuild room
				List<int> rebuiledCarriers = new List<int> ();
				foreach (Point coordToAdd in remeberCoordinatesOfTiles) {
					Tile rebuildTile = GetTileAt (coordToAdd.x, coordToAdd.y);
					AddTileToRoom (rebuildTile.X, rebuildTile.Y, rebuildRoomType);
					rebuiledCarriers = RebuildCarrierForPieces (rebuildTile.PiecesOnTile, rebuiledCarriers);
				}

				// room is rebuilt, get new roomID
				Point coordToGetRoomID = remeberCoordinatesOfTiles [0];	// get X,Y coordinates so tile can be found in sub
				int newRoomID = GetTileAt (coordToGetRoomID.x, coordToGetRoomID.y).RoomID;

				// return new room
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
						foundSameRoomType = CheckNeigborSameRoomType (buildRoomOfType, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 1;		// add wall type for North
							BuildWallsAroundTile (checkTile);	// rebuild wall of neighborer
						}
						// get info of Tile East
						checkTile = GetTileAt (x + 1, y);
						foundSameRoomType = CheckNeigborSameRoomType (buildRoomOfType, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 2; // add wall type for East
							BuildWallsAroundTile (checkTile);// rebuild wall of neighborer
						}
						// get info of Tile South
						checkTile = GetTileAt (x, y - 1);
						foundSameRoomType = CheckNeigborSameRoomType (buildRoomOfType, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 4; // add wall type for South
							BuildWallsAroundTile (checkTile);// rebuild wall of neighborer
						}
						// get info of Tile West
						checkTile = GetTileAt (x - 1, y);
						foundSameRoomType = CheckNeigborSameRoomType (buildRoomOfType, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 8; // add wall type for West
							BuildWallsAroundTile (checkTile);// rebuild wall of neighborer
						}

						//UnityEngine.Debug.Log ("New tile has wall type: " + newRoomTile.WallType);

						if (newRoomTile.RoomID == 0) {
							// if no neighbor Tile is part of same room type then start a new room with this Tile
							Room newRoom = Room.CreateRoomOfType (buildRoomOfType, inThisSub: this);     // create new room of this room type
							newRoom.RoomID = _nextRoomID;					// set RoomID in new room
							AddRoomToSubmarine (newRoom);                   // add new room to submarine, _nextRoomID well be inc.
							newRoom.AddTile (newRoomTile);                  // add Tile to room
							// set RoomID in Tile, this will also fire the Update Sprite so be sure everything else is set !!
							newRoomTile.RoomID = newRoom.RoomID;           	

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
				// remove all pieces form tile
				RemovePiecesFromTile (x, y);

				// remove Tile of room
				int roomID = TileToBeRemoved.RoomID;
				Room removeFromThisRoom = GetRoom (roomID);
				
				if (removeFromThisRoom != null) {
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
						// compare new valid layout
						newRoom.WarnTilesIfRoomLayoutValidationIsChanged (oldRoomLayoutValid);	 
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
				return true; // no resources needs for this room = requirements always ok
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
			foundSameRoomType = CheckNeigborSameRoomType (checkRoomType, checkTile);
			if (foundSameRoomType)
				checkAroundThisTile.WallType += 1;	// add wall type for North
		
			// get info of Tile East
			checkTile = GetTileAt (x + 1, y);
			foundSameRoomType = CheckNeigborSameRoomType (checkRoomType, checkTile);
			if (foundSameRoomType)
				checkAroundThisTile.WallType += 2; // add wall type for East

			// get info of Tile South
			checkTile = GetTileAt (x, y - 1);
			foundSameRoomType = CheckNeigborSameRoomType (checkRoomType, checkTile);
			if (foundSameRoomType)
				checkAroundThisTile.WallType += 4; // add wall type for South

			// get info of Tile West
			checkTile = GetTileAt (x - 1, y);
			foundSameRoomType = CheckNeigborSameRoomType (checkRoomType, checkTile);
			if (foundSameRoomType)
				checkAroundThisTile.WallType += 8; // add wall type for West

			//UnityEngine.Debug.Log ("(re)build walls around (" + x + "," + y + ") wall type is now: " + checkAroundThisTile.WallType);
		}

		// check is neighbor Tile is same room type, add or merge. Return if same room type is found
		private bool CheckNeigborSameRoomType (RoomType wantedRoomType, Tile checkTile) {
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
				addToThisRoom.WarnTilesIfRoomLayoutValidationIsChanged (oldRoomLayoutValid);	// compare new valid layout 
				
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

		#region Crew

		public void AddCrew (Units crewType) {
			// Add officer
			if (crewType == Units.Officers) {
				// add only if there is enough spaces left
				if (SpacesForOfficers > 0) {
					AddCrewToSub (crewType);
				}
			}
//			// Add cook (doesn't rest in Bunks because of loop: bunk needs food, food needs cook, cook needs bunk)
//			if (crewType == Units.Cook) {
//				// add only if there is enough spaces left
//				if (SpacesForCooks > 0) {
//					AddCrewToSub (crewType);
//				}
//			}
			// Add normal crew (not Cooks)
			if (Resource.isEnlisted (crewType)) {
				// add only if there is enough spaces left (bunks have Enlisted as generic crew type)
				if (SpacesForEnlisted > 0) {
					AddCrewToSub (crewType);
				}
			}
		}

		private void AddCrewToSub (Units crewType) {
			CrewList.Add (new Crew (crewType, _nextCrewID));
			_nextCrewID++;
		}

		public void RemoveCrew (Units crewType) {
			int stillCrewAboard = AmountOfCrewType (crewType);
			if (stillCrewAboard > 0) {
				Crew foundCrew = CrewList.Where (c => c.Type == crewType).First ();
				CrewList.Remove (foundCrew);
			}
		}

		public int AmountOfCrewType (Units crewType) {
			return CrewList.Where (c => c.Type == crewType).Count ();
		}

		public int AmountOfEnlisted () {
			int amount = 0;
			amount += CrewList.Where (c => c.Type == Units.Engineers).Count ();
			amount += CrewList.Where (c => c.Type == Units.Radioman).Count ();
			amount += CrewList.Where (c => c.Type == Units.Sonarman).Count ();
			amount += CrewList.Where (c => c.Type == Units.Torpedoman).Count ();
			amount += CrewList.Where (c => c.Type == Units.Watchstanders).Count ();
			return amount;
		}

		#endregion

		#region Carrier / Pieces

		private bool AddPieceToNeighor (Piece newPiece, Carrier foundSameCarrierType, Point checkCoord, bool remember1found) {
			if (foundSameCarrierType != null) {
				remember1found = remember1found || true;
				AddToOrMergeCarrier (newPiece, foundSameCarrierType);
				RecalculatedNeighboreCount (GetTileAt (checkCoord.x, checkCoord.y));
			}
			return remember1found;
		}

		public void AddPieceToTile (int x, int y, Units unitOfCarrier, bool isConnection = false) {
			Tile onTile = GetTileAt (x, y);
			if (onTile == null) {
				UnityEngine.Debug.LogError ("ERROR: cannot add an Piece on a not existing Tile");
				return;
			}
		
			// still place on this tile for an other piece ?
			if (onTile.FindPieceOfTypeOnTile (PieceType.None) == null) {
				UnityEngine.Debug.Log ("Already in the " + Sub.MaxPiecesOnTile + " items on tile (" + x + "," + y + ")");
				return;
			}

			// already a same piece type on this tile ?
			PieceType typeOfPiece = Piece.FindPieceType (unitOfCarrier);
			Piece findPieceOnTile = onTile.FindPieceOfTypeOnTile (typeOfPiece);

			if (typeOfPiece != PieceType.None) { // only check not-empty piece slots
				if (findPieceOnTile != null) {
					UnityEngine.Debug.Log ("Already a " + typeOfPiece + " piece on this tile");
					if (isConnection) {
						// set piece on tile as connected
						findPieceOnTile.IsConnection = true;
						// set already existing piece now as connection in carrier
						int index = ResourceCarriers [findPieceOnTile.carrierID].Pieces.FindIndex
							(p => p.coord.x == findPieceOnTile.coord.x && p.coord.y == findPieceOnTile.coord.y);
						if (index != -1) {
							ResourceCarriers [findPieceOnTile.carrierID].Pieces [index].IsConnection = true;
							onTile.TileChangedActions (onTile);
						}
						return;
					}
				}


				// create new piece according unit
				Piece newPiece = new Piece (typeOfPiece, new Point (x, y), this);
//				// set connected if its a connection //(may be it's automatic a connecton so OR it with wanted isConnection)
//				newPiece.IsConnection = isConnection;

				// add to tile, will call TileChanged Action to update UI
				onTile.AddItem (newPiece);

//				Tile checkTile;
				Carrier foundSameCarrierType;
				bool remember1found = false;
				Point checkCoord;

				// get info of Tile North
				if (typeOfPiece != PieceType.Shaft) { // shaft only connects horizontal
					checkCoord = new Point (x, y + 1);
					foundSameCarrierType = CheckSameNeighborCarrier (unitOfCarrier, checkCoord, 1, newPiece);
					remember1found = AddPieceToNeighor (newPiece, foundSameCarrierType, checkCoord, remember1found);
				}

				// get info of Tile East
				checkCoord = new Point (x + 1, y);
				foundSameCarrierType = CheckSameNeighborCarrier (unitOfCarrier, checkCoord, 2, newPiece);
				remember1found = AddPieceToNeighor (newPiece, foundSameCarrierType, checkCoord, remember1found);

				// get info of Tile South
				if (typeOfPiece != PieceType.Shaft) {// shaft only connects horizontal
					checkCoord = new Point (x, y - 1);
					foundSameCarrierType = CheckSameNeighborCarrier (unitOfCarrier, checkCoord, 4, newPiece);
					remember1found = AddPieceToNeighor (newPiece, foundSameCarrierType, checkCoord, remember1found);
				}

				// get info of Tile West
				checkCoord = new Point (x - 1, y);
				foundSameCarrierType = CheckSameNeighborCarrier (unitOfCarrier, checkCoord, 8, newPiece);
				remember1found = AddPieceToNeighor (newPiece, foundSameCarrierType, checkCoord, remember1found);
				
		
				if (remember1found == false) { // not found in any neigbores
					//  start a new Carrier with this piece
					Carrier newCarrier = new Carrier (_nextCarrierID, unitOfCarrier); // create concrete carrier
					ResourceCarriers [_nextCarrierID] = newCarrier;	// add new carrier to submarine
					newPiece.carrierID = _nextCarrierID;			// add carrier to piece
					_nextCarrierID++;		

					newPiece.IsConnection = true; // first piece in a carrier is always an end piece (0) so it's a connection to the room

					newCarrier.AddPiece (newPiece);					// add piece to carrier


					//UnityEngine.Debug.Log ("Added Piece on (" + x + "," + y + "): no neighbor piece has a " + typeOfPiece
					//+ ", so started a new Carrier " + newCarrier.ID);
					//+ ", wall type is " + newRoomTile.NeighboreCount);
				}

				// now Neighbor count and carrier ID are know draw the piece on the tile
				if (newPiece.OnTile.TileChangedActions != null)
					newPiece.OnTile.TileChangedActions (newPiece.OnTile);



			}
		}

		public void RemovePiecesFromTile (int x, int y) {
			Tile onTile = GetTileAt (x, y);
			if (onTile != null) {
				for (int i = 0; i < Sub.MaxPiecesOnTile; i++) {
					Piece piece = onTile.PiecesOnTile [i];
					// only for non-empty piece slots on tile
					if (piece.Type != PieceType.None) {
						int oldCarrierID = piece.carrierID;
						// remove piece from carrier
						ResourceCarriers [piece.carrierID].RemovePiece (piece); 	
						// remove piece from tile while be done in rebuild carrier
						onTile.RemoveItemInCarrier (piece.carrierID);					

						// rebuild carrier, can be split now into 2 not connected carriers
						RebuildCarrier (oldCarrierID);
					}
				}

			}
		}

		public List<int> RebuildCarrierForPieces (List<Piece> piecesNeedRebuild, List<int> rebuiledCarriers) {
			// rebuild carriers in this new room
			foreach (Piece piece in piecesNeedRebuild) {
				if (rebuiledCarriers.Contains (piece.carrierID) == false) {
					// only rebuild each carrier in this room 1 time
					RebuildCarrier (piece.carrierID);
					rebuiledCarriers.Add (piece.carrierID);
				}
			}
			return rebuiledCarriers;
		}

		//		public void AddConnectionToPieceOnTile (int x, int y, PieceType typeOfPiece) {
		//			Tile onTile = GetTileAt (x, y);
		//			if (onTile == null) {
		//				UnityEngine.Debug.LogError ("ERROR: cannot add an connection on a not existing Tile");
		//			}
		//			else {
		//				Piece addToPiece = onTile.FindPieceOfTypeOnTile (typeOfPiece);
		//				addToPiece.IsConnection = true;
		//			}
		//		}

		private Carrier CheckSameNeighborCarrier (Units findUnitOfCarrier, Point checkCoord, int addNeigborCount, Piece mainPiece) {
			Tile checkTile = GetTileAt (checkCoord.x, checkCoord.y);
			Carrier foundCarrier = null;
			// if builing on a tile at the edge of the outline then the N/E/S/W check will try to check a tile that's outside the submarine
			// so check if tile exists
			if (checkTile != null) {
				//	PieceType searchPieceType = Piece.FindPieceType (unitOfContent);
				foreach (Piece piece in checkTile.PiecesOnTile) {
					if (piece.Type != PieceType.None) {
						Carrier carrierOfpiece = ResourceCarriers [piece.carrierID];// get carrier of found piece
						if (carrierOfpiece.UnitOfContent == findUnitOfCarrier)		// found same carrier type
						foundCarrier = carrierOfpiece; 
					}
				}
			}
			// add neighbor count if neighbor is found
			if (foundCarrier != null)
				mainPiece.NeighboreCount += addNeigborCount;
			return foundCarrier;
		}

		private void AddToOrMergeCarrier (Piece piece, Carrier neigboreCarrier) {
			if (piece.carrierID == 0) {
				// piece is not part of carrier yet, add it to neigbore
				//UnityEngine.Debug.Log ("New  " + piece.Type + " is not part of carrier yet, add it to neigbore carrier: " + neigboreCarrier.ID);
				piece.carrierID = neigboreCarrier.ID;
				neigboreCarrier.AddPiece (piece);
			}
			else {
				// piece is already part of carrier, merge carriers
				if (piece.carrierID != neigboreCarrier.ID) {
					// only merge if piece is in other carrier (create a loop with pieces)
					//UnityEngine.Debug.Log ("New piece is already part of carrier, merge neighbore carrier");
					MergeCarriers (ResourceCarriers [piece.carrierID], neigboreCarrier);
				}
			}
		}

		private void MergeCarriers (Carrier oldCarrier, Carrier newCarrier) {
			foreach (Piece piece in oldCarrier.Pieces) {
				piece.carrierID = newCarrier.ID;	// set other carrier in each piece

				newCarrier.AddPiece (piece);		// add piece to new carrier
//				if (piece.IsConnection) 
//					// need to use AddConnectionToPieceOnTile to add connection to new created piece, not to the old rememberd piece
//					AddPieceToTile (piece.coord.x, piece.coord.y, piece.Type, true);
			}
			ResourceCarriers.Remove (oldCarrier.ID);// delete old carrier
			newCarrier.WarnAllPiecesOfCarrier ();	// warn pieces of merge (newly connected piece can have content now)
		}

		private void RecalculatedNeighboreCount (Tile checkTile) {
			foreach (Piece checkPiece in checkTile.PiecesOnTile) {
				PieceType typeOfPiece = checkPiece.Type;
				// only check not-empty piece slots on a tile
				if (typeOfPiece != PieceType.None) {
					int x = checkPiece.coord.x;
					int y = checkPiece.coord.y;

					Carrier carrierOfpiece = ResourceCarriers [checkPiece.carrierID];

					//Carrier foundSameCarrierType;
					// reset count
					checkPiece.NeighboreCount = 0;

					// get info of Tile North
					if (typeOfPiece != PieceType.Shaft) { // shaft only connects horizontal
//						checkTile = GetTileAt (x, y + 1);
						CheckSameNeighborCarrier (carrierOfpiece.UnitOfContent, new Point (x, y + 1), 1, checkPiece);
//						if (foundSameCarrierType != null)
//							checkPiece.NeighboreCount += 1;	
					}
					// get info of Tile East

//					checkTile = GetTileAt (x + 1, y);
					CheckSameNeighborCarrier (carrierOfpiece.UnitOfContent, new Point (x + 1, y), 2, checkPiece);
//					if (foundSameCarrierType != null)
//						checkPiece.NeighboreCount += 2;
				
					// get info of Tile South
					if (typeOfPiece != PieceType.Shaft) { // shaft only connects horizontal
//						checkTile = GetTileAt (x, y - 1);
						CheckSameNeighborCarrier (carrierOfpiece.UnitOfContent, new Point (x, y - 1), 4, checkPiece);
//						if (foundSameCarrierType != null)
//							checkPiece.NeighboreCount += 4;
					}
					// get info of Tile West
//					checkTile = GetTileAt (x - 1, y);
					CheckSameNeighborCarrier (carrierOfpiece.UnitOfContent, new Point (x - 1, y), 8, checkPiece);
//					if (foundSameCarrierType != null)
//						checkPiece.NeighboreCount += 8;
				
				}
			}
		}

		private void RebuildCarrier (int carrierID) {
			if (ResourceCarriers.ContainsKey (carrierID) == false) {
				if (carrierID != 0)
					UnityEngine.Debug.Log ("ERROR: cannot rebuild a carrier that doesn't exist, carrierID:" + carrierID);
				return;
			}
			
			if (ResourceCarriers [carrierID].Pieces.Count == 0) {
				UnityEngine.Debug.Log ("No pieces in carrier, remove carrier now");
				ResourceCarriers.Remove (carrierID);
				return;
			}

			//PieceType rebuiltPieceType = ResourceCarriers [carrierID].Pieces.First ().Type; // get type of carrier be looking at type of first piece in carrier
			List<Piece> rememberPieces = ResourceCarriers [carrierID].Pieces;

			// remove pieces in the old carrier (does also reset tile)
			foreach (Piece piece in rememberPieces) {
						
				Tile onTile = GetTileAt (piece.coord.x, piece.coord.y);
				onTile.RemoveItemInCarrier (piece.carrierID); // remove form tile
			}

			// remove the carrier so it can be rebuild
			Carrier oldCarrier = ResourceCarriers [carrierID];
			ResourceCarriers.Remove (oldCarrier.ID);

			// rebuild carrier
			foreach (Piece piece in rememberPieces) {
				// re-add piece to same tile as previous (will evaluated if pieces is part of with carrierID)
				AddPieceToTile (piece.coord.x, piece.coord.y, oldCarrier.UnitOfContent, piece.IsConnection);
//				if (piece.IsConnection) 
//							// need to use AddConnectionToPieceOnTile to add connection to new created piece, not to the old rememberd piece
//							AddConnectionToPieceOnTile (piece.coord.x, piece.coord.y, rebuiltPieceType);
			}
		}

		

		#endregion
	}

}
