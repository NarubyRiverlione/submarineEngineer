
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


		Tile[,] _Tile;
		
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
			startOfBridgeTower = 21;

			smallerTailUpper = 1;
			smallerTailLower = 1;
			smallerTailLenght = 4;

			// initialize 2D array, still doesn't contain anything
			_Tile = new Tile[lengthOfSub, heightOfSub]; 
			// instantiate rooms
			rooms = new Dictionary<int, Room> ();

			// instantiate Tiles
			for (int x = 0; x < lengthOfSub; x++) {
				for (int y = 0; y < heightOfSub; y++) {
					_Tile [x, y] = new Tile (x, y);
				}
			}

			// set Tile's outside sub outlines as unavailable
			// upper smaller tail section
			for (int x = 0; x <= smallerTailLenght; x++) {
				for (int y = heightOfSub - heightOfBridgeTower - smallerTailUpper; y < heightOfSub - heightOfBridgeTower; y++) {
					_Tile [x, y].canContainRoom = false;
				}
			}
			// lower  smaller tail section 
			for (int x = 0; x <= smallerTailLenght; x++) {
				for (int y = 0; y < smallerTailLower; y++) {
					_Tile [x, y].canContainRoom = false;
				}
			}

			

			// left of Bridge tower
			for (int x = 0; x < startOfBridgeTower; x++) {
				for (int y = heightOfSub - heightOfBridgeTower; y < heightOfSub; y++) {
					_Tile [x, y].canContainRoom = false;
				}
			}
			//right of Bridge tower
			for (int x = startOfBridgeTower + lenghtOfBridgeTower; x < lengthOfSub; x++) {
				for (int y = heightOfSub - heightOfBridgeTower; y < heightOfSub; y++) {
					_Tile [x, y].canContainRoom = false;
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

		public bool IsRoomValid (int RoomID) {
			Room checkRoom = GetRoom (RoomID);
			if (checkRoom != null)
				return checkRoom.IsLayoutValid;
			else
				return false;
		}

		public void MergeRooms (int newRoomID, int oldRoomID) {
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
					foreach (Tile oldRoomTile in oldRoom.TilesInRoom) {
						// change RoomID of each Tile in old room
						oldRoomTile.RoomID = newRoomID;
						// add Tiles to new room (no need to removed them form old room as old room will be destroyed)
						newRoom.AddTile (oldRoomTile);
					}
					// remove old room form sub
					RemoveRoomFromSubmarine (oldRoomID);
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
			return _Tile [x, y];
		}

		// add a Tile to a existing room, or start a new room
		public void AddTileToRoom (int x, int y, RoomType type) {
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
						Tile checkTile;
						// get info of Tile North
						checkTile = GetTileAt (x, y - 1);
						CheckSameRoomType (x, y, type, newRoomTile, checkTile);
						// get info of Tile East
						checkTile = GetTileAt (x + 1, y);
						CheckSameRoomType (x, y, type, newRoomTile, checkTile);
						// get info of Tile South
						checkTile = GetTileAt (x, y + 1);
						CheckSameRoomType (x, y, type, newRoomTile, checkTile);
						// get info of Tile West
						checkTile = GetTileAt (x - 1, y);
						CheckSameRoomType (x, y, type, newRoomTile, checkTile);

						if (newRoomTile.RoomID == 0) {
							// if no neighbor Tile is part of same room type then start a new room with this Tile
							Debug.WriteLine ("Add Tile (" + x + "," + y + ") no neighbor Tile is part of a room, then start a new room with this Tile");

							Room newRoom = Room.CreateRoomOfType (type, inThisSub: this);     // create new room of this room type
							AddRoomToSubmarine (newRoom);                    // add new room to submarine
							newRoom.AddTile (newRoomTile);                 // add Tile to room
							newRoomTile.RoomID = _nextRoomID - 1;            // set RoomID in Tile
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
				Room ofRoom = GetRoom (TileToBeRemoved.RoomID);
				if (ofRoom == null) {
					Debug.WriteLine ("ERROR Tile doesn't belong to a room");
				}
				else {
					ofRoom.RemoveTile (TileToBeRemoved);
					// check if it was the last Tile of the room
					if (ofRoom.Size == 0) {
						// destroy room
						RemoveRoomFromSubmarine (TileToBeRemoved.RoomID);
					}
					// set RoomID of Tile to 0
					TileToBeRemoved.RoomID = 0;
				}
			}
		}



		// check is neighbor Tile is in a room
		private void CheckSameRoomType (int x, int y, RoomType wantedRoomType, Tile newRoomTile, Tile checkTile) {
			if (checkTile != null) {
				RoomType roomTypeOfNeighbor = GetRoomTypeOfTile (checkTile);
				// neighbor Tile exists
				if (wantedRoomType == roomTypeOfNeighbor && roomTypeOfNeighbor != RoomType.Empty) {
					// if neighborer Tile has same room type as this Tile
					if (newRoomTile.RoomID == 0) {
						// Tile is not assigned to a room yet, add it tot neighbor room now
						Debug.WriteLine ("Add Tile (" + x + "," + y + ") to existing room ID:" + checkTile.RoomID);
						newRoomTile.RoomID = checkTile.RoomID;            // store existing RoomID in newRoomTile
						rooms [newRoomTile.RoomID].AddTile (newRoomTile);  // add Tile to room
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

		#endregion
	}

	

		

}
