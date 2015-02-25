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
using System.Xml;
using System.IO;
using System.Collections.Generic;

public struct EntryNodeInfo
{
	public string id;
	public TransportType tt; // Transport Type
	public float tbs; 		 // Time Between Spawns
}

public class ApplicationController : MonoBehaviour {
	
	public static string map_filename = "";
	
	// Control variables to instantiate vehicles
	private Dictionary<string, EntryNodeInfo> entryNodes;

	// Control variables predetermined positions of the camera
	private GameObject main_camera;
	private Vector3 initial_camera_position;
	private Vector3 initial_camera_direction;
	private Vector2[] node_positions;

	// Actions to take when the application starts
	void Start () {
		
		// Get the reference to the simulator's camera
		main_camera = GameObject.Find("Main Camera");

		// Create new map
		RoadMap.CreateNewMap(map_filename);

		// Load map data
		MapLoader loader = new MapLoader();
		loader.LoadMap(map_filename);

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

	private void saveNodePositions () {
		List<string> node_IDs = RoadMap.getNodeIDs ();

		node_positions = new Vector2[node_IDs.Count];

		int i = 0;

		foreach (string ID in node_IDs) {
			Vector2 pos = RoadMap.getNodePosition (ID);

			node_positions[i] = new Vector2(pos.x,pos.y);
			i++;
		}
	}
	
	private void calculateCameraInitialPosition () {
		float max_x = node_positions[0].x;
		float min_x = node_positions[0].x;
		float max_z = node_positions[0].y;
		float min_z = node_positions[0].y;
		
		foreach (Vector2 node in node_positions) {
			
			if (node.x > max_x) {
				max_x = node.x;
			}
			else if (node.x < min_x) {
				min_x = node.x;
			}
			
			if (node.y > max_z) {
				max_z = node.y;
			}
			else if (node.y < min_z) {
				min_z = node.y;
			}
		}
		initial_camera_position = new Vector3(min_x,20f,min_z);
		initial_camera_direction = new Vector3(max_x - min_x, -20f, max_z-min_z);
		
		main_camera.GetComponent<MainCameraController> ().setLimits (max_x,min_x,max_z,min_z);
	}
	
	private void saveEntryNodes () {
		entryNodes = new Dictionary<string, EntryNodeInfo>();
		
		List<string> node_IDs = RoadMap.getNodeIDs ();
		TransportType tt;
		
		foreach (string ID in node_IDs) {
		
			if (RoadMap.isEntryNode(ID, out tt)) {
				
				EntryNodeInfo new_entry = new EntryNodeInfo();
				new_entry.id = ID;
				new_entry.tt = tt;
				new_entry.tbs = Random.value * 3;
				
				entryNodes.Add(ID,new_entry);
			}
		}
	}

	private IEnumerator spawnVehicles () {
		// Load vehicle prefabs
		GameObject Chevrolet_Camaro_prefab = Resources.Load("Prefabs/Chevrolet_Camaro", typeof(GameObject)) as GameObject;
		GameObject green_jeep_prefab = Resources.Load("Prefabs/GreenJeep", typeof(GameObject)) as GameObject;
		GameObject orange_jeep_prefab = Resources.Load("Prefabs/OrangeJeep", typeof(GameObject)) as GameObject;
		GameObject bus_prefab = Resources.Load("Prefabs/Bus", typeof(GameObject)) as GameObject;
		GameObject truck1_prefab = Resources.Load("Prefabs/Truck1", typeof(GameObject)) as GameObject;
		GameObject Pontiac_GTO_67_prefab = Resources.Load("Prefabs/Pontiac_GTO_67", typeof(GameObject)) as GameObject;
		GameObject Taxi_prefab = Resources.Load("Prefabs/Checker_Marathon", typeof(GameObject)) as GameObject;
		Vector2 dir_prefab = new Vector3 (0,1);
		
		int num_prefabs = 7;
		GameObject [] prefab = new GameObject[num_prefabs];
		prefab[0] = Chevrolet_Camaro_prefab;
		prefab[1] = green_jeep_prefab;
		prefab[2] = orange_jeep_prefab;
		prefab[3] = bus_prefab;
		prefab[4] = truck1_prefab;
		prefab[5] = Pontiac_GTO_67_prefab;
		prefab[6] = Taxi_prefab;
		
		// Get the node ids
		List<string> node_IDs = RoadMap.getNodeIDs();
		int random;
		
		while (true) {
			if (!SimulationUIController.is_paused) {
				foreach (string id in node_IDs) {
					random = Random.Range(0,num_prefabs);
					spawnVehicle (prefab[random], dir_prefab, id);
				}
			}
			yield return new WaitForSeconds(5);
		}
	}

	/**
	 * @brief Instantiante the vehicle prefab in the node node_id
	 * @param[in] prefab The prefab to instantiate
	 * @param[in] prefab_orientation Orientation of the prefab
	 * @param[in] node_id The limit node identifier where the vehicle will be instantiated
	 * @return Returns a reference to the instantiated object
	 * @post If node_id not exist or is not a limit node type or not an input node of vehicles,
	 * no vehicle shall be instantiated
	 */
	private GameObject spawnVehicle (GameObject prefab, Vector2 prefab_orientation, string node_id) {

		NodeType node_type = RoadMap.getNodeType (node_id);
		TransportType valid_transports_types;

		if (RoadMap.existsNode(node_id) && node_type == NodeType.Limit && RoadMap.isEntryNode(node_id, out valid_transports_types)) {
		
			// Get the type of vehicle
			TransportType vehicle_tt = prefab.GetComponent<VehicleController>().transport_type;
			
			// Check if it can instantiate the vehicle due to their type
			if (valid_transports_types == vehicle_tt || valid_transports_types == TransportType.PublicAndPrivate) {
			
				// Get the entry points to the lanes
				List<GameObject> startPoints = RoadMap.getLaneStartPoints(node_id);
				
				// Select the lane it will enter
				// If there are multiple lanes of the required type, select randomly
				List<GameObject> candidates = new List<GameObject>();
				
				startPoints.ForEach(delegate(GameObject point) {
					
					if ((point.name == Constants.Lane_Name_Public && vehicle_tt == TransportType.Public) || 
					    (point.name == Constants.Lane_Name_Normal && vehicle_tt == TransportType.Private)) {
						candidates.Add (point);
					}
				});
				
				if (candidates.Count > 0) {
					Vector3 start_position = candidates[0].transform.position;
					Vector2 dir_road = RoadMap.entryOrientation(node_id);
					Vector3 dir_road3D = new Vector3(dir_road.x,0,dir_road.y);
					Vector3 pos = new Vector3 (start_position.x, Constants.road_thickness/2, start_position.z);
					GameObject vehicle = GameObject.Instantiate (prefab, pos, Quaternion.LookRotation(dir_road3D)) as GameObject;
					vehicle.tag = Constants.Tag_Vehicle;
					vehicle.GetComponent<VehicleController>().current_location = RoadMap.getLimitEdge(node_id);
					MyUtilitiesClass.MoveToLayer(vehicle.transform,LayerMask.NameToLayer(Constants.Layer_Vehicles));
					return vehicle;
				}
				else {
					return null;
				}
			}
			else {
				return null;
			}
		}
		return null;
	}
}
