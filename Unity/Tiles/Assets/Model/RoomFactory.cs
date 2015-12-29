using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Submarine.Model {
	static class RoomFactory {
		public static Sub inThisSub;

		public static Room CreateRoomOfType (RoomType type) {
			switch (type) {
				case RoomType.EngineRoom:
					return new EngineRoom (type, inThisSub, getPropInt ("EngineRoom_Min"), getPropInt ("EngineRoom_CapPerTile"), getPropReqRes (RoomType.EngineRoom));
				case RoomType.Generator:
					return new Generator (type, inThisSub, getPropInt ("Generator_Min"), getPropInt ("Generator_CapPerTile"), getPropReqRes (RoomType.Generator));
				case RoomType.Battery:
					return new Battery (type, inThisSub, getPropInt ("Battery_Min"), getPropInt ("Battery_CapPerTile"), getPropReqRes (RoomType.Battery));
				case RoomType.Bridge:
					return new Bridge (type, inThisSub, getPropInt ("Bridge_Min"), getPropInt ("Bridge_CapPerTile"), getPropReqRes (RoomType.Bridge));
				case RoomType.Gallery:
					return new Gallery (type, inThisSub, getPropInt ("Gallery_Min"), getPropInt ("Gallery_CapPerTile"), getPropReqRes (RoomType.Gallery));
				case RoomType.Cabin:
					return new Cabin (type, inThisSub, getPropInt ("Cabin_Min"), getPropInt ("Cabin_CapPerTile"), getPropReqRes (RoomType.Cabin));
				case RoomType.Bunks:
					return new Bunks (type, inThisSub, getPropInt ("Bunks_Min"), getPropInt ("Bunks_CapPerTile"), getPropReqRes (RoomType.Bunks));
				case RoomType.Conn:
					return new Conn (type, inThisSub, getPropInt ("Conn_Min"), getPropInt ("Conn_CapPerTile"), getPropReqRes (RoomType.Conn));
				case RoomType.Sonar:
					return new Sonar (type, inThisSub, getPropInt ("Sonar_Min"), getPropInt ("Sonar_CapPerTile"), getPropReqRes (RoomType.Sonar));
				case RoomType.RadioRoom:
					return new RadioRoom (type, inThisSub, getPropInt ("RadioRoom_Min"), getPropInt ("RadioRoom_CapPerTile"), getPropReqRes (RoomType.RadioRoom));
				case RoomType.FuelTank:
					return new FuelTank (type, inThisSub, getPropInt ("FuelTank_Min"), getPropInt ("FuelTank_CapPerTile"), getPropReqRes (RoomType.FuelTank));
				case RoomType.PumpRoom:
					return new PumpRoom (type, inThisSub, getPropInt ("PumpRoom_Min"), getPropInt ("PumpRoom_CapPerTile"), getPropReqRes (RoomType.PumpRoom));
				case RoomType.StorageRoom:
					return new StorageRoom (type, inThisSub, getPropInt ("StorageRoom_Min"), getPropInt ("StorageRoom_CapPerTile"), getPropReqRes (RoomType.StorageRoom));
				case RoomType.EscapeHatch:
					return new EscapeHatch (type, inThisSub, getPropInt ("EscapeHatch_Min"), getPropInt ("EscapeHatch_CapPerTile"), getPropReqRes (RoomType.EscapeHatch));
				case RoomType.TorpedoRoom:
					return new TorpedoRoom (type, inThisSub, getPropInt ("TorpedoRoom_Min"), getPropInt ("TorpedoRoom_CapPerTile"), getPropReqRes (RoomType.TorpedoRoom));

				case RoomType.Empty:
					return new EmptyRoom (type, inThisSub, 0, 0, null);

				default:
					throw new NotImplementedException ("ERROR: Room type " + type + "isn't implemented yet.");
			//return null;
			}
		}

		static int getPropInt (string key) {
			if (inThisSub.RoomPropertiesInt.ContainsKey (key))
				return inThisSub.RoomPropertiesInt [key];
			else
				throw new Exception ("Room property '" + key + "' isn't found.");
		}

		static List<Resource> getPropReqRes (RoomType key) {
			if (inThisSub.RoomPropertiesReqRes.ContainsKey (key))
				return inThisSub.RoomPropertiesReqRes [key];
			else
				throw new Exception ("Room Req Resourcess '" + key + "' isn't found.");
		}
	}
}
