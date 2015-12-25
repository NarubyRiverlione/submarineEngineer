﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;

//using System.Threading.Tasks;
//using Newtonsoft.Json;


namespace Submarine.Model {
	/// [JsonObject]
	public class Sub {


		public int lengthOfSub { get; private set; }

		public int heightOfSub { get; private set; }


		public int startOfBridgeTower { get; private set; }

		public int lenghtOfBridgeTower { get; private set; }

		public int heightOfBridgeTower { get; private set; }

		public int smallerTailUpper { get; private set; }

		public int smallerTailLower { get; private set; }

		public int smallerTailLenght { get; private set; }


		Tile[,] _tile;
		
		Dictionary<int, Room> rooms;

		public int AmountOfRooms {
			get {
				if (rooms != null)
					return rooms.Count;
				else
					return 0;
			}
		}

		
		int _nextRoomID = 1;
		// ID 0 = no room assigned to Tile


		//		#region SAVE / LOAD
		//		public void Save(string fileName) {
		//			string jsonString = JsonConvert.SerializeObject(this);
		//			//Console.WriteLine("");Console.WriteLine(jsonString);
		//			// save to file
		//			System.IO.File.WriteAllText(fileName, jsonString);
		//			}
		//
		//		public static Sub Load(string fileName) {
		//			string jsonString = System.IO.File.ReadAllText(fileName);
		//			Sub loadedSub = JsonConvert.DeserializeObject<Sub>(jsonString);
		//			return loadedSub;
		//			}
		//		#endregion


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
			// set tiles around tail and tower as unavailibe for building, create Tower
			CreateSub ();

			SetRoomProperties ();

		}
		// set tiles around tail and tower as unavailibe for building, create Tower
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
			RoomFactory.EngineRoom_Min = 9;
			RoomFactory.EngineRoom_CapPerTile = 1000;
			RoomFactory.EngineRoom_unitOfCap = "pk'";

			RoomFactory.Generator_Min = 6;
			RoomFactory.Generator_CapPerTile = 500;
			RoomFactory.Generator_unitOfCap = "MW";

			RoomFactory.Battery_Min = 8;
			RoomFactory.Battery_CapPerTile = 500;
			RoomFactory.Battery_unitOfCap = "AH";

			RoomFactory.Bridge_Min = 4;
			RoomFactory.Bridge_CapPerTile = 2;
			RoomFactory.Bridge_unitOfCap = "crew";

			RoomFactory.Gallery_Min = 2;
			RoomFactory.Gallery_CapPerTile = 1;
			RoomFactory.Gallery_unitOfCap = "crew";

			RoomFactory.Cabin_Min = 2;
			RoomFactory.Cabin_CapPerTile = 1;
			RoomFactory.Cabin_unitOfCap = "crew";

			RoomFactory.Bunks_Min = 6;
			RoomFactory.Bunks_CapPerTile = 2;
			RoomFactory.Bunks_unitOfCap = "crew";

			RoomFactory.Conn_Min = 6;
			RoomFactory.Conn_CapPerTile = 1;
			RoomFactory.Conn_unitOfCap = "crew";

			RoomFactory.Sonar_Min = 2;
			RoomFactory.Sonar_CapPerTile = 1;
			RoomFactory.Sonar_unitOfCap = "crew";

			RoomFactory.RadioRoom_Min = 2;
			RoomFactory.RadioRoom_CapPerTile = 1;
			RoomFactory.RadioRoom_unitOfCap = "crew";

			RoomFactory.FuelTank_Min = 6;
			RoomFactory.FuelTank_CapPerTile = 1000;
			RoomFactory.FuelTank_unitOfCap = "liters";

			RoomFactory.BalastTank_Min = 4;
			RoomFactory.BalastTank_CapPerTile = 1000;
			RoomFactory.BalastTank_unitOfCap = "liters";

			RoomFactory.StorageRoom_Min = 4;
			RoomFactory.StorageRoom_CapPerTile = 500;
			RoomFactory.StorageRoom_unitOfCap = "food";

			RoomFactory.EscapeHatch_Min = 2;
			RoomFactory.EscapeHatch_CapPerTile = 1;
			RoomFactory.EscapeHatch_unitOfCap = "";

			RoomFactory.TorpedoRoom_Min = 6;
			RoomFactory.TorpedoRoom_CapPerTile = 1;
			RoomFactory.TorpedoRoom_unitOfCap = "torpedos";


		}

		#endregion

		#region Rooms

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
					bool newRoomValid = newRoom.IsLayoutValid;	// remember is room was (in)valid before adding this tile
					foreach (Tile oldRoomTile in oldRoom.TilesInRoom) {
						// change RoomID of each Tile in old room
						oldRoomTile.RoomID = newRoomID;
						// add Tiles to new room (no need to removed them form old room as old room will be destroyed)
						newRoom.AddTile (oldRoomTile);
					}
					newRoom.WarnTilesInRoomIfLayoutChanged (newRoomValid);  // compare new valid layout
																								 
					RemoveRoomFromSubmarine (oldRoomID);   // remove old room form sub

				}
			}
		}

		private void RebuildRoom (int roomID) {
			if (rooms [roomID] == null) {
				UnityEngine.Debug.LogError ("ERROR: cannot rebuild a room that doesn't exist, roomID:" + roomID);
			}
			else {
				RoomType rebuildRoomType = rooms [roomID].TypeOfRoom;
				foreach (Tile checkTile in rooms[roomID].TilesInRoom) {
					checkTile.WallType = 0; // reset wall type
					checkTile.RoomID = 0;   // reset roomID
					AddTileToRoom (checkTile.X, checkTile.Y, rebuildRoomType);
				}
			}
		}

		#endregion

		#region Tiles

		public Tile GetTileAt (int x, int y) {
			if (x > lengthOfSub - 1 || x < 0) {
				Debug.WriteLine ("ERROR: get Tile x (" + x + ")is outside length (" + (lengthOfSub - 1) + ") of submarine");
				return null;
			}
			if (y > heightOfSub - 1 || y < 0) {
				Debug.WriteLine ("ERROR: get Tile x (" + y + ")is outside height (" + (heightOfSub - 1) + ") of submarine");
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
						UnityEngine.Debug.LogError ("ERROR: already in the " + GetRoomTypeOfTile (newRoomTile) + " room (" + newRoomTile.RoomID + "), remove me first");
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
							BuildWallsAroundTile (checkTile);	// rebuild wall of neigbore
						}
						// get info of Tile East
						checkTile = GetTileAt (x + 1, y);
						foundSameRoomType = CheckNeigborSameRoomType (x, y, buildRoomOfType, newRoomTile, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 2; // add wall type for East
							BuildWallsAroundTile (checkTile);// rebuild wall of neigbore
						}
						// get info of Tile South
						checkTile = GetTileAt (x, y - 1);
						foundSameRoomType = CheckNeigborSameRoomType (x, y, buildRoomOfType, newRoomTile, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 4; // add wall type for South
							BuildWallsAroundTile (checkTile);// rebuild wall of neigbore
						}
						// get info of Tile West
						checkTile = GetTileAt (x - 1, y);
						foundSameRoomType = CheckNeigborSameRoomType (x, y, buildRoomOfType, newRoomTile, checkTile);
						if (foundSameRoomType) {
							AddToOrMergeRooms (newRoomTile, checkTile);
							newRoomTile.WallType += 8; // add wall type for West
							BuildWallsAroundTile (checkTile);// rebuild wall of neigbore
						}

						UnityEngine.Debug.Log ("New tile has wall type: " + newRoomTile.WallType);

						if (newRoomTile.RoomID == 0) {
							// if no neighbor Tile is part of same room type then start a new room with this Tile
							Room newRoom = Room.CreateRoomOfType (buildRoomOfType, inThisSub: this);     // create new room of this room type
							AddRoomToSubmarine (newRoom);                   // add new room to submarine
							newRoom.AddTile (newRoomTile);                  // add Tile to room
							newRoomTile.RoomID = _nextRoomID - 1;           // set RoomID in Tile
							UnityEngine.Debug.Log ("Added tile (" + x + "," + y + "): no neighbor Tile is part of a " + buildRoomOfType + ", so started a new room " + newRoomTile.RoomID + ", wall type is " + newRoomTile.WallType);

						}
					}
				}
			}
		}

		// remove Tile of room (remove room is it's last Tile)
		public void RemoveTileOfRoom (int x, int y) {
			Tile TileToBeRemoved = GetTileAt (x, y);
			if (TileToBeRemoved == null) {
				UnityEngine.Debug.Log ("ERROR cannot remove Tile that doesn't exists");
			}
			else {
				// remove Tile of room
				Room removeFromThisRoom = GetRoom (TileToBeRemoved.RoomID);
				if (removeFromThisRoom == null) {
					UnityEngine.Debug.Log ("ERROR Tile doesn't belong to a room");
				}
				else {
					bool oldRoomLayoutValid = removeFromThisRoom.IsLayoutValid;	            // remember layout validation before removing tile
					removeFromThisRoom.RemoveTile (TileToBeRemoved);
					RebuildRoom (TileToBeRemoved.RoomID);    // rebuild room to be sure the wall type and layout is still ok
					removeFromThisRoom.WarnTilesInRoomIfLayoutChanged (oldRoomLayoutValid);	// compare new valid layout 
					
					// check if it was the last Tile of the room
					if (removeFromThisRoom.Size == 0) {
						// destroy room
						RemoveRoomFromSubmarine (TileToBeRemoved.RoomID);
					}
					// set RoomID of Tile to 0
					TileToBeRemoved.RoomID = 0;
			
				}
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

			UnityEngine.Debug.Log ("(re)builded walls around (" + x + "," + y + ") wall type is now" + checkAroundThisTile.WallType);

		}

		// check is neighbor Tile is same room type, add or merge. Return if same room type is found
		private bool CheckNeigborSameRoomType (int x, int y, RoomType wantedRoomType, Tile newRoomTile, Tile checkTile) {
			if (checkTile != null) {
				RoomType roomTypeOfNeighbor = GetRoomTypeOfTile (checkTile);
				// neighbor Tile exists
				if (wantedRoomType == roomTypeOfNeighbor && roomTypeOfNeighbor != RoomType.Empty)
					return true;
			}
			return false; // all other cases return not same roomtype was found
		}

		private void AddToOrMergeRooms (Tile newRoomTile, Tile neigboreTile) {
			// if neighborer Tile has same room type as this Tile
			if (newRoomTile.RoomID == 0) {
				// Tile is not assigned to a room yet, add it tot neighbor room now
				Room addToThisRoom = rooms [neigboreTile.RoomID];	// room to add this tile too

				UnityEngine.Debug.Log ("Add Tile (" + newRoomTile.X + "," + newRoomTile.Y + ") to existing room ID:" + neigboreTile.RoomID);
				bool oldRoomLayoutValid = addToThisRoom.IsLayoutValid;      // remember layout validation before removing tile
				newRoomTile.RoomID = neigboreTile.RoomID;                              // store existing RoomID in newRoomTile
				addToThisRoom.AddTile (newRoomTile);                                // add Tile to room
				addToThisRoom.WarnTilesInRoomIfLayoutChanged (oldRoomLayoutValid);	// compare new valid layout 
				
			}
			else {// Tile is already in a room: check if neighborer is in same room
				if (newRoomTile.RoomID != neigboreTile.RoomID) {
					// neighbor Tile is same room type but another room (id) = merge rooms now
					UnityEngine.Debug.Log ("Tile (" + newRoomTile.X + "," + newRoomTile.Y + ") has RoomID " + newRoomTile.RoomID + " neighbor has RoomID " + neigboreTile.RoomID);
					UnityEngine.Debug.Log ("Merge rooms now");
					MergeRooms (newRoomTile.RoomID, neigboreTile.RoomID);
	
				}
			}
		}


		public RoomType GetRoomTypeOfTile (Tile ofThisTile) {
			if (ofThisTile.RoomID == 0) {
				// UnityEngine.Debug.WriteLine("Room at (" + ofThisTile.x + "," + ofThisTile.y + ") is not part of a room");
				return RoomType.Empty;
			}
			else {
				return rooms [ofThisTile.RoomID].TypeOfRoom;
			}
		}

		public bool IsTilePartOfValidRoomLayout (Tile checkTile) {
			if (checkTile.RoomID == 0) {
				UnityEngine.Debug.Log ("Title isn't part of a room, so it cannot check if it's layout is valid");
				return false;
			}
			else {
				return rooms [checkTile.RoomID].IsLayoutValid;
			}
		}



		#endregion
	}

	

		

}
