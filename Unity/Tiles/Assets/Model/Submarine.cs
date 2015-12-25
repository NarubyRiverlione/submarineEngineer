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
				Debug.WriteLine ("ERROR: cannot change room because old room (ID:" + oldRoomID + ") doesn't exist");
			}
			else {
				Room newRoom = GetRoom (newRoomID);
				if (newRoom == null) {
					Debug.WriteLine ("ERROR: cannot change room because new room (ID:" + newRoomID + ") doesn't exist");
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
				Debug.WriteLine ("ERROR: cannot rebuild a room that doesn't exist, roomID:" + roomID);
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
				Debug.WriteLine ("ERROR: cannot create/expand a room outside the submarine");
			}
			else {
				if (!newRoomTile.canContainRoom) {
					Debug.WriteLine ("ERROR: cannot create/expand a room at unavailable Tile (" + x + "," + y + ")");
				}
				else {
					if (newRoomTile.RoomID != 0) {
						Debug.WriteLine ("ERROR: already in the " + GetRoomTypeOfTile (newRoomTile) + " room (" + newRoomTile.RoomID + "), remove me first");
					}
					else {
						newRoomTile = CheckNeigboreTiles (newRoomTile, buildRoomOfType);

						if (newRoomTile.RoomID == 0) {
							// if no neighbor Tile is part of same room type then start a new room with this Tile
							Debug.WriteLine ("Adding Tile (" + x + "," + y + "): no neighbor Tile is part of a room, then start a new room with this Tile");

							Room newRoom = Room.CreateRoomOfType (buildRoomOfType, inThisSub: this);     // create new room of this room type
							AddRoomToSubmarine (newRoom);                   // add new room to submarine
							newRoom.AddTile (newRoomTile);                  // add Tile to room
							newRoomTile.RoomID = _nextRoomID - 1;           // set RoomID in Tile
						}
					}
				}
			}
		}

		// remove Tile of room (remove room is it's last Tile)
		public void RemoveTileOfRoom (int x, int y) {
			Tile TileToBeRemoved = GetTileAt (x, y);
			if (TileToBeRemoved == null) {
				Debug.WriteLine ("ERROR cannot remove Tile that doesn't exists");
			}
			else {
				// remove Tile of room
				Room removeFromThisRoom = GetRoom (TileToBeRemoved.RoomID);
				if (removeFromThisRoom == null) {
					Debug.WriteLine ("ERROR Tile doesn't belong to a room");
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



		// check is neighbor Tile is same room type, add or merge. Add wallType
		private void CheckSameRoomType (int x, int y, RoomType wantedRoomType, Tile newRoomTile, Tile checkTile) {
			if (checkTile != null) {
				RoomType roomTypeOfNeighbor = GetRoomTypeOfTile (checkTile);
				// neighbor Tile exists
				if (wantedRoomType == roomTypeOfNeighbor && roomTypeOfNeighbor != RoomType.Empty) {
					// if neighborer Tile has same room type as this Tile
					if (newRoomTile.RoomID == 0) {
						// Tile is not assigned to a room yet, add it tot neighbor room now
						Room addToThisRoom = rooms [checkTile.RoomID];	// room to add this tile too

						Debug.WriteLine ("Add Tile (" + x + "," + y + ") to existing room ID:" + checkTile.RoomID);
						bool oldRoomLayoutValid = addToThisRoom.IsLayoutValid;      // remember layout validation before removing tile
						newRoomTile.RoomID = checkTile.RoomID;                              // store existing RoomID in newRoomTile
						addToThisRoom.AddTile (newRoomTile);                                // add Tile to room
						addToThisRoom.WarnTilesInRoomIfLayoutChanged (oldRoomLayoutValid);	// compare new valid layout 
					
					}
					else {// Tile is already in a room: check if neighborer is in same room
						if (newRoomTile.RoomID != checkTile.RoomID) {
							// neighbor Tile is same room type but another room (id) = merge rooms now
							Debug.WriteLine ("Tile (" + x + "," + y + ") has RoomID " + newRoomTile.RoomID + " neighbor has RoomID " + checkTile.RoomID);
							Debug.WriteLine ("Merge rooms now");
							MergeRooms (newRoomTile.RoomID, checkTile.RoomID);
						}
					}
				}
			}
		}

		public RoomType GetRoomTypeOfTile (Tile ofThisTile) {
			if (ofThisTile.RoomID == 0) {
				// Debug.WriteLine("Room at (" + ofThisTile.x + "," + ofThisTile.y + ") is not part of a room");
				return RoomType.Empty;
			}
			else {
				return rooms [ofThisTile.RoomID].TypeOfRoom;
			}
		}

		public bool IsTilePartOfValidRoomLayout (Tile checkTile) {
			if (checkTile.RoomID == 0) {
				Debug.WriteLine ("Title isn't part of a room, so it cannot check if it's layout is valid");
				return false;
			}
			else {
				return rooms [checkTile.RoomID].IsLayoutValid;
			}
		}

		private Tile CheckNeigboreTiles (Tile aroundThisTile, RoomType buildRoomOfType) {
			Tile checkTile;
			int x = aroundThisTile.X, y = aroundThisTile.Y;
			// get info of Tile North
			checkTile = GetTileAt (x, y - 1);
			CheckSameRoomType (x, y, buildRoomOfType, aroundThisTile, checkTile);
			// get info of Tile East
			checkTile = GetTileAt (x + 1, y);
			CheckSameRoomType (x, y, buildRoomOfType, aroundThisTile, checkTile);
			// get info of Tile South
			checkTile = GetTileAt (x, y + 1);
			CheckSameRoomType (x, y, buildRoomOfType, aroundThisTile, checkTile);
			// get info of Tile West
			checkTile = GetTileAt (x - 1, y);
			CheckSameRoomType (x, y, buildRoomOfType, aroundThisTile, checkTile);

			return aroundThisTile;
		}

		#endregion
	}

	

		

}
