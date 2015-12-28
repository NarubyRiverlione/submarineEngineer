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
					return new EngineRoom (type, inThisSub, getPropInt ("EngineRoom_Min"), getPropInt ("EngineRoom_CapPerTile"), getPropInt ("EngineRoom_reqRes"));
				case RoomType.Generator:
					return new Generator (type, inThisSub, getPropInt ("Generator_Min"), getPropInt ("Generator_CapPerTile"), getPropInt ("Generator_reqRes"));
				case RoomType.Battery:
					return new Battery (type, inThisSub, getPropInt ("Battery_Min"), getPropInt ("Battery_CapPerTile"), getPropInt ("Battery_reqRes"));
				case RoomType.Bridge:
					return new Bridge (type, inThisSub, getPropInt ("Bridge_Min"), getPropInt ("Bridge_CapPerTile"), getPropInt ("Bridge_reqRes"));
				case RoomType.Gallery:
					return new Gallery (type, inThisSub, getPropInt ("Gallery_Min"), getPropInt ("Gallery_CapPerTile"), getPropInt ("Gallery_reqRes"));
				case RoomType.Cabin:
					return new Cabin (type, inThisSub, getPropInt ("Cabin_Min"), getPropInt ("Cabin_CapPerTile"), getPropInt ("Cabin_reqRes"));
				case RoomType.Bunks:
					return new Bunks (type, inThisSub, getPropInt ("Bunks_Min"), getPropInt ("Bunks_CapPerTile"), getPropInt ("Bunks_reqRes"));
				case RoomType.Conn:
					return new Conn (type, inThisSub, getPropInt ("Conn_Min"), getPropInt ("Conn_CapPerTile"), getPropInt ("Conn_reqRes"));
				case RoomType.Sonar:
					return new Sonar (type, inThisSub, getPropInt ("Sonar_Min"), getPropInt ("Sonar_CapPerTile"), getPropInt ("Sonar_reqRes"));
				case RoomType.RadioRoom:
					return new RadioRoom (type, inThisSub, getPropInt ("RadioRoom_Min"), getPropInt ("RadioRoom_CapPerTile"), getPropInt ("RadioRoom_reqRes"));
				case RoomType.FuelTank:
					return new FuelTank (type, inThisSub, getPropInt ("FuelTank_Min"), getPropInt ("FuelTank_CapPerTile"), getPropInt ("FuelTank_reqRes"));
				case RoomType.PumpRoom:
					return new PumpRoom (type, inThisSub, getPropInt ("PumpRoom_Min"), getPropInt ("PumpRoom_CapPerTile"), getPropInt ("PumpRoom_reqRes"));
				case RoomType.StorageRoom:
					return new StorageRoom (type, inThisSub, getPropInt ("StorageRoom_Min"), getPropInt ("StorageRoom_CapPerTile"), getPropInt ("StorageRoom_reqRes"));
				case RoomType.EscapeHatch:
					return new EscapeHatch (type, inThisSub, getPropInt ("EscapeHatch_Min"), getPropInt ("EscapeHatch_CapPerTile"), getPropInt ("EscapeHatch_reqRes"));
				case RoomType.TorpedoRoom:
					return new TorpedoRoom (type, inThisSub, getPropInt ("TorpedoRoom_Min"), getPropInt ("TorpedoRoom_CapPerTile"), getPropInt ("TorpedoRoom_reqRes"));

				case RoomType.Empty:
					return new EmptyRoom (type, inThisSub, 0, 0, 0);

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
			
	}
}
