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
	public Vector2 fixed_position;
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
	public const float center_lines_separation = 0.2f;
	public const float discontinuous_line_length = 2f;

	private string map_name;
	private Dictionary<string, Node> nodes;
	private Dictionary<string, Edge> edges;
	
	// Constructor
	public RoadMap (string map_name) {
		this.map_name = map_name;
		this.nodes = new Dictionary<string, Node> ();
		this.edges = new Dictionary<string, Edge> ();
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
	 * @brief Obtiene el nombre del mapa
	 * @return Una cadena de texto con el nombre del mapa
	 */
	public string getMapName () {
		return map_name;
	}

	/**
	 * @brief Obtiene el numero de nodos del mapa
	 * @return El numero de nodos del mapa
	 */
	public int getNodeCount () {
		return nodes.Count;
	}

	/**
	 * @brief Obtiene una lista con los identificadores de los nodos del mapa
	 * @return Una lista de cadenas de texto
	 */
	public List<string> getNodeIDs () {
		List<string> l = new List<string>(nodes.Keys);
		return l;
	}

	/**
	 * @brief Obtiene la posicion del nodo con identificador n en el plano XZ
	 * @param[in] node_id El identificador del nodo
	 * @return Un Vector2 con la posicion del nodo en el plano XZ
	 * @post Si el identificador no existe se devolvera un vector (0,0)
	 */
	public Vector2 getPositionNode(string node_id) {

		Vector2 pos = new Vector2 ();

		if (nodes.ContainsKey (node_id)) {
			pos.x = nodes [node_id].x;
			pos.y = nodes [node_id].y;
		}

		return pos;
	}

	/**
	 * @brief Obtiene el numero de arcos del mapa
	 * @return El numero de arcos del mapa
	 */
	public int getEdgeCount () {
		return edges.Count;
	}

	/**
	 * @brief Obtiene una lista con los identificadores de los arcos del mapa
	 * @return Una lista de cadenas de texto
	 */
	public List<string> getEdgeIDs () {
		List<string> l = new List<string>(edges.Keys);
		return l;
	}

	/**
	 * @brief Dibuja el mapa en el entorno 3D
	 */
	public void draw () {

		prepareEdges ();

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
	 * @brief Procesa los arcos calculando su longitud, anchura y numero de carriles, asi como el ajuste de longitud y posicion para las intersecciones
	 * @pre Este metodo debe ser llamado antes de ejecutar el metodo drawEdge
	 */
	private void prepareEdges () {
		Debug.Log ("Preparing edges");

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
			e.length = Distance(src_node_position, dst_node_position);
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
			float polar_angle = PolarAngle(e.direction);
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
			e.fixed_position = PolarToCartesian (fixed_length, polar_angle);
			// Actualizar el arco
			edges[e.id] = e;
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

			float width = (e.lane_num*lane_width) + 2*lane_width; // Para que sobresalga por ambos lados

			Material black_material = Resources.Load ("Materials/Simple_Black", typeof(Material)) as Material;

			GameObject aux_road = GameObject.CreatePrimitive(PrimitiveType.Cube);
			aux_road.name = node_id + " - limit";
			aux_road.renderer.material = black_material;
			pos.y += (limit_height/2);
			aux_road.transform.localScale = new Vector3(width,limit_height,limit_depth);
			// Vector del nodo limite
			Vector2 dir = new Vector2 (0,1);
			aux_road.transform.rotation = Quaternion.Euler(0,RotationAngle(dir,e.direction),0);
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
				if (n.node_type == NodeType.CONTINUATION) {
					aux_road.name = node_id + " - continuation";
				}
				else if (n.node_type == NodeType.INTERSECTION) {
					aux_road.name = node_id + " - intersection";
				}
				else {
					aux_road.name = node_id + " - unknown type";
				}
			}
		}
	}

	/**
	 * @brief Devuelve el id del arco que llega al nodo limite pasado como argumento
	 * @param[in] node_id Identificador del nodo limite
	 * @return Un string con el id del arco buscado
	 */
	private string edgeLimit (string node_id) {
		foreach (KeyValuePair<string, Edge> edge in edges) {
			if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {
				return edge.Value.id;
			}
		}
		return "";
	}

	/**
	 * @brief Dibuja el arco con id "edge_id" en el entorno 3D
	 * @param[in] edge_id Identificador del arco a dibujar
	 * @pre Antes de ejecutar este metodo se debe ejecutar una vez el metodo prepareEdges
	 */
	private void drawEdge (string edge_id) {

		Material asphalt_material = Resources.Load ("Materials/Asphalt", typeof(Material)) as Material;
		Material asphalt_white_material = Resources.Load ("Materials/Asphalt_White", typeof(Material)) as Material;

		Debug.Log ("Drawing edge "+edge_id);
		Edge e = edges[edge_id];

		// Plataforma
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.name = edge_id;
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
		draw_continuous_line (line_width, line_thickness, e.length, position, "Left line", platform);

		position.x = platform.transform.position.x + ((e.width / 2) - hard_shoulder_width);
		draw_continuous_line (line_width, line_thickness, e.length, position, "Right line", platform);

		// Lineas centrales
		if (e.src_des != "0" && e.des_src != "0") { // Si ambos sentidos tienen carriles

			if (e.src_des.Length == e.des_src.Length) { // Mismo numero de carriles en cada sentido
				position.x = platform.transform.position.x - (center_lines_separation/2);
				draw_continuous_line (line_width, line_thickness, e.length, position, "Center line", platform);
				position.x = platform.transform.position.x + (center_lines_separation/2);
				draw_continuous_line (line_width, line_thickness, e.length, position, "Center line", platform);
			}
			else { // Distinto numero de carriles en cada sentido
				int lane_diff = e.src_des.Length - e.des_src.Length;

				position.x = platform.transform.position.x - (center_lines_separation/2) - (lane_diff * (lane_width/2));
				draw_continuous_line (line_width, line_thickness, e.length, position, "Center line", platform);
				position.x = platform.transform.position.x + (center_lines_separation/2) - (lane_diff * (lane_width/2));
				draw_continuous_line (line_width, line_thickness, e.length, position, "Center line", platform);
			}
		}

		// Lineas de carril

		// Pintar tantas lineas de tipo de carril como carriles menos uno haya en cada direccion
		if (e.src_des != "0") {
			for (int i=0; i<e.src_des.Length-1; i++) {
				position.x = platform.transform.position.x + ((e.width / 2) - hard_shoulder_width) - ((lane_width + line_width) * (i+1));
				char lane_type = e.src_des[i];
				draw_lane_line (lane_type, e.length, position, platform);

			}
		}

		if (e.des_src != "0") {
			for (int i=0; i<e.des_src.Length-1; i++) {
				position.x = platform.transform.position.x - ((e.width / 2) - hard_shoulder_width) + ((lane_width + line_width) * (i+1));
				char lane_type = e.des_src[i];
				draw_lane_line (lane_type, e.length, position, platform);
			}
		}

		// Fin marcas viales

		// Vector del arco recien dibujado
		Vector2 dir_pref = new Vector2 (0,1);

		Node src_node = nodes[e.source_id];
		Node dst_node = nodes[e.destination_id];
		Vector3 pos = new Vector3( (dst_node.x + src_node.x)/2, 0, (dst_node.y + src_node.y)/2);
		pos.x += e.fixed_position.x;
		pos.z += e.fixed_position.y;

		platform.transform.rotation = Quaternion.Euler(0,RotationAngle(dir_pref,e.direction),0);
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
	 * @brief Calcula el modulo de un vector de dos dimensiones
	 * @param[in] v Un vector de dos dimensiones
	 * @return El modulo del vector v
	 */
	private float module2D (Vector2 v) {
		return Mathf.Sqrt (v.x * v.x + v.y * v.y);
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

	/**
	 * @brief Convierte coordenadas polares a cartesianas
	 * @param[in] magnitude La magnitud del vector
	 * @param[in] polar_angle El angulo en grados de las coordenadas polares
	 */
	private Vector2 PolarToCartesian (float magnitude, float polar_angle) {
		Vector2 v = new Vector2 ();

		float polar_angle_rad = (polar_angle * Mathf.PI) / 180;

		v.x = magnitude * Mathf.Cos (polar_angle_rad);
		v.y = magnitude * Mathf.Sin (polar_angle_rad);

		return v;
	}
}
