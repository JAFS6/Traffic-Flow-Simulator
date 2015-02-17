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
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public static class RoadMap {

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
	 * @brief A�ade un nuevo nodo al mapa
	 * @param[in] id Identificador alfanum�rico del nodo
	 * @param[in] node_type Tipo de nodo (0: nodo de cruce, 1: nodo de l�mite de v�a, 2: nodo de continuaci�n)
	 * @param[in] x Coordenada X
	 * @param[in] y Coordenada Y
	 * @param[in] intersection_type Tipo de cruce: normal (0) o rotonda (1) (Solo se aplica a los nodos de tipo cruce)
	 * @pre El id debe ser distinto a todos los ids ya insertados, si coincide con alguno, el nuevo nodo no se insertar�
	 */
	public static void addNode (string id, NodeType node_type, float x, float y, IntersectionType intersection_type = IntersectionType.Normal) {
		
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
	 * @brief A�ade un nuevo arco al mapa
	 * @param[in] id Identificador alfanum�rico del arco
	 * @param[in] source_id Identificador del nodo origen
	 * @param[in] destination_id Identificador del nodo destino
	 * @param[in] name Nombre de la v�a
	 * @param[in] src_des Cadena de tipos de carriles en el sentido origen-destino
	 * @param[in] des_src Cadena de tipos de carriles en el sentido destino-origen
	 * @pre El id debe ser distinto a todos los ids ya insertados, si coincide con alguno, el nuevo arco no se a�adir�
	 * @pre Los ids de los nodos origen y destino deben existir, si no existe alguno el arco no se insertar�
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
	 * @param[in] id Identificador alfanum�rico del nodo
	 * @param[in] new_intersection_type Nuevo tipo de intersecci�n
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
	 * @brief Establece el nombre del mapa
	 * @param[in] name Una cadena de texto con el nombre del mapa
	 */
	public static void setMapName (string name) {
		map_name = name;
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
	 * @brief Comprueba si un identificador de nodo existe
	 * @param[in] node_id El identificador a comprobar
	 * @return True si el nodo existe, false en caso contrario
	 */
	public static bool existsNode (string node_id) {
		
		if (nodes.ContainsKey (node_id)) {
			return true;
		}
		return false;
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
			return NodeType.Unknown;
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
	 * @brief Obtiene la direccion del arco con identificador edge_id en el plano XZ
	 * @param[in] edge_id El identificador del nodo
	 * @param[in] d Indicador de direccion fuente-destino o destino-fuente
	 * @return Un Vector2 con la direccion del arco en el plano XZ
	 * @post Si el identificador no existe se devolvera un vector (0,0)
	 */
	public static Vector2 getEdgeDirection (string edge_id, DirectionType d) {
		Vector2 pos = new Vector2 ();
		
		if (edges.ContainsKey (edge_id)) {
			pos.x = edges[edge_id].direction.x;
			pos.y = edges[edge_id].direction.y;
			
			if (d == DirectionType.Destination_Source) {
				pos.x = -pos.x;
				pos.y = -pos.y;
			}
		}
		
		return pos;
	}
	
	/**
	 * @brief Devuelve el id del arco que llega al nodo limite pasado como argumento
	 * @param[in] node_id Identificador del nodo limite
	 * @return Un string con el id del arco buscado o 
	 * la cadena Constants.String_Unknown si el nodo no es de tipo limite
	 */
	public static string getLimitEdge (string node_id) {
		
		if (nodes[node_id].node_type == NodeType.Limit) {
		
			foreach (KeyValuePair<string, Edge> edge in edges) {
				
				if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {
					return edge.Value.id;
				}
			}
		}
		return Constants.String_Unknown;
	}
	
	/**
	 * @brief Devuelve los IDs de los arcos que llegan al nodo de continuacion pasado como argumento
	 * @param[in] node_id Identificador del nodo continuacion
	 * @param[out] edge1 Identificador de uno de los arcos que llega al nodo pasado como argumento
	 * @param[out] edge2 Identificador de otro de los arcos que llega al nodo pasado como argumento
	 * @post El finalizar la ejecucion del metodo, los parametros edge1 y edge2 tendran los identificadores
	 * buscados o la cadena Constants.String_Unknown si el nodo no es de tipo continuacion
	 */
	public static void getContinuationEdges (string node_id, out string edge1, out string edge2) {
		
		edge1 = Constants.String_Unknown;
		edge2 = Constants.String_Unknown;
		
		if (nodes[node_id].node_type == NodeType.Continuation) {
		
			bool first_found = false;
			
			foreach (KeyValuePair<string, Edge> edge in edges) {
				
				if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {
				
					if (!first_found) {
						first_found = true;
						edge1 = edge.Value.id;
					}
					else {
						edge2 = edge.Value.id;
						break;
					}
				}
			}
		}
	}

	/**
	 * @brief Comprueba si el nodo limite pasado como argumento es un nodo por el que entran vehiculos al mapa
	 * @param[in] node_id Identificador del nodo limite
	 * @param[out] tt Tipo de transporte que entrara al mapa a traves de ese nodo
	 * @return True si hay algun carril de entrada al mapa desde ese nodo, false si no, o si el nodo pasado no es un nodo limite
	 */
	public static bool isEntryNode (string node_id, out TransportType tt) {
		
		tt = TransportType.Unknown;
		
		if (nodes[node_id].node_type == NodeType.Limit) {
			string edge_id = getLimitEdge(node_id);
			
			if (edges[edge_id].source_id == node_id && edges[edge_id].src_des != Constants.String_No_Lane) {
			
				if (edges[edge_id].src_des.Contains(Constants.String_Public_Lane) && edges[edge_id].src_des.Contains(Constants.String_Normal_Lane)) { // Contiene P y N
					tt = TransportType.PublicAndPrivate;
				}
				else if (edges[edge_id].src_des.Contains(Constants.String_Public_Lane)) { // Contiene solo P
					tt = TransportType.Public;
				}
				else if (edges[edge_id].src_des.Contains(Constants.String_Normal_Lane)) { // Contiene solo N
					tt = TransportType.Private;
				}
				
				return true;
			}
			else if (edges[edge_id].destination_id == node_id && edges[edge_id].des_src != Constants.String_No_Lane) {
			
				if (edges[edge_id].des_src.Contains(Constants.String_Public_Lane) && edges[edge_id].des_src.Contains(Constants.String_Normal_Lane)) { // Contiene P y N
					tt = TransportType.PublicAndPrivate;
				}
				else if (edges[edge_id].des_src.Contains(Constants.String_Public_Lane)) { // Contiene solo P
					tt = TransportType.Public;
				}
				else if (edges[edge_id].des_src.Contains(Constants.String_Normal_Lane)) { // Contiene solo N
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
			if (nodes[node_id].node_type == NodeType.Limit) {
				string edge_id = getLimitEdge(node_id);

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
	 * @brief Obtiene una lista de objetos que marcan los puntos de entrada al carril del nodo limite indicado como
	 * argumento.
	 * @param[in] node_id El identificador de un nodo limite
	 * @return Una lista de objetos o una lista vacia si el nodo indicado no era un nodo limite
	 */
	public static List<GameObject> getLaneStartPoints (string node_id) {
		
		List<GameObject> list = new List<GameObject>();
		
		if (nodes[node_id].node_type == NodeType.Limit) {
			string edge_id = RoadMap.getLimitEdge(node_id);
			
			GameObject object_edge = GameObject.Find(edge_id);
			
			GameObject StartPointsObject = null;
			
			if (edges[edge_id].source_id == node_id) {
				StartPointsObject = object_edge.transform.FindChild(Constants.Name_Source_Start_Points).gameObject;
			}
			else if (edges[edge_id].destination_id == node_id) {
				StartPointsObject = object_edge.transform.FindChild(Constants.Name_Destination_Start_Points).gameObject;
			}
			
			if (StartPointsObject != null) {
				
				foreach (Transform child in StartPointsObject.transform) {
					list.Add(child.gameObject);
				}
			}
		}
		
		return list;
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
				
				if (node_id == edges[edge_id].source_id && edges[edge_id].src_des != Constants.String_No_Lane) { // Hay algun carril de salida
					
					if ((tt == TransportType.Public && edges[edge_id].src_des.Contains(Constants.String_Public_Lane)) || 
						(tt == TransportType.Private && edges[edge_id].src_des.Contains(Constants.String_Normal_Lane))) { // Hay al menos un carril que se corresponde con el tipo de vehiculo
						
						exits.Add(edge_id);
					}
				}
				else if (node_id == edges[edge_id].destination_id && edges[edge_id].des_src != Constants.String_No_Lane) { // Hay algun carril de salida
					
					if ((tt == TransportType.Public && edges[edge_id].des_src.Contains(Constants.String_Public_Lane)) || 
					    (tt == TransportType.Private && edges[edge_id].des_src.Contains(Constants.String_Normal_Lane))) { // Hay al menos un carril que se corresponde con el tipo de vehiculo
						
						exits.Add(edge_id);
					}
				}
			}
		}
		
		return exits;
	}

	/**
	 * @brief Dibuja el mapa en el entorno 3D
	 */
	public static void draw () {
		
		prepareEdges ();
		
		// Dibujar el suelo base
		drawGround ();
		
		foreach (KeyValuePair<string, Edge> edge in edges){
			drawEdge (edge.Key);
		}
		
		foreach (KeyValuePair<string, Node> node in nodes){
			drawNode (node.Key);
		}
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
			e.length = MyMathClass.Distance(src_node_position, dst_node_position);
			// Ancho del arco
			e.width = (Constants.lane_width * e.lane_num) + ((e.lane_num + 1) * Constants.line_width) + 2 * (Constants.hard_shoulder_width);
			// Vector direccion del arco
			e.direction = new Vector2 (dst_node_position.x - src_node_position.x, dst_node_position.z - src_node_position.z);
			// Actualizar el arco
			edges[e.id] = e;
		}

		// Actualizar el arco mas ancho en cada nodo
		// y actualiza en los nodos de continuacion si son de un sentido o dos
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
					
					// Actualizacion de sentido
					if (n.node_type == NodeType.Continuation) {
						if (edges[edge_key].src_des != Constants.String_No_Lane && edges[edge_key].des_src != Constants.String_No_Lane) {
							n.two_ways = true;
						}
						else {
							n.two_ways = false;
						}
					}
				}
			}
			// Actualizar el nodo
			nodes[n.id] = n;
		}

		// Calcular el vector de ajuste de posicion del arco
		foreach (string edge_key in edge_keys) {
			Edge e = edges[edge_key];
			string source_node_id = e.source_id;
			string destination_node_id = e.destination_id;
			// Obtener el angulo polar del vector director del arco
			float polar_angle = MyMathClass.PolarAngle(e.direction);
			// Calcular la magnitud del vector de ajuste de posicion segun los nodos en los extremos del arco
			float fixed_length = 0;

			NodeType src_node_type = nodes[source_node_id].node_type;				// Tipo del nodo de origen
			NodeType dst_node_type = nodes[destination_node_id].node_type;			// Tipo del nodo de destino
			string src_id_widest_edge = nodes[source_node_id].widest_edge_id;		// Identificador del arco mas ancho en el nodo de origen
			string dst_id_widest_edge = nodes[destination_node_id].widest_edge_id;	// Identificador del arco mas ancho en el nodo de destino

			if (src_node_type == NodeType.Intersection || src_node_type == NodeType.Continuation) {
				float aux_width = edges[ src_id_widest_edge ].width /2;
				fixed_length += aux_width; // Desplazamiento en el sentido del vector director del arco
				e.length -= aux_width;
			}
			/*else if (src_node_type == NodeType.Limit) {
				No hacer nada, las carreteras comienzan en el centro de los nodos limite
			}*/

			if (dst_node_type == NodeType.Intersection || dst_node_type == NodeType.Continuation) {
				float aux_width = edges[ dst_id_widest_edge ].width /2;
				fixed_length -= aux_width; // Desplazamiento en sentido contrario del vector director del arco
				e.length -= aux_width;
			}
			/*else if (dst_node_type == NodeType.Limit) {
				No hacer nada, las carreteras comienzan en el centro de los nodos limite
			}*/
			
			// Dividir la longitud ajustada a la mitad para equiparar los m�rgenes
			fixed_length = fixed_length / 2;

			if (fixed_length < 0) {
				fixed_length = Mathf.Abs(fixed_length);
				polar_angle = (polar_angle + 180) % 360;
			}

			// Calcular el vector de ajuste de posicion
			e.fixed_position_vector = MyMathClass.PolarToCartesian (fixed_length, polar_angle);
			// Calcular la posicion ya ajustada
			Node src_node = nodes[source_node_id];
			Node dst_node = nodes[destination_node_id];
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

		if (n.node_type == NodeType.Limit) {  // DRAW LIMIT NODE
			Edge e = edges[getLimitEdge(n.id)];
			Node src_node = nodes[e.source_id];
			Node dst_node = nodes[e.destination_id];

			float width = (e.lane_num*Constants.lane_width) + 2*Constants.lane_width; // Para que sobresalga por ambos lados

			GameObject aux_road = GameObject.CreatePrimitive(PrimitiveType.Cube);
			aux_road.name = node_id;
			aux_road.tag = Constants.Tag_Node_Limit;
			aux_road.renderer.material = black_material;
			pos.y += (Constants.limit_height/2);
			aux_road.transform.localScale = new Vector3(width,Constants.limit_height,Constants.limit_depth);
			// Vector del nodo limite
			Vector2 dir = new Vector2 (0,1);
			aux_road.transform.rotation = Quaternion.AngleAxis(MyMathClass.RotationAngle(dir,e.direction),Vector3.up);
			aux_road.transform.position = pos;
		}
		else if (n.node_type == NodeType.Continuation) {  // DRAW CONTINUATION NODE
			GameObject aux_road = new GameObject();
			aux_road.name = node_id;
			aux_road.tag = Constants.Tag_Node_Continuation;
			float edge_width = nodeWidth(n.id);
			
			// Obtener los identificadores de los arcos involucrados con el nodo de continuacion
			string edgeID1, edgeID2;
			RoadMap.getContinuationEdges (n.id, out edgeID1, out edgeID2);
			// Elegir el arco cuya coordenada x sea menor
			string selected_edge = edgeID1;
			
			if (edges[edgeID2].fixed_position.x < edges[edgeID1].fixed_position.x) {
				selected_edge = edgeID2;
			}
			// Crear el nodo de continuacion
			CreateContinuationNode(aux_road, edge_width, edge_width, nodeAngle(n.id), selected_edge);
			
			Vector2 edge_direction = edges[selected_edge].direction;
			float rotation_degrees = MyMathClass.RotationAngle(new Vector2(0,-1),edge_direction);
			aux_road.transform.rotation = Quaternion.AngleAxis (rotation_degrees, new Vector3(0,1,0));
			aux_road.transform.position = pos;
		}
		else if (n.node_type == NodeType.Intersection) {  // DRAW INTERSECTION NODE
		
			GameObject road_prefab = Resources.Load("Prefabs/Road", typeof(GameObject)) as GameObject;
			
			if (road_prefab == null) {
				Debug.Log ("road_prefab is null");
			}
			else {
				GameObject aux_road = GameObject.Instantiate (road_prefab, pos, Quaternion.identity) as GameObject;
				aux_road.transform.localScale = new Vector3(edges[n.widest_edge_id].width,Constants.road_thickness,edges[n.widest_edge_id].width);

				if (n.node_type == NodeType.Intersection) {
					aux_road.name = node_id;
					aux_road.tag = Constants.Tag_Node_Intersection;
				}
				else {
					aux_road.name = node_id;
					aux_road.tag = Constants.Tag_Unknown;
				}
			}
		}
		else {
			Debug.Log("Trying to draw invalid node type");
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
		float angle_deg = MyMathClass.RotationAngle (vector_1, vector_2); //(-360,360)

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
		platform.tag = Constants.Tag_Edge;
		platform.transform.localScale = new Vector3(e.width, Constants.road_thickness, e.length);
		platform.renderer.material.color = Color.gray;
		platform.renderer.material = asphalt_material;
		platform.renderer.material.mainTextureScale = new Vector2(platform.transform.localScale.x, platform.transform.localScale.z);

		Vector3 position;

		// Marcas viales

		// Lineas de los arcenes
		position = new Vector3();
		position.x = platform.transform.position.x - ((e.width / 2) - Constants.hard_shoulder_width);
		position.y = platform.transform.position.y + (Constants.road_thickness/2)+(Constants.line_thickness/2);
		position.z = 0;
		draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, position, Constants.Line_Name_Hard_Shoulder, platform);

		position.x = platform.transform.position.x + ((e.width / 2) - Constants.hard_shoulder_width);
		draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, position, Constants.Line_Name_Hard_Shoulder, platform);

		// Lineas centrales
		if (e.src_des != Constants.String_No_Lane && e.des_src != Constants.String_No_Lane) { // Si ambos sentidos tienen carriles

			int lane_diff = 0; // Mismo numero de carriles en cada sentido
			
			if (e.src_des.Length != e.des_src.Length) { // Distinto numero de carriles en cada sentido
				lane_diff = e.src_des.Length - e.des_src.Length;
			}
			
			position.x = platform.transform.position.x - (Constants.center_lines_separation/2) - (lane_diff * (Constants.lane_width/2));
			draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, position, Constants.Line_Name_Center, platform);
			position.x = platform.transform.position.x + (Constants.center_lines_separation/2) - (lane_diff * (Constants.lane_width/2));
			draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, position, Constants.Line_Name_Center, platform);
		}

		// Lineas de carril

		Vector3 save_position = new Vector3 (position.x, position.y, position.z);

		// Pintar tantas lineas de tipo de carril como carriles menos uno haya en cada direccion
		// y poner tantos inicios de carril como carriles haya
		if (e.src_des != Constants.String_No_Lane) {
		
			GameObject source_start_points = new GameObject();
			source_start_points.transform.parent = platform.transform;
			source_start_points.name = Constants.Name_Source_Start_Points;
			source_start_points.tag = Constants.Tag_Lane_Start_Point_Group;
		
			for (int i=0; i<e.src_des.Length; i++) {
				char lane_type = e.src_des[i];
				position.x = platform.transform.position.x + ((e.width / 2) - Constants.hard_shoulder_width) - ((Constants.lane_width + Constants.line_width) * (i+1));
				
				if (i < e.src_des.Length-1) {
					draw_lane_line (lane_type, e.length, position, platform);
				}
				setLaneStartPoint (lane_type, new Vector3 (position.x + (Constants.lane_width/2), position.y, position.z - (e.length/2)), source_start_points);
			}

			// Lineas de detencion antes del cruce
			if (nodes[e.destination_id].node_type != NodeType.Continuation && nodes[e.destination_id].node_type != NodeType.Limit) {
				position.x = platform.transform.position.x + ((e.width / 2) - Constants.hard_shoulder_width) - (e.src_des.Length * (Constants.lane_width + Constants.line_width))/2;
				position.z = platform.transform.position.z + (e.length/2 - Constants.public_transport_line_width/2);
				draw_continuous_line (e.src_des.Length * (Constants.lane_width + Constants.line_width),Constants.line_thickness,Constants.public_transport_line_width,position,Constants.Line_Name_Detention,platform); // Intercambiado ancho por largo para hacer linea perpendicular
			}
		}

		if (e.des_src != Constants.String_No_Lane) {
			position = save_position;
			
			GameObject destination_start_points = new GameObject();
			destination_start_points.transform.parent = platform.transform;
			destination_start_points.name = Constants.Name_Destination_Start_Points;
			destination_start_points.tag = Constants.Tag_Lane_Start_Point_Group;

			for (int i=0; i<e.des_src.Length; i++) {
				char lane_type = e.des_src[i];
				position.x = platform.transform.position.x - ((e.width / 2) - Constants.hard_shoulder_width) + ((Constants.lane_width + Constants.line_width) * (i+1));
				
				if (i < e.des_src.Length-1) {
					draw_lane_line (lane_type, e.length, position, platform);
				}
				setLaneStartPoint (lane_type, new Vector3 (position.x - (Constants.lane_width/2), position.y, position.z + (e.length/2)), destination_start_points);
			}

			// Lineas de detencion antes del cruce
			if (nodes[e.source_id].node_type != NodeType.Continuation && nodes[e.source_id].node_type != NodeType.Limit) {
				position.x = platform.transform.position.x - ((e.width / 2) - Constants.hard_shoulder_width) + (e.des_src.Length * (Constants.lane_width + Constants.line_width))/2;
				position.z = platform.transform.position.z - (e.length/2 - Constants.public_transport_line_width/2);
				draw_continuous_line (e.des_src.Length * (Constants.lane_width + Constants.line_width),Constants.line_thickness,Constants.public_transport_line_width,position,Constants.Line_Name_Detention,platform); // Intercambiado ancho por largo para hacer linea perpendicular
			}
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
			case Constants.Char_Public_Lane:
				GameObject publicLaneStart = new GameObject();
				publicLaneStart.name = Constants.Lane_Name_Public;
				publicLaneStart.tag = Constants.Tag_Lane_Start_Point;
				publicLaneStart.transform.position = position;
				publicLaneStart.transform.parent = parent.transform;
				break;
			case Constants.Char_Normal_Lane:
				GameObject normalLaneStart = new GameObject();
				normalLaneStart.name = Constants.Lane_Name_Normal;
				normalLaneStart.tag = Constants.Tag_Lane_Start_Point;
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
			case Constants.Char_Public_Lane:
				draw_continuous_line (Constants.public_transport_line_width, Constants.line_thickness, length, position, Constants.Line_Name_Public_Transport_Lane, parent);
				break;
			case Constants.Char_Normal_Lane:
				draw_discontinuous_line (Constants.line_width, Constants.line_thickness, length, position, Constants.Line_Name_Normal_Lane, parent);
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
	 * @brief Dibuja una linea de carril segun su tipo
	 * @param[in] lane_type Tipo de carril (P: Transporte publico, N: Normal, A: Aparcamiento, V: Carril Bus/VAO)
	 * @param[in] position1 Posicion de un extremo de la linea
	 * @param[in] position2 Posicion de otro extremo de la linea
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private static void draw_lane_line (char lane_type, Vector3 position1, Vector3 position2, GameObject parent) {
	
		switch (lane_type) {
		case Constants.Char_Public_Lane:
			draw_continuous_line (Constants.public_transport_line_width, Constants.line_thickness, position1, position2, Constants.Line_Name_Public_Transport_Lane, parent);
			break;
		case Constants.Char_Normal_Lane:
			draw_discontinuous_line (Constants.line_width, Constants.line_thickness, position1, position2, Constants.Line_Name_Normal_Lane, parent);
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
	 * @brief Dibuja una linea continua blanca alineada con el eje Z
	 * @param[in] width Ancho de la linea
	 * @param[in] height Grosor de la linea
	 * @param[in] length Longitud de la linea
	 * @param[in] position Posicion central de la linea
	 * @param[in] name Nombre para el objeto
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private static void draw_continuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent) {
		
		Vector3 position1 = new Vector3 (position.x, position.y, position.z - (length/2));
		Vector3 position2 = new Vector3 (position.x, position.y, position.z + (length/2));
		
		draw_continuous_line (width, height, position1, position2, name, parent);
	}
	
	/**
	 * @brief Dibuja una linea continua blanca entre las posiciones position1 y position2
	 * @param[in] width Ancho de la linea
	 * @param[in] height Grosor de la linea
	 * @param[in] position1 Posicion de un extremo de la linea
	 * @param[in] position2 Posicion de otro extremo de la linea
	 * @param[in] name Nombre para el objeto
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private static void draw_continuous_line (float width, float height, Vector3 position1, Vector3 position2,string name, GameObject parent) {
		GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
		line.name = name;
		line.transform.localScale = new Vector3(width, height, MyMathClass.Distance(position1,position2));
		line.transform.position = MyMathClass.middlePoint(position1,position2);
		line.transform.rotation = Quaternion.LookRotation(MyMathClass.orientationVector(position1,position2));
		line.renderer.material.color = Color.white;
		//line3.renderer.material = asphalt_white_material;
		//line3.renderer.material.mainTextureScale = new Vector2(line3.transform.localScale.x,line3.transform.localScale.z);
		line.transform.parent = parent.transform;
	}

	/**
	 * @brief Dibuja una linea discontinua blanca alineada con el eje Z
	 * @param[in] width Ancho de la linea
	 * @param[in] height Grosor de la linea
	 * @param[in] length Longitud de la linea
	 * @param[in] position Posicion central de la linea
	 * @param[in] name Nombre para el objeto
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private static void draw_discontinuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent) {

		Vector3 position1 = new Vector3 (position.x, position.y, position.z - (length/2));
		Vector3 position2 = new Vector3 (position.x, position.y, position.z + (length/2));
		
		draw_discontinuous_line (width, height, position1, position2, name, parent);
	}
	
	/**
	 * @brief Dibuja una linea discontinua blanca entre las posiciones position1 y position2
	 * @param[in] width Ancho de la linea
	 * @param[in] height Grosor de la linea
	 * @param[in] position1 Posicion de un extremo de la linea
	 * @param[in] position2 Posicion de otro extremo de la linea
	 * @param[in] name Nombre para el objeto
	 * @param[in] parent Objeto padre al que se unira la linea
	 */
	private static void draw_discontinuous_line (float width, float height, Vector3 position1, Vector3 position2, string name, GameObject new_parent) {
	
		GameObject discontinuous_line = new GameObject ();
		discontinuous_line.name = Constants.Line_Name_Discontinuous;
		discontinuous_line.transform.parent = new_parent.transform;
		discontinuous_line.transform.position = MyMathClass.middlePoint(position1,position2);
		float length = MyMathClass.Distance(position1,position2);
		
		int piece_num = 0;
		
		while ( (((piece_num * 2) - 1) * Constants.discontinuous_line_length) + (2 * Constants.discontinuous_line_min_margin) <= length ) {
			piece_num++;
		}
		
		if ((((piece_num * 2) - 1) * Constants.discontinuous_line_length) + (2 * Constants.discontinuous_line_min_margin) > length) {
			piece_num--;
		}
		
		Vector3 pos_aux = MyMathClass.middlePoint(position1,position2);
		
		float white_and_no_line_lenght = ((piece_num * 2) - 1) * Constants.discontinuous_line_length;
		
		pos_aux.z -= Constants.discontinuous_line_length * ((float)piece_num - 1);
		
		for (int i=0; i < piece_num; i++) {
			
			GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
			line.name = name;
			line.transform.localScale = new Vector3(width, height, Constants.discontinuous_line_length);
			line.transform.position = pos_aux;
			line.renderer.material.color = Color.white;
			//line3.renderer.material = asphalt_white_material;
			//line3.renderer.material.mainTextureScale = new Vector2(line3.transform.localScale.x,line3.transform.localScale.z);
			line.transform.parent = discontinuous_line.transform;
			
			pos_aux.z += Constants.discontinuous_line_length * 2;
		}
		discontinuous_line.transform.rotation = Quaternion.LookRotation(MyMathClass.orientationVector(position1,position2));
		discontinuous_line.AddComponent<BoxCollider>();
		discontinuous_line.GetComponent<BoxCollider>().size = new Vector3(width, height, length);
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
		
		if (src_des != Constants.String_No_Lane) {
			lane_num += src_des.Length;
		}
		
		if (des_src != Constants.String_No_Lane) {
			lane_num += des_src.Length;
		}

		return lane_num;
	}

	/**
	 * @brief Crea una malla para los nodos de tipo continuacion. El algoritmo ha sido obtenido de 
	 * http://wiki.unity3d.com/index.php/ProceduralPrimitives y adaptado a las necesidades de esta aplicacion
	 * @param[in] radius Radio de la circunferencia circunscrita por los arcos
	 * @param[in] width Ancho de los arcos
	 * @param[in] angle Angulo menor que forman los arcos [0,360)
	 * @param[in] ref_edge_id Identificador del arco que se tomara como referencia para dibujar las marcas viales
	 */
	private static void CreateContinuationNode (GameObject node, float radius, float edge_width, float angle, string ref_edge_id) {
		
		node.AddComponent< BoxCollider >();
		node.AddComponent< MeshRenderer >();
		node.renderer.material = asphalt_material;
		MeshFilter filter = node.AddComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		mesh.Clear();
		
		float half_road_thickness = Constants.road_thickness * 0.5f;
		float half_negative_radius = -radius * 0.5f;
		float half_edge_width = edge_width * 0.5f;
		
		
		Vector2 left_point  = new Vector2 (-half_edge_width, half_negative_radius);
		Vector2 right_point = new Vector2 ( half_edge_width, half_negative_radius);

		// Rotar angle grados los puntos left y right
		Vector2 left_point_rotated  = MyMathClass.rotatePoint(left_point , angle);
		Vector2 right_point_rotated = MyMathClass.rotatePoint(right_point, angle);
		
		#region Vertices 
		Vector3 p0 = new Vector3(  right_point_rotated.x,	-half_road_thickness,	right_point_rotated.y );
		Vector3 p1 = new Vector3(  left_point_rotated.x, 	-half_road_thickness,	left_point_rotated.y  );
		Vector3 p2 = new Vector3(  half_edge_width, 		-half_road_thickness,	half_negative_radius  );
		Vector3 p3 = new Vector3( -half_edge_width,			-half_road_thickness,	half_negative_radius  );
		
		Vector3 p4 = new Vector3(  right_point_rotated.x,	 half_road_thickness,	right_point_rotated.y );
		Vector3 p5 = new Vector3(  left_point_rotated.x, 	 half_road_thickness,	left_point_rotated.y  );
		Vector3 p6 = new Vector3(  half_edge_width, 		 half_road_thickness,	half_negative_radius  );
		Vector3 p7 = new Vector3( -half_edge_width,	 		 half_road_thickness,	half_negative_radius  );
		
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
		
		// Fin plataforma
		
		Edge e = edges[ref_edge_id];
		
		// Calculos comunes
		float pos_y_lines = (Constants.road_thickness/2)+(Constants.line_thickness/2);
		float pos_z_lines = -((radius * 0.5f) + 0.1f);
		Vector2 road_center_point = new Vector2(0, pos_z_lines);
		Vector2 road_center_point_rotated = MyMathClass.rotatePoint(road_center_point, angle);
		float right_hard_shoulder_pos_x = (e.width / 2) - Constants.hard_shoulder_width;
		float left_hard_shoulder_pos_x = -((e.width / 2) - Constants.hard_shoulder_width);
		float lane_w_plus_line_w = Constants.lane_width + Constants.line_width;
		
		// Comprobar si el nodo es fuente o destino del arco de referencia
		bool is_source = true;
		
		if (node.name == e.destination_id) {
			is_source = false;
		}
		
		// Lineas del arcen
		
		Vector2 hard_shoulder_right_point = new Vector2 (right_hard_shoulder_pos_x, half_negative_radius);
		Vector2 hard_shoulder_left_point  = new Vector2 (left_hard_shoulder_pos_x , half_negative_radius);
		
		Vector2 hard_shoulder_right_rotated = MyMathClass.rotatePoint(hard_shoulder_right_point, angle);
		Vector2 hard_shoulder_left_rotated  = MyMathClass.rotatePoint(hard_shoulder_left_point, angle);
		
		draw_continuous_line(Constants.line_width,
		                     Constants.line_thickness,
		                     new Vector3( hard_shoulder_left_point.x,    pos_y_lines, hard_shoulder_left_point.y ),
		                     new Vector3( hard_shoulder_right_rotated.x, pos_y_lines, hard_shoulder_right_rotated.y ),
		                     Constants.Line_Name_Hard_Shoulder,
		                     node);
		
		draw_continuous_line(Constants.line_width,
		                     Constants.line_thickness,
		                     new Vector3( hard_shoulder_right_point.x,  pos_y_lines, hard_shoulder_right_point.y ),
		                     new Vector3( hard_shoulder_left_rotated.x, pos_y_lines, hard_shoulder_left_rotated.y ),
		                     Constants.Line_Name_Hard_Shoulder,
		                     node);
		
		// Lineas del centro
		
		if (nodes[node.name].two_ways) {
		
			int lane_diff = 0; // Mismo numero de carriles en cada sentido
			
			if (edges[ref_edge_id].src_des.Length != edges[ref_edge_id].des_src.Length) { // Distinto numero de carriles en cada sentido
				lane_diff = edges[ref_edge_id].src_des.Length - edges[ref_edge_id].des_src.Length;
			}
			
			// Calcular el punto central de las lineas centrales y el correspondiente girado
			Vector2 center_point;
			
			if (is_source) {
				center_point = new Vector2 (+ (lane_diff * (Constants.lane_width/2)), pos_z_lines);
			}
			else {
				center_point = new Vector2 (- (lane_diff * (Constants.lane_width/2)), pos_z_lines);
			}
			Vector2 center_point_rotated = MyMathClass.rotatePoint(center_point, angle);
			
			// Restarle dos veces el vector con origen en el punto road_center_point_rotated y extremo en el punto center_point_rotated
			// Para llevar el punto simetricamente al otro lado del punto central de la carretera
			Vector2 cp_cpr = new Vector2(center_point_rotated.x - road_center_point_rotated.x, center_point_rotated.y - road_center_point_rotated.y);
			center_point_rotated -= 2*cp_cpr;
			
			// Linea izquierda
			
			// Calcular el punto de inicio de la linea
			Vector2 line_begin_point = new Vector2(center_point.x - (Constants.center_lines_separation/2), center_point.y);
				
			// Calcular el punto girado
			Vector2 line_begin_point_rotated = MyMathClass.rotatePoint(line_begin_point, angle);
			
			// Restarle dos veces el vector con origen en el punto road_center_point_rotated y extremo en el punto line_begin_point_rotated
			// Para llevar el punto simetricamente al otro lado del punto central de la carretera
			Vector2 rcpr_lbpr = new Vector2(line_begin_point_rotated.x - road_center_point_rotated.x, line_begin_point_rotated.y - road_center_point_rotated.y);
			line_begin_point_rotated -= 2*rcpr_lbpr;
			
			// Preparar las posiciones y pintar las lineas
			Vector3 pos1 = new Vector3(line_begin_point.x,			pos_y_lines, line_begin_point.y);
			Vector3 pos2 = new Vector3(line_begin_point_rotated.x,	pos_y_lines, line_begin_point_rotated.y);
			
			draw_continuous_line(Constants.line_width,Constants.line_thickness,pos1,pos2,Constants.Line_Name_Center,node);
			
			// Repetir lo mismo para la linea derecha ajustando line_begin_point
			
			line_begin_point = new Vector2(center_point.x + (Constants.center_lines_separation/2), center_point.y);
			line_begin_point_rotated = MyMathClass.rotatePoint(line_begin_point, angle);
			rcpr_lbpr = new Vector2(line_begin_point_rotated.x - road_center_point_rotated.x, line_begin_point_rotated.y - road_center_point_rotated.y);
			line_begin_point_rotated -= 2*rcpr_lbpr;
			pos1 = new Vector3(line_begin_point.x,			pos_y_lines, line_begin_point.y);
			pos2 = new Vector3(line_begin_point_rotated.x,	pos_y_lines, line_begin_point_rotated.y);
			draw_continuous_line(Constants.line_width,Constants.line_thickness,pos1,pos2,Constants.Line_Name_Center,node);
		}
		
		// Lineas de carril
		
		// Si la direccion source -> destination tiene carriles
		if (e.src_des != Constants.String_No_Lane) {
			
			// Pintar una linea por cada uno de los carriles excepto el mas centrico
			for (int i=0; i < e.src_des.Length-1; i++) {
				
				// Calcular el punto de inicio de la linea
				Vector2 line_begin_point;
				
				if (is_source) {
					line_begin_point = new Vector2(left_hard_shoulder_pos_x + (lane_w_plus_line_w * (i+1)), pos_z_lines);
				}
				else {
					line_begin_point = new Vector2(right_hard_shoulder_pos_x - (lane_w_plus_line_w * (i+1)), pos_z_lines);
				}
				
				// Calcular el punto girado
				Vector2 line_begin_point_rotated = MyMathClass.rotatePoint(line_begin_point, angle);
				
				// Restarle dos veces el vector con origen en el punto road_center_point_rotated y extremo en el punto line_begin_point_rotated
				// Para llevar el punto simetricamente al otro lado del punto central de la carretera
				Vector2 rcpr_lbpr = new Vector2(line_begin_point_rotated.x - road_center_point_rotated.x, line_begin_point_rotated.y - road_center_point_rotated.y);
				line_begin_point_rotated -= 2*rcpr_lbpr;
				
				// Preparar las posiciones y pintar las lineas
				Vector3 pos1 = new Vector3(line_begin_point.x,			pos_y_lines, line_begin_point.y);
				Vector3 pos2 = new Vector3(line_begin_point_rotated.x,	pos_y_lines, line_begin_point_rotated.y);
				
				char lane_type = e.src_des[i];
				draw_lane_line (lane_type, pos1, pos2, node);
			}
		}
		
		// Si la direccion destination -> source tiene carriles
		if (e.des_src != Constants.String_No_Lane) {
		
			// Pintar una linea por cada uno de los carriles excepto el mas centrico
			for (int i=0; i < e.des_src.Length-1; i++) {
				
				// Calcular el punto de inicio de la linea
				Vector2 line_begin_point;
				
				if (is_source) {
					line_begin_point = new Vector2(right_hard_shoulder_pos_x - (lane_w_plus_line_w * (i+1)), pos_z_lines);
				}
				else {
					line_begin_point = new Vector2(left_hard_shoulder_pos_x + (lane_w_plus_line_w * (i+1)), pos_z_lines);
				}
				
				// Calcular el punto girado
				Vector2 line_begin_point_rotated = MyMathClass.rotatePoint(line_begin_point, angle);
				
				// Restarle dos veces el vector con origen en el punto road_center_point_rotated y extremo en el punto line_begin_point_rotated
				// Para llevar el punto simetricamente al otro lado del punto central de la carretera
				Vector2 rcpr_lbpr = new Vector2(line_begin_point_rotated.x - road_center_point_rotated.x, line_begin_point_rotated.y - road_center_point_rotated.y);
				line_begin_point_rotated -= 2*rcpr_lbpr;
				
				// Preparar las posiciones y pintar las lineas
				Vector3 pos1 = new Vector3(line_begin_point.x,			pos_y_lines, line_begin_point.y);
				Vector3 pos2 = new Vector3(line_begin_point_rotated.x,	pos_y_lines, line_begin_point_rotated.y);
				
				char lane_type = e.des_src[i];
				draw_lane_line (lane_type, pos1, pos2, node);
			}
		}
		// Fin marcas viales
	}
	
	/**
	 * @brief Dibuja el suelo de hierba
	 */
	private static void drawGround () {
		List<string> node_IDs = RoadMap.getNodeIDs ();
		
		Vector2 first_pos = RoadMap.getNodePosition (node_IDs [0]);
		
		float min_x = first_pos.x;
		float max_x = first_pos.x;
		float min_y = first_pos.y;
		float max_y = first_pos.y;
		
		foreach (string ID in node_IDs) {
			Vector2 pos = RoadMap.getNodePosition (ID);
			
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
		ground.name = Constants.Name_Ground;
		ground.tag = Constants.Tag_Ground;
		ground.transform.localScale = new Vector3((max_x-min_x)/10, 1, (max_y-min_y)/10); // Se divide por 10 porque las medidas del plano de unity son 10x10
		ground.renderer.material = grass_material;
		ground.renderer.material.mainTextureScale = new Vector2(ground.transform.localScale.x, ground.transform.localScale.z);
		
		Vector3 ground_position = new Vector3((max_x+min_x)/2,0,(max_y+min_y)/2);
		ground.transform.position = ground_position;
	}
}
