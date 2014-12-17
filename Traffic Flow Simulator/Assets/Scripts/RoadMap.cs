using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public enum NodeType : byte {INTERSECTION, LIMIT, CONTINUATION, UNKNOWN};
public enum IntersectionType : byte {NORMAL, ROUNDABOUT, UNKNOWN};

public struct Node
{
	public string id;
	public NodeType node_type;
	public float x;
	public float y;
	public IntersectionType intersection_type;
	public string widest_edge_id;
}

public struct Edge
{
	public string id;
	public string source_id;
	public string destination_id;
	public string name;
	public string src_des;
	public string des_src;
	public float length;
	public float width;
	public int lane_num;
	public Vector2 direction;
	public Vector2 fixed_position_vector; // Vector de ajuste de posicion
	public Vector3 fixed_position; // Posicion ya ajustada
}

public static class RoadMap {

	public const float lane_width = 3f;
	public const float line_width = 0.1f;
	public const float public_transport_line_width = 0.3f;
	public const float hard_shoulder_width = 1f;
	public const float limit_height = 10f;
	public const float limit_depth = 3f;
	public const float road_thickness = 0.1f;
	public const float line_thickness = 0.01f;
	public const float center_lines_separation = 0.2f;
	public const float discontinuous_line_length = 2f;

	public const string hard_shoulder_line_name = "Hard shoulder line";
	public const string public_transport_lane_line_name = "Public transport lane line";
	public const string center_line_name = "Center line";
	public const string detention_line_name = "Detention line";
	public const string normal_lane_line_name = "Normal lane line";
	public const string discontinuous_line_name = "Discontinuous line";
	
	// Tags
	public const string limit_node_tag = "Limit_node";
	public const string continuation_node_tag = "Continuation_node";
	public const string intersection_node_tag = "Intersection_node";
	public const string unknown_tag = "Unknown";
	public const string edge_tag = "Edge";
	public const string lane_start_point_tag = "LaneStartPoint";
	
	public const string no_lane_string = "0";
	public const string normal_lane_string = "N";
	public const string public_lane_string = "P";

	private static string map_name;
	private static Dictionary<string, Node> nodes;
	private static Dictionary<string, Edge> edges;

	// Materials
	private static Material black_material;
	private static Material asphalt_material;
	private static Material asphalt_white_material;

	public static void CreateNewMap (string map_name) {
		map_name = map_name;
		nodes = new Dictionary<string, Node> ();
		edges = new Dictionary<string, Edge> ();
		
		black_material = Resources.Load ("Materials/Simple_Black", typeof(Material)) as Material;
		asphalt_material = Resources.Load ("Materials/Asphalt", typeof(Material)) as Material;
		asphalt_white_material = Resources.Load ("Materials/Asphalt_White", typeof(Material)) as Material;
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
	public static void addNode (string id, NodeType node_type, float x, float y, IntersectionType intersection_type = IntersectionType.NORMAL) {
		
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
	public static void addEdge (string id, string source_id, string destination_id, string name, string src_des, string des_src) {
		
		if ( (!edges.ContainsKey (id)) && nodes.ContainsKey (source_id) && nodes.ContainsKey (destination_id)) {
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
	public static void setIntersectionType (string id, IntersectionType new_intersection_type) {
		
		if ( nodes.ContainsKey (id) ) {
			Node aux_node = nodes[id];
			aux_node.intersection_type = new_intersection_type;
			nodes[id] = aux_node;
		}
	}

	/**
	 * @brief Obtiene el nombre del mapa
	 * @return Una cadena de texto con el nombre del mapa
	 */
	public static string getMapName () {
		return map_name;
	}

	/**
	 * @brief Obtiene el numero de nodos del mapa
	 * @return El numero de nodos del mapa
	 */
	public static int getNodeCount () {
		return nodes.Count;
	}

	/**
	 * @brief Obtiene una lista con los identificadores de los nodos del mapa
	 * @return Una lista de cadenas de texto
	 */
	public static List<string> getNodeIDs () {
		List<string> l = new List<string>(nodes.Keys);
		return l;
	}

	/**
	 * @brief Obtiene la posicion del nodo con identificador node_id en el plano XZ
	 * @param[in] node_id El identificador del nodo
	 * @return Un Vector2 con la posicion del nodo en el plano XZ
	 * @post Si el identificador no existe se devolvera un vector (0,0)
	 */
	public static Vector2 getNodePosition (string node_id) {

		Vector2 pos = new Vector2 ();

		if (nodes.ContainsKey (node_id)) {
			pos.x = nodes [node_id].x;
			pos.y = nodes [node_id].y;
		}

		return pos;
	}

	/**
	 * @brief Obtiene el tipo del nodo con identificador node_id
	 * @param[in] node_id El identificador del nodo
	 * @return El tipo del nodo consultado
	 * @post Si el identificador no existe se devolvera el tipo UNKNOWN
	 */
	public static NodeType getNodeType (string node_id) {

		if (nodes.ContainsKey (node_id)) {
			return nodes[node_id].node_type;
		}
		else {
			return NodeType.UNKNOWN;
		}
	}

	/**
	 * @brief Obtiene el numero de arcos del mapa
	 * @return El numero de arcos del mapa
	 */
	public static int getEdgeCount () {
		return edges.Count;
	}

	/**
	 * @brief Obtiene una lista con los identificadores de los arcos del mapa
	 * @return Una lista de cadenas de texto
	 */
	public static List<string> getEdgeIDs () {
		List<string> l = new List<string>(edges.Keys);
		return l;
	}
	
	/**
	 * @brief Obtiene la posicion central del arco con identificador edge_id en el plano XZ
	 * @param[in] edge_id El identificador del nodo
	 * @return Un Vector2 con la posicion central del arco en el plano XZ
	 * @post Si el identificador no existe se devolvera un vector (0,0)
	 */
	public static Vector2 getEdgePosition (string edge_id) {
		
		Vector2 pos = new Vector2 ();
		
		if (edges.ContainsKey (edge_id)) {
			string src_node_id = edges[edge_id].source_id;
			string des_node_id = edges[edge_id].destination_id;
		
			Vector2 src_pos = new Vector2 (nodes[src_node_id].x, nodes[src_node_id].y);
			Vector2 des_pos = new Vector2 (nodes[des_node_id].x, nodes[des_node_id].y);
			
			pos.x = (src_pos.x+des_pos.x)/2;
			pos.y = (src_pos.y+des_pos.y)/2;
		}
		
		return pos;
	}

	/**
	 * @brief Dibuja el mapa en el entorno 3D
	 */
	public static void draw () {

		prepareEdges ();

		foreach (KeyValuePair<string, Edge> edge in edges){
			drawEdge (edge.Key);
		}

		foreach (KeyValuePair<string, Node> node in nodes){
			drawNode (node.Key);
		}
	}

	/**
	 * @brief Devuelve el id del arco que llega al nodo limite pasado como argumento
	 * @param[in] node_id Identificador del nodo limite
	 * @return Un string con el id del arco buscado
	 */
	public static string edgeLimit (string node_id) {
		
		foreach (KeyValuePair<string, Edge> edge in edges) {
			
			if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {
				return edge.Value.id;
			}
		}
		return "";
	}

	/**
	 * @brief Comprueba si el nodo limite pasado como argumento es un nodo por el que entran vehiculos al mapa
	 * @param[in] node_id Identificador del nodo limite
	 * @param[out] tt Tipo de transporte que entrara al mapa a traves de ese nodo
	 * @return True si hay algun carril de entrada al mapa desde ese nodo, false si no, o si el nodo pasado no es un nodo limite
	 */
	public static bool isEntryNode (string node_id, out TransportType tt) {
		
		tt = TransportType.Unknown;
		
		if (nodes[node_id].node_type == NodeType.LIMIT) {
			string edge_id = edgeLimit(node_id);
			
			if (edges[edge_id].source_id == node_id && edges[edge_id].src_des != no_lane_string) {
			
				if (edges[edge_id].src_des.Contains(public_lane_string) && edges[edge_id].src_des.Contains(normal_lane_string)) { // Contiene P y N
					tt = TransportType.PublicAndPrivate;
				}
				else if (edges[edge_id].src_des.Contains(public_lane_string)) { // Contiene solo P
					tt = TransportType.Public;
				}
				else if (edges[edge_id].src_des.Contains(normal_lane_string)) { // Contiene solo N
					tt = TransportType.Private;
				}
				
				return true;
			}
			else if (edges[edge_id].destination_id == node_id && edges[edge_id].des_src != no_lane_string) {
			
				if (edges[edge_id].des_src.Contains(public_lane_string) && edges[edge_id].des_src.Contains(normal_lane_string)) { // Contiene P y N
					tt = TransportType.PublicAndPrivate;
				}
				else if (edges[edge_id].des_src.Contains(public_lane_string)) { // Contiene solo P
					tt = TransportType.Public;
				}
				else if (edges[edge_id].des_src.Contains(normal_lane_string)) { // Contiene solo N
					tt = TransportType.Private;
				}
				
				return true;
			}
			else {
				return false;
			}
		}
		else {
			return false;
		}
	}

	/**
	 * @brief Calcula el vector que indica la direccion de entrada al mapa desde el nodo limite pasado como argumento
	 * @param[in] node_id Identificador del nodo limite
	 * @return El vector orientacion calculado
	 * @post Si el identificador no existe o no es nodo limite se devolvera el vector (0,0)
	 */
	public static Vector2 entryOrientation (string node_id) {
		Vector3 v = new Vector3 (0,0,0);

		if (nodes.ContainsKey (node_id)) {
			if (nodes[node_id].node_type == NodeType.LIMIT) {
				string edge_id = edgeLimit(node_id);

				if (edges[edge_id].source_id == node_id) {
					// Destino - fuente
					v.x = nodes[ edges[edge_id].destination_id ].x - nodes[ edges[edge_id].source_id ].x;
					v.z = nodes[ edges[edge_id].destination_id ].y - nodes[ edges[edge_id].source_id ].y;
				}
				else {
					// Fuente - destino
					v.x = nodes[ edges[edge_id].source_id ].x - nodes[ edges[edge_id].destination_id ].x;
					v.z = nodes[ edges[edge_id].source_id ].y - nodes[ edges[edge_id].destination_id ].y;
				}
			}
		}

		v.Normalize ();

		Vector2 orientation = new Vector2 (v.x,v.z);

		return orientation;
	}
	
	/**
	 * @brief Obtiene una lista con los identificadores de los arcos por los que un vehiculo puede salir del nodo
	 * habiendo entrado a el por el arco indicado
	 * @param[in] node_id Id del nodo en el que se encuentra el vehiculo
	 * @param[in] entry_edge_id Id del arco por el que ha entrado al nodo
	 * @param[in] tt Tipo de transporte del vehiculo
	 */
	public static List<string> exitPaths (string node_id, string entry_edge_id, TransportType tt) {
	
		List<string> exits = new List<string>();
		exits.Clear();
		
		List<string> edge_keys = new List<string> (edges.Keys);
		
		foreach (string edge_id in edge_keys) {
			
			if (edge_id != entry_edge_id) { // El arco no es el arco por el que ha entrado
				
				if (node_id == edges[edge_id].source_id && edges[edge_id].src_des != no_lane_string) { // Hay algun carril de salida
					
					if ((tt == TransportType.Public && edges[edge_id].src_des.Contains(public_lane_string)) || 
						(tt == TransportType.Private && edges[edge_id].src_des.Contains(normal_lane_string))) { // Hay al menos un carril que se corresponde con el tipo de vehiculo
						
						exits.Add(edge_id);
					}
				}
				else if (node_id == edges[edge_id].destination_id && edges[edge_id].des_src != no_lane_string) { // Hay algun carril de salida
					
					if ((tt == TransportType.Public && edges[edge_id].des_src.Contains(public_lane_string)) || 
					    (tt == TransportType.Private && edges[edge_id].des_src.Contains(normal_lane_string))) { // Hay al menos un carril que se corresponde con el tipo de vehiculo
						
						exits.Add(edge_id);
					}
				}
			}
		}
		
		return exits;
	}

	/**
	 * @brief Procesa los arcos calculando su longitud, anchura y numero de carriles, asi como el ajuste de longitud y posicion para las intersecciones
	 * @pre Este metodo debe ser llamado antes de ejecutar el metodo drawEdge
	 */
	private static void prepareEdges () {

		List<string> edge_keys = new List<string> (edges.Keys);

		// Calcular el numero de carriles, la longitud del arco, la anchura del arco y el vector director del arco
		foreach (string edge_key in edge_keys) {
			Edge e = edges[edge_key];
			// Numero de carriles
			e.lane_num = lanes (e.id);
			Node src_node = nodes[e.source_id];
			Node dst_node = nodes[e.destination_id];
			Vector3 src_node_position = new Vector3 (src_node.x,0,src_node.y);
			Vector3 dst_node_position = new Vector3 (dst_node.x,0,dst_node.y);
			// Longitud del arco
			e.length = MyMathClass.Distance3D(src_node_position, dst_node_position);
			// Ancho del arco
			e.width = (lane_width * e.lane_num) + ((e.lane_num + 1) * line_width) + 2 * (hard_shoulder_width);
			// Vector direccion del arco
			e.direction = new Vector2 (dst_node_position.x - src_node_position.x, dst_node_position.z - src_node_position.z);
			// Actualizar el arco
			edges[e.id] = e;
		}

		// Actualizar el arco mas ancho en cada nodo
		List<string> node_keys = new List<string> (nodes.Keys);

		foreach (string node_key in node_keys) {
			Node n = nodes[node_key];
			float best_width = 0f;

			foreach (string edge_key in edge_keys) {

				if (edges[edge_key].source_id == n.id || edges[edge_key].destination_id == n.id) {

					if (edges[edge_key].width > best_width) {
						best_width = edges[edge_key].width;
						n.widest_edge_id = edge_key;
					}
				}
			}
			// Actualizar el nodo
			nodes[n.id] = n;
		}

		// Calcular el vector de ajuste de posicion del arco
		foreach (string edge_key in edge_keys) { 
			Edge e = edges[edge_key];
			// Obtener el angulo polar del vector director del arco
			float polar_angle = MyMathClass.PolarAngle(e.direction);
			// Calcular la magnitud del vector de ajuste de posicion segun los nodos en los extremos del arco
			float fixed_length = 0;

			NodeType src_node_type = nodes[e.source_id].node_type;				// Tipo del nodo de origen
			NodeType dst_node_type = nodes[e.destination_id].node_type;			// Tipo del nodo de destino
			string src_id_widest_edge = nodes[e.source_id].widest_edge_id;		// Identificador del arco mas ancho en el nodo de origen
			string dst_id_widest_edge = nodes[e.destination_id].widest_edge_id;	// Identificador del arco mas ancho en el nodo de destino

			if (src_node_type == NodeType.INTERSECTION || src_node_type == NodeType.CONTINUATION) {
				float aux_width = edges[ src_id_widest_edge ].width /2;
				fixed_length += aux_width; // Desplazamiento en el sentido del vector director del arco
				e.length -= aux_width;
			}
			else if (src_node_type == NodeType.LIMIT) {
				fixed_length += limit_depth*1.5f; // Desplazamiento en el sentido del vector director del arco
			}

			if (dst_node_type == NodeType.INTERSECTION || dst_node_type == NodeType.CONTINUATION) {
				float aux_width = edges[ dst_id_widest_edge ].width /2;
				fixed_length -= aux_width; // Desplazamiento en sentido contrario del vector director del arco
				e.length -= aux_width;
			}
			else if (dst_node_type == NodeType.LIMIT) {
				fixed_length -= limit_depth*1.5f; // Desplazamiento en sentido contrario del vector director del arco
			}

			if (fixed_length < 0) {
				fixed_length = Mathf.Abs(fixed_length);
				polar_angle = (polar_angle + 180) % 360;
			}

			// Calcular el vector de ajuste de posicion
			e.fixed_position_vector = MyMathClass.PolarToCartesian (fixed_length, polar_angle);
			// Calcular la posicion ya ajustada
			Node src_node = nodes[e.source_id];
			Node dst_node = nodes[e.destination_id];
			e.fixed_position = new Vector3( (dst_node.x + src_node.x)/2, 0, (dst_node.y + src_node.y)/2);
			e.fixed_position.x += e.fixed_position_vector.x;
			e.fixed_position.z += e.fixed_position_vector.y;

			// Actualizar el arco
			edges[e.id] = e;
		}
	}

	/**
	 * @brief Dibuja el nodo con id "nodo_id" en el entorno 3D
	 * @param[in] node_id Identificador del nodo a dibujar
	 */
	private static void drawNode (string node_id) {

		Node n = nodes[node_id];
		Vector3 pos = new Vector3 (n.x, 0, n.y);

		if (n.node_type == NodeType.LIMIT) {
			Edge e = edges[edgeLimit(n.id)];
			Node src_node = nodes[e.source_id];
			Node dst_node = nodes[e.destination_id];

			float width = (e.lane_num*lane_width) + 2*lane_width; // Para que sobresalga por ambos lados

			GameObject aux_road = GameObject.CreatePrimitive(PrimitiveType.Cube);
			aux_road.name = node_id;
			aux_road.tag = limit_node_tag;
			aux_road.renderer.material = black_material;
			pos.y += (limit_height/2);
			aux_road.transform.localScale = new Vector3(width,limit_height,limit_depth);
			// Vector del nodo limite
			Vector2 dir = new Vector2 (0,1);
			aux_road.transform.rotation = Quaternion.AngleAxis(MyMathClass.RotationAngle(dir,e.direction),Vector3.up);
			aux_road.transform.position = pos;
		}
		else if (n.node_type == NodeType.CONTINUATION) {
			GameObject aux_road = new GameObject();
			aux_road.name = node_id;
			aux_road.tag = continuation_node_tag;
			float edge_width = nodeWidth(n.id);
			CreateContinuationNode(aux_road, edge_width, edge_width, nodeAngle(n.id));
			aux_road.transform.position = pos;
		}
		else {
			GameObject road_prefab = Resources.Load("Prefabs/Road", typeof(GameObject)) as GameObject;
			
			if (road_prefab == null) {
				Debug.Log ("road_prefab is null");
			}
			else {
				GameObject aux_road = GameObject.Instantiate (road_prefab, pos, Quaternion.identity) as GameObject;
				aux_road.transform.localScale = new Vector3(17.6f,road_thickness,17.6f);

				if (n.node_type == NodeType.INTERSECTION) {
					aux_road.name = node_id;
					aux_road.tag = intersection_node_tag;
				}
				else {
					aux_road.name = node_id;
					aux_road.tag = unknown_tag;
				}
			}
		}
	}

	/**
	 * @brief Devuelve el ancho de los arcos que llegan el nodo continuacion pasado como argumento (ambos tienen el mismo ancho)
	 * @param[in] node_id Identificador del nodo continuacion
	 * @return El ancho buscado
	 */
	private static float nodeWidth (string node_id) {

		// Dado que el ancho de ambos arcos sera el mismo, en cuanto encontremos el primero se detiene la busqueda

		foreach (KeyValuePair<string, Edge> edge in edges) {

			if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {
				return edge.Value.width;
			}
		}
		return -1f;
	}

	/**
	 * @brief Calcula el angulo que se produce entre los arcos que llegan al nodo de continuacion pasado como argumento
	 * @param[in] node_id Identificador del nodo continuacion
	 * @return El angulo calculado en grados [0,360)
	 */
	private static float nodeAngle (string node_id) {
		Vector2 edge_1 = new Vector2();
		Vector2 edge_2 = new Vector2();
		bool first_found = false;
		bool second_found = false;

		// Encontrar los dos arcos que llegan al nodo continuacion y guardar sus posiciones

		foreach (KeyValuePair<string, Edge> edge in edges) {
			
			if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {

				if (!first_found && !second_found) {
					first_found = true;
					edge_1.x = edge.Value.fixed_position.x;
					edge_1.y = edge.Value.fixed_position.z;
				}
				else if (first_found && !second_found) {
					second_found = true;
					edge_2.x = edge.Value.fixed_position.x;
					edge_2.y = edge.Value.fixed_position.z;
				}
			}

			if (second_found) {
				break;
			}
		}

		// Calcular dos vectores con origen en la posicion del nodo y vertice en la posicion de los arcos
		Vector2 vector_1 = new Vector2 (edge_1.x - nodes[node_id].x, edge_1.y - nodes[node_id].y);
		Vector2 vector_2 = new Vector2 (edge_2.x - nodes[node_id].x, edge_2.y - nodes[node_id].y);

		// Calcular el menor angulo entre los vectores
		float angle_deg = MyMathClass.RotationAngle (vector_1, vector_2);

		if (angle_deg < 0) {
			angle_deg += 360f;
		}
		return angle_deg;
	}

	/**
	 * @brief Dibuja el arco con id "edge_id" en el entorno 3D
	 * @param[in] edge_id Identificador del arco a dibujar
	 * @pre Antes de ejecutar este metodo se debe ejecutar una vez el metodo prepareEdges
	 */
	private static void drawEdge (string edge_id) {
		Edge e = edges[edge_id];

		// Plataforma
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.name = edge_id;
		platform.tag = edge_tag;
		platform.transform.localScale = new Vector3(e.width, road_thickness, e.length);
		platform.renderer.material.color = Color.gray;
		platform.renderer.material = asphalt_material;
		platform.renderer.material.mainTextureScale = new Vector2(platform.transform.localScale.x, platform.transform.localScale.z);

		Vector3 position;

		// Marcas viales

		// Lineas de los arcenes
		position = new Vector3();
		position.x = platform.transform.position.x - ((e.width / 2) - hard_shoulder_width);
		position.y = platform.transform.position.y + (road_thickness/2)+(line_thickness/2);
		position.z = 0;
		draw_continuous_line (line_width, line_thickness, e.length, position, hard_shoulder_line_name, platform);

		position.x = platform.transform.position.x + ((e.width / 2) - hard_shoulder_width);
		draw_continuous_line (line_width, line_thickness, e.length, position, hard_shoulder_line_name, platform);

		// Lineas centrales
		if (e.src_des != no_lane_string && e.des_src != no_lane_string) { // Si ambos sentidos tienen carriles

			if (e.src_des.Length == e.des_src.Length) { // Mismo numero de carriles en cada sentido
				position.x = platform.transform.position.x - (center_lines_separation/2);
				draw_continuous_line (line_width, line_thickness, e.length, position, center_line_name, platform);
				position.x = platform.transform.position.x + (center_lines_separation/2);
				draw_continuous_line (line_width, line_thickness, e.length, position, center_line_name, platform);
			}
			else { // Distinto numero de carriles en cada sentido
				int lane_diff = e.src_des.Length - e.des_src.Length;

				position.x = platform.transform.position.x - (center_lines_separation/2) - (lane_diff * (lane_width/2));
				draw_continuous_line (line_width, line_thickness, e.length, position, center_line_name, platform);
				position.x = platform.transform.position.x + (center_lines_separation/2) - (lane_diff * (lane_width/2));
				draw_continuous_line (line_width, line_thickness, e.length, position, center_line_name, platform);
			}
		}

		// Lineas de carril

		Vector3 save_position = new Vector3 (position.x, position.y, position.z);

		// Pintar tantas lineas de tipo de carril como carriles menos uno haya en cada direccion
		// y poner tantos inicios de carril como carriles haya
		if (e.src_des != no_lane_string) {
			for (int i=0; i<e.src_des.Length; i++) {
				char lane_type = e.src_des[i];
				position.x = platform.transform.position.x + ((e.width / 2) - hard_shoulder_width) - ((lane_width + line_width) * (i+1));
				
				if (i < e.src_des.Length-1) {
					draw_lane_line (lane_type, e.length, position, platform);
				}
				setLaneStartPoint (lane_type, new Vector3 (position.x + (lane_width/2), position.y, position.z - (e.length/2)), platform);
			}

			// Lineas de detencion antes del cruce
			position.x = platform.transform.position.x + ((e.width / 2) - hard_shoulder_width) - (e.src_des.Length * (lane_width + line_width))/2;
			position.z = platform.transform.position.z + (e.length/2 - public_transport_line_width/2);

			draw_continuous_line (e.src_des.Length * (lane_width + line_width),line_thickness,public_transport_line_width,position,detention_line_name,platform); // Intercambiado ancho por largo para hacer linea perpendicular

		}

		if (e.des_src != no_lane_string) {
			position = save_position;

			for (int i=0; i<e.des_src.Length; i++) {
				char lane_type = e.des_src[i];
				position.x = platform.transform.position.x - ((e.width / 2) - hard_shoulder_width) + ((lane_width + line_width) * (i+1));
				
				if (i < e.des_src.Length-1) {
					draw_lane_line (lane_type, e.length, position, platform);
				}
				setLaneStartPoint (lane_type, new Vector3 (position.x - (lane_width/2), position.y, position.z + (e.length/2)), platform);
			}

			// Lineas de detencion antes del cruce
			position.x = platform.transform.position.x - ((e.width / 2) - hard_shoulder_width) + (e.des_src.Length * (lane_width + line_width))/2;
			position.z = platform.transform.position.z - (e.length/2 - public_transport_line_width/2);
			
			draw_continuous_line (e.des_src.Length * (lane_width + line_width),line_thickness,public_transport_line_width,position,detention_line_name,platform); // Intercambiado ancho por largo para hacer linea perpendicular
		}

		// Fin marcas viales

		// Vector del arco recien dibujado
		Vector2 dir_pref = new Vector2 (0,1);

		platform.transform.rotation = Quaternion.AngleAxis(MyMathClass.RotationAngle(dir_pref,e.direction),Vector3.up);
		platform.transform.position = e.fixed_position;
	}
	
	/**
	 * @brief Establece un objeto LaneStart en la posicion indicada
	 * @param[in] lane_type Tipo de carril (P: Transporte publico, N: Normal, A: Aparcamiento, V: Carril Bus/VAO)
	 * @param[in] position Posicion donde se colocara el objeto
	 * @param[in] parent Objeto padre al que se unira el objeto
	 */
	private static void setLaneStartPoint (char lane_type, Vector3 position, GameObject parent) {
		
		switch (lane_type) {
			case 'P':
				GameObject publicLaneStart = new GameObject();
				publicLaneStart.name = "Public Lane";
				publicLaneStart.tag = lane_start_point_tag;
				publicLaneStart.transform.position = position;
				publicLaneStart.transform.parent = parent.transform;
				break;
			case 'N':
				GameObject normalLaneStart = new GameObject();
				normalLaneStart.name = "Normal Lane";
				normalLaneStart.tag = lane_start_point_tag;
				normalLaneStart.transform.position = position;
				normalLaneStart.transform.parent = parent.transform;
				break;
			case 'A':
				Debug.Log("Parking lane start point not designed yet");
				break;
			case 'V':
				Debug.Log("Bus/VAO lane start point not designed yet");
				break;
			default:
				Debug.Log("Trying to draw invalid type of lane");
				break;
		}
	}

	/**
	 * @brief Dibuja una linea de carril segun su tipo
	 * @param[in] lane_type Tipo de carril (P: Transporte publico, N: Normal, A: Aparcamiento, V: Carril Bus/VAO)
	 * @param[in] length Longitud de la linea
	 * @param[in] position Posicion central de la linea
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private static void draw_lane_line (char lane_type, float length, Vector3 position, GameObject parent) {

		switch (lane_type) {
			case 'P':
				draw_continuous_line (public_transport_line_width, line_thickness, length, position, public_transport_lane_line_name, parent);
				break;
			case 'N':
				draw_discontinuous_line (line_width, line_thickness, length, position, normal_lane_line_name, parent);
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
	 * @param[in] position Posicion central de la linea
	 * @param[in] name Nombre para el objeto
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private static void draw_continuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent) {
		GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
		line.name = name;
		line.transform.localScale = new Vector3(width, height, length);
		line.transform.position = position;
		line.renderer.material.color = Color.white;
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
	private static void draw_discontinuous_line (float width, float height, float length, Vector3 position, string name, GameObject new_parent) {

		GameObject discontinuous_line = new GameObject ();
		discontinuous_line.name = discontinuous_line_name;
		discontinuous_line.transform.parent = new_parent.transform;

		int piece_num = (int)((length / discontinuous_line_length) / 2);
		Vector3 pos_aux = position;
		pos_aux.z -= (length / 2) - (discontinuous_line_length * 1.5f);

		for (int i=0; i < piece_num; i++) {

			GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
			line.name = name;
			line.transform.localScale = new Vector3(width, height, discontinuous_line_length);
			line.transform.position = pos_aux;
			line.renderer.material.color = Color.white;
			//line3.renderer.material = asphalt_white_material;
			//line3.renderer.material.mainTextureScale = new Vector2(line3.transform.localScale.x,line3.transform.localScale.z);
			line.transform.parent = discontinuous_line.transform;

			pos_aux.z += discontinuous_line_length * 2;
		}
	}

	/**
	 * @brief Calcula el numero total de carriles del arco cuyo identificador se pasa como argumento
	 * @param[in] edge_id Identificador del arco
	 * @return El numero entero de carriles que tiene el arco
	 */
	private static int lanes (string edge_id) {
		string src_des = edges [edge_id].src_des;
		string des_src = edges [edge_id].des_src;
		
		int lane_num = 0;
		
		if (src_des != no_lane_string) {
			lane_num += src_des.Length;
		}
		
		if (des_src != no_lane_string) {
			lane_num += des_src.Length;
		}

		return lane_num;
	}

	/**
	 * @brief Crea una malla para los nodos de tipo continuacion. El algoritmo ha sido obtenido de 
	 * http://wiki.unity3d.com/index.php/ProceduralPrimitives y adaptado a las necesidades de esta aplicacion
	 * @param[in] radius Radio de la circunferencia circunscrita por los arcos
	 * @param[in] width Ancho de los arcos
	 * @param[in] angle Angulo menor que forman los arcos
	 */
	private static void CreateContinuationNode (GameObject gameObject, float radius, float edge_width, float angle) {

		// TODO Añadir collider
		gameObject.AddComponent< MeshRenderer > ();
		gameObject.renderer.material = asphalt_material;
		MeshFilter filter = gameObject.AddComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		mesh.Clear();

		Vector2 left_point = new Vector2 (-edge_width * .5f, -radius * .5f);
		Vector2 right_point = new Vector2 (edge_width * .5f, -radius * .5f);

		// Rotar angle grados los puntos left y right

		float angle_rad = (angle * Mathf.PI) / 180f;

		Vector2 left_point_rotated = new Vector2 ();
		left_point_rotated.x = (left_point.x * Mathf.Cos(angle_rad)) - (left_point.y * Mathf.Sin(angle_rad));
		left_point_rotated.y = (left_point.x * Mathf.Sin(angle_rad)) + (left_point.y * Mathf.Cos(angle_rad));

		Vector2 right_point_rotated = new Vector2 ();
		right_point_rotated.x = (right_point.x * Mathf.Cos(angle_rad)) - (right_point.y * Mathf.Sin(angle_rad));
		right_point_rotated.y = (right_point.x * Mathf.Sin(angle_rad)) + (right_point.y * Mathf.Cos(angle_rad));
		
		#region Vertices
		Vector3 p0 = new Vector3(  right_point_rotated.x,	-road_thickness * .5f,		 right_point_rotated.y );
		Vector3 p1 = new Vector3(  left_point_rotated.x, 	-road_thickness * .5f,		 left_point_rotated.y );
		Vector3 p2 = new Vector3(  edge_width * .5f, 		-road_thickness * .5f,		-radius * .5f );
		Vector3 p3 = new Vector3( -edge_width * .5f,		-road_thickness * .5f,		-radius * .5f );
		
		Vector3 p4 = new Vector3(  right_point_rotated.x,	 road_thickness * .5f,   	 right_point_rotated.y );
		Vector3 p5 = new Vector3(  left_point_rotated.x, 	 road_thickness * .5f,   	 left_point_rotated.y );
		Vector3 p6 = new Vector3(  edge_width * .5f, 		 road_thickness * .5f,  	-radius * .5f );
		Vector3 p7 = new Vector3( -edge_width * .5f,	 	 road_thickness * .5f,  	-radius * .5f );

		
		Vector3[] vertices = new Vector3[]
		{
			// Bottom
			p0, p1, p2, p3,
			
			// Left
			p7, p4, p0, p3,
			
			// Front
			p4, p5, p1, p0,
			
			// Back
			p6, p7, p3, p2,
			
			// Right
			p5, p6, p2, p1,
			
			// Top
			p7, p6, p5, p4
		};
		#endregion
		
		#region Normales
		Vector3 up 	= Vector3.up;
		Vector3 down 	= Vector3.down;
		Vector3 front 	= Vector3.forward;
		Vector3 back 	= Vector3.back;
		Vector3 left 	= Vector3.left;
		Vector3 right 	= Vector3.right;
		
		Vector3[] normales = new Vector3[]
		{
			// Bottom
			down, down, down, down,
			
			// Left
			left, left, left, left,
			
			// Front
			front, front, front, front,
			
			// Back
			back, back, back, back,
			
			// Right
			right, right, right, right,
			
			// Top
			up, up, up, up
		};
		#endregion	
		
		#region UVs
		Vector2 _00 = new Vector2( 0f, 0f );
		Vector2 _10 = new Vector2( 1f, 0f );
		Vector2 _01 = new Vector2( 0f, 1f );
		Vector2 _11 = new Vector2( 1f, 1f );
		
		Vector2[] uvs = new Vector2[]
		{
			// Bottom
			_11, _01, _00, _10,
			
			// Left
			_11, _01, _00, _10,
			
			// Front
			_11, _01, _00, _10,
			
			// Back
			_11, _01, _00, _10,
			
			// Right
			_11, _01, _00, _10,
			
			// Top
			_11, _01, _00, _10,
		};
		#endregion
		
		#region Triangles
		int[] triangles = new int[]
		{
			// Bottom
			3, 1, 0,
			3, 2, 1,			
			
			// Left
			3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
			3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
			
			// Front
			3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
			3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
			
			// Back
			3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
			3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
			
			// Right
			3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
			3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
			
			// Top
			3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
			3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
			
		};
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
	}
}
