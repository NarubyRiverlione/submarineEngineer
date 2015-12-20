using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Submarine;

namespace Console_App {
    class Program {
        static void Main(string[] args) {

            // create submarine (with all spaces)
            Sub mySub = new Sub();
            // tuple of x,y coordinates
            Tuple<int, int> coordinate;

            // show spaces
            ShowSubmarine(mySub);

            // get command
            char command='0' ;
            while (! command.Equals('q')) {
                Console.Write("Your command (Add/Remove/Quit)? ");
                command = Console.ReadKey().KeyChar;

                switch (command) {
                    case 'a':
                        // get coordinates
                        Console.WriteLine("Create/expand a room at coordinates :");
                        coordinate= ReadCoordinates();
                        // get room type to add
                        RoomType addRoomType = ReadRoomType();

                        Console.WriteLine("ADDING "+ addRoomType.ToString() + " to x=" + coordinate.Item1 + ", y=" + coordinate.Item2);

                        mySub.AddSpaceToRoom(coordinate.Item1, coordinate.Item2, addRoomType);

                        Console.Clear();
                        ShowSubmarine(mySub);
                        break;
                    case 'r':
                        // get coordinates
                        Console.WriteLine("Shrink / remove a room at coordinates :");
                        coordinate = ReadCoordinates();
             
                        Console.WriteLine("REMOVING at x=" + coordinate.Item1 + ", y=" + coordinate.Item2);

                        mySub.RemoveSpaceOfRoom(coordinate.Item1, coordinate.Item2);

                        Console.Clear();
                        ShowSubmarine(mySub);
                        break;

                    }

                }

            // END !!
            Console.WriteLine(""); Console.WriteLine("");
            Console.WriteLine("Done !");
            Console.ReadKey();
            }

        private static RoomType ReadRoomType() {
            int roomTypeInt = ReadNumbers  ("Select room type (int) ");
            switch (roomTypeInt) {
                case 0: return RoomType.Empty;
                case 1: return RoomType.Engine;
                case 2: return RoomType.Generator;
				case 3:	return RoomType.Battery;
				case 4:	return RoomType.Bridge;
				case 5:	return RoomType.Gallery;
				case 6:	return RoomType.Mess;
				case 7:	return RoomType.Cabin;
				case 8:	return RoomType.Bunks;
				case 9:	return RoomType.Conn;
                default: return RoomType.Empty;
                }

        
            }

        static Tuple<int, int> ReadCoordinates() {
            Console.WriteLine("");
            int x = ReadNumbers("X:");
            int y = ReadNumbers("Y:");
            return new Tuple<int, int>(x, y);
            }
        static int ReadNumbers(string ask) {
            bool readOk = false;
            int readNumber=0;
            while (! readOk) {
                Console.Write(ask);
                string reading = Console.ReadLine();
                try { readNumber = Convert.ToInt16(reading); }
                catch { readOk = false; }
                readOk = true;
                }
            return readNumber;
            }
            
        
        // show room type of each space
        static void ShowSubmarine(Sub subToShow) {
            for (int y = 0; y < subToShow.heightOfSub; y++) {
                for (int x = 0; x < subToShow.lengthOfSub; x++) {
                    Space showSpace = subToShow.GetSpaceAt(x, y);
                    if (!showSpace.canContainRoom) Console.Write("XXX");
                    else {
                        RoomType showRoomType = subToShow.GetRoomTypeOfSpace(showSpace);
                        Console.Write((int)showRoomType); // show int value for room type to limited to 1 char
                        if (showSpace.roomID!=0) { Console.Write("R" + showSpace.roomID); } // show roomID
                        else { Console.Write("  "); }
                        }
                    Console.Write(" ");
                    }
                Console.WriteLine("");
                }
            
            }

        }
    }
