using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Submarine.Model {
    static class RoomFactory {
        public static Room CreateRoomOfType(RoomType type, Sub inThisSub) {
            switch (type) {


                case RoomType.EngineRoom:
                    return new EngineRoom(type, inThisSub);
                case RoomType.Generator:
                    return new Generator(type, inThisSub);
                case RoomType.Battery:
                    return new Battery(type, inThisSub);
                case RoomType.Bridge:
                    return new Bridge(type, inThisSub);
                case RoomType.Gallery:
                    return new Gallery(type, inThisSub);
                case RoomType.Mess:
                    return new Mess(type, inThisSub);
                case RoomType.Cabin:
                    return new Cabin(type, inThisSub);
                case RoomType.Bunks:
                    return new Bunks(type, inThisSub);
                case RoomType.Conn:
                    return new Conn(type, inThisSub);
                case RoomType.Sonar:
                    return new Sonar(type, inThisSub);
                case RoomType.RadioRoom:
                    return new RadioRoom(type, inThisSub);
                case RoomType.FuelTank:
                    return new FuelTank(type, inThisSub);
                case RoomType.BalastTank:
                    return new BalastTank(type, inThisSub);
                case RoomType.StorageRoom:
                    return new StorageRoom(type, inThisSub);
                case RoomType.EscapeHatch:
                    return new EscapeHatch(type, inThisSub);
                case RoomType.TorpedoRoom:
                    return new TorpedoRoom(type, inThisSub);

                default:
                    throw new NotImplementedException("ERROR: Room type " + type + " isn't implemented yet.");
                    //return null;
                }
            }
        }
    }
