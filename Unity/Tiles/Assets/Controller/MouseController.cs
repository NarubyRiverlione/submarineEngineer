using UnityEngine;
using System.Collections;
using Submarine.Model;

namespace Submarine.Controller {
	public class MouseController : MonoBehaviour {

		public GameObject cursorBuilder;

		// Use this for initialization
		void Start () {
	
		}
	
		// Update is called once per frame
		void Update () {
			Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			currentMousePosition.z = 0; // set Z to zero so mouse position isn't on the camera (and be clipped so it isn't visible)

			WorldController world = GameObject.FindObjectOfType<WorldController>;

			Vector3 spaceBelowMouseCoordinates = world.GetTitleAtWorldCoordinates (currentMousePosition);
					
			cursorBuilder.transform.position = spaceBelowMouseCoordinates; 
		}
	}

}
