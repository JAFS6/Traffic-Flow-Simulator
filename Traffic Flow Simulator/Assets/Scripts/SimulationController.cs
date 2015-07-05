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
using System.Xml;
using System.IO;
using System.Collections.Generic;

public struct EntryNodeInfo
{
	public string id;
	public TransportType tt; // Transport Type
	public float tbs; 		 // Time Between Spawns
}

public class SimulationController : MonoBehaviour
{	
	public static string map_filename = "";
	
	// Control variables to instantiate vehicles
	private Dictionary<string, EntryNodeInfo> entryNodes;
	[SerializeField]
	private GameObject maxVehicles_slider;
	[SerializeField]
	private GameObject publicVehicles_slider;
	[SerializeField]
	private GameObject privateVehicles_slider;
	[SerializeField]
	private GameObject goodDrivers_slider;
	[SerializeField]
	private GameObject averageDrivers_slider;
	[SerializeField]
	private GameObject badDrivers_slider;
	[SerializeField]
	private GameObject help_panel_toggle;
	
	private int max_vehicles;
	private int num_spawn_errors = 0;

	// Control variables predetermined positions of the camera
	private GameObject 	main_camera;
	private Vector3 	initial_camera_position;
	private Vector3 	initial_camera_direction;
	private Vector2[] 	node_positions;
	
	// Vehicle transport types on simulation
	private int num_private_vehicles_running = 0;
	private int num_public_vehicles_running = 0;
	
	// Driver types on simulation
	private int num_good_drivers_running = 0;
	private int num_average_drivers_running = 0;
	private int num_bad_drivers_running = 0;

	// Actions to take when the application starts
	public void Start ()
	{
		if (GlobalStatus.isFirstTimeLoadMap)
		{
			help_panel_toggle.GetComponent<Toggle>().isOn = true;
			GlobalStatus.isFirstTimeLoadMap = false;
		}
		// Get the reference to the simulator's camera
		main_camera = GameObject.Find("Main Camera");

		// Create new map
		RoadMap.CreateNewMap(map_filename);

		// Load map data
		MapLoader loader = new MapLoader();
		loader.LoadMap(map_filename);
		
		this.gameObject.GetComponent<SimulationUIController>().setMapName(map_filename);
		// Draw the map
		RoadMap.draw ();

		// Store the positions of the nodes to position the camera
		saveNodePositions ();
		
		// Calculate initial camera position
		calculateCameraInitialPosition();

		// Place the camera in the initial position
		main_camera.GetComponent<MainCameraController> ().goTo (initial_camera_position,initial_camera_direction);
		
		// Save the identifiers of the input nodes to the map
		saveEntryNodes ();
		
		// Instantiate vehicles
		StartCoroutine(spawnVehicles ());
	}

	private void saveNodePositions ()
	{
		List<string> node_IDs = RoadMap.getNodeIDs ();

		node_positions = new Vector2[node_IDs.Count];

		int i = 0;

		foreach (string ID in node_IDs)
		{
			Vector2 pos = RoadMap.getNodePosition (ID);

			node_positions[i] = new Vector2(pos.x,pos.y);
			i++;
		}
	}
	
	private void calculateCameraInitialPosition ()
	{
		float max_x = node_positions[0].x;
		float min_x = node_positions[0].x;
		float max_z = node_positions[0].y;
		float min_z = node_positions[0].y;
		
		foreach (Vector2 node in node_positions)
		{
			if (node.x > max_x)
			{
				max_x = node.x;
			}
			else if (node.x < min_x)
			{
				min_x = node.x;
			}
			
			if (node.y > max_z)
			{
				max_z = node.y;
			}
			else if (node.y < min_z)
			{
				min_z = node.y;
			}
		}
		initial_camera_position = new Vector3(min_x,20f,min_z);
		initial_camera_direction = new Vector3(max_x - min_x, -20f, max_z-min_z);
		
		float half_grass_ground_padding = Constants.grass_ground_padding/2;
		main_camera.GetComponent<MainCameraController> ().setLimits (max_x+half_grass_ground_padding,min_x-half_grass_ground_padding,max_z+half_grass_ground_padding,min_z-half_grass_ground_padding);
	}
	
	private void saveEntryNodes ()
	{
		entryNodes = new Dictionary<string, EntryNodeInfo>();
		
		List<string> node_IDs = RoadMap.getNodeIDs ();
		TransportType tt;
		
		foreach (string ID in node_IDs)
		{
			if (RoadMap.isEntryNode(ID, out tt))
			{
				EntryNodeInfo new_entry = new EntryNodeInfo();
				new_entry.id = ID;
				new_entry.tt = tt;
				new_entry.tbs = Random.value * 3;
				
				entryNodes.Add(ID,new_entry);
			}
		}
	}
	
	public void publicVehicleDestroyed ()
	{
		num_public_vehicles_running--;
	}
	
	public void privateVehicleDestroyed ()
	{
		num_private_vehicles_running--;
	}
	
	public void goodDriverDestroyed ()
	{
		num_good_drivers_running--;
	}
	
	public void averageDriverDestroyed ()
	{
		num_average_drivers_running--;
	}
	
	public void badDriverDestroyed ()
	{
		num_bad_drivers_running--;
	}
	
	public void synchronizeTrafficLights ()
	{
		RoadMap.synchronizeTrafficLights();
	}
	
	private void updateMaxVehicles ()
	{
		max_vehicles = Mathf.FloorToInt(maxVehicles_slider.GetComponent<Slider>().value);
	}
	
	private DriverType selectDriverToSpawn ()
	{
		int max_good_drivers 	= Mathf.FloorToInt( (float)max_vehicles * (goodDrivers_slider.GetComponent<Slider>().value) );
		int max_average_drivers = Mathf.FloorToInt( (float)max_vehicles * (averageDrivers_slider.GetComponent<Slider>().value) );
		int max_bad_drivers 	= Mathf.FloorToInt( (float)max_vehicles * (badDrivers_slider.GetComponent<Slider>().value) );
		
		int count_error = max_vehicles - max_good_drivers - max_average_drivers - max_bad_drivers;
		
		if (count_error > 0)
		{
			max_good_drivers += count_error;
		}
		
		bool good_allowed	 = (num_good_drivers_running    < max_good_drivers);
		bool average_allowed = (num_average_drivers_running < max_average_drivers);
		bool bad_allowed     = (num_bad_drivers_running     < max_bad_drivers);
		DriverType selectedType;
		
		if (good_allowed && average_allowed && bad_allowed)
		{
			int select = Random.Range(0,3);
			
			if (select == 0)
			{
				selectedType = DriverType.Good;
			}
			else if (select == 1)
			{
				selectedType = DriverType.Average;
			}
			else
			{
				selectedType = DriverType.Bad;
			}
		}
		else if (good_allowed && average_allowed)
		{
			int select = Random.Range(0,2);
			
			if (select == 0)
			{
				selectedType = DriverType.Good;
			}
			else
			{
				selectedType = DriverType.Average;
			}
		}
		else if (good_allowed && bad_allowed)
		{
			int select = Random.Range(0,2);
			
			if (select == 0)
			{
				selectedType = DriverType.Good;
			}
			else
			{
				selectedType = DriverType.Bad;
			}
		}
		else if (average_allowed && bad_allowed)
		{
			int select = Random.Range(0,2);
			
			if (select == 0)
			{
				selectedType = DriverType.Average;
			}
			else
			{
				selectedType = DriverType.Bad;
			}
		}
		else if (good_allowed)
		{
			selectedType = DriverType.Good;
		}
		else if (average_allowed)
		{
			selectedType = DriverType.Average;
		}
		else
		{
			selectedType = DriverType.Bad;
		}
		return selectedType;
	}
	
	private void selectPrefabToSpawn (int num_public_prefabs, int num_private_prefabs, out int selectedType, out int selectedPrefab)
	{
		int max_public_vehicles  = Mathf.FloorToInt( (float)max_vehicles * (publicVehicles_slider.GetComponent<Slider>().value) );
		int max_private_vehicles = Mathf.FloorToInt( (float)max_vehicles * (privateVehicles_slider.GetComponent<Slider>().value) );
		
		int count_error = max_vehicles - max_public_vehicles - max_private_vehicles;
		
		if (count_error > 0)
		{
			max_private_vehicles += count_error;
		}
		
		bool public_allowed  = (num_public_vehicles_running  < max_public_vehicles);
		bool private_allowed = (num_private_vehicles_running < max_private_vehicles);
		
		if (public_allowed && private_allowed)
		{
			selectedType = Random.Range(0,2);
			
			if (selectedType == 0) 	// Public
			{
				selectedPrefab = Random.Range(0,num_public_prefabs);
			}
			else					// Private
			{
				selectedPrefab = Random.Range(0,num_private_prefabs);
			}
		}
		else if (public_allowed)
		{
			selectedType = 0;
			selectedPrefab = Random.Range(0,num_public_prefabs);
		}
		else
		{
			selectedType = 1;
			selectedPrefab = Random.Range(0,num_private_prefabs);
		}
	}

	private IEnumerator spawnVehicles ()
	{
		// Load vehicle prefabs
		GameObject Chevrolet_Camaro_prefab 	= Resources.Load("Prefabs/Vehicles/Chevrolet_Camaro", typeof(GameObject)) as GameObject;
		GameObject green_jeep_prefab 		= Resources.Load("Prefabs/Vehicles/GreenJeep"		, typeof(GameObject)) as GameObject;
		GameObject orange_jeep_prefab 		= Resources.Load("Prefabs/Vehicles/OrangeJeep"		, typeof(GameObject)) as GameObject;
		GameObject bus_prefab 				= Resources.Load("Prefabs/Vehicles/Bus"				, typeof(GameObject)) as GameObject;
		GameObject truck1_prefab 			= Resources.Load("Prefabs/Vehicles/Truck1"			, typeof(GameObject)) as GameObject;
		GameObject Taxi_prefab 				= Resources.Load("Prefabs/Vehicles/Checker_Marathon", typeof(GameObject)) as GameObject;
		Vector2 dir_prefab = new Vector3 (0,1);
		
		int num_public_prefabs = 2;
		int num_private_prefabs = 4;
		GameObject [,] prefab = new GameObject[2,4];
		prefab[0,0] = bus_prefab;
		prefab[0,1] = Taxi_prefab;
		prefab[1,0] = Chevrolet_Camaro_prefab;
		prefab[1,1] = green_jeep_prefab;
		prefab[1,2] = orange_jeep_prefab;
		prefab[1,3] = truck1_prefab;
		
		updateMaxVehicles ();
		
		while (true)
		{	
			//debugMessage ();
			
			while ( (num_public_vehicles_running + num_private_vehicles_running) < max_vehicles )
			{
				//debugMessage ();
				
				if (!SimulationUIController.is_paused)
				{
					int selectedType, selectedPrefab;
					selectPrefabToSpawn (num_public_prefabs, num_private_prefabs, out selectedType, out selectedPrefab);
					GameObject selected_prefab = prefab[selectedType,selectedPrefab];
					List<string> candidates = selectCandidateNodesToSpawn (selected_prefab.GetComponent<VehicleController>().getTransportType());
					GameObject spawned_vehicle = spawnVehicle (selected_prefab, dir_prefab, candidates[Random.Range(0,candidates.Count)], selectDriverToSpawn ());
					
					if (spawned_vehicle != null)
					{
						VehicleController controller = spawned_vehicle.GetComponent<VehicleController>();
						
						TransportType v_tt = controller.getTransportType();
						
						if (v_tt == TransportType.Public)
						{
							num_public_vehicles_running++;
						}
						else
						{
							num_private_vehicles_running++;
						}
						
						DriverType dt = controller.getDriverType();
						
						if (dt == DriverType.Good)
						{
							num_good_drivers_running++;
						}
						else if (dt == DriverType.Average)
						{
							num_average_drivers_running++;
						}
						else
						{
							num_bad_drivers_running++;
						}
					}
					else
					{
						num_spawn_errors++;
					}
				}
				yield return new WaitForSeconds(0.1f); // Time between spawns
				updateMaxVehicles ();
			}
			yield return new WaitForSeconds(1); // Time between attempts of spawn
			updateMaxVehicles ();
		}
	}
	
	private void debugMessage ()
	{
		Debug.Log("Vehicles: Public ("+num_public_vehicles_running+") + Private ("+num_private_vehicles_running+ ")"+
				  " = " + (num_public_vehicles_running + num_private_vehicles_running) + " / " + max_vehicles + "  " +
				  "Drivers: Good ("+num_good_drivers_running+") Average("+num_average_drivers_running+") Bad("+num_bad_drivers_running+")"+
				  " --- Spawn errors: " + num_spawn_errors);
	}

	/**
	 * @brief Instantiante the vehicle prefab in the node node_id
	 * @param[in] prefab The prefab to instantiate
	 * @param[in] prefab_orientation Orientation of the prefab
	 * @param[in] node_id The limit node identifier where the vehicle will be instantiated
	 * @param[in] dt The driver type that will be assigned to the vehicle
	 * @return Returns a reference to the instantiated object
	 * @post If node_id not exist or is not a limit node type or not an input node of vehicles,
	 * no vehicle shall be instantiated
	 */
	private GameObject spawnVehicle (GameObject prefab, Vector2 prefab_orientation, string node_id, DriverType dt)
	{
		TransportType vehicle_tt = prefab.GetComponent<VehicleController>().getTransportType();
		
		// Get the entry points to the lanes
		List<GameObject> startPoints = RoadMap.getLaneStartPoints(node_id);
		
		// Select the lane it will enter
		// If there are multiple lanes of the required type, select randomly
		List<GameObject> candidates = new List<GameObject>();
		
		foreach (GameObject point in startPoints)
		{
			if ((point.name.Contains(Constants.Lane_Name_Public) && vehicle_tt == TransportType.Public) || 
			    (point.name.Contains(Constants.Lane_Name_Normal) && vehicle_tt == TransportType.Private))
			{
				candidates.Add (point);
			}
		}
		
		if (candidates.Count > 0)
		{
			GameObject selected_guide_node = candidates[Random.Range(0,candidates.Count)];
			Vector2 dir_road = RoadMap.entryOrientation(node_id);
			Vector3 dir_road3D = new Vector3(dir_road.x,0,dir_road.y);
			
			Vector3 in_Position = selected_guide_node.transform.position +
								 (dir_road3D * ((prefab.GetComponent<VehicleController>().getVehicleLength()/2)+2)); // Position inside of the lane
			
			// Check if it is possible spawn the vehicle
			if (!Physics.CheckCapsule(selected_guide_node.transform.position, in_Position, Constants.lane_width / 2, Constants.vehicles_layer_mask))
			{
				GameObject vehicle = GameObject.Instantiate (prefab, selected_guide_node.transform.position, Quaternion.LookRotation(dir_road3D)) as GameObject;
				vehicle.tag = Constants.Tag_Vehicle;
				VehicleController controller = vehicle.GetComponent<VehicleController>();
				controller.setGuideNode(selected_guide_node);
				controller.setCurrentSpeed(Constants.urban_speed_limit);
				controller.setDriverType(dt);
				MyUtilities.MoveToLayer(vehicle.transform,LayerMask.NameToLayer(Constants.Layer_Vehicles));
				return vehicle;
			}
			else
			{
				//Debug.LogWarning("Spawn point is occupied.");
				return null;
			}
		}
		else
		{
			Debug.LogWarning("There are no candidates for spawn vehicle.");
			return null;
		}
	}
	
	private List<string> selectCandidateNodesToSpawn (TransportType vehicle_tt)
	{
		List<string> candidates = new List<string>();
		
		foreach (KeyValuePair<string,EntryNodeInfo> node in entryNodes)
		{
			TransportType node_tt = node.Value.tt;
			
			if (node_tt == TransportType.PublicAndPrivate || node_tt == vehicle_tt)
			{
				candidates.Add(node.Value.id);
			}
		}
		return candidates;
	}
}
