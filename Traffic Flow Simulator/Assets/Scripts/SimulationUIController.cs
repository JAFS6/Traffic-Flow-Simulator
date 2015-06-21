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

public class SimulationUIController : MonoBehaviour
{
	// Pause control
	public static bool is_paused;
	
	// Fields to assign from editor
	[SerializeField]
	private GameObject mapNameLabel;
	[SerializeField]
	private GameObject camera_obj;
	[SerializeField]
	private GameObject pause_panel;
	[SerializeField]
	private GameObject pause_hint;
	[SerializeField]
	private GameObject roadNames_group;
	[SerializeField]
	private GameObject roadNames_toggle;
	
	// Prefabs
	private static GameObject roadName3D_prefab;
	
	void Start ()
	{
		is_paused = false;
		load_roadName3D_prefab ();
	}
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!is_paused) // Key ESC shows pause menu
			{
				is_paused = true;
				hidePauseHint();
				showPausePanel();
			}
			else if (is_paused) // Key ESC resumes simulation
			{
				resumeSimulation ();
			}
		}
	}
	
	/**
	 * @Creates a sign with the name passed as an argument and puts it as child of roadNames_group.
	 * @param[in] name The name to show.
	 * @param[in] position The position for the object on the XZ plane.
	 */
	public void nameSign (string name, Vector2 position)
	{
		if (roadName3D_prefab == null)
		{
			load_roadName3D_prefab ();
		}
		GameObject g = GameObject.Instantiate (roadName3D_prefab, Vector3.zero, Quaternion.identity) as GameObject;;
		g.GetComponentInChildren<TextMesh>().text = name;
		g.transform.SetParent(roadNames_group.transform);
		Vector3 p = new Vector3(position.x, Constants.nameSign_Y_position, position.y);
		g.transform.position = p;
		g.GetComponent<LockLookTo>().setTarget(camera_obj);
		g.SetActive(false);
	}
	
	public void setMapName (string name)
	{
		mapNameLabel.GetComponent<Text>().text = name;
	}
	
	public void resumeSimulation ()
	{
		hidePausePanel();
		showPauseHint();
		is_paused = false;
		Cursor.visible = true;
	}
	
	public void backToMainMenu ()
	{
		hidePausePanel();
		is_paused = false;
		Application.LoadLevel("Main_Menu");
	}
	
	public void toggleRoadNames ()
	{
		bool active = roadNames_toggle.GetComponent<Toggle>().isOn;
		
		if (active)
		{
			hideRoadNames ();
		}
		else
		{
			showRoadNames ();
		}
	}
	
	private void showPausePanel ()
	{
		pause_panel.SetActive(true);
	}
	
	private void hidePausePanel ()
	{
		pause_panel.SetActive(false);
	}
	
	private void showPauseHint ()
	{
		pause_hint.SetActive(true);
	}
	
	private void hidePauseHint ()
	{
		pause_hint.SetActive(false);
	}
	
	private void load_roadName3D_prefab ()
	{
		if (roadName3D_prefab == null)
		{
			roadName3D_prefab = Resources.Load ("Prefabs/RoadName3D", typeof(GameObject)) as GameObject;
		}
	}
	
	private void showRoadNames ()
	{
		foreach (Transform child in roadNames_group.transform)
		{
			child.gameObject.SetActive(true);
		}
	}
	
	private void hideRoadNames ()
	{
		foreach (Transform child in roadNames_group.transform)
		{
			child.gameObject.SetActive(false);
		}
	}
}
