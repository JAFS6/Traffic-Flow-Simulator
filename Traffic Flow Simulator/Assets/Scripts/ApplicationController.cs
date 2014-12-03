using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public class ApplicationController : MonoBehaviour {

	public float initial_camera_position_x = 500f;
	public float initial_camera_position_y = 15f;
	public float initial_camera_position_z = 480f;

	// Variables de control del mapa
	private RoadMap roadMap;

	// Variables de control de la carga del mapa
	private int nodos_control = 1; // Numero de nodos a procesar en MapLoader antes de devolver el control a Unity

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

		//LoadMap("ejemplo_topologia");

		// Crear mapa nuevo
		roadMap = new RoadMap("ejemplo2");

		// Cargar los datos del mapa
		DebugMapLoader();

		// Dibujar el suelo base
		drawGround ();

		// Dibujar el mapa
		roadMap.draw ();

		// Guardar las posiciones de los nodos para posicionar la camara
		saveNodePositions ();

		// Colocar la camara en la posicion inicial
		main_camera.GetComponent<MainCameraController> ().goTo (initial_camera_position_x,initial_camera_position_y,initial_camera_position_z);

		// Instanciar vehiculo de prueba
		spawnVehicle ();
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

	private void LoadMap (string nombre_fichero_mapa) {
		// Liberar recursos en caso de que hubiese un mapa previo
		Unload();
		
		// Preparar nombre del fichero del mapa a cargar
		string nombre_fichero_completo = Application.dataPath + "/Data/Maps/" + nombre_fichero_mapa + ".graphml";

		roadMap = new RoadMap(nombre_fichero_mapa);

		StartCoroutine( MapLoader(nombre_fichero_completo) );
	}

	private void Unload () {
		
	}

	private IEnumerator MapLoader (string nombre_fichero_completo) {

		// Variables para tomar valores por defecto
		NodeType node_type_default = NodeType.INTERSECTION;
		IntersectionType intersection_type_default = IntersectionType.NORMAL;
		float x_default = 0.0f;
		float y_default = 0.0f;
		string name_default = "";
		string src_des_default = "0";
		string des_src_default = "0";

		// Variables para tomar valores leidos
		string id = "";
		NodeType node_type_value = NodeType.INTERSECTION;
		IntersectionType intersection_type_value = IntersectionType.NORMAL;
		float x_value = 0.0f;
		float y_value = 0.0f;
		string source_id_value = "";
		string destination_id_value = "";
		string name_value = "";
		string src_des_value = "0";
		string des_src_value = "0";
		string key = "";

		int j = 0;
		string attribute = "";
		bool parsing_edges = false;

		XmlReader reader = XmlReader.Create(nombre_fichero_completo);

		while (reader.Read()) {
			// Detectar los elementos de inicio como por ejemplo <key> pero no </key>
			if (reader.IsStartElement()) {
				// Obtener el nombre del elemento y elegir en funcion del mismo
				switch (reader.Name) {
					case "key":
						attribute = reader["id"];
						
						if (attribute != null) {
							id = attribute;
						}
						else {
							id = "";
						}
						break;

					case "default":
						if (id != "") {
							if (reader.Read()) {
								switch (id) {
									case "node_type":
										node_type_default = (NodeType) XmlConvert.ToByte(reader.Value.Trim());
										break;
									case "intersection_type":
										intersection_type_default = (IntersectionType) XmlConvert.ToByte(reader.Value.Trim());
										break;
									case "pos_x":
										x_default = (float) XmlConvert.ToDouble(reader.Value.Trim());
										break;
									case "pos_y":
										y_default = (float) XmlConvert.ToDouble(reader.Value.Trim());
										break;
									case "name":
										name_default = reader.Value.Trim();
										break;
									case "src_des":
										src_des_default = reader.Value.Trim();
										break;
									case "des_src":
										des_src_default = reader.Value.Trim();
										break;
								}
								id = "";
							}
						}
						break;

					case "graph":
						id = "";
						break;

					case "node":
						if (id == "") { // Primer nodo
							attribute = reader["id"];
							
							if (attribute != null) {
								id = attribute;
							}
						}
						else { // Resto de nodos
							// Guardar nodo anterior
							SaveNode(id,node_type_value,x_value,y_value,intersection_type_value);
							
							// Poner valores default
							node_type_value = node_type_default;
							x_value = x_default;
							y_value = y_default;
							intersection_type_value = intersection_type_default;

							// Leer id del siguiente nodo
							attribute = reader["id"];
							
							if (attribute != null) {
								id = attribute;
							}
						}
						break;

					case "edge":
						if (id == "" && !parsing_edges) { // Primer arco
							parsing_edges = true;
							// Guardar ultimo nodo
							SaveNode(id,node_type_value,x_value,y_value,intersection_type_value);
							
							// Poner valores defautl
							source_id_value = "";
							destination_id_value = "";
							name = "";
							src_des_value = "";
							des_src_value = "";

							// Leer primer arco
							attribute = reader["id"];
							
							if (attribute != null) {
								id = attribute;
							}

							attribute = reader["source"];

							if (attribute != null) {
								source_id_value = attribute;
							}

							attribute = reader["target"];

							if (attribute != null) {
								destination_id_value = attribute;
							}
						}
						else if (parsing_edges) { // Resto de arcos
							// Guardar arco anterior
							SaveEdge(id,source_id_value,destination_id_value,name,src_des_value,des_src_value);
							
							// Poner valores defautl
							source_id_value = "";
							destination_id_value = "";
							name = "";
							src_des_value = "";
							des_src_value = "";
						
						// Leer id del siguiente nodo
							attribute = reader["id"];
							
							if (attribute != null) {
								id = attribute;
							}
						}
						break;

					case "data":
						if (id != null) {
							attribute = reader["key"];
							
							if (attribute != null) {
								key = attribute;
								
								if (reader.Read()) {

									switch (key) {
										case "node_type":
											node_type_value = (NodeType) XmlConvert.ToByte(reader.Value.Trim());
											break;
										case "intersection_type":
											intersection_type_value = (IntersectionType) XmlConvert.ToByte(reader.Value.Trim());
											break;
										case "pos_x":
											x_value = (float) XmlConvert.ToDouble(reader.Value.Trim());
											break;
										case "pos_y":
											y_value = (float) XmlConvert.ToDouble(reader.Value.Trim());
											break;
										case "name":
											name_value = reader.Value.Trim();
											break;
										case "src_des":
											src_des_value = reader.Value.Trim();
											break;
										case "des_src":
											des_src_value = reader.Value.Trim();
											break;
									}
								}
							}
						}
						break;
				}
			}

			j++;
			// Devolver el control a Unity
			if (j % nodos_control == 0) {
				yield return true;
			}
		} // while (reader.Read())

		// Guardar ultimo arco
		SaveEdge(id,source_id_value,destination_id_value,name,src_des_value,des_src_value);
	}
	
	private void SaveNode (string id, NodeType node_type, float x, float y, IntersectionType intersection_type) {
		// Guardar nodo anterior
		if (node_type == NodeType.INTERSECTION) {
			roadMap.addNode(id,node_type,x,y,intersection_type);
		}
		else {
			roadMap.addNode(id,node_type,x,y);
		}
	}

	private void SaveEdge (string id, string source_id, string destination_id, string name, string src_des, string des_src) {
		roadMap.addEdge (id, source_id, destination_id, name, src_des, des_src);
	}

	static byte[] GetBytes (string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	private void DebugMapLoader () {
		Debug.Log ("Starting DebugMapLoader");
		
		roadMap.addNode ("n0", NodeType.CONTINUATION, 500, 1000);
		roadMap.addNode ("n1", NodeType.LIMIT, 1000, 1200);
		roadMap.addNode ("n2", NodeType.LIMIT, 0, 500);
		roadMap.addNode ("n3", NodeType.INTERSECTION, 500, 500, IntersectionType.NORMAL);
		roadMap.addNode ("n4", NodeType.LIMIT, 1000, 500);
		roadMap.addNode ("n5", NodeType.LIMIT, 500, 0);

		roadMap.addEdge ("a0", "n0", "n1", "", "PNN", "PN");
		roadMap.addEdge ("a1", "n0", "n3", "", "PN", "PNN");
		roadMap.addEdge ("a2", "n2", "n3", "", "N", "0");
		roadMap.addEdge ("a3", "n3", "n4", "", "NN", "NN");
		roadMap.addEdge ("a4", "n3", "n5", "", "NN", "0");
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

	private void spawnVehicle () {
		GameObject car_prefab = Resources.Load("Prefabs/Sport_Car", typeof(GameObject)) as GameObject;
		Vector2 n0_pos = roadMap.getNodePosition ("n3");
		Vector2 dir_prefab = new Vector2 (0,1);
		Vector2 dir_road = new Vector2 (0.2f,1);
		Vector3 pos = new Vector3 (n0_pos.x,RoadMap.road_thickness/2,n0_pos.y);
		GameObject car = GameObject.Instantiate (car_prefab, pos, Quaternion.AngleAxis(MyMathClass.RotationAngle(dir_prefab,dir_road),Vector3.up)) as GameObject;
		//car.GetComponent<VehicleController> ().setSpeed (1.38f);
		car.GetComponent<VehicleController> ().setDirection (new Vector3(dir_road.x,0,dir_road.y));
	}
}
