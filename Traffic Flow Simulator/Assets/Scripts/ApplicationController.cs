﻿using UnityEngine;
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
	private MapLoader map_loader;

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

	private void DebugMapLoader () {
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
