using System;
using System.Linq;
using Submarine.Model;

using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {

	public GameObject cursorBuilder;
	public GameObject cursorDestroyer;

	public Text UI_Room_Info_Text;
	public Text UI_Information_Text;

	WorldController world;
	// TODO: potential problem as default = Destroy
	RoomType RoomTypeToBeBuild = RoomType.Empty;
	// remember previous tile below mouse so cursor and UI Information text is only updated where mouse is above an other tile
	Tile prevTileBelowMouse;
	// remember where the mouse was so we can detect dragging
	Vector3 prevMousePosition;


	// Use this for initialization
	void Start () {
		// hide cursors
		cursorBuilder.SetActive (false);
		cursorDestroyer.SetActive (false);
		
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
				
			if (tileBelowMouse != null) { // only show builder icon if mouse is above a tile
				Vector3 spaceBelowMouseCoordinates = new Vector3 (tileBelowMouse.X, tileBelowMouse.Y, 0);
				if (RoomTypeToBeBuild != RoomType.Empty) { // selected a room = show builder icon
					cursorBuilder.transform.position = spaceBelowMouseCoordinates;
					cursorBuilder.SetActive (true);
					cursorDestroyer.SetActive (false);
				}
				else { // selected Empty room = show destroyer icon
					cursorDestroyer.transform.position = spaceBelowMouseCoordinates;
					cursorBuilder.SetActive (false);
					cursorDestroyer.SetActive (true);
				}
				ShowRoomInformation (tileBelowMouse);
			}
			else {
				// hide if cursor isn't on a tile
				cursorBuilder.SetActive (false);
				cursorDestroyer.SetActive (false);
			}	
		}
		// remember previous tile below mouse so cursor and UI Information text is only updated where mouse is above an other tile
		prevTileBelowMouse = tileBelowMouse;

		// change title type = build or destroy room if clicked on (release left mouse)
		if (Input.GetMouseButtonUp (0)) {
			if (tileBelowMouse != null) {//check were above a tile
				if (RoomTypeToBeBuild != RoomType.Empty)
					world.mySub.AddTileToRoom (tileBelowMouse.X, tileBelowMouse.Y, RoomTypeToBeBuild);	// add
					else
					world.mySub.RemoveTileOfRoom (tileBelowMouse.X, tileBelowMouse.Y);					// remove
			}
		}
	}

	// Set zoom  level
	public void SetZoomLevel (int zoomLevel) {
		if (zoomLevel == 1) {//reset camera
			Camera.main.transform.position = new Vector3 (19.5f, 2, -20); // TODO: check if this hard coded offset will work with other submarine outline images or if it needs to be set via Model
			Camera.main.GetComponent<Camera> ().orthographicSize = 14;
		}
		else {
			Camera.main.GetComponent<Camera> ().orthographicSize = 14 / zoomLevel;
			Camera.main.transform.Translate (new Vector3 (0, zoomLevel, 0));
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

	void ShowRoomInformation (Tile tileBelowMouse) {
		string info = "Above tile (" + tileBelowMouse.X + "," + tileBelowMouse.Y + ")";
		if (tileBelowMouse.RoomID != 0) {
			Room room = world.mySub.GetRoom (tileBelowMouse.RoomID);
			info += " witch is part of the "	+ room.TypeOfRoom;// + "\n" + room.ValidationText;
			#if DEBUG
			//TODO: remove next line before production build
			info += "\n DEBIG INFO:"
			+ " RoomID: " + tileBelowMouse.RoomID
			+ " wall type: " + tileBelowMouse.WallType
			+ " layout validate: " + room.IsLayoutValid
			+ " resources available " + room.ResourcesAvailable;
			#endif
		}
		UI_Information_Text.text = info;
	}


}
	

