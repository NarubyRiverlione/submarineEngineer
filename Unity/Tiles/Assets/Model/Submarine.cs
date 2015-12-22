
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
		//[JsonProperty]
		public int lengthOfSub { get; private set; }
		//[JsonProperty]
		public int heightOfSub { get; private set; }
		//[JsonProperty]
		public int lenghtOfBridgeTower { get; private set; }
		//[JsonProperty]
		public int heightOfBridgeTower { get; private set; }

		public int smallerTail { get; private set; }

		//[JsonProperty]
		Tile[,] _space;
		//[JsonProperty]
		Dictionary<int, Room> rooms;

		public int AmountOfRooms {
			get {
				if (rooms != null)
					return rooms.Count;
				else
					return 0;
			}
		}

		//[JsonProperty]
		int _nextRoomID = 1;
		// ID 0 = no room assigned to space


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
			lengthOfSub = 20;
			heightOfSub = 6;
			lenghtOfBridgeTower = 4;
			heightOfBridgeTower = 2;
			smallerTail = 1;

			// initialize 2D array, still doesn't contain anything
			_space = new Tile[lengthOfSub, heightOfSub]; 
			// instantiate rooms
			rooms = new Dictionary<int, Room> ();

			// instantiate spaces
			for (int x = 0; x < lengthOfSub; x++) {
				for (int y = 0; y < heightOfSub; y++) {
					_space [x, y] = new Tile (x, y);
				}
			}

			// set space's outside sub outlines as unavailable
			// upper smaller tail section
			for (int x = 0; x <= lengthOfSub / 10; x++) {
				for (int y = heightOfSub - heightOfBridgeTower - smallerTail; y < heightOfSub - heightOfBridgeTower; y++) {
					_space [x, y].canContainRoom = false;
				}
			}
			// lower  smaller tail section 
			for (int x = 0; x <= lengthOfSub / 10; x++) {
				for (int y = 0; y < smallerTail; y++) {
					_space [x, y].canContainRoom = false;
				}
			}

			// left of Bridge tower
			for (int x = 0; x < lengthOfSub / 3 * 2; x++) {
				for (int y = heightOfSub - heightOfBridgeTower; y < heightOfSub; y++) {
					_space [x, y].canContainRoom = false;
				}
			}
			//right of Bridge tower
			for (int x = lengthOfSub / 3 * 2 + lenghtOfBridgeTower; x < lengthOfSub; x++) {
				for (int y = heightOfSub - heightOfBridgeTower; y < heightOfSub; y++) {
					_space [x, y].canContainRoom = false;
				}
			}

			// add Bridge Tower
			for (int x = lengthOfSub / 3 * 2; x < lengthOfSub / 3 * 2 + lenghtOfBridgeTower; x++) {
				for (int y = heightOfSub - heightOfBridgeTower; y < heightOfSub; y++) {
					AddSpaceToRoom (x, y, RoomType.Bridge);
				}
			}
		}

		#endregion

		#region Rooms

		private void AddRoomToSubmarine (Room addThisRoom) {
			rooms.Add (_nextRoomID, addThisRoom);
			_nextRoomID++;
		}

		private void RemoveRoomFromSubmarine (int roomID) {
			rooms.Remove (roomID);
		}

		public Room GetRoom (int roomID) {
			return rooms.ContainsKey (roomID) ? rooms [roomID] : null;
		}

		public bool IsRoomValid (int roomID) {
			Room checkRoom = GetRoom (roomID);
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
					foreach (Tile oldRoomSpace in oldRoom.spacesInRoom) {
						// change roomID of each space in old room
						oldRoomSpace.roomID = newRoomID;
						// add spaces to new room (no need to removed them form old room as old room will be destroyed)
						newRoom.AddSpace (oldRoomSpace);
					}
					// remove old room form sub
					RemoveRoomFromSubmarine (oldRoomID);
				}
			}
		}



		#endregion


		#region Spaces

		public Tile GetSpaceAt (int x, int y) {
			if (x > lengthOfSub - 1 || x < 0) {
				Debug.WriteLine ("ERROR: get space x (" + x + ")is outside length (" + (lengthOfSub - 1) + ") of submarine");
				return null;
			}
			if (y > heightOfSub - 1 || y < 0) {
				Debug.WriteLine ("ERROR: get space x (" + y + ")is outside height (" + (heightOfSub - 1) + ") of submarine");
				return null;
			}
			return _space [x, y];
		}

		// add a space to a existing room, or start a new room
		public void AddSpaceToRoom (int x, int y, RoomType type) {
			Tile newRoomSpace = GetSpaceAt (x, y);
			if (newRoomSpace == null) {
				Debug.WriteLine ("ERROR: cannot create/expand a room outside the submarine");
			}
			else {
				if (!newRoomSpace.canContainRoom) {
					Debug.WriteLine ("ERROR: cannot create/expand a room at unavailable space (" + x + "," + y + ")");
				}
				else {
					if (newRoomSpace.roomID != 0) {
						Debug.WriteLine ("ERROR: already in the " + GetRoomTypeOfSpace (newRoomSpace) + " room (" + newRoomSpace.roomID + "), remove me first");
					}
					else {
						Tile checkSpace;
						// get info of space North
						checkSpace = GetSpaceAt (x, y - 1);
						CheckSameRoomType (x, y, type, newRoomSpace, checkSpace);
						// get info of space East
						checkSpace = GetSpaceAt (x + 1, y);
						CheckSameRoomType (x, y, type, newRoomSpace, checkSpace);
						// get info of space South
						checkSpace = GetSpaceAt (x, y + 1);
						CheckSameRoomType (x, y, type, newRoomSpace, checkSpace);
						// get info of space West
						checkSpace = GetSpaceAt (x - 1, y);
						CheckSameRoomType (x, y, type, newRoomSpace, checkSpace);

						if (newRoomSpace.roomID == 0) {
							// if no neighbor space is part of same room type then start a new room with this space
							Debug.WriteLine ("Add space (" + x + "," + y + ") no neighbor space is part of a room, then start a new room with this space");

							Room newRoom = Room.CreateRoomOfType (type, inThisSub: this);     // create new room of this room type
							AddRoomToSubmarine (newRoom);                    // add new room to submarine
							newRoom.AddSpace (newRoomSpace);                 // add space to room
							newRoomSpace.roomID = _nextRoomID - 1;            // set roomID in space
						}
					}
				}
			}
		}
		// remove space of room (remove room is it's last space)
		public void RemoveSpaceOfRoom (int x, int y) {
			Tile spaceToBeRemoved = GetSpaceAt (x, y);
			if (spaceToBeRemoved == null) {
				Debug.WriteLine ("ERROR cannot remove space that doesn't exists");
			}
			else {
				// remove space of room
				Room ofRoom = GetRoom (spaceToBeRemoved.roomID);
				if (ofRoom == null) {
					Debug.WriteLine ("ERROR space doesn't belong to a room");
				}
				else {
					ofRoom.RemoveSpace (spaceToBeRemoved);
					// check if it was the last space of the room
					if (ofRoom.Size == 0) {
						// destroy room
						RemoveRoomFromSubmarine (spaceToBeRemoved.roomID);
					}
					// set roomID of space to 0
					spaceToBeRemoved.roomID = 0;
				}
			}
		}



		// check is neighbor space is in a room
		private void CheckSameRoomType (int x, int y, RoomType wantedRoomType, Tile newRoomSpace, Tile checkSpace) {
			if (checkSpace != null) {
				RoomType roomTypeOfNeighbor = GetRoomTypeOfSpace (checkSpace);
				// neighbor space exists
				if (wantedRoomType == roomTypeOfNeighbor && roomTypeOfNeighbor != RoomType.Empty) {
					// if neighborer space has same room type as this space
					if (newRoomSpace.roomID == 0) {
						// space is not assigned to a room yet, add it tot neighbor room now
						Debug.WriteLine ("Add space (" + x + "," + y + ") to existing room ID:" + checkSpace.roomID);
						newRoomSpace.roomID = checkSpace.roomID;            // store existing roomID in newRoomSpace
						rooms [newRoomSpace.roomID].AddSpace (newRoomSpace);  // add space to room
					}
					else {// space is already in a room: check if neighborer is in same room
						if (newRoomSpace.roomID != checkSpace.roomID) {
							// neighbor space is same room type but another room (id) = merge rooms now
							Debug.WriteLine ("space (" + x + "," + y + ") has roomID " + newRoomSpace.roomID + " neighbor has roomID " + checkSpace.roomID);
							Debug.WriteLine ("Merge rooms now");
							MergeRooms (newRoomSpace.roomID, checkSpace.roomID);
						}
					}
				}
			}
		}

		public RoomType GetRoomTypeOfSpace (Tile ofThisSpace) {
			if (ofThisSpace.roomID == 0) {
				// Debug.WriteLine("Room at (" + ofThisSpace.x + "," + ofThisSpace.y + ") is not part of a room");
				return RoomType.Empty;
			}
			else {
				return rooms [ofThisSpace.roomID].TypeOfRoom;
			}
		}

		#endregion
	}

	

		

}
