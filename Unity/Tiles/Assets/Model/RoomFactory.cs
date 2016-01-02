using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Submarine.Model {
	static class RoomFactory {

		public static Room CreateRoomOfType (RoomType type, Sub inThisSub) {
			switch (type) {
				case RoomType.EngineRoom:
					return new EngineRoom (type, inThisSub, getPropInt ("EngineRoom_Min", inThisSub), getPropInt ("EngineRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.EngineRoom, inThisSub));
				case RoomType.Generator:
					return new Generator (type, inThisSub, getPropInt ("Generator_Min", inThisSub), getPropInt ("Generator_CapPerTile", inThisSub), getPropReqRes (RoomType.Generator, inThisSub));
				case RoomType.Battery:
					return new Battery (type, inThisSub, getPropInt ("Battery_Min", inThisSub), getPropInt ("Battery_CapPerTile", inThisSub), getPropReqRes (RoomType.Battery, inThisSub));
				case RoomType.Bridge:
					return new Bridge (type, inThisSub, getPropInt ("Bridge_Min", inThisSub), getPropInt ("Bridge_CapPerTile", inThisSub), getPropReqRes (RoomType.Bridge, inThisSub));
				case RoomType.Gallery:
					return new Gallery (type, inThisSub, getPropInt ("Gallery_Min", inThisSub), getPropInt ("Gallery_CapPerTile", inThisSub), getPropReqRes (RoomType.Gallery, inThisSub));
				case RoomType.Cabin:
					return new Cabin (type, inThisSub, getPropInt ("Cabin_Min", inThisSub), getPropInt ("Cabin_CapPerTile", inThisSub), getPropReqRes (RoomType.Cabin, inThisSub));
				case RoomType.Bunks:
					return new Bunks (type, inThisSub, getPropInt ("Bunks_Min", inThisSub), getPropInt ("Bunks_CapPerTile", inThisSub), getPropReqRes (RoomType.Bunks, inThisSub));
				case RoomType.Conn:
					return new Conn (type, inThisSub, getPropInt ("Conn_Min", inThisSub), getPropInt ("Conn_CapPerTile", inThisSub), getPropReqRes (RoomType.Conn, inThisSub));
				case RoomType.Sonar:
					return new Sonar (type, inThisSub, getPropInt ("Sonar_Min", inThisSub), getPropInt ("Sonar_CapPerTile", inThisSub), getPropReqRes (RoomType.Sonar, inThisSub));
				case RoomType.RadioRoom:
					return new RadioRoom (type, inThisSub, getPropInt ("RadioRoom_Min", inThisSub), getPropInt ("RadioRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.RadioRoom, inThisSub));
				case RoomType.FuelTank:
					return new FuelTank (type, inThisSub, getPropInt ("FuelTank_Min", inThisSub), getPropInt ("FuelTank_CapPerTile", inThisSub), getPropReqRes (RoomType.FuelTank, inThisSub));
				case RoomType.PumpRoom: // TODO: make room creation button for pump room
					return new PumpRoom (type, inThisSub, getPropInt ("PumpRoom_Min", inThisSub), getPropInt ("PumpRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.PumpRoom, inThisSub));
				case RoomType.StorageRoom:
					return new StorageRoom (type, inThisSub, getPropInt ("StorageRoom_Min", inThisSub), getPropInt ("StorageRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.StorageRoom, inThisSub));
				case RoomType.EscapeHatch:
					return new EscapeHatch (type, inThisSub, getPropInt ("EscapeHatch_Min", inThisSub), getPropInt ("EscapeHatch_CapPerTile", inThisSub), getPropReqRes (RoomType.EscapeHatch, inThisSub));
				case RoomType.TorpedoRoom:
					return new TorpedoRoom (type, inThisSub, getPropInt ("TorpedoRoom_Min", inThisSub), getPropInt ("TorpedoRoom_CapPerTile", inThisSub), getPropReqRes (RoomType.TorpedoRoom, inThisSub));
				
				case RoomType.Stairs:
					return new Stairs (type, inThisSub, getPropInt ("Stairs_Min", inThisSub), getPropInt ("Stairs_CapPerTile", inThisSub), getPropReqRes (RoomType.Stairs, inThisSub));
				
				case RoomType.BalastTank:
					return new BalastTank (type, inThisSub, getPropInt ("Balasttank_Min", inThisSub), getPropInt ("Balasttank_CapPerTile", inThisSub), getPropReqRes (RoomType.BalastTank, inThisSub));
				
				case RoomType.Empty:
					return new EmptyRoom (type, inThisSub, 0, 0, null);

				default:
					throw new NotImplementedException ("ERROR: Room type " + type + "isn't implemented yet.");
			//return null;
			}
		}

		static int getPropInt (string key, Sub inThisSub) {
			if (inThisSub.RoomPropertiesInt.ContainsKey (key))
				return inThisSub.RoomPropertiesInt [key];
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
