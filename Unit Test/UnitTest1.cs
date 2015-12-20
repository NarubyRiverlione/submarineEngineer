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

        // create room
        [TestMethod]
        public void CreateNewRoom() {
            Sub testSub = new Sub();
            int x = 10, y = 2;
            RoomType testRoomType = RoomType.Bridge;

            testRoomCreateOrExpand(testSub, x, y, testRoomType,1);
            }

        // add spaces N,E,S,W of existing room => room ID should be always 1
        [TestMethod]
        public void AddSpacesToExistingRoom() {
            Sub testSub = new Sub();
            int x = 10, y = 2;
            RoomType testRoomType = RoomType.Bridge;
            // first space will create new room = expect room ID = 1
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);

            // add space North of existing room => add to existing room ==> room ID =1
            x = 10;  y = 1;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);
            // add space East of existing room => add to existing room ==> room ID =1
            x = 11; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);
            // add space South of existing room => add to existing room ==> room ID =1
            x = 10; y = 3;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);
            // add space West of existing room => add to existing room ==> room ID =1
            x = 9; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);

            }

        // create rooms (N,E,S,W) around existing room  (other room type = not adding to neighbor)
        [TestMethod]
        public void NewRoomsAroundExistingRoom() {
            Sub testSub = new Sub();
            int x = 10, y = 2;
            RoomType testRoomType = RoomType.Bridge;
            // room 1  = bridge
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 1);

            // set other room type
            testRoomType = RoomType.Cabin;
            // add space North of existing room => create new room ==> room ID =2
            x = 10;  y = 1;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 2);
            // add space East of existing room => add to existing room ==> room ID =3
            x = 11; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 3);
            // add space South of existing room => add to existing room ==> room ID =4
            x = 10; y = 3;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 4);
            // add space West of existing room => add to existing room ==> room ID =5
            x = 9; y = 2;
            testRoomCreateOrExpand(testSub, x, y, testRoomType, 5);




            }

        // set room type of a space = create new room or add it to existing neighbor
        private static void testRoomCreateOrExpand(Sub testSub, int x, int y, RoomType testRoomType,int expectRoomID) {
            testSub.AddSpaceToRoom(x, y, testRoomType);
            Space testSpace = testSpaceExists(testSub, x, y);
            RoomType roomTypeOfTestSpace = testSub.GetRoomTypeOfSpace(testSpace);

            Assert.AreEqual(testRoomType, roomTypeOfTestSpace, "Room type of space isn't correct");
            Assert.AreEqual(testSpace.roomID, expectRoomID, "Room ID of new room isn't '"+ expectRoomID + "'");
            }

        private static Space testSpaceExists(Sub testSub, int x, int y) {
            Space testSpace = testSub.GetSpaceAt(x, y);
            Assert.IsNotNull(testSpace, "space doesn't exist at (" + x + "," + y + ")");
            return testSpace;
            }
        }
    }
