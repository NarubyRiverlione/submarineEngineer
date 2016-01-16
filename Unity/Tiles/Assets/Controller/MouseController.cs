using System;
using System.Linq;
using Submarine.Model;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	public GameObject cursorRoomBuilder;
	public GameObject cursorPiece;
	public GameObject cursorRoomDestroyer;
	public GameObject cursorRemovePieces;

	public GameObject scrollView_RoomButtons;
	public GameObject scrollView_CrewButtons;
	public GameObject scrollView_ItemButtons;

	public Text UI_Room_Info_Text;
	public Text UI_Information_Text;

	WorldController world;

	RoomType RoomTypeToBeBuild = RoomType.Empty;
	// remember previous tile below mouse so cursor and UI Information text is only updated where mouse is above an other tile
	Tile prevTileBelowMouse;
	// remember where the mouse was so we can detect dragging
	Vector3 prevMousePosition;

	Units unitOfCarrierToBuild = Units.None;
	Units unitConnectionPieceToBuild = Units.None;

	// Use this for initialization
	void Start () {
		// hide cursors
		cursorRoomBuilder.SetActive (false);
		cursorRoomDestroyer.SetActive (false);
		cursorPiece.SetActive (false);
		cursorRemovePieces.SetActive (false);
		
	}

	// Update is called once per frame
	void Update () {
      
         
		Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		// move camera
		if (Input.GetMouseButton (1)) { // Right mouse button is held down
			Vector3 diff = prevMousePosition - currentMousePosition;
			diff.y = 0;
			Camera.main.transform.Translate (diff);
			currentMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); // get updated mouse position
		}
		prevMousePosition = currentMousePosition; // remember where the mouse was so we can detect dragging 


		if (world == null)  // sometimes the MouseController does an update before de World is created on start of the game
                world = WorldController.instance;

		currentMousePosition.z = 0; // set Z to zero so mouse position isn't on the camera (and be clipped so it isn't visible)

		// get tile below mouse
		Tile tileBelowMouse = world.GetTileAtWorldCoordinates (currentMousePosition);

		// update Cursor only when mouse is in other tile the previous.
		if (tileBelowMouse != prevTileBelowMouse) {
			// reset tile if title isn't build able 
			if (tileBelowMouse != null && !tileBelowMouse.canContainRoom)
				tileBelowMouse = null;

			// show correct cursor on tile
			// default: hide all cursors 
			cursorRoomBuilder.SetActive (false);
			cursorRoomDestroyer.SetActive (false);
			cursorPiece.SetActive (false);
			cursorRemovePieces.SetActive (false);

			if (tileBelowMouse != null) { // only show builder icon if mouse is above a tile
				Vector3 spaceBelowMouseCoordinates = new Vector3 (tileBelowMouse.X, tileBelowMouse.Y, 0);
				// selected a room = show builder icon
				if (RoomTypeToBeBuild != RoomType.Empty && RoomTypeToBeBuild != RoomType.Remove) {
					cursorRoomBuilder.transform.position = spaceBelowMouseCoordinates;
					cursorRoomBuilder.SetActive (true);
					cursorRoomDestroyer.SetActive (false);
					cursorPiece.SetActive (false);
					cursorRemovePieces.SetActive (false);
				} 
				// selected a carrier or connection to be build = show carrier building icon
				if (unitConnectionPieceToBuild != Units.None || unitOfCarrierToBuild != Units.Remove) {
					cursorPiece.transform.position = spaceBelowMouseCoordinates;
					cursorPiece.SetActive (true);
					cursorRoomBuilder.SetActive (false);
					cursorRoomDestroyer.SetActive (false);
					cursorRemovePieces.SetActive (false);
				}
				// selected remove room = show destroyer icon
				if (RoomTypeToBeBuild == RoomType.Remove) { 
					cursorRoomDestroyer.transform.position = spaceBelowMouseCoordinates;
					cursorRoomBuilder.SetActive (false);
					cursorPiece.SetActive (false);
					cursorRoomDestroyer.SetActive (true);
					cursorRemovePieces.SetActive (false);
				}
				// selected remove carrier/connection
				if (unitOfCarrierToBuild == Units.Remove) {
					cursorRemovePieces.transform.position = spaceBelowMouseCoordinates;
					cursorRoomBuilder.SetActive (false);
					cursorPiece.SetActive (false);
					cursorRoomDestroyer.SetActive (false);
					cursorRemovePieces.SetActive (true);
				
				}

				ShowCursorInformation (tileBelowMouse);
			}
		}
		// remember previous tile below mouse so cursor and UI Information text is only updated where mouse is above an other tile
		prevTileBelowMouse = tileBelowMouse;

		// change title type = build or destroy room if clicked on (release left mouse)
		if (Input.GetMouseButtonUp (0) && EventSystem.current.IsPointerOverGameObject ()) {
			if (tileBelowMouse != null) {//check were above a tile
				// add tile to room
				if (RoomTypeToBeBuild != RoomType.Empty && RoomTypeToBeBuild != RoomType.Remove)
					world.mySub.AddTileToRoom (tileBelowMouse.X, tileBelowMouse.Y, RoomTypeToBeBuild);  
				// remove tile form room
				if (RoomTypeToBeBuild == RoomType.Remove)
					world.mySub.RemoveTileOfRoom (tileBelowMouse.X, tileBelowMouse.Y);                 

				// add piece
				if (unitConnectionPieceToBuild == Units.None && unitOfCarrierToBuild != Units.Remove)
					world.mySub.AddPieceToTile (tileBelowMouse.X, tileBelowMouse.Y, unitOfCarrierToBuild);
				// remove piece and connection				
				if (unitOfCarrierToBuild == Units.Remove) {
					world.mySub.RemovePiecesFromTile (tileBelowMouse.X, tileBelowMouse.Y);
				}
				// add connection
				if (unitConnectionPieceToBuild != Units.None) {
					world.mySub.AddPieceToTile (tileBelowMouse.X, tileBelowMouse.Y, unitConnectionPieceToBuild, true);
				}
			}
		}
            
	}

	// Set zoom  level
	public void SetZoomLevel (int zoomLevel) {
		if (zoomLevel == 1) {
			//reset camera
			Camera.main.transform.position = new Vector3 (19.5f, 4, -20); // TODO: check if this hard coded offset will work with other submarine outline images or if it needs to be set via Model
			Camera.main.GetComponent<Camera> ().orthographicSize = 14;
		}
		else {
			Camera.main.GetComponent<Camera> ().orthographicSize = 14 / zoomLevel;
			Camera.main.transform.Translate (new Vector3 (0, -1, 0));
		}
	}

	// Room buttons use this to set the room type that will be build
	public void SetRoomTypeToBeBuild () {
		ToggleGroup toggleGroup_Rooms = GameObject.FindWithTag ("ToggleGroup_Rooms").GetComponent<ToggleGroup> ();
		if (toggleGroup_Rooms != null) {
			Toggle activeRoomToggle = toggleGroup_Rooms.ActiveToggles ().FirstOrDefault ();
			if (activeRoomToggle != null) {
				string nameOfRoomType = activeRoomToggle.name;
				string typeOfRoom = nameOfRoomType.Split ('_') [2]; // Toggle_Room_xxxx
				// set room type to be build
				RoomTypeToBeBuild = (RoomType)Enum.Parse (typeof(RoomType), typeOfRoom);
				//TODO: other way then creating a room ?
				// create 'prototype' of room to get the validation text as validation text is set in Constructor and uses needs requirements
				Room prototypeRoom = Room.CreateRoomOfType (RoomTypeToBeBuild, world.mySub);
				// show building rules
				UI_Room_Info_Text.text = prototypeRoom.ValidationText;
			}
		}
	}
	// Set piece type to be build
	public void SetPieceUnitsToBeBuild (string unitString) {
		unitOfCarrierToBuild = (Units)Enum.Parse (typeof(Units), unitString);
		unitConnectionPieceToBuild = Units.None;
	}
	// Add connection
	public void SetConnectionPieceUnitsToBeBuild (string unitString) {
		unitConnectionPieceToBuild = (Units)Enum.Parse (typeof(Units), unitString);
		unitOfCarrierToBuild = Units.None;
	}

	void ShowCursorInformation (Tile tileBelowMouse) {
		string info = "Above tile (" + tileBelowMouse.X + "," + tileBelowMouse.Y + ")";
		if (tileBelowMouse.RoomID != 0) {
			Room room = world.mySub.GetRoom (tileBelowMouse.RoomID);
			info += " witch is part of the "	+ room.TypeOfRoom;// + "\n" + room.ValidationText;
		}
		foreach (Piece piece in tileBelowMouse.PiecesOnTile) {
			if (piece.Type != PieceType.None) {
				Carrier partOfCarrier = world.mySub.ResourceCarriers [piece.carrierID];
				string connectedString = piece.IsConnection ? " IS " : " is NOT ";
				string inputString = piece.Input == 0 ? " has NO input " : " has " + piece.Input + " " + piece.UnitOfContent + " input";
				string contentString = partOfCarrier.Content == 0 ? " has NO content" : " has " + partOfCarrier.Content + " " + partOfCarrier.UnitOfContent + " content";

				info += " contains piece " + piece.Type
				+ connectedString + " connection"
				+ " neigbore count = " + piece.NeighboreCount
				+ inputString
				+ contentString
				+ " wich is part of " + partOfCarrier.UnitOfContent + " carrier (" + partOfCarrier.ID + ")";
			}
		}
//			#if DEBUG
//			info += "\n DEBUG:"
//			+ " RoomID: " + tileBelowMouse.RoomID
//			+ " wall type: " + tileBelowMouse.WallType
//			+ " layout validate: " + room.IsLayoutValid
		//	+ " resources available " + room.ResourcesAvailable;
//			#endif

		UI_Information_Text.text = info;
	}

	// Select between rooms - crew - items
	public void SelectBuildingButtons () {
		ToggleGroup toggleGroup_Rooms = GameObject.Find ("Panel_Building").GetComponent<ToggleGroup> ();
		if (toggleGroup_Rooms != null) {
			Toggle activeRoomToggle = toggleGroup_Rooms.ActiveToggles ().FirstOrDefault ();
			if (activeRoomToggle != null) {
				switch (activeRoomToggle.name) {
					case "Toggle_RoomButtons":
						scrollView_RoomButtons.SetActive (true);
						scrollView_CrewButtons.SetActive (false);
						scrollView_ItemButtons.SetActive (false);
						break;
					case "Toggle_CrewButtons":
						scrollView_RoomButtons.SetActive (false);
						scrollView_CrewButtons.SetActive (true);
						scrollView_ItemButtons.SetActive (false);
						break;
					case "Toggle_ItemButtons":
						scrollView_RoomButtons.SetActive (false);
						scrollView_CrewButtons.SetActive (false);
						scrollView_ItemButtons.SetActive (true);
						break;
					default:
						throw new Exception ("Selected an unknow toggle in Panel building, check names");

				}
				// reset RoomTypeToBeBuild & PieceUnitsToBeBuild so a selection can be made
				unitOfCarrierToBuild = Units.None;
				unitConnectionPieceToBuild = Units.None;
				RoomTypeToBeBuild = RoomType.Empty;
			}
		}
	}

	public void AddCrew (string typeOfCrew) {
		Units crewType = (Units)Enum.Parse (typeof(Units), typeOfCrew);
		world.mySub.AddCrew (crewType);
		world.UpdateUIpanels ();
	}

	public void RemoveCrew (string typeOfCrew) {
		Units crewType = (Units)Enum.Parse (typeof(Units), typeOfCrew);
		world.mySub.RemoveCrew (crewType);
		world.UpdateUIpanels ();
	}
}
	

