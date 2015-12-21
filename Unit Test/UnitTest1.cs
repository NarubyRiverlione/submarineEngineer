using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Submarine;

namespace Unit_Test {
    [TestClass]
    public sealed class RoomTests {
        // create sub
        [TestMethod]
        public void IsSubCreated() {
            Sub testSub = new Sub();
            Assert.IsNotNull(testSub);
            }

        // create new room
        [TestMethod]
        public void CreateNewRoom() {
            Sub testSub = new Sub();
            int x = 10, y = 2;
            RoomType testRoomType = RoomType.Bunks;

            testRoomCreateOrExpand(testSub, x, y, testRoomType,1);

         
            }

        // try creating new rooms outside sub
        [TestMethod]
        public void TryCreatingRoomsOutsideSub() {
            Sub testSub = new Sub();
            int x = testSub.lengthOfSub, y = testSub.heightOfSub/2;
            RoomType testRoomType = RoomType.Bunks;

            int roomCountAtCreation = testSub.AmountOfRooms;

            testSub.AddSpaceToRoom(x, y, testRoomType);
            Assert.AreEqual(roomCountAtCreation,testSub.AmountOfRooms,"No room should be added as it shouldn't exist outside submarine");

            x = -2;
            testSub.AddSpaceToRoom(x, y, testRoomType);
            Assert.AreEqual(roomCountAtCreation, testSub.AmountOfRooms, "No room should be added as it shouldn't exist outside submarine");

            x = testSub.lengthOfSub / 2;
            y = -1;
            testSub.AddSpaceToRoom(x, y, testRoomType);
            Assert.AreEqual(roomCountAtCreation, testSub.AmountOfRooms, "No room should be added as it shouldn't exist outside submarine");

            y = testSub.heightOfSub;
            testSub.AddSpaceToRoom(x, y, testRoomType);
            Assert.AreEqual(roomCountAtCreation, testSub.AmountOfRooms, "No room should be added as it shouldn't exist outside submarine");
            }

        // try creating new rooms in unavailable space
        [TestMethod]
        public void TryCreatingRoomInUnavailableSpace() {
            Sub testSub = new Sub();
            int roomCountAtCreation = testSub.AmountOfRooms;
            int x = 0, y = 0;
            RoomType testRoomType = RoomType.Bunks;

            // make sure test space is set unavailable
            Assert.AreEqual(false, testSub.GetSpaceAt(x, y).canContainRoom);

            testSub.AddSpaceToRoom(x, y, testRoomType);
            Assert.AreEqual(roomCountAtCreation, testSub.AmountOfRooms, "No room should be added");
            }

        // add spaces N,E,S,W of existing room => room ID should be always 1
        [TestMethod]
        public void AddSpacesToExistingRoom() {
            createCrossRoom(RoomType.Bunks);

            }

        // create rooms (N,E,S,W) around existing room  (other room type = not adding to neighbor)
        [TestMethod]
        public void NewRoomsAroundExistingRoom() {
            Sub testSub = new Sub();
            int start_x = testSub.lengthOfSub / 2;
            int start_y = testSub.heightOfSub / 2;
            RoomType testRoomType = RoomType.Bunks;
            int startCountOfRooms = testSub.AmountOfRooms;

            // room 1  = Bunks
            testRoomCreateOrExpand(testSub, start_x, start_y, testRoomType, 1);
            Assert.AreEqual(startCountOfRooms + 1, testSub.AmountOfRooms);

            // set other room type
            testRoomType = RoomType.Cabin;
            // add space North of existing room => create new room
            int x = start_x,  y = start_y-1;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);
            Assert.AreEqual(startCountOfRooms + 2, testSub.AmountOfRooms);
            // add space East of existing room => add to existing room 
            x = start_x+1;  y = start_y;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);
            Assert.AreEqual(startCountOfRooms + 3, testSub.AmountOfRooms);
            // add space South of existing room => add to existing room 
            x = start_x; y = start_y +1;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);
            Assert.AreEqual(startCountOfRooms + 4, testSub.AmountOfRooms);
            // add space West of existing room => add to existing room
            x = start_x-1; y = start_y ;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);
            Assert.AreEqual(startCountOfRooms + 5, testSub.AmountOfRooms);




            }

        // merge 2 rooms
        [TestMethod]
        public void Merge2Rooms() {
            Sub testSub = new Sub();
            int start_x = testSub.lengthOfSub / 2;
            int start_y = testSub.heightOfSub / 2;
            RoomType testRoomType = RoomType.Bunks
                ;
            // create room East
            testRoomCreateOrExpand(testSub, start_x+1, start_y, testRoomType, 1);
            int roomID_East = testSub.GetSpaceAt(start_x + 1, start_y).roomID;
            // create room West
            testRoomCreateOrExpand(testSub, start_x-1, start_y, testRoomType, 1);
            int roomID_West = testSub.GetSpaceAt(start_x - 1, start_y).roomID;

            // add space between= should merge (adding checks East before West => should add to room id 1 and change room west to 1 also)
            testRoomCreateOrExpand(testSub, start_x, start_y, testRoomType, 3); // size should be now 3
          
            // check space East: should be still Room ID 1
            Assert.AreEqual( roomID_East, testSpaceExists(testSub, start_x+1,start_y).roomID, "space East should be still room ID east");
            // check space  West should now also be room ID 1
            Assert.AreEqual(roomID_East, testSpaceExists(testSub, start_x-1, start_y).roomID, "space West should now also be room ID east");
            // no room with ID 2 should exist any more
            Assert.IsNull(testSub.GetRoom(roomID_West), "Room ID west still exists, should be removed");
            }

        // remove space from room
        [TestMethod]
        public void RemoveSpaceFromRoom() {
       
            Sub testSub = createCrossRoom(RoomType.Cabin);
            int start_x = testSub.lengthOfSub / 2;
            int start_y = testSub.heightOfSub / 2;
            int expectedSize;
            int roomID = testSub.GetSpaceAt(start_x, start_y).roomID;

            // remove North
            int x = start_x, y = start_y-1; expectedSize = 4;
            testSub.RemoveSpaceOfRoom(x,y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.AreEqual(expectedSize, testSub.GetRoom(roomID).Size, "Size of room isn't " + expectedSize);
            // remove East
            x = start_x + 1; y = start_y ; expectedSize = 3;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.AreEqual(expectedSize, testSub.GetRoom(roomID).Size, "Size of room isn't " + expectedSize);
            // remove South
            x = start_x; y = start_y+ 1; expectedSize = 2;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.AreEqual(expectedSize, testSub.GetRoom(roomID).Size, "Size of room isn't " + expectedSize);
            // remove West
            x = start_x-1; y = start_y; expectedSize = 1;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.AreEqual(expectedSize, testSub.GetRoom(roomID).Size, "Size of room isn't " + expectedSize);
            // remove last space
            x = start_x; y = start_y; expectedSize = 0;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.IsNull(testSub.GetRoom(roomID), "Room should exist any more, has no spaces left");
            }

        // remove space from not-existing room
        [TestMethod]
        public void RemoveFormNotExistingRoom() {
            Sub testSub = new Sub();
            int x = 7, y = 3;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            }



        // create valid FuelTank
        [TestMethod]
        public void CreateValidSized_FuelTank() {
            CreateValidSizedRoom(RoomType.FuelTank);
            }
        // create valid Cabin
        [TestMethod]
        public void CreateValidSized_fCabin() {
            CreateValidSizedRoom(RoomType.Cabin);
            }
        // create valid Bunks
        [TestMethod]
        public void CreateValidSized_Bunks() {
            CreateValidSizedRoom(RoomType.Bunks);
            }
        // create valid Conn (should be below Bridge)
        [TestMethod]
        public void CreateValid_Conn() {
            CreateValidSizedRoom(RoomType.Bunks);
            }

        // add spaces until room size is valid (will fail if there are extra requirements)
        private void CreateValidSizedRoom(RoomType testRoomType) {
            Sub testSub = new Sub();
            int x = 10, y = 1;
      
            testSub.AddSpaceToRoom(x, y, testRoomType);
            Room testRoom = testSub.GetRoom(1);
            Assert.IsNotNull(testRoom);

            // add spaces unit there are enough
            while (testRoom.Size < testRoom.MinimimValidSize) {
                Assert.IsFalse(testRoom.IsLayoutValid);     // room is still to small, should return false
                x++; // new coordinate for next space
                testSub.AddSpaceToRoom(x, y, testRoomType); // add a space
                }
            // size is now ok 
            Assert.IsTrue(testRoom.IsLayoutValid); 
            }


        // set room type of a space = create new room or add it to existing neighbor
        private static void testRoomCreateOrExpand(Sub testSub, int x, int y, RoomType testRoomType, int expectedRoomSize) {
            testSub.AddSpaceToRoom(x, y, testRoomType);
            Space testSpace = testSpaceExists(testSub, x, y);
            RoomType roomTypeOfTestSpace = testSub.GetRoomTypeOfSpace(testSpace);
            Room testRoom = testSub.GetRoom(testSpace.roomID);

            Assert.AreEqual(testRoomType, roomTypeOfTestSpace, "Room type of space isn't correct");
          //  Assert.AreEqual(testSpace.roomID, expectRoomID, "Room ID of new room isn't '"+ expectRoomID + "'");
            Assert.IsNotNull(testRoom,"Room doesn't exist in submarine");
            Assert.AreEqual(expectedRoomSize, testRoom.Size,"Expect room size '"+ expectedRoomSize +"' but is '"+testRoom.Size+"'");
            }

        // create test room  with 5 spaces : N E W S and center(x,y)
        private static Sub createCrossRoom(RoomType testRoomType) {
            Sub testSub = new Sub();
            int start_x = testSub.lengthOfSub / 2;
            int start_y = testSub.heightOfSub / 2;

            // make sure were at a start position that has enough spaces around inside of the sub
            Assert.IsTrue(start_x - 1 > 0, "Start position X to far to the left to create test room");
            Assert.IsTrue(start_x + 1 < testSub.lengthOfSub - 1, "Start position X to far to the right to create test room");
            Assert.IsTrue(start_y - 1 > 0, "Start position Y to far to the top to create test room");
            Assert.IsTrue(start_y + 1 < testSub.heightOfSub - 1, "Start position Y to far to the bottom to create test room");
            // first space will create new room 
            int x = start_x, y = start_y;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);
            int roomIDofStartRoom = testSub.GetSpaceAt(x, y).roomID;

            // add space North of existing room => add to existing room 
            x = start_x ; y = start_y-1;
            testRoomCreateOrExpand(testSub, x, y, testRoomType,  2);
            Assert.AreEqual(roomIDofStartRoom, testSub.GetSpaceAt(x, y).roomID, "Room ID of added space isn't equal to neighbor room = didn't add space to existing room");
            // add space East of existing room => add to existing room 
            x = start_x+1; y = start_y;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 3);
            Assert.AreEqual(roomIDofStartRoom, testSub.GetSpaceAt(x, y).roomID, "Room ID of added space isn't equal to neighbor room = didn't add space to existing room");
            // add space South of existing room => add to existing room
            x = start_x ; y = start_y+1;
            testRoomCreateOrExpand(testSub, x, y, testRoomType,  4);
            Assert.AreEqual(roomIDofStartRoom, testSub.GetSpaceAt(x, y).roomID, "Room ID of added space isn't equal to neighbor room = didn't add space to existing room");
            // add space West of existing room => add to existing room
            x = start_x -1; y = start_y;
            testRoomCreateOrExpand(testSub, x, y, testRoomType,  5);
            Assert.AreEqual(roomIDofStartRoom, testSub.GetSpaceAt(x, y).roomID, "Room ID of added space isn't equal to neighbor room = didn't add space to existing room");

            return testSub;
            }

        private static Space testSpaceExists(Sub testSub, int x, int y) {
            Space testSpace = testSub.GetSpaceAt(x, y);
            Assert.IsNotNull(testSpace, "space doesn't exist at (" + x + "," + y + ")");
            return testSpace;
            }
        }
    }
