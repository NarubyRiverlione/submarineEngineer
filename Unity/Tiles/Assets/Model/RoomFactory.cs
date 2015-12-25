using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Submarine.Model {
	static class RoomFactory {

		public static int EngineRoom_Min, EngineRoom_CapPerTile;
		public static string EngineRoom_unitOfCap;

		public static int Generator_Min, Generator_CapPerTile;
		public static string Generator_unitOfCap;

		public static int Battery_Min, Battery_CapPerTile;
		public static string Battery_unitOfCap;

		public static int Bridge_Min, Bridge_CapPerTile;
		public static string Bridge_unitOfCap;

		public static int Gallery_Min, Gallery_CapPerTile;
		public static string Gallery_unitOfCap;

		public static int Cabin_Min, Cabin_CapPerTile;
		public static string Cabin_unitOfCap;

		public static int Bunks_Min, Bunks_CapPerTile;
		public static string Bunks_unitOfCap;

		public static int Conn_Min, Conn_CapPerTile;
		public static string Conn_unitOfCap;

		public static int Sonar_Min, Sonar_CapPerTile;
		public static string Sonar_unitOfCap;

		public static int RadioRoom_Min, RadioRoom_CapPerTile;
		public static string RadioRoom_unitOfCap;

		public static int FuelTank_Min, FuelTank_CapPerTile;
		public static string FuelTank_unitOfCap;

		public static int BalastTank_Min, BalastTank_CapPerTile;
		public static string BalastTank_unitOfCap;

		public static int StorageRoom_Min, StorageRoom_CapPerTile;
		public static string StorageRoom_unitOfCap;

		public static int EscapeHatch_Min, EscapeHatch_CapPerTile;
		public static string EscapeHatch_unitOfCap;

		public static int TorpedoRoom_Min, TorpedoRoom_CapPerTile;
		public static string TorpedoRoom_unitOfCap;


		public static Room CreateRoomOfType (RoomType type, Sub inThisSub) {
			switch (type) {


				case RoomType.EngineRoom:
					return new EngineRoom (type, inThisSub, EngineRoom_Min, EngineRoom_CapPerTile, EngineRoom_unitOfCap);
				case RoomType.Generator:
					return new Generator (type, inThisSub, Generator_Min, Generator_CapPerTile, Generator_unitOfCap);
				case RoomType.Battery:
					return new Battery (type, inThisSub, BalastTank_Min, BalastTank_CapPerTile, BalastTank_unitOfCap);
				case RoomType.Bridge:
					return new Bridge (type, inThisSub, Bridge_Min, Bridge_CapPerTile, Bridge_unitOfCap);
				case RoomType.Gallery:
					return new Gallery (type, inThisSub, Gallery_Min, Gallery_CapPerTile, Gallery_unitOfCap);
				case RoomType.Cabin:
					return new Cabin (type, inThisSub, Cabin_Min, Cabin_CapPerTile, Cabin_unitOfCap);
				case RoomType.Bunks:
					return new Bunks (type, inThisSub, Bunks_Min, Bunks_CapPerTile, Bunks_unitOfCap);
				case RoomType.Conn:
					return new Conn (type, inThisSub, Conn_Min, Conn_CapPerTile, Conn_unitOfCap);
				case RoomType.Sonar:
					return new Sonar (type, inThisSub, Sonar_Min, Sonar_CapPerTile, Sonar_unitOfCap);
				case RoomType.RadioRoom:
					return new RadioRoom (type, inThisSub, RadioRoom_Min, RadioRoom_CapPerTile, RadioRoom_unitOfCap);
				case RoomType.FuelTank:
					return new FuelTank (type, inThisSub, FuelTank_Min, FuelTank_CapPerTile, FuelTank_unitOfCap);
				case RoomType.BalastTank:
					return new BalastTank (type, inThisSub, BalastTank_Min, BalastTank_CapPerTile, BalastTank_unitOfCap);
				case RoomType.StorageRoom:
					return new StorageRoom (type, inThisSub, StorageRoom_Min, StorageRoom_CapPerTile, StorageRoom_unitOfCap);
				case RoomType.EscapeHatch:
					return new EscapeHatch (type, inThisSub, EscapeHatch_Min, EscapeHatch_CapPerTile, EscapeHatch_unitOfCap);
				case RoomType.TorpedoRoom:
					return new TorpedoRoom (type, inThisSub, TorpedoRoom_Min, TorpedoRoom_CapPerTile, TorpedoRoom_unitOfCap);

				default:
					throw new NotImplementedException ("ERROR: Room type " + type + " isn't implemented yet.");
			//return null;
			}
		}
	}
}
