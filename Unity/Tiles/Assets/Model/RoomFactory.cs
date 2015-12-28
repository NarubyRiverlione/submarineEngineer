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
					return new EngineRoom (type, inThisSub, getPropInt ("EngineRoom_Min"), getPropInt ("EngineRoom_CapPerTile"), getPropUnit ("EngineRoom_unitOfCap"), getPropUnit ("EngineRoom_resource"), getPropInt ("EngineRoom_reqRes"));
				case RoomType.Generator:
					return new Generator (type, inThisSub, getPropInt ("Generator_Min"), getPropInt ("Generator_CapPerTile"), getPropUnit ("Generator_unitOfCap"), getPropUnit ("Generator_resource"), getPropInt ("Generator_reqRes"));
				case RoomType.Battery:
					return new Battery (type, inThisSub, getPropInt ("Battery_Min"), getPropInt ("Battery_CapPerTile"), getPropUnit ("Battery_unitOfCap"), getPropUnit ("Battery_resource"), getPropInt ("Battery_reqRes"));
				case RoomType.Bridge:
					return new Bridge (type, inThisSub, getPropInt ("Bridge_Min"), getPropInt ("Bridge_CapPerTile"), getPropUnit ("Bridge_unitOfCap"), getPropUnit ("Bridge_resource"), getPropInt ("Bridge_reqRes"));
				case RoomType.Gallery:
					return new Gallery (type, inThisSub, getPropInt ("Gallery_Min"), getPropInt ("Gallery_CapPerTile"), getPropUnit ("Gallery_unitOfCap"), getPropUnit ("Gallery_resource"), getPropInt ("Gallery_reqRes"));
				case RoomType.Cabin:
					return new Cabin (type, inThisSub, getPropInt ("Cabin_Min"), getPropInt ("Cabin_CapPerTile"), getPropUnit ("Cabin_unitOfCap"), getPropUnit ("Cabin_resource"), getPropInt ("Cabin_reqRes"));
				case RoomType.Bunks:
					return new Bunks (type, inThisSub, getPropInt ("Bunks_Min"), getPropInt ("Bunks_CapPerTile"), getPropUnit ("Bunks_unitOfCap"), getPropUnit ("Bunks_resource"), getPropInt ("Bunks_reqRes"));
				case RoomType.Conn:
					return new Conn (type, inThisSub, getPropInt ("Conn_Min"), getPropInt ("Conn_CapPerTile"), getPropUnit ("Conn_unitOfCap"), getPropUnit ("Conn_resource"), getPropInt ("Conn_reqRes"));
				case RoomType.Sonar:
					return new Sonar (type, inThisSub, getPropInt ("Sonar_Min"), getPropInt ("Sonar_CapPerTile"), getPropUnit ("Sonar_unitOfCap"), getPropUnit ("Sonar_resource"), getPropInt ("Sonar_reqRes"));
				case RoomType.RadioRoom:
					return new RadioRoom (type, inThisSub, getPropInt ("RadioRoom_Min"), getPropInt ("RadioRoom_CapPerTile"), getPropUnit ("RadioRoom_unitOfCap"), getPropUnit ("RadioRoom_resource"), getPropInt ("RadioRoom_reqRes"));
				case RoomType.FuelTank:
					return new FuelTank (type, inThisSub, getPropInt ("FuelTank_Min"), getPropInt ("FuelTank_CapPerTile"), getPropUnit ("FuelTank_unitOfCap"), getPropUnit ("FuelTank_resource"), getPropInt ("FuelTank_reqRes"));
				case RoomType.PumpRoom:
					return new PumpRoom (type, inThisSub, getPropInt ("PumpRoom_Min"), getPropInt ("PumpRoom_CapPerTile"), getPropUnit ("PumpRoom_unitOfCap"), getPropUnit ("PumpRoom_resource"), getPropInt ("PumpRoom_reqRes"));
				case RoomType.StorageRoom:
					return new StorageRoom (type, inThisSub, getPropInt ("StorageRoom_Min"), getPropInt ("StorageRoom_CapPerTile"), getPropUnit ("StorageRoom_unitOfCap"), getPropUnit ("StorageRoom_resource"), getPropInt ("StorageRoom_reqRes"));
				case RoomType.EscapeHatch:
					return new EscapeHatch (type, inThisSub, getPropInt ("EscapeHatch_Min"), getPropInt ("EscapeHatch_CapPerTile"), getPropUnit ("EscapeHatch_unitOfCap"), getPropUnit ("EscapeHatch_resource"), getPropInt ("EscapeHatch_reqRes"));
				case RoomType.TorpedoRoom:
					return new TorpedoRoom (type, inThisSub, getPropInt ("TorpedoRoom_Min"), getPropInt ("TorpedoRoom_CapPerTile"), getPropUnit ("TorpedoRoom_unitOfCap"), getPropUnit ("TorpedoRoom_resource"), getPropInt ("TorpedoRoom_reqRes"));

				case RoomType.Empty:
					return new EmptyRoom (type, inThisSub, 0, 0, Units.None, Units.None, 0);

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

		static Units getPropUnit (string key) {
			if (inThisSub.RoomPropertiesUnits.ContainsKey (key))
				return inThisSub.RoomPropertiesUnits [key];
			else
				throw new Exception ("Room property '" + key + "' isn't found.");
		}
	}
}
