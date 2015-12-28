using System;
using System.Linq;
using Submarine.Model;
using UnityEngine;
using UnityEngine.UI;


public class MouseController : MonoBehaviour {

	public GameObject cursorBuilder;
	public GameObject cursorDestroyer;

	public Text UI_StatusText;

	WorldController world;

	RoomType RoomTypeToBeBuild = RoomType.Empty;
	Tile prevTileBelowMouse;

	// Use this for initialization
	void Start () {
		// hide cursors
		cursorBuilder.SetActive (false);
		cursorDestroyer.SetActive (false);
		
	}
	// Update is called once per frame
	void Update () {
		if (world == null) { // sometimes the MouseController does an update before de World is created on start of the game
			world = WorldController.instance;
		}
		else {
			Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			currentMousePosition.z = 0; // set Z to zero so mouse position isn't on the camera (and be clipped so it isn't visible)

			// get tile below mouse
			Tile tileBelowMouse = world.GetTileAtWorldCoordinates (currentMousePosition);

			// update Cursor only when mouse is in other tile the prev.
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
					Debug.Log ("Above tile (" + tileBelowMouse.X + "," + tileBelowMouse.Y + "), part of room: "
					+ world.mySub.GetRoomTypeOfTile (tileBelowMouse)
					+ "(" + tileBelowMouse.RoomID + ")"
					+ " wall type: " + tileBelowMouse.WallType);
				}
				else {
					// hide if cursor isn't on a tile
					cursorBuilder.SetActive (false);
					cursorDestroyer.SetActive (false);
				}	
			}
			prevTileBelowMouse = tileBelowMouse;

			// change title type if clicked on (release left mouse)
			if (Input.GetMouseButtonUp (0)) {
				if (tileBelowMouse != null) {//check were above a tile
					if (RoomTypeToBeBuild != RoomType.Empty)
						world.mySub.AddTileToRoom (tileBelowMouse.X, tileBelowMouse.Y, RoomTypeToBeBuild);	// add
					else
						world.mySub.RemoveTileOfRoom (tileBelowMouse.X, tileBelowMouse.Y);					// remove

				}
			}
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
				// create 'prototype' of room to get the validation text
				Room prototypeRoom = Room.CreateRoomOfType (RoomTypeToBeBuild, world.mySub);
				// show building rules
				UI_StatusText.text = prototypeRoom.ValidationText;

				//Debug.Log ("Toggle " + nameOfRoomType + " is active: " + typeOfRoom + " = Enum " + RoomTypeToBeBuild);

			}

		}
	}

	// Save sub
	public void SaveSub () {
		UI_StatusText.text = "Saving submarine....";
		string filename = UnityEditor.EditorUtility.SaveFilePanel ("Saving submarine", "", "My Submarine", "json");
		world.mySub.Save (filename);
		UI_StatusText.text = "Submarine saved.";
	}

	// Load sub
	public void LoadSub () {
		UI_StatusText.text = "Loading submarine....";
		string filename = UnityEditor.EditorUtility.OpenFilePanel ("Saving submarine", "", "json");

		// destroy all Tile game objects (and the wall, warning,.. childeren)
		// (maybe new loaded sub has other dimensions)
		world.RemoveAllTileGameObjects ();
		// load new Sub 
		world.mySub.Load (filename);
		// add all tiles game objects 
		world.CreateAllTileGameObjects (); 	// also subscript too the  .TileChangedActions with UpdateTileSprite 

		UI_StatusText.text = "Submarine loading.";
	}
}
	

