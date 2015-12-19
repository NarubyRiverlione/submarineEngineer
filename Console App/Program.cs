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

            // show spaces
            ShowSubmarine(mySub);


            // END !!
            Console.WriteLine(""); Console.WriteLine("");
            Console.WriteLine("Done !");
            Console.ReadKey();
            }


        // show room type of each space
        static void ShowSubmarine(Sub subToShow) {
            for (int y = 0; y < subToShow.heightOfSub; y++) {
                for (int x = 0; x < subToShow.lengthOfSub; x++) {
                    Console.Write(VisulizeSpace(subToShow.space[x, y]));
                    Console.Write(" ");
                    }
                Console.WriteLine("");
                }
            
            }

        // visualize room type of 1 space
        private static string VisulizeSpace(Space showSpace) {
            
            int spaceAsString = (int)showSpace.roomType;   // get room type of space
            if (!showSpace.canContainRoom) spaceAsString = -1;  // cannot contain a room

        
            return spaceAsString.ToString().Length<2 ?  spaceAsString.ToString()+ ' ' : spaceAsString.ToString();
            }
        }
    }
