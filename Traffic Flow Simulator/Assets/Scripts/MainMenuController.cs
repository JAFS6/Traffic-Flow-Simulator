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
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	
	public void loadMap (string filename) {
		ApplicationController.map_filename = filename;
		Application.LoadLevel("Simulation");
	}
	
	public void showStart () {
		// Delete possible previous load map buttons
		GameObject [] buttons = GameObject.FindGameObjectsWithTag ("LoadMapButton");
		
		foreach (GameObject item in buttons) {
			Destroy(item);
		}
	
		// Hide other panels
		hideOptionsPanel();
		hideCreditsPanel();
		// Show the start panel
		showStartPanel();
		
		// Search maps available and add a load map button for each one
		DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/Resources/Maps/");
		FileInfo [] fileInfo = info.GetFiles();
		float i = 0;
		GameObject start_panel = GameObject.Find("Start Panel");
		GameObject button_prefab = Resources.Load("Prefabs/LoadMapButton", typeof(GameObject)) as GameObject;
		
		foreach (FileInfo file in fileInfo) {
		
			string filename_w_extension = file.Name;
			string [] split = filename_w_extension.Split(new char[] {'.'});
			string filename = split[0];
			
			if (split.Length == 3 && split[2] == Constants.String_graphml_ext && split[1] == Constants.String_topology_ext) {
			
				GameObject button = (GameObject)GameObject.Instantiate(button_prefab,new Vector3(0,0,0),Quaternion.identity);
				button.transform.SetParent(start_panel.transform.FindChild("MapLoadButtons").transform,false);
				button.name = "LoadMapButton"+"_"+i;
				button.tag = "LoadMapButton";
				button.GetComponentInChildren<Text>().text = filename;
				button.AddComponent<LayoutElement>();
				button.GetComponentInChildren<LayoutElement>().minHeight = 30;
				button.GetComponentInChildren<LayoutElement>().preferredHeight = 30;
				button.GetComponent<Button>().onClick.AddListener(delegate { loadMap(filename); });
				i++;
			}
		}
	}
	
	public void showOptions () {
		// Hide other panels
		hideStartPanel();
		hideCreditsPanel();
		// Show the options panel
		showOptionsPanel();
	}
	
	public void showCredits () {
		// Hide other panels
		hideStartPanel();
		hideOptionsPanel();
		// Show the credits panel
		showCreditsPanel();
	}
	
	public void exitApplication () {
		hideStartPanel();
		hideOptionsPanel();
		hideCreditsPanel();
		Application.Quit();
	}
	
	private void showStartPanel () {
		GameObject start_panel = GameObject.Find("Start Panel");
		start_panel.GetComponent<CanvasGroup>().alpha = 1;
		start_panel.GetComponent<CanvasGroup>().interactable = true;
		start_panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}
	
	private void showOptionsPanel () {
		GameObject options_panel = GameObject.Find("Options Panel");
		options_panel.GetComponent<CanvasGroup>().alpha = 1;
		options_panel.GetComponent<CanvasGroup>().interactable = true;
		options_panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}
	
	private void showCreditsPanel () {
		GameObject credits_panel = GameObject.Find("Credits Panel");
		credits_panel.GetComponent<CanvasGroup>().alpha = 1;
	}
	
	private void hideStartPanel () {
		GameObject start_panel = GameObject.Find("Start Panel");
		start_panel.GetComponent<CanvasGroup>().alpha = 0;
		start_panel.GetComponent<CanvasGroup>().interactable = false;
		start_panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	private void hideOptionsPanel () {
		GameObject options_panel = GameObject.Find("Options Panel");
		options_panel.GetComponent<CanvasGroup>().alpha = 0;
		options_panel.GetComponent<CanvasGroup>().interactable = false;
		options_panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	private void hideCreditsPanel () {
		GameObject credits_panel = GameObject.Find("Credits Panel");
		credits_panel.GetComponent<CanvasGroup>().alpha = 0;
	}
}
