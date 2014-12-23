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

	public float initial_camera_position_x = 500f;
	public float initial_camera_position_y = 15f;
	public float initial_camera_position_z = 480f;
	
	// Variables de control para instanciar vehiculos
	private Dictionary<string, EntryNodeInfo> entryNodes;

	// Variables de control de las posiciones predeterminadas de la camara
	private GameObject main_camera;
	private Vector2[] node_positions;
	private int camera_node = 0; // Nodo en el que se situa la camara
	private float camera_height = 5f; // Altura de la camara al cambiar de nodo

	// Acciones a realizar cuando se inicia la aplicacion
	void Start () {
		
		// Obtener la referencia a la camara del simulador
		main_camera = GameObject.Find("Main Camera");

		// Crear mapa nuevo
		//RoadMap.CreateNewMap("ejemplo_topologia");
		RoadMap.CreateNewMap("ejemplo2");

		// Cargar los datos del mapa
		//MapLoader.LoadMap("ejemplo_topologia");
		DebugMapLoader();

		// Dibujar el mapa
		RoadMap.draw ();

		// Guardar las posiciones de los nodos para posicionar la camara
		saveNodePositions ();

		// Colocar la camara en la posicion inicial
		main_camera.GetComponent<MainCameraController> ().goTo (initial_camera_position_x,initial_camera_position_y,initial_camera_position_z);
		
		// Guardar los identificadores de los nodos de entrada al mapa
		saveEntryNodes ();
		
		// Instanciar vehiculos
		StartCoroutine(spawnVehicles ());
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			camera_node = (camera_node + 1) % RoadMap.getNodeCount();
			main_camera.GetComponent<MainCameraController> ().goTo (node_positions[camera_node].x, camera_height, node_positions[camera_node].y);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha1)) {
			camera_node = (camera_node - 1) % RoadMap.getNodeCount();

			if (camera_node < 0) {
				camera_node = RoadMap.getNodeCount() + camera_node;
			}
			main_camera.GetComponent<MainCameraController> ().goTo (node_positions[camera_node].x, camera_height, node_positions[camera_node].y);
		}
	}

	private void DebugMapLoader () {
		RoadMap.addNode ("n0", NodeType.Continuation, 510, 600);
		RoadMap.addNode ("n1", NodeType.Limit, 600, 700);
		RoadMap.addNode ("n2", NodeType.Limit, 400, 500);
		RoadMap.addNode ("n3", NodeType.Intersection, 500, 500, IntersectionType.Normal);
		RoadMap.addNode ("n4", NodeType.Limit, 600, 500);
		RoadMap.addNode ("n5", NodeType.Limit, 500, 400);

		RoadMap.addEdge ("a0", "n0", "n1", "", "PNN", "PN");
		RoadMap.addEdge ("a1", "n0", "n3", "", "PN", "PNN");
		RoadMap.addEdge ("a2", "n2", "n3", "", "N", "0");
		RoadMap.addEdge ("a3", "n3", "n4", "", "NN", "NN");
		RoadMap.addEdge ("a4", "n3", "n5", "", "NN", "0");
	}
	
	private void DebugMapLoader2 () {
		RoadMap.addNode ("n0", NodeType.Limit, 2168, 1044);
		RoadMap.addNode ("n1", NodeType.Continuation, 1095, 751);
		RoadMap.addNode ("n2", NodeType.Intersection, 2168, 751, IntersectionType.Normal);
		RoadMap.addNode ("n3", NodeType.Limit, 2623, 751);
		RoadMap.addNode ("n4", NodeType.Limit, 53, 381);
		RoadMap.addNode ("n5", NodeType.Intersection, 1095, 381, IntersectionType.Normal);
		RoadMap.addNode ("n6", NodeType.Intersection, 2168, 381, IntersectionType.Normal);
		RoadMap.addNode ("n7", NodeType.Limit, 2723, 381);
		RoadMap.addNode ("n8", NodeType.Limit, 1095, 7);
		RoadMap.addNode ("n9", NodeType.Limit, 2168, 83);
		
		RoadMap.addEdge ("a0", "n0", "n2", "", "PN", "N");
		RoadMap.addEdge ("a1", "n1", "n2", "", "N", "N");
		RoadMap.addEdge ("a2", "n2", "n3", "", "0", "NN");
		RoadMap.addEdge ("a3", "n1", "n5", "", "N", "N");
		RoadMap.addEdge ("a4", "n2", "n6", "", "PN", "N");
		RoadMap.addEdge ("a5", "n4", "n5", "", "PNN", "PNN");
		RoadMap.addEdge ("a6", "n5", "n6", "", "PNN", "PNN");
		RoadMap.addEdge ("a7", "n6", "n7", "", "PNN", "PNN");
		RoadMap.addEdge ("a8", "n5", "n8", "", "N", "N");
		RoadMap.addEdge ("a9", "n6", "n9", "", "PN", "N");
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
		GameObject sport_car_prefab = Resources.Load("Prefabs/Sport_Car", typeof(GameObject)) as GameObject;
		GameObject green_jeep_prefab = Resources.Load("Prefabs/GreenJeep", typeof(GameObject)) as GameObject;
		GameObject orange_jeep_prefab = Resources.Load("Prefabs/OrangeJeep", typeof(GameObject)) as GameObject;
		Vector2 dir_prefab = new Vector3 (0,1);
		
		GameObject [] prefab = new GameObject[3];
		prefab[0] = sport_car_prefab;
		prefab[1] = green_jeep_prefab;
		prefab[2] = orange_jeep_prefab;
		
		GameObject vehicle = null;
		int random;
		
		while (true) {
			random = Random.Range(0,3);
			vehicle = spawnVehicle (prefab[random], dir_prefab, "n2");
			random = Random.Range(0,3);
			vehicle = spawnVehicle (prefab[random], dir_prefab, "n1");
			yield return new WaitForSeconds(5);
		}
	}

	/**
	 * @brief Instancia el vehiculo prefab en el nodo node_id
	 * @param[in] prefab El prefab a instanciar
	 * @param[in] prefab_orientation La orientacion del prefab
	 * @param[in] node_id El identificador del nodo limite donde se instanciara el vehiculo
	 * @return Devuelve una referencia al objeto instanciado
	 * @post Si node_id no existe o no es un nodo de tipo limite o no es un nodo de entrada de vehiculos
	 * no se instanciara el vehiculo
	 */
	private GameObject spawnVehicle (GameObject prefab, Vector2 prefab_orientation, string node_id) {

		NodeType node_type = RoadMap.getNodeType (node_id);
		TransportType valid_transports_types;

		if (RoadMap.existsNode(node_id) && node_type == NodeType.Limit && RoadMap.isEntryNode(node_id, out valid_transports_types)) {
		
			// Obtener el tipo de vehiculo
			TransportType vehicle_tt = prefab.GetComponent<VehicleController>().transport_type;
			
			// Comprobar si se puede instanciar el vehiculo debido a su tipo
			if (valid_transports_types == vehicle_tt || valid_transports_types == TransportType.PublicAndPrivate) {
			
				// Obtener los puntos de entrada a los carriles
				List<GameObject> startPoints = RoadMap.getLaneStartPoints(node_id);
				
				// Seleccionar el carril por el que entrara
				// Si hay varios del tipo requerido seleccionarlo aleatoriamente
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
		else {
			Debug.Log ("Node ID: "+node_id+" is not a valid node ID");
		}
		return null;
	}
}
