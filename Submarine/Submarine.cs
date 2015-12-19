using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Submarine
{
    public class Sub
    {
        public Space[,] space { get; private set; }
        public int lengthOfSub { get; private set; }
        public int heightOfSub { get; private set; }

        #region CONSTRUCTOR
        public Sub() {
            lengthOfSub = 40;heightOfSub = 5;
            space = new Space[lengthOfSub, heightOfSub]; // initialize 2D array, still doesn't contain anything
            // instantiate spaces
            for (int x = 0; x < lengthOfSub; x++) {
                for (int y = 0; y < heightOfSub; y++) {
                    space[x, y] = new Space();
                    }
                }

            // set space's outside sub outlines as unavailable
            // upper engine room
            for (int x=0;x<=3;x++) {space[x, 1].canContainRoom = false;}
            // lower engine room
            for (int x = 0; x <= 3; x++) {space[x, heightOfSub-1].canContainRoom = false;}
            // left of Conn tower
            for (int x = 0; x < lengthOfSub / 3 * 2; x++) {space[x, 0].canContainRoom = false;}
            //right of Conn tower
            for (int x = lengthOfSub / 3 * 2+5; x < lengthOfSub; x++) { space[x, 0].canContainRoom = false; }
    
            }
        #endregion

      
        }

    public enum RoomType {
        Empty=0,
        Engine=1,
        Generator=2,
        Battery=3,
        Bridge,
        Gallery,
        Mess,
        Cabin,
        Bunks,
        ConnTower
        };


    public class Space {
        public int x { get; private set; }
        public int y { get; private set; }

        public int roomID { get; private set; } = 0;
        public RoomType roomType { get; private set; } 

        public bool canContainRoom { get; set; } = true;    // used to exclude spaces that are outside te outline of the sub

        public Space() {
            roomType = RoomType.Empty;
            }
        }
}
