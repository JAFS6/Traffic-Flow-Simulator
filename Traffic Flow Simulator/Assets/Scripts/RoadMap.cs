using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public enum NodeType : byte {INTERSECTION, LIMIT, CONTINUATION};
public enum IntersectionType : byte {NORMAL, ROUNDABOUT};

public struct Node
{
	public string id;
	public NodeType node_type;
	public float x;
	public float y;
	public IntersectionType intersection_type;
}

public struct Edge
{
	public string id;
	public string source_id;
	public string destination_id;
	public string name;
	public string src_des;
	public string des_src;
}

public class RoadMap {
	
	private string map_name;
	private Dictionary<string, Node> nodes;
	private Dictionary<string, Edge> edges;
	private Dictionary<string, GameObject> objects;
	
	// Constructor
	public RoadMap (string map_name) {
		Debug.Log ("Creating map");
		this.map_name = map_name;
		this.nodes = new Dictionary<string, Node> ();
		this.edges = new Dictionary<string, Edge> ();
		this.objects = new Dictionary<string, GameObject> ();
	}
	
	/**
	 * @brief Añade un nuevo nodo al mapa
	 * @param[in] id Identificador alfanumérico del nodo
	 * @param[in] node_type Tipo de nodo (0: nodo de cruce, 1: nodo de límite de vía, 2: nodo de continuación)
	 * @param[in] x Coordenada X
	 * @param[in] y Coordenada Y
	 * @param[in] intersection_type Tipo de cruce: normal (0) o rotonda (1) (Solo se aplica a los nodos de tipo cruce)
	 * @pre El id debe ser distinto a todos los ids ya insertados, si coincide con alguno, el nuevo nodo no se insertará
	 */
	public void addNode (string id, NodeType node_type, float x, float y, IntersectionType intersection_type = IntersectionType.NORMAL) {
		
		if ( !nodes.ContainsKey (id) ) {
			Debug.Log ("Adding node "+id+" to the map");
			
			Node newnode = new Node ();
			newnode.id = id;
			newnode.node_type = node_type;
			newnode.x = x;
			newnode.y = y;
			newnode.intersection_type = intersection_type;
			nodes.Add (newnode.id, newnode);
			
			Debug.Log ("Map now has "+nodes.Count+" nodes");
		}
	}
	
	/**
	 * @brief Añade un nuevo arco al mapa
	 * @param[in] id Identificador alfanumérico del arco
	 * @param[in] source_id Identificador del nodo origen
	 * @param[in] destination_id Identificador del nodo destino
	 * @param[in] name Nombre de la vía
	 * @param[in] src_des Cadena de tipos de carriles en el sentido origen-destino
	 * @param[in] des_src Cadena de tipos de carriles en el sentido destino-origen
	 * @pre El id debe ser distinto a todos los ids ya insertados, si coincide con alguno, el nuevo arco no se añadirá
	 * @pre Los ids de los nodos origen y destino deben existir, si no existe alguno el arco no se insertará
	 */
	public void addEdge (string id, string source_id, string destination_id, string name, string src_des, string des_src) {
		
		if ( !edges.ContainsKey (id) && nodes.ContainsKey (source_id) && nodes.ContainsKey (destination_id)) {
			Edge newedge = new Edge ();
			newedge.id = id;
			newedge.source_id = source_id;
			newedge.destination_id = destination_id;
			newedge.name = name;
			newedge.src_des = src_des;
			newedge.des_src = des_src;
			edges.Add (newedge.id, newedge);
		}
	}
	
	/**
	 * @brief Hace que el nodo 'id' cambie su tipo al tipo pasado como argumento
	 * @param[in] id Identificador alfanumérico del nodo
	 * @param[in] new_intersection_type Nuevo tipo de intersección
	 * @pre Si el nodo no existe no se hace nada
	 */
	public void setIntersectionType (string id, IntersectionType new_intersection_type) {
		
		if ( nodes.ContainsKey (id) ) {
			Node aux_node = nodes[id];
			aux_node.intersection_type = new_intersection_type;
			nodes[id] = aux_node;
		}
	}
	
	/**
	 * @brief Dibuja el mapa en el entorno 3D
	 */
	public void draw () {
		
		foreach (KeyValuePair<string, Edge> edge in edges){
			drawEdge (edge.Key);
		}
		
		foreach (KeyValuePair<string, Node> node in nodes){
			drawNode (node.Key);
		}
	}
	
	private void drawNode (string node_id) {
		Debug.Log ("Drawing node "+node_id);
		
		GameObject road_prefab = Resources.Load("Prefabs/Road", typeof(GameObject)) as GameObject;
		
		if (road_prefab == null) {
			Debug.Log ("road_prefab is null");
		}
		
		Node n = nodes[node_id];
		
		Vector3 pos = new Vector3 (n.x, 0, n.y);
		GameObject aux_road = GameObject.Instantiate (road_prefab, pos, Quaternion.identity) as GameObject;
		objects.Add (node_id, aux_road);
	}
	
	private void drawEdge (string edge_id) {
		/*Edge e = edges [edge_id];
		Node src_node = nodes[e.source_id];
		Node dst_node = nodes[e.destination_id];
		
		Vector3 src_pos = new Vector3( (dst_node.x - src_node.x)/2, 0, (dst_node.y - src_node.y)/2);
		Instantiate(road_prefab, src_pos, Quaternion.identity);*/
	}
}
