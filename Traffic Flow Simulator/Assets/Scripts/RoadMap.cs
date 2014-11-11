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

	public const float lane_width = 3f;
	public const float line_width = 0.1f;
	public const float hard_shoulder_width = 1f;
	public const float limit_height = 10f;
	public const float limit_depth = 3f;
	public const float road_thickness = 0.1f;
	public const float line_thickness = 0.01f;
	public const float intersection_margin = 20f;

	private string map_name;
	private Dictionary<string, Node> nodes;
	private Dictionary<string, Edge> edges;
	//private Dictionary<string, GameObject> objects;
	
	// Constructor
	public RoadMap (string map_name) {
		this.map_name = map_name;
		this.nodes = new Dictionary<string, Node> ();
		this.edges = new Dictionary<string, Edge> ();
		//this.objects = new Dictionary<string, GameObject> ();
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
			
			Node newnode = new Node ();
			newnode.id = id;
			newnode.node_type = node_type;
			newnode.x = x;
			newnode.y = y;
			newnode.intersection_type = intersection_type;
			nodes.Add (newnode.id, newnode);
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
		
		if ( (!edges.ContainsKey (id)) && nodes.ContainsKey (source_id) && nodes.ContainsKey (destination_id)) {
			Edge newedge = new Edge ();
			newedge.id = id;
			newedge.source_id = source_id;
			newedge.destination_id = destination_id;
			newedge.name = name;
			newedge.src_des = src_des;
			newedge.des_src = des_src;
			Debug.Log ("Adding edge "+id+": source: "+source_id+" destination: "+destination_id+" name: "+name+" src_des: "+src_des+" des_src: "+des_src);
			edges.Add (newedge.id, newedge);
		}
		Debug.Log ("edges.Count = "+edges.Count);
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
		Debug.Log ("Drawing Edges");
		foreach (KeyValuePair<string, Edge> edge in edges){
			drawEdge (edge.Key);
		}
		Debug.Log ("Drawing Nodes");
		foreach (KeyValuePair<string, Node> node in nodes){
			drawNode (node.Key);
		}
	}
	
	private void drawNode (string node_id) {

		Node n = nodes[node_id];
		Vector3 pos = new Vector3 (n.x, 0, n.y);

		if (n.node_type == NodeType.LIMIT) {
			Edge e = edges[edgeLimit(n.id)];
			Node src_node = nodes[e.source_id];
			Node dst_node = nodes[e.destination_id];

			// Vector direccion del arco
			Vector3 direction = new Vector3 (dst_node.x - src_node.x, 0, dst_node.y - src_node.y);
			// Vector del nodo limite
			Vector3 dir = new Vector3 (0,0,1);

			int lane_num = lanes (e.id);

			float width = (lane_num*lane_width) + 2*lane_width; // Para que sobresalga por ambos lados

			Material black_material = Resources.Load ("Materials/Simple_Black", typeof(Material)) as Material;

			GameObject aux_road = GameObject.CreatePrimitive(PrimitiveType.Cube);
			aux_road.name = node_id + " - limit";
			pos.y += (limit_height/2);
			aux_road.transform.position = pos;
			aux_road.transform.localScale = new Vector3(width,limit_height,limit_depth);
			aux_road.transform.rotation = Quaternion.Euler(0,RotationAngle(dir,direction),0);
			aux_road.renderer.material = black_material;
		}
		else {
			GameObject road_prefab = Resources.Load("Prefabs/Road", typeof(GameObject)) as GameObject;
			
			if (road_prefab == null) {
				Debug.Log ("road_prefab is null");
			}
			else {
				GameObject aux_road = GameObject.Instantiate (road_prefab, pos, Quaternion.identity) as GameObject;
				if (n.node_type == NodeType.CONTINUATION) {
					aux_road.name = node_id + " - continuation";
				}
				else if (n.node_type == NodeType.INTERSECTION) {
					aux_road.name = node_id + " - intersection";
				}
				else {
					aux_road.name = node_id + " - unknown type";
				}
				//objects.Add (node_id, aux_road);
			}
		}
	}

	// Devuelve el id del arco que llega al nodo limite pasado como argumento
	private string edgeLimit (string node_id) {
		foreach (KeyValuePair<string, Edge> edge in edges){
			if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {
				return edge.Value.id;
			}
		}
		return "";
	}
	
	private void drawEdge (string edge_id) {

		Material asphalt_material = Resources.Load ("Materials/Asphalt", typeof(Material)) as Material;
		Material asphalt_white_material = Resources.Load ("Materials/Asphalt_White", typeof(Material)) as Material;

		Debug.Log ("Drawing edge "+edge_id);
		Edge e = edges[edge_id];

		int lane_num = lanes (edge_id);

		Node src_node = nodes[e.source_id];
		Node dst_node = nodes[e.destination_id];

		Vector3 src_node_position = new Vector3 (src_node.x,0,src_node.y);
		Vector3 dst_node_position = new Vector3 (dst_node.x,0,dst_node.y);
		// Vector direccion del arco
		Vector3 direction = new Vector3 (dst_node_position.x - src_node_position.x, 0, dst_node_position.z - src_node_position.z);
		// Vector del prefab
		Vector3 dir_pref = new Vector3 (0,0,1);
		// Longitud del arco
		float lenght = Distance(src_node_position,dst_node_position)-intersection_margin;
		// Anchura del arco
		float width = (3 * lane_num) + ((lane_num + 1) * line_width) + 2 * (hard_shoulder_width);
		
		Vector3 pos = new Vector3( (dst_node.x + src_node.x)/2, 0, (dst_node.y + src_node.y)/2);
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.name = edge_id;
		platform.transform.localScale = new Vector3(width,road_thickness,lenght);

		// Marcas viales
		// Lineas de los arcenes
		GameObject line1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		line1.name = "line1";
		line1.transform.localScale = new Vector3(line_width,line_thickness,lenght);
		Vector3 line1_pos = platform.transform.position;
		line1_pos.x -= (width / 2) - hard_shoulder_width;
		line1_pos.y += (road_thickness/2)+(line_thickness/2);
		line1.transform.position = line1_pos;
		line1.transform.parent = platform.transform;
		line1.renderer.material.color = Color.white;
		//line1.renderer.material = asphalt_white_material;
		//line1.renderer.material.mainTextureScale = new Vector2(line1.transform.localScale.x,line1.transform.localScale.z);

		GameObject line2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		line2.name = "line2";
		line2.transform.localScale = new Vector3(line_width,line_thickness,lenght);
		Vector3 line2_pos = platform.transform.position;
		line2_pos.x += (width / 2) - hard_shoulder_width;
		line2_pos.y += (road_thickness/2)+(line_thickness/2);
		line2.transform.position = line2_pos;
		line2.transform.parent = platform.transform;
		line1.renderer.material.color = Color.white;
		//line2.renderer.material = asphalt_white_material;
		//line2.renderer.material.mainTextureScale = new Vector2(line2.transform.localScale.x,line2.transform.localScale.z);
		// Fin lineas arcenes

		// Linea central
		if (e.src_des != "0" && e.des_src != "0") {
			GameObject line3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
			line3.name = "line3";
			line3.transform.localScale = new Vector3(line_width,line_thickness,lenght);
			Vector3 line3_pos = platform.transform.position;
			line3_pos.y += (road_thickness/2)+(line_thickness/2);
			line3.transform.position = line3_pos;
			line3.transform.parent = platform.transform;
			line1.renderer.material.color = Color.white;
			//line3.renderer.material = asphalt_white_material;
			//line3.renderer.material.mainTextureScale = new Vector2(line3.transform.localScale.x,line3.transform.localScale.z);
		}
		// Fin Linea central

		// Fin marcas viales

		platform.transform.rotation = Quaternion.Euler(0,RotationAngle(dir_pref,direction),0);
		platform.transform.position = pos;
		platform.renderer.material.color = Color.gray;
		platform.renderer.material = asphalt_material;
		platform.renderer.material.mainTextureScale = new Vector2(platform.transform.localScale.x,platform.transform.localScale.z);
	}

	// Calcula el numero total de carriles del arco
	private int lanes (string edge_id) {
		string src_des = edges [edge_id].src_des;
		string des_src = edges [edge_id].des_src;
		
		int lane_num = 0;
		
		if (src_des != "0") {
			lane_num += src_des.Length;
		}
		
		if (des_src != "0") {
			lane_num += des_src.Length;
		}

		return lane_num;
	}

	private float Distance (Vector3 p1, Vector3 p2) {
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		float dz = p2.z - p1.z;

		float dx2 = dx * dx;
		float dy2 = dy * dy;
		float dz2 = dz * dz;

		float d = Mathf.Sqrt (dx2 + dy2 + dz2);

		return d;
	}

	// Devuelve el angulo en grados
	private float RotationAngle (Vector3 p1, Vector3 p2) {
		float scalar_product = Mathf.Abs(p1.x * p2.x + p1.y * p2.y + p1.z * p2.z);
		float p1_module = Mathf.Sqrt (p1.x*p1.x + p1.y*p1.y + p1.z*p1.z);
		float p2_module = Mathf.Sqrt (p2.x*p2.x + p2.y*p2.y + p2.z*p2.z);
		float angle = Mathf.Acos(scalar_product / (p1_module*p2_module));
		return ((angle * 180f) / Mathf.PI);
	}
}
