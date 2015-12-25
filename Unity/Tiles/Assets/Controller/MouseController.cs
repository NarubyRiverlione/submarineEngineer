using System;
using System.Linq;
using Submarine.Model;
using UnityEngine;
using UnityEngine.UI;


public class MouseController : MonoBehaviour {

	public GameObject cursorBuilder;
	public GameObject cursorDestroyer;

	public Text UI_Builder_text;

	WorldController world;

	RoomType RoomTypeToBeBuild = RoomType.Empty;

	// Use this for initialization
	void Start () {
		// hide cusors
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

			// move mouse icon to tile below mouse

			Tile tileBelowMouse = world.GetTileAtWorldCoordinates (currentMousePosition);
			// reset title if title isn't build able 
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
			}
			else {
				// hide if cursor isn"t on a tile
				cursorBuilder.SetActive (false);
				cursorDestroyer.SetActive (false);
			}	

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

	public void SetRoomTypeToBeBuild () {
		ToggleGroup toggleGroup_Rooms = GameObject.Find ("ToggleGroup_Rooms").GetComponent<ToggleGroup> ();
		if (toggleGroup_Rooms != null) {
			Toggle activeRoomToggle = toggleGroup_Rooms.ActiveToggles ().FirstOrDefault ();
			if (activeRoomToggle != null) {
				string nameOfRoomType = activeRoomToggle.name;
				string typeOfRoom = nameOfRoomType.Split ('_') [2]; // Toggle_Room_xxxx
				// set room type to be build
				RoomTypeToBeBuild = (RoomType)Enum.Parse (typeof(RoomType), typeOfRoom);
				// create 'prototype' of room to get the validation text
				Room prototypeRoom = Room.CreateRoomOfType (RoomTypeToBeBuild, world.mySub);
				// show building rules
				UI_Builder_text.text = prototypeRoom.ValidationText;

				Debug.Log ("Toggle " + nameOfRoomType + " is active: " + typeOfRoom + " = Enum " + RoomTypeToBeBuild);

			}

		}
	}

}
	

