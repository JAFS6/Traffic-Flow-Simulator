using UnityEngine;
using System.Collections;

public class SimulationUIController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Screen.showCursor = true;
			Application.LoadLevel("Main_Menu");
		}
	}
}
