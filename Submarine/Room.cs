using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Submarine {
    public enum RoomType {
        Empty = 0,
        Engine = 1,
        Generator = 2,
        Battery = 3,
        Bridge=4,
        Gallery=5,
        Mess=6,
        Cabin=7,
        Bunks=8,
        ConnTower=9
        };

    public class Room : IRoomValidation {
        public RoomType type { get; set; }
        //public bool isValid { get; private set; }
        public int size { get; set; }

        public bool isRoomValid(Room checkThisRoom) {
            return false;
            }

        public Room(RoomType setType) {
            type = setType;
            }
        }

    public interface IRoomValidation {
        bool isRoomValid(Room checkThisRoom);
        }
    }
