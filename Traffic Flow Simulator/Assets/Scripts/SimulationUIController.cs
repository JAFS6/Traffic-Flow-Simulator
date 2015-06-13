/*
	Copyright 2014-2015 Juan Antonio Fajardo Serrano

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimulationUIController : MonoBehaviour {
	// Pause control
	public static bool is_paused;
	[SerializeField]
	private GameObject mapNameLabel;
	
	void Start () {
		is_paused = false;
	}
	
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			
			if (!is_paused) { // Key ESC shows pause menu
				is_paused = true;
				hidePauseHint();
				showPausePanel();
			}
			else if (is_paused) { // Key ESC resumes simulation
				resumeSimulation ();
			}
		}
	}
	
	public void setMapName (string name)
	{
		mapNameLabel.GetComponent<Text>().text = name;
	}
	
	public void resumeSimulation () {
		hidePausePanel();
		showPauseHint();
		is_paused = false;
		Cursor.visible = true;
	}
	
	public void backToMainMenu () {
		hidePausePanel();
		is_paused = false;
		Application.LoadLevel("Main_Menu");
	}
	
	private void showPausePanel () {
		GameObject pause_panel = GameObject.Find("Pause Panel");
		pause_panel.GetComponent<CanvasGroup>().alpha = 1;
		pause_panel.GetComponent<CanvasGroup>().interactable = true;
		pause_panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}
	
	private void hidePausePanel () {
		GameObject pause_panel = GameObject.Find("Pause Panel");
		pause_panel.GetComponent<CanvasGroup>().alpha = 0;
		pause_panel.GetComponent<CanvasGroup>().interactable = false;
		pause_panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	private void showPauseHint () {
		GameObject o = GameObject.Find("Pause Hint");
		o.GetComponent<CanvasGroup>().alpha = 1;
	}
	
	private void hidePauseHint () {
		GameObject o = GameObject.Find("Pause Hint");
		o.GetComponent<CanvasGroup>().alpha = 0;
	}
}
