using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public struct EntryNodeInfo
{
	public string id;
	public TransportType tt; // Transport Type
	public float tbs; // Time Between Spawns
}

public class ApplicationController : MonoBehaviour {

	public float initial_camera_position_x = 500f;
	public float initial_camera_position_y = 15f;
	public float initial_camera_position_z = 480f;

	// Variables de control del mapa
	private RoadMap roadMap;
	private MapLoader map_loader;
	
	// Variables de control para instanciar vehiculos
	private Dictionary<string, EntryNodeInfo> entryNodes;

	// Variables de control de las posiciones predeterminadas de la camara
	private GameObject main_camera;
	private Vector2[] node_positions;
	private int camera_node = 0; // Nodo en el que se situa la camara
	private float camera_height = 5f; // Altura de la camara al cambiar de nodo

	// Acciones a realizar cuando se inicia la aplicacion
	void Start () {

		Debug.Log("Starting application");

		// Obtener la referencia a la camara del simulador
		main_camera = GameObject.Find("Main Camera");

		// Crear mapa nuevo
		//roadMap = new RoadMap("ejemplo_topologia");
		roadMap = new RoadMap("ejemplo2");

		// Cargar los datos del mapa
		//map_loader = new MapLoader (ref roadMap);
		//map_loader.LoadMap("ejemplo_topologia");
		DebugMapLoader();

		// Dibujar el suelo base
		drawGround ();

		// Dibujar el mapa
		roadMap.draw ();

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
			camera_node = (camera_node + 1) % roadMap.getNodeCount();
			main_camera.GetComponent<MainCameraController> ().goTo (node_positions[camera_node].x, camera_height, node_positions[camera_node].y);
		}
		else if (Input.GetKeyDown (KeyCode.Alpha1)) {
			camera_node = (camera_node - 1) % roadMap.getNodeCount();

			if (camera_node < 0) {
				camera_node = roadMap.getNodeCount() + camera_node;
			}
			main_camera.GetComponent<MainCameraController> ().goTo (node_positions[camera_node].x, camera_height, node_positions[camera_node].y);
		}
	}

	private void DebugMapLoader () {
		roadMap.addNode ("n0", NodeType.CONTINUATION, 500, 600);
		roadMap.addNode ("n1", NodeType.LIMIT, 600, 700);
		roadMap.addNode ("n2", NodeType.LIMIT, 400, 500);
		roadMap.addNode ("n3", NodeType.INTERSECTION, 500, 500, IntersectionType.NORMAL);
		roadMap.addNode ("n4", NodeType.LIMIT, 600, 500);
		roadMap.addNode ("n5", NodeType.LIMIT, 500, 400);

		roadMap.addEdge ("a0", "n0", "n1", "", "PNN", "PN");
		roadMap.addEdge ("a1", "n0", "n3", "", "PN", "PNN");
		roadMap.addEdge ("a2", "n2", "n3", "", "N", "0");
		roadMap.addEdge ("a3", "n3", "n4", "", "NN", "NN");
		roadMap.addEdge ("a4", "n3", "n5", "", "NN", "0");
	}
	
	private void DebugMapLoader2 () {
		roadMap.addNode ("n0", NodeType.LIMIT, 2168, 1044);
		roadMap.addNode ("n1", NodeType.CONTINUATION, 1095, 751);
		roadMap.addNode ("n2", NodeType.INTERSECTION, 2168, 751, IntersectionType.NORMAL);
		roadMap.addNode ("n3", NodeType.LIMIT, 2623, 751);
		roadMap.addNode ("n4", NodeType.LIMIT, 53, 381);
		roadMap.addNode ("n5", NodeType.INTERSECTION, 1095, 381, IntersectionType.NORMAL);
		roadMap.addNode ("n6", NodeType.INTERSECTION, 2168, 381, IntersectionType.NORMAL);
		roadMap.addNode ("n7", NodeType.LIMIT, 2723, 381);
		roadMap.addNode ("n8", NodeType.LIMIT, 1095, 7);
		roadMap.addNode ("n9", NodeType.LIMIT, 2168, 83);
		
		roadMap.addEdge ("a0", "n0", "n2", "", "PN", "N");
		roadMap.addEdge ("a1", "n1", "n2", "", "N", "N");
		roadMap.addEdge ("a2", "n2", "n3", "", "0", "NN");
		roadMap.addEdge ("a3", "n1", "n5", "", "N", "N");
		roadMap.addEdge ("a4", "n2", "n6", "", "PN", "N");
		roadMap.addEdge ("a5", "n4", "n5", "", "PNN", "PNN");
		roadMap.addEdge ("a6", "n5", "n6", "", "PNN", "PNN");
		roadMap.addEdge ("a7", "n6", "n7", "", "PNN", "PNN");
		roadMap.addEdge ("a8", "n5", "n8", "", "N", "N");
		roadMap.addEdge ("a9", "n6", "n9", "", "PN", "N");
	}

	/**
	 * @brief Dibuja el suelo de hierba
	 */
	private void drawGround () {
		List<string> node_IDs = roadMap.getNodeIDs ();

		Vector2 first_pos = roadMap.getNodePosition (node_IDs [0]);

		float min_x = first_pos.x;
		float max_x = first_pos.x;
		float min_y = first_pos.y;
		float max_y = first_pos.y;

		foreach (string ID in node_IDs) {
			Vector2 pos = roadMap.getNodePosition (ID);

			if (pos.x < min_x) {
				min_x = pos.x;
			}
			else if (pos.x > max_x) {
				max_x = pos.x;
			}

			if (pos.y < min_y) {
				min_y = pos.y;
			}
			else if (pos.y > max_y) {
				max_y = pos.y;
			}
		}

		max_x += 100;
		max_y += 100;
		min_x -= 100;
		min_y -= 100;

		Material grass_material = Resources.Load ("Materials/Grass", typeof(Material)) as Material;

		GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
		ground.name = "Ground";
		ground.transform.localScale = new Vector3((max_x-min_x)/10, 1, (max_y-min_y)/10); // Se divide por 10 porque las medidas del plano de unity son 10x10
		ground.renderer.material = grass_material;
		ground.renderer.material.mainTextureScale = new Vector2(ground.transform.localScale.x, ground.transform.localScale.z);

		Vector3 ground_position = new Vector3((max_x+min_x)/2,0,(max_y+min_y)/2);
		ground.transform.position = ground_position;
	}

	private void saveNodePositions () {
		List<string> node_IDs = roadMap.getNodeIDs ();

		node_positions = new Vector2[node_IDs.Count];

		int i = 0;

		foreach (string ID in node_IDs) {
			Vector2 pos = roadMap.getNodePosition (ID);

			node_positions[i] = new Vector2(pos.x,pos.y);
			i++;
		}
	}
	
	private void saveEntryNodes () {
		entryNodes = new Dictionary<string, EntryNodeInfo>();
		
		List<string> node_IDs = roadMap.getNodeIDs ();
		TransportType tt;
		
		foreach (string ID in node_IDs) {
		
			if (roadMap.isEntryNode(ID, out tt)) {
				
				EntryNodeInfo new_entry = new EntryNodeInfo();
				new_entry.id = ID;
				new_entry.tt = tt;
				new_entry.tbs = Random.value * 3;
				
				entryNodes.Add(ID,new_entry);
			}
		}
	}

	private IEnumerator spawnVehicles () {
		GameObject car_prefab = Resources.Load("Prefabs/Sport_Car", typeof(GameObject)) as GameObject;
		Vector2 dir_prefab = new Vector3 (0,1);
		
		while (true) {
			spawnVehicle (car_prefab, dir_prefab, "n2");
			spawnVehicle (car_prefab, dir_prefab, "n1");
			yield return new WaitForSeconds(5);
		}
	}

	/**
	 * @brief Instancia el vehiculo prefab en el nodo node_id
	 * @param[in] prefab El prefab a instanciar
	 * @param[in] prefab_orientation La orientacion del prefab
	 * @param[in] node_id El identificador del nodo limite donde se instanciara el vehiculo
	 * @post Si node_id no existe o no es un nodo de tipo limite no se instanciara el vehiculo
	 */
	private void spawnVehicle (GameObject prefab, Vector2 prefab_orientation, string node_id) {

		NodeType node_type = roadMap.getNodeType (node_id);

		if (node_type == NodeType.LIMIT) {
			Vector2 node_position = roadMap.getNodePosition (node_id);

			string edge_id = roadMap.edgeLimit(node_id);

			Vector2 dir_road = roadMap.entryOrientation(node_id);

			Quaternion q = Quaternion.AngleAxis(5,new Vector3(0,1,0)); // Rotacion de 5 grados hacia la derecha respecto al eje y

			Vector3 dir_road_fixed = new Vector3(dir_road.x,0,dir_road.y);

			dir_road_fixed = q * dir_road_fixed;

			Vector3 pos = new Vector3 (node_position.x,RoadMap.road_thickness/2,node_position.y);
			GameObject vehicle = GameObject.Instantiate (prefab, pos, Quaternion.LookRotation(dir_road_fixed)) as GameObject;
			vehicle.tag = "Vehicle";
		}
		else {
			Debug.Log ("Node ID: "+node_id+" is not a limit node ID");
		}
	}
}
