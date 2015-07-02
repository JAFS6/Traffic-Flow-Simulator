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

public class MainMenuController : MonoBehaviour
{
	[SerializeField]
	private GameObject MapLoadButtons;
	
	private bool isQuiting = false;
	
	public void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			exitApplication ();
		}
	}
	
	public void exitApplication ()
	{
		if (!isQuiting)
		{
			isQuiting = true;
			Application.Quit();
		}
	}
	
	public void loadMap (string filename)
	{
		SimulationController.map_filename = filename;
		Application.LoadLevel("Simulation");
	}
	
	public void loadMapButtons ()
	{
		// Delete possible previous load map buttons
		GameObject [] buttons = GameObject.FindGameObjectsWithTag ("LoadMapButton");
		
		foreach (GameObject item in buttons)
		{
			Destroy(item);
		}
		
		// Search maps available and add a load map button for each one
		DirectoryInfo info = new DirectoryInfo(Application.dataPath + Constants.maps_path);
		FileInfo [] fileInfo = info.GetFiles();
		float i = 0;
		GameObject button_prefab = Resources.Load("Prefabs/LoadMapButton", typeof(GameObject)) as GameObject;
		
		foreach (FileInfo file in fileInfo)
		{
			string filename_w_extension = file.Name;
			string [] split = filename_w_extension.Split(new char[] {'.'});
			string filename = split[0];
			
			if (split.Length == 3 && split[2] == Constants.String_graphml_ext && split[1] == Constants.String_topology_ext)
			{
				GameObject button = (GameObject)GameObject.Instantiate(button_prefab,new Vector3(0,0,0),Quaternion.identity);
				button.transform.SetParent(MapLoadButtons.transform,false);
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
}
