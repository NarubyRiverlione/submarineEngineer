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
            RoomType testRoomType = RoomType.Bridge;

            testRoomCreateOrExpand(testSub, x, y, testRoomType,1,1);
   
            }

        // add spaces N,E,S,W of existing room => room ID should be always 1
        [TestMethod]
        public void AddSpacesToExistingRoom() {
            createCrossRoom(10,2,RoomType.Bridge);

            }

    

        // create rooms (N,E,S,W) around existing room  (other room type = not adding to neighbor)
        [TestMethod]
        public void NewRoomsAroundExistingRoom() {
            Sub testSub = new Sub();
            int x = 10, y = 2;
            RoomType testRoomType = RoomType.Bridge;
            // room 1  = bridge
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1,1);

            // set other room type
            testRoomType = RoomType.Cabin;
            // add space North of existing room => create new room ==> room ID =2
            x = 10;  y = 1;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 2,1);
            // add space East of existing room => add to existing room ==> room ID =3
            x = 11; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 3,1);
            // add space South of existing room => add to existing room ==> room ID =4
            x = 10; y = 3;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 4,1);
            // add space West of existing room => add to existing room ==> room ID =5
            x = 9; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 5,1);




            }

        // merge 2 rooms
        [TestMethod]
        public void Merge2Rooms() {
            Sub testSub = new Sub();
            int x = 10, y = 2;
            RoomType testRoomType = RoomType.Bridge;
            // create room 1
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1,1);
            // create room 2
            x = 8; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 2,1);
            
            // add space between= should merge (adding checks East before West => should add to room id 1 and change room west to 1 also)
            x = 9; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1,3); // size should be now 3
            // check space East: should be still Room ID 1
            Assert.AreEqual( 1, testSpaceExists(testSub, 10, 2).roomID, "space East should be still room ID 1");
            // check space  West should now also be room ID 1
            Assert.AreEqual(1, testSpaceExists(testSub, 8, 2).roomID, "space West should now also be room ID 1");
            // no room with ID 2 should exist any more
            Assert.IsNull(testSub.GetRoom(2), "Room with ID 2 still exists, should be removed");
            }

        // remove space from room
        [TestMethod]
        public void RemoveSpaceFromRoom() {
            int x = 10, y = 2, expectedSize = 5;
            Sub testSub = createCrossRoom(x,y, RoomType.Cabin);

            // remove North
            x = 10;y = 1; expectedSize = 4;
            testSub.RemoveSpaceOfRoom(x,y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.AreEqual(expectedSize, testSub.GetRoom(1).size, "Size of room isn't " + expectedSize);
            // remove East
            x = 11; y = 2; expectedSize = 3;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.AreEqual(expectedSize, testSub.GetRoom(1).size, "Size of room isn't " + expectedSize);
            // remove South
            x = 10; y = 3; expectedSize = 2;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.AreEqual(expectedSize, testSub.GetRoom(1).size, "Size of room isn't " + expectedSize);
            // remove West
            x = 9; y = 2; expectedSize = 1;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.AreEqual(expectedSize, testSub.GetRoom(1).size, "Size of room isn't " + expectedSize);
            // remove last space
            x = 10; y = 2; expectedSize = 0;
            testSub.RemoveSpaceOfRoom(x, y);
            Assert.AreEqual(0, testSub.GetSpaceAt(x, y).roomID, "Space has still a room ID, should be '0'");
            Assert.IsNull(testSub.GetRoom(1), "Room should exist any more, has no spaces left");
            }

        // set room type of a space = create new room or add it to existing neighbor
        private static void testRoomCreateOrExpand(Sub testSub, int x, int y, RoomType testRoomType,int expectRoomID, int expectedRoomSize) {
            testSub.AddSpaceToRoom(x, y, testRoomType);
            Space testSpace = testSpaceExists(testSub, x, y);
            RoomType roomTypeOfTestSpace = testSub.GetRoomTypeOfSpace(testSpace);
            Room testRoom = testSub.GetRoom(expectRoomID);

            Assert.AreEqual(testRoomType, roomTypeOfTestSpace, "Room type of space isn't correct");
            Assert.AreEqual(testSpace.roomID, expectRoomID, "Room ID of new room isn't '"+ expectRoomID + "'");
            Assert.IsNotNull(testRoom,"Room with id  '"+expectRoomID+"' doesn't exist in submarine");
            Assert.AreEqual(expectedRoomSize, testRoom.size,"Expect room size '"+ expectedRoomSize +"' but is '"+testRoom.size+"'");
            }

        // create test room  with 5 spaces : N E W S and center(x,y)
        private static Sub createCrossRoom(int x, int y, RoomType testRoomType) {
            Sub testSub = new Sub();
            
            // first space will create new room = expect room ID = 1
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1, 1);

            // add space North of existing room => add to existing room ==> room ID =1
            x = 10; y = 1;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1, 2);
            // add space East of existing room => add to existing room ==> room ID =1
            x = 11; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1, 3);
            // add space South of existing room => add to existing room ==> room ID =1
            x = 10; y = 3;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1, 4);
            // add space West of existing room => add to existing room ==> room ID =1
            x = 9; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1, 5);
            return testSub;
            }

        private static Space testSpaceExists(Sub testSub, int x, int y) {
            Space testSpace = testSub.GetSpaceAt(x, y);
            Assert.IsNotNull(testSpace, "space doesn't exist at (" + x + "," + y + ")");
            return testSpace;
            }
        }
    }
