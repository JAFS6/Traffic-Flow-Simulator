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
	public const float public_transport_line_width = 0.3f;
	public const float hard_shoulder_width = 1f;
	public const float limit_height = 10f;
	public const float limit_depth = 3f;
	public const float road_thickness = 0.1f;
	public const float line_thickness = 0.01f;
	public const float intersection_margin = 20f;
	public const float center_lines_separation = 0.2f;
	public const float discontinuous_line_length = 2f;

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

	/**
	 * @brief Dibuja el nodo con id "nodo_id" en el entorno 3D
	 * @param[in] node_id Identificador del nodo a dibujar
	 */
	private void drawNode (string node_id) {

		Node n = nodes[node_id];
		Vector3 pos = new Vector3 (n.x, 0, n.y);

		if (n.node_type == NodeType.LIMIT) {
			Edge e = edges[edgeLimit(n.id)];
			Node src_node = nodes[e.source_id];
			Node dst_node = nodes[e.destination_id];

			// Vector direccion del arco
			Vector2 direction = new Vector2 (dst_node.x - src_node.x, dst_node.y - src_node.y);
			// Vector del nodo limite
			Vector2 dir = new Vector3 (0,1);

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

	/**
	 * @brief Devuelve el id del arco que llega al nodo limite pasado como argumento
	 * @param[in] node_id Identificador del nodo limite
	 * @return Un string con el id del arco buscado
	 */
	private string edgeLimit (string node_id) {
		foreach (KeyValuePair<string, Edge> edge in edges){
			if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {
				return edge.Value.id;
			}
		}
		return "";
	}

	/**
	 * @brief Dibuja el arco con id "edge_id" en el entorno 3D
	 * @param[in] edge_id Identificador del arco a dibujar
	 */
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
		Vector2 direction = new Vector2 (dst_node_position.x - src_node_position.x, dst_node_position.z - src_node_position.z);
		// Vector del prefab
		Vector2 dir_pref = new Vector2 (0,1);
		// Longitud del arco
		float length = Distance(src_node_position,dst_node_position)-intersection_margin;
		// Anchura del arco
		float width = (lane_width * lane_num) + ((lane_num + 1) * line_width) + 2 * (hard_shoulder_width);

		// Plataforma
		Vector3 pos = new Vector3( (dst_node.x + src_node.x)/2, 0, (dst_node.y + src_node.y)/2);
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.name = edge_id;
		platform.transform.localScale = new Vector3(width,road_thickness,length);
		platform.renderer.material.color = Color.gray;
		platform.renderer.material = asphalt_material;
		platform.renderer.material.mainTextureScale = new Vector2(platform.transform.localScale.x,platform.transform.localScale.z);

		Vector3 position;
		/*
		Debug
		GameObject debug = GameObject.CreatePrimitive(PrimitiveType.Cube);
		debug.name = "debug";
		debug.transform.localScale = new Vector3(5,5,5);
		position = new Vector3();
		position.x = platform.transform.position.x;
		position.y = platform.transform.position.y + 2.5f;
		position.z = platform.transform.position.z - (length/2) + 10;
		debug.transform.position = position;
		debug.renderer.material.color = Color.red;
		debug.transform.parent = platform.transform;
		*/

		// Marcas viales

		// Lineas de los arcenes
		position = new Vector3();
		position.x = platform.transform.position.x - ((width / 2) - hard_shoulder_width);
		position.y = platform.transform.position.y + (road_thickness/2)+(line_thickness/2);
		position.z = 0;
		draw_continuous_line (line_width, line_thickness, length, position, "Left line", platform);

		position.x = platform.transform.position.x + ((width / 2) - hard_shoulder_width);
		draw_continuous_line (line_width, line_thickness, length, position, "Right line", platform);

		// Lineas centrales
		if (e.src_des != "0" && e.des_src != "0") { // Si ambos sentidos tienen carriles

			if (e.src_des.Length == e.des_src.Length) { // Mismo numero de carriles en cada sentido
				position.x = platform.transform.position.x - (center_lines_separation/2);
				draw_continuous_line (line_width, line_thickness, length, position, "Center line", platform);
				position.x = platform.transform.position.x + (center_lines_separation/2);
				draw_continuous_line (line_width, line_thickness, length, position, "Center line", platform);
			}
			else { // Distinto numero de carriles en cada sentido
				int lane_diff = e.src_des.Length - e.des_src.Length;

				position.x = platform.transform.position.x - (center_lines_separation/2) - (lane_diff * (lane_width/2));
				draw_continuous_line (line_width, line_thickness, length, position, "Center line", platform);
				position.x = platform.transform.position.x + (center_lines_separation/2) - (lane_diff * (lane_width/2));
				draw_continuous_line (line_width, line_thickness, length, position, "Center line", platform);
			}
		}

		// Lineas de carril

		// Pintar tantas lineas de tipo de carril como carriles menos uno haya en cada direccion
		if (e.src_des != "0") {
			for (int i=0; i<e.src_des.Length-1; i++) {
				position.x = platform.transform.position.x + ((width / 2) - hard_shoulder_width) - ((lane_width + line_width) * (i+1));
				char lane_type = e.src_des[i];
				draw_lane_line (lane_type, length, position, platform);

			}
		}

		if (e.des_src != "0") {
			for (int i=0; i<e.des_src.Length-1; i++) {
				position.x = platform.transform.position.x - ((width / 2) - hard_shoulder_width) + ((lane_width + line_width) * (i+1));
				char lane_type = e.des_src[i];
				draw_lane_line (lane_type, length, position, platform);
			}
		}

		// Fin marcas viales

		platform.transform.rotation = Quaternion.Euler(0,RotationAngle(dir_pref,direction),0);
		platform.transform.position = pos;
	}

	/**
	 * @brief Dibuja una linea de carril segun su tipo
	 * @param[in] lane_type Tipo de carril (P: Transporte publico, N: Normal, A: Aparcamiento, V: Carril Bus/VAO)
	 * @param[in] length Longitud de la linea
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private void draw_lane_line (char lane_type, float length, Vector3 position, GameObject parent) {

		switch (lane_type) {
			case 'P':
				draw_continuous_line (public_transport_line_width, line_thickness, length, position, "Public transport lane line", parent);
				break;
			case 'N':
				draw_discontinuous_line (line_width, line_thickness, length, position, "Normal lane line", parent);
				break;
			case 'A':
				Debug.Log("Parking not designed yet");
				break;
			case 'V':
				Debug.Log("Bus/VAO not designed yet");
				break;
			default:
				Debug.Log("Trying to draw invalid type of lane");
				break;
		}
	}

	/**
	 * @brief Dibuja una linea continua blanca
	 * @param[in] width Ancho de la linea
	 * @param[in] height Grosor de la linea
	 * @param[in] length Longitud de la linea
	 * @param[in] name Nombre para el objeto
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private void draw_continuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent) {
		GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
		line.name = name;
		line.transform.localScale = new Vector3(width, height, length);
		line.transform.position = position;
		line.renderer.material.color = Color.white;
		//Material asphalt_white_material = Resources.Load ("Materials/Asphalt_White", typeof(Material)) as Material;
		//line3.renderer.material = asphalt_white_material;
		//line3.renderer.material.mainTextureScale = new Vector2(line3.transform.localScale.x,line3.transform.localScale.z);
		line.transform.parent = parent.transform;
	}

	/**
	 * @brief Dibuja una linea continua blanca
	 * @param[in] width Ancho de la linea
	 * @param[in] height Grosor de la linea
	 * @param[in] length Longitud de la linea
	 * @param[in] name Nombre para el objeto
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private void draw_discontinuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent) {
		int piece_num = (int)((length / discontinuous_line_length) / 2);
		Vector3 pos_aux = position;
		pos_aux.z -= length/2 - discontinuous_line_length;

		for (int i=0; i < piece_num; i++) {

			GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
			line.name = name;
			line.transform.localScale = new Vector3(width, height, discontinuous_line_length);
			line.transform.position = pos_aux;
			line.renderer.material.color = Color.white;
			//Material asphalt_white_material = Resources.Load ("Materials/Asphalt_White", typeof(Material)) as Material;
			//line3.renderer.material = asphalt_white_material;
			//line3.renderer.material.mainTextureScale = new Vector2(line3.transform.localScale.x,line3.transform.localScale.z);
			line.transform.parent = parent.transform;

			pos_aux.z += discontinuous_line_length * 2;
		}
	}

	/**
	 * @brief Calcula el numero total de carriles del arco cuyo identificador se pasa como argumento
	 * @param[in] edge_id Identificador del arco
	 * @return El numero entero de carriles que tiene el arco
	 */
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

	/**
	 * @brief Calcula la distancia minima entre dos puntos del espacio tridimensional
	 * @param[in] p1 Un vector (x,y,z) con las coordenadas del primer punto
	 * @param[in] p2 Un vector (x,y,z) con las coordenadas del segundo punto
	 * @return La distancia minima entre los dos puntos
	 */
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

	/**
	 * @brief Calcula el angulo en grados que hay que girar el vector 
	 * v1 para ponerlo en la direccion y sentido del vector v2
	 * @param[in] v1 El primer vector
	 * @param[in] v2 El segundo vector
	 * @return El angulo calculado en grados [0,360)
	 */
	private float RotationAngle (Vector2 v1, Vector2 v2) {

		float v1_theta = PolarAngle (v1);
		float v2_theta = PolarAngle (v2);
		float angle = v1_theta - v2_theta;

		return angle;
	}

	/**
	 * @brief Calcula el angulo (en grados) de las coordenadas polares del vector pasado como argumento
	 * @param[in] v El vector
	 * @return El angulo calculado en grados
	 */
	private float PolarAngle (Vector2 v) {
		float angle_rad = Mathf.Atan2 (v.y, v.x);
		float angle_deg = ((angle_rad * 180f) / Mathf.PI);

		if (v.x < 0) {
			if (v.y >= 0) { // Segundo cuadrante
				angle_deg += 180f;
			}
			else if (v.y < 0) { // Tercer cuadrante
				angle_deg -= 180f;
			}
		}

		return angle_deg;
	}
}
