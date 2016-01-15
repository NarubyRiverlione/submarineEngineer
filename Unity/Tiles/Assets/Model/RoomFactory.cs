using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Submarine.Model {
	static class RoomFactory {

		public static Room CreateRoomOfType (RoomType type, Sub inThisSub) {
			switch (type) {
				case RoomType.EngineRoom:
					return new EngineRoom (type, inThisSub, getRoomProp ("EngineRoom_Min", inThisSub), getRoomProp ("EngineRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.EngineRoom, inThisSub));
				case RoomType.Generator:
					return new Generator (type, inThisSub, getRoomProp ("Generator_Min", inThisSub), getRoomProp ("Generator_CapPerTile", inThisSub), getPropReqRes (RoomType.Generator, inThisSub));
				case RoomType.Battery:
					return new Battery (type, inThisSub, getRoomProp ("Battery_Min", inThisSub), getRoomProp ("Battery_CapPerTile", inThisSub), getPropReqRes (RoomType.Battery, inThisSub));
				case RoomType.Bridge:
					return new Bridge (type, inThisSub, getRoomProp ("Bridge_Min", inThisSub), getRoomProp ("Bridge_CapPerTile", inThisSub), getPropReqRes (RoomType.Bridge, inThisSub));
				case RoomType.Gallery:
					return new Gallery (type, inThisSub, getRoomProp ("Gallery_Min", inThisSub), getRoomProp ("Gallery_CapPerTile", inThisSub), getPropReqRes (RoomType.Gallery, inThisSub));
				case RoomType.Cabin:
					return new Cabin (type, inThisSub, getRoomProp ("Cabin_Min", inThisSub), getRoomProp ("Cabin_CapPerTile", inThisSub), getPropReqRes (RoomType.Cabin, inThisSub));
				case RoomType.Bunks:
					return new Bunks (type, inThisSub, getRoomProp ("Bunks_Min", inThisSub), getRoomProp ("Bunks_CapPerTile", inThisSub), getPropReqRes (RoomType.Bunks, inThisSub));
				case RoomType.Conn:
					return new Conn (type, inThisSub, getRoomProp ("Conn_Min", inThisSub), getRoomProp ("Conn_CapPerTile", inThisSub), getPropReqRes (RoomType.Conn, inThisSub));
				case RoomType.Sonar:
					return new Sonar (type, inThisSub, getRoomProp ("Sonar_Min", inThisSub), getRoomProp ("Sonar_CapPerTile", inThisSub), getPropReqRes (RoomType.Sonar, inThisSub));
				case RoomType.RadioRoom:
					return new RadioRoom (type, inThisSub, getRoomProp ("RadioRoom_Min", inThisSub), getRoomProp ("RadioRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.RadioRoom, inThisSub));
				case RoomType.FuelTank:
					return new FuelTank (type, inThisSub, getRoomProp ("FuelTank_Min", inThisSub), getRoomProp ("FuelTank_CapPerTile", inThisSub), getPropReqRes (RoomType.FuelTank, inThisSub));
				case RoomType.PumpRoom: 
					return new PumpRoom (type, inThisSub, getRoomProp ("PumpRoom_Min", inThisSub), getRoomProp ("PumpRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.PumpRoom, inThisSub));
				case RoomType.StorageRoom:
					return new StorageRoom (type, inThisSub, getRoomProp ("StorageRoom_Min", inThisSub), getRoomProp ("StorageRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.StorageRoom, inThisSub));
				case RoomType.EscapeHatch:
					return new EscapeHatch (type, inThisSub, getRoomProp ("EscapeHatch_Min", inThisSub), getRoomProp ("EscapeHatch_CapPerTile", inThisSub), getPropReqRes (RoomType.EscapeHatch, inThisSub));
				case RoomType.TorpedoRoom:
					return new TorpedoRoom (type, inThisSub, getRoomProp ("TorpedoRoom_Min", inThisSub), getRoomProp ("TorpedoRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.TorpedoRoom, inThisSub));
				case RoomType.Stairs:
					return new Stairs (type, inThisSub, getRoomProp ("Stairs_Min", inThisSub), getRoomProp ("Stairs_CapPerTile", inThisSub), getPropReqRes (RoomType.Stairs, inThisSub));
				case RoomType.BalastTank:
					return new BalastTank (type, inThisSub, getRoomProp ("Balasttank_Min", inThisSub), getRoomProp ("Balasttank_CapPerTile", inThisSub), getPropReqRes (RoomType.BalastTank, inThisSub));
				case RoomType.Propellor:
					return new Propellor (type, inThisSub, getRoomProp ("Propellor_Min", inThisSub), getRoomProp ("Propellor_CapPerTile", inThisSub), getPropReqRes (RoomType.Propellor, inThisSub));

				case RoomType.Empty:
					return new EmptyRoom (type, inThisSub, 0, 0, null);
				case RoomType.Remove:
					return new EmptyRoom (type, inThisSub, 0, 0, null);

				default:
					throw new NotImplementedException ("ERROR: Room type " + type + "isn't implemented yet.");
			//return null;
			}
		}

		static int getRoomProp (string key, Sub inThisSub) {
			if (inThisSub.RoomProperties.ContainsKey (key))
				return (int)inThisSub.RoomProperties [key];
			else
				throw new Exception ("Room property '" + key + "' isn't found.");
		}

		static List<Resource> getPropReqRes (RoomType key, Sub inThisSub) {
			if (inThisSub.RoomPropertiesReqRes.ContainsKey (key))
				return inThisSub.RoomPropertiesReqRes [key];
			else
				throw new Exception ("Room Req Resourcess '" + key + "' isn't found.");
		}
	}
}
