using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Submarine
{
    public class Sub {
        public int lengthOfSub { get; private set; }
        public int heightOfSub { get; private set; }

        Space[,] space;
        Dictionary<int, Room> rooms;

        #region CONSTRUCTOR
        public Sub() {
            lengthOfSub = 40; heightOfSub = 5;
            space = new Space[lengthOfSub, heightOfSub]; // initialize 2D array, still doesn't contain anything
            // instantiate spaces
            for (int x = 0; x < lengthOfSub; x++) {
                for (int y = 0; y < heightOfSub; y++) {
                    space[x, y] = new Space();
                    }
                }

            // set space's outside sub outlines as unavailable
            // upper engine room
            for (int x = 0; x <= 3; x++) { space[x, 1].canContainRoom = false; }
            // lower engine room
            for (int x = 0; x <= 3; x++) { space[x, heightOfSub - 1].canContainRoom = false; }
            // left of Conn tower
            for (int x = 0; x < lengthOfSub / 3 * 2; x++) { space[x, 0].canContainRoom = false; }
            //right of Conn tower
            for (int x = lengthOfSub / 3 * 2 + 5; x < lengthOfSub; x++) { space[x, 0].canContainRoom = false; }

            // instantiate rooms
            rooms = new Dictionary<int, Room>();
            }
        #endregion

        #region Rooms
        private void AddRoom(Room addThisRoom, int roomID) {
            rooms.Add(roomID, addThisRoom);
            }
        private void RemoveRoom(int roomID) {
            rooms.Remove(roomID);
            }
        public Room GetRoom(int roomID) {
            return rooms.ContainsKey(roomID) ? rooms[roomID] : null;
            }
        #endregion

        #region Spaces
        public Space GetSpaceAt(int x, int y) {
            if (x > lengthOfSub || x < 0) { Debug.WriteLine("ERROR: get space x (" + x + ")is outside length (" + lengthOfSub + ") of submarine"); return null; }
            if (y > heightOfSub || y < 0) { Debug.WriteLine("ERROR: get space x (" + y + ")is outside height (" + heightOfSub + ") of submarine"); return null; }
            return space[x, y];
            }

        // add a space to a existing room, or start a new room
        public void AddSpaceToRoom(int x, int y, RoomType type) {
            Space newRoomSpace = space[x, y];
            Space checkSpace;
            // get info of space North
            checkSpace = GetSpaceAt(x, y - 1);
            CheckSameRoomType(x, y, type, newRoomSpace, checkSpace);
            // get info of space East
            checkSpace = GetSpaceAt(x + 1, y);
            CheckSameRoomType(x, y, type, newRoomSpace, checkSpace);
            // get info of space South
            checkSpace = GetSpaceAt(x, y + 1);
            CheckSameRoomType(x, y, type, newRoomSpace, checkSpace);
            // get info of space West
            checkSpace = GetSpaceAt(x - 1, y);
            CheckSameRoomType(x, y, type, newRoomSpace, checkSpace);

            if (newRoomSpace.roomID == 0) {
                // if no neighbor space is part of same room type then start a new room with this space
                Debug.WriteLine("Add space (" + x + "," + y + ") no neighbor space is part of a room, then start a new room with this space");
                int newRoomID = GetNewRoomID();
                newRoomSpace.roomID = newRoomID;        // set roomID in space
                Room newRoom = new Room(type);          // create new room of this room type
                AddRoom(newRoom, newRoomID);            // add new room to submarine
                rooms[newRoomID].AddSpace(newRoomSpace);// add space to room
                }
            }

        private int GetNewRoomID() {
            return rooms.Keys.Count()+1;
            }

        // check is neighbor space is in a room
        private void CheckSameRoomType(int x, int y, RoomType type, Space newRoomSpace, Space checkSpace) {
            if (checkSpace != null) {
                // neighbor space exists
                if (type == GetRoomTypeOfSpace(checkSpace)) {
                    // if neighborer space has same room type as this space
                    if (newRoomSpace.roomID == 0) {
                        // space is not assigned to a room yet, add it tot neighbor room now
                        Debug.WriteLine("Add space (" + x + "," + y + ") to existing room ID:" + checkSpace.roomID);
                        newRoomSpace.roomID = checkSpace.roomID;            // store existing roomID in newRoomSpace
                        rooms[newRoomSpace.roomID].AddSpace(newRoomSpace);  // add space to room
                        }
                    else {// space is already in a room: check if neighborer is in same room
                        if (newRoomSpace.roomID != checkSpace.roomID) {
                            // neighbor space is same room type but another room (id) = merge rooms now
                            Debug.WriteLine("space (" + x + "," + y + ") has roomID " + newRoomSpace.roomID + " neighbor has roomID " + checkSpace.roomID);
                            Debug.WriteLine("Merge rooms now");
                            //TODO: merge rooms
                            }
                        }
                    }
                }
            }

        public RoomType GetRoomTypeOfSpace(Space ofThisSpace) {
            if (ofThisSpace.roomID == 0) {
               // Debug.WriteLine("Room at (" + ofThisSpace.x + "," + ofThisSpace.y + ") is not part of a room");
                return RoomType.Empty;
                }
            else {
                return rooms[ofThisSpace.roomID].type;
                }
            }
        #endregion
    }

    public class Space {
     //   public int x { get; private set; }
     //   public int y { get; private set; }

        public int roomID { get;  set; } = 0;
       // public RoomType roomType { get; private set; } 

        public bool canContainRoom { get; set; } = true;    // used to exclude spaces that are outside the outline of the sub

        //public Space() {
        //    roomType = RoomType.Empty;
        //    }

        
        }
}
