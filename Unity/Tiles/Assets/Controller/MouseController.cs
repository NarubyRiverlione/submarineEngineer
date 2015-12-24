using System;
using System.Linq;
using Submarine.Model;
using UnityEngine;
using UnityEngine.UI;


public class MouseController : MonoBehaviour {

	public GameObject cursorBuilder;

	RoomType RoomTypeToBeBuild=RoomType.Empty;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		currentMousePosition.z = 0; // set Z to zero so mouse position isn't on the camera (and be clipped so it isn't visible)

		// move builder icon to tile below mouse
		WorldController world = WorldController.instance; //GameObject.FindObjectOfType<WorldController> ();
		Tile tileBelowMouse = world.GetTileAtWorldCoordinates (currentMousePosition);
		// reset title if title isn't build able 
		if (tileBelowMouse != null && !tileBelowMouse.canContainRoom)
			tileBelowMouse = null;
				
		if (tileBelowMouse != null && RoomTypeToBeBuild != RoomType.Empty) { // only show builder icon if mouse is above a tile
			Vector3 spaceBelowMouseCoordinates = new Vector3 (tileBelowMouse.X, tileBelowMouse.Y, 0);
			cursorBuilder.transform.position = spaceBelowMouseCoordinates;
			cursorBuilder.SetActive (true);
		}
		else {
			cursorBuilder.SetActive (false); // hide if cursor isn"t on a tile
		}	

		// change title type if clicked on (release left mouse)
		if (Input.GetMouseButtonUp (0)) {
			if (tileBelowMouse != null) {//check were above a tile
				world.mySub.AddTileToRoom (tileBelowMouse.X, tileBelowMouse.Y, RoomTypeToBeBuild);
				
			}
		}
	}

	public void SetRoomTypeToBeBuild () {
		ToggleGroup toggleGroup_Rooms = GameObject.Find ("ToggleGroup_Rooms").GetComponent<ToggleGroup> ();
		if (toggleGroup_Rooms != null) {
			Toggle activeRoomToggle = toggleGroup_Rooms.ActiveToggles ().FirstOrDefault ();
			if (activeRoomToggle != null) {
				string nameOfRoomType = activeRoomToggle.name;
				string typeOfRoom = nameOfRoomType.Split ('_') [2];
				RoomTypeToBeBuild = (RoomType)Enum.Parse (typeof(RoomType), typeOfRoom);
			
				Debug.Log ("Toggle " + nameOfRoomType + " is active: " + typeOfRoom + " = Enum " + RoomTypeToBeBuild);

			}

		}
	}

}
	

