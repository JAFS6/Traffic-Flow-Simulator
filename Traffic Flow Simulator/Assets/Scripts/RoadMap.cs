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
//using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public static class RoadMap {

	private static string map_name;
	private static Dictionary<string, Node> nodes;
	private static Dictionary<string, Edge> edges;
	
	public static float max_x,min_x,max_z,min_z; // Ground limits

	// Materials
	private static Material black_material;
	private static Material asphalt_material;
	private static Material white_asphalt_material;

	public static void CreateNewMap (string name) {
		map_name = name;
		nodes = new Dictionary<string, Node> ();
		edges = new Dictionary<string, Edge> ();
		
		black_material = Resources.Load ("Materials/Simple_Black", typeof(Material)) as Material;
		asphalt_material = Resources.Load ("Materials/Asphalt", typeof(Material)) as Material;
		white_asphalt_material = Resources.Load ("Materials/White_asphalt", typeof(Material)) as Material;
	}
	
	/**
	 * @brief Add a new node to the map
	 * @param[in] id Alphanumeric identifier of the node
	 * @param[in] node_type Node Type (0: intersection node, 1: node limit of track 2: continuation node)
	 * @param[in] x X coordinate of the new node
	 * @param[in] y Y coordinate of the new node
	 * @param[in] intersection_type Intersection type: normal (0) or roundabout (1) (Only applies to intersection nodes)
	 * @pre The id must be different from all previously inserted ids, if it matches one, the new node will not be inserted
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
	 * @brief Add a new edge to the map
	 * @param[in] id Alphanumeric identifier of the edge
	 * @param[in] source_id Source node identifier
	 * @param[in] destination_id Destination node identifier
	 * @param[in] name Name of the road
	 * @param[in] src_des Chain types of lanes in the direction source-destination
	 * @param[in] des_src Chain types of lanes in the direction destination-source
	 * @pre The id must be different from all previously inserted ids, if it matches one, the new edge will not be inserted
	 * @pre Ids source and destination nodes must exist, if some of them not exist the edge will not be inserted
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
	 * @brief Makes the 'id' node change its type to the type passed as an argument
	 * @param[in] id Alphanumeric identifier of the node
	 * @param[in] new_intersection_type New type of intersection
	 * @pre If the node does not exist, nothing is done
	 */
	public static void setIntersectionType (string id, IntersectionType new_intersection_type) {
		
		if ( nodes.ContainsKey (id) ) {
			Node aux_node = nodes[id];
			aux_node.intersection_type = new_intersection_type;
			nodes[id] = aux_node;
		}
	}
	
	/**
	 * @brief Sets the name of the map
	 * @param[in] name A string containing the name of the map
	 */
	public static void setMapName (string name) {
		map_name = name;
	}

	/**
	 * @brief Gets the name of the map
	 * @return A string containing the name of the map
	 */
	public static string getMapName () {
		return map_name;
	}

	/**
	 * @brief Gets the number of nodes in the map
	 * @return The number of nodes in the map
	 */
	public static int getNodeCount () {
		return nodes.Count;
	}
	
	/**
	 * @brief Checks if a node identifier exists
	 * @param[in] node_id The identifier to check
	 * @return True if the node exists, false otherwise
	 */
	public static bool existsNode (string node_id) {
		
		if (nodes.ContainsKey (node_id)) {
			return true;
		}
		return false;
	}

	/**
	 * @brief Gets a list of identifiers of the nodes of the map
	 * @return A list of strings
	 */
	public static List<string> getNodeIDs () {
		List<string> l = new List<string>(nodes.Keys);
		return l;
	}

	/**
	 * @brief Gets the position of the node with ID node_id in the XZ plane
	 * @param[in] node_id Node ID
	 * @return A Vector2 with the node position in the plane XZ
	 * @post If the ID does not exist a vector (0,0) is returned
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
	 * @brief Gets the type of the node with ID node_id
	 * @param[in] node_id Node ID
	 * @return The type of node
	 * @post If the ID does not exist the return type will be UNKNOWN
	 */
	public static NodeType getNodeType (string node_id) {

		if (nodes.ContainsKey (node_id)) {
			return nodes[node_id].node_type;
		}
		else {
			Debug.LogError ("Error on getNodeType, node " + node_id + " doesn't exists. Returning default.");
			return NodeType.Limit;
		}
	}

	/**
	 * @brief Gets the number of edges in the map
	 * @return The number of edges in the map
	 */
	public static int getEdgeCount () {
		return edges.Count;
	}

	/**
	 * @brief Gets a list of identifiers of the edges of the map
	 * @return A list of strings
	 */
	public static List<string> getEdgeIDs () {
		List<string> l = new List<string>(edges.Keys);
		return l;
	}
	
	/**
	 * @brief Gets the central position of the edge with ID edge_id in the plane XZ
	 * @param[in] edge_id Edge ID
	 * @return A Vector2 with the central position of the edge in the XZ plane
	 * @post If the ID does not exist a vector (0,0) is returned
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
	 * @brief Gets the direction of the edge with identifier edge_id in the plane XZ
	 * @param[in] edge_id Edge ID
	 * @param[in] d Direction indicator source-destination or destination-source
	 * @return A Vector2 with the direction of the edge in the XZ plane
	 * @post If the ID does not exist a vector (0,0) is returned
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
	 * @brief Returns the ids of the source and destination nodes of the edge given as an argument.
	 * @param[in] edge_id Edge ID
	 * @param[out] src_id Source node ID
	 * @param[out] dst_id Destination node ID
	 * @post After the execution of the method, the parameters src_id and dst_id will have the desired identifiers or 
	 * Constants.String_Unknown string if the edge does not exist
	 */
	public static void getEdgeNodeLimits (string edge_id, out string src_id, out string dst_id) {
		src_id = Constants.String_Unknown;
		dst_id = Constants.String_Unknown;
		
		if (edges.ContainsKey (edge_id)) {
			src_id = edges[edge_id].source_id;
			dst_id = edges[edge_id].destination_id;
		}
	}
	
	/**
	 * @brief Returns the position of each end of the edge given as an argument
	 * @param[in] edge_id Edge ID
	 * @param[out] src_pos The position of the source end of the edge
	 * @param[out] dst_pos The position of the destination end of the edge
	 * @post After the execution of the method, the parameters src_pos and dst_pos will have searched positions or
	 * (0,0) if the edge does not exist
	 */
	public static void getEdgeLimitsPositions (string edge_id, out Vector2 src_pos, out Vector2 dst_pos) {
		src_pos = new Vector2(0,0);
		dst_pos = new Vector2(0,0);
		
		if (edges.ContainsKey (edge_id)) {
			Vector2 aux = new Vector2(0,0);
			aux = edges[edge_id].direction * (edges[edge_id].length / 2);
			src_pos = (Vector2) edges[edge_id].fixed_position - aux;
			dst_pos = (Vector2) edges[edge_id].fixed_position + aux;
		}
	}
	
	/**
	 * @brief Returns the edge ID who arrive to the limit node given as an argument
	 * @param[in] node_id Limit node ID
	 * @return A string with the edge ID searched or Constants.String_Unknown string if the node type is not limit.
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
	 * @brief Returns the edge IDs who arrive to the continuation node given as an argument
	 * @param[in] node_id Continuation node ID
	 * @param[out] edge1 ID of one of the edges who arrive to the node given as an argument
	 * @param[out] edge2 ID of other of the edges who arrive to the node given as an argument
	 * @post After the execution of the method, the edge1 and Edge2 parameters will have the desired identifiers or 
	 * Constants.String_Unknown string if the node type is not continuation or does not exist
	 */
	public static void getContinuationEdges (string node_id, out string edge1, out string edge2) {
		
		edge1 = Constants.String_Unknown;
		edge2 = Constants.String_Unknown;
		
		if (nodes.ContainsKey (node_id)) {
		
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
	}

	/**
	 * @brief Checks if the limit node passed as an argument is a node by entering vehicles to map
	 * @param[in] node_id Limit node ID
	 * @param[out] tt Type of transport that will enter the map through that node
	 * @return True if there is a lane entry to the map from that node, false if not or if the node passed is not a limit node
	 */
	public static bool isEntryNode (string node_id, out TransportType tt) {
		
		tt = TransportType.PublicAndPrivate; // Default initialization
		
		if (nodes[node_id].node_type == NodeType.Limit) {
			string edge_id = getLimitEdge(node_id);
			
			if (edges[edge_id].source_id == node_id && edges[edge_id].src_des != Constants.String_No_Lane) {
			
				if (edges[edge_id].src_des.Contains(Constants.String_Public_Lane) && edges[edge_id].src_des.Contains(Constants.String_Normal_Lane)) { // Contains P and N
					tt = TransportType.PublicAndPrivate;
				}
				else if (edges[edge_id].src_des.Contains(Constants.String_Public_Lane)) { // Only contains P
					tt = TransportType.Public;
				}
				else if (edges[edge_id].src_des.Contains(Constants.String_Normal_Lane)) { // Only contains N
					tt = TransportType.Private;
				}
				
				return true;
			}
			else if (edges[edge_id].destination_id == node_id && edges[edge_id].des_src != Constants.String_No_Lane) {
			
				if (edges[edge_id].des_src.Contains(Constants.String_Public_Lane) && edges[edge_id].des_src.Contains(Constants.String_Normal_Lane)) { // Contains P and N
					tt = TransportType.PublicAndPrivate;
				}
				else if (edges[edge_id].des_src.Contains(Constants.String_Public_Lane)) { // Only contains P
					tt = TransportType.Public;
				}
				else if (edges[edge_id].des_src.Contains(Constants.String_Normal_Lane)) { // Only contains N
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
	 * @brief Calculate the vector indicating the direction of entry to map from the limit node passed as an argument.
	 * @param[in] node_id Limit node ID
	 * @return The orientation vector calculated
	 * @post If the ID does not exist or is no limit node, (0,0) is returned
	 */
	public static Vector2 entryOrientation (string node_id) {
		Vector3 v = new Vector3 (0,0,0);

		if (nodes.ContainsKey (node_id)) {
			if (nodes[node_id].node_type == NodeType.Limit) {
				string edge_id = getLimitEdge(node_id);

				if (edges[edge_id].source_id == node_id) {
					// Destination - source
					v.x = nodes[ edges[edge_id].destination_id ].x - nodes[ edges[edge_id].source_id ].x;
					v.z = nodes[ edges[edge_id].destination_id ].y - nodes[ edges[edge_id].source_id ].y;
				}
				else {
					// Source - destination
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
	 * @brief Gets a list of objects that mark the entry points to the lane of the limit node specified as an argument
	 * @param[in] node_id Limit node ID
	 * @return A list of objects or an empty list if the specified node was not a limit node
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
	 * @brief Gets a list of identifiers of the edges through which a vehicle can leave the node 
	 * having entered it by the edge indicated.
	 * @param[in] node_id Id of the node on which the vehicle is
	 * @param[in] entry_edge_id Edge ID through which entered the node
	 * @param[in] tt Vehicle transport type
	 */
	public static List<string> exitPaths (string node_id, string entry_edge_id, TransportType tt) {
	
		List<string> exits = new List<string>();
		exits.Clear();
		
		if (nodes.ContainsKey(node_id) && edges.ContainsKey(entry_edge_id)) {
			List<string> edge_keys = new List<string> (edges.Keys);
			
			foreach (string edge_id in edge_keys) {
				
				if (edge_id != entry_edge_id) { // The edge is not the edge through it has entered
					
					if (node_id == edges[edge_id].source_id && edges[edge_id].src_des != Constants.String_No_Lane) { // There is at least one exit lane
						
						if ((tt == TransportType.Public && edges[edge_id].src_des.Contains(Constants.String_Public_Lane)) || 
							(tt == TransportType.Private && edges[edge_id].src_des.Contains(Constants.String_Normal_Lane))) { // There is at least one lane which corresponds to the vehicle type
							
							exits.Add(edge_id);
						}
					}
					else if (node_id == edges[edge_id].destination_id && edges[edge_id].des_src != Constants.String_No_Lane) { // There is at least one exit lane
						
						if ((tt == TransportType.Public && edges[edge_id].des_src.Contains(Constants.String_Public_Lane)) || 
						    (tt == TransportType.Private && edges[edge_id].des_src.Contains(Constants.String_Normal_Lane))) { // There is at least one lane which corresponds to the vehicle type
							
							exits.Add(edge_id);
						}
					}
				}
			}
		}
		else {
			if (!nodes.ContainsKey(node_id)) {
				Debug.LogError("exitPaths error: Node "+node_id+" don't exists.");
			}
			if (!edges.ContainsKey(entry_edge_id)) {
				Debug.LogError("exitPaths error: Edge "+entry_edge_id+" don't exists.");
			}
		}
		
		return exits;
	}

	/**
	 * @brief Draw the map in 3D environment
	 */
	public static void draw ()
	{
		calculateMapLimits ();
		// Draw the ground
		DrawEnviroment.Ground ();
		// Calculate info before draw
		prepareEdges ();
		// Draw the edges
		foreach (KeyValuePair<string, Edge> edge in edges)
			drawEdge (edge.Key);
		// Draw the nodes
		foreach (KeyValuePair<string, Node> node in nodes)
			drawNode (node.Key);
	}
	
	/**
	 * @brief Calculates the map limits.
	 */
	private static void calculateMapLimits ()
	{
		List<string> node_IDs = RoadMap.getNodeIDs ();
		
		Vector2 first_pos = RoadMap.getNodePosition (node_IDs [0]);
		
		min_x = first_pos.x;
		max_x = first_pos.x;
		min_z = first_pos.y;
		max_z = first_pos.y;
		
		foreach (string ID in node_IDs)
		{
			Vector2 pos = RoadMap.getNodePosition (ID);
			
			if (pos.x < min_x) {
				min_x = pos.x;
			}
			else if (pos.x > max_x) {
				max_x = pos.x;
			}
			
			if (pos.y < min_z) {
				min_z = pos.y;
			}
			else if (pos.y > max_z) {
				max_z = pos.y;
			}
		}
		max_x += Constants.grass_ground_padding;
		max_z += Constants.grass_ground_padding;
		min_x -= Constants.grass_ground_padding;
		min_z -= Constants.grass_ground_padding;
	}
	
	/**
	 * @brief Processes edges calculating its length, width and number of lanes as well as the length and position adjustment for intersections
	 * @pre This method must be called before running the method DrawEdge
	 */
	private static void prepareEdges () {

		List<string> edge_keys = new List<string> (edges.Keys);

		// Calculate the number of lanes, the edge length, the width of the edge and edge direction vector
		foreach (string edge_key in edge_keys) {
			Edge e = edges[edge_key];
			// Number of lanes
			e.lane_num = lanes (e.id);
			Node src_node = nodes[e.source_id];
			Node dst_node = nodes[e.destination_id];
			Vector3 src_node_position = new Vector3 (src_node.x,0,src_node.y);
			Vector3 dst_node_position = new Vector3 (dst_node.x,0,dst_node.y);
			// Edge length
			e.length = MyMathClass.Distance(src_node_position, dst_node_position);
			// Edge width
			e.width = (Constants.lane_width * e.lane_num) + ((e.lane_num + 1) * Constants.line_width) + 2 * (Constants.hard_shoulder_width);
			// Edge direction vector
			e.direction = new Vector2 (dst_node_position.x - src_node_position.x, dst_node_position.z - src_node_position.z);
			// Reload edge
			edges[e.id] = e;
		}

		// Update the widest edge on each node
		// and update continuation nodes if they are of one direction or two.
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
					
					// Sense update
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
			// Update node
			nodes[n.id] = n;
		}

		// Calculate the edge vector position adjustment.
		foreach (string edge_key in edge_keys) {
			Edge e = edges[edge_key];
			string source_node_id = e.source_id;
			string destination_node_id = e.destination_id;
			// Get the polar angle of the edge direction vector
			float polar_angle = MyMathClass.PolarAngle(e.direction);
			// Calculate the magnitude of the vector position adjustment depending on the nodes at the ends of the edge
			float fixed_length = 0;

			NodeType src_node_type = nodes[source_node_id].node_type;				// Source node type
			NodeType dst_node_type = nodes[destination_node_id].node_type;			// Destination node type
			string src_id_widest_edge = nodes[source_node_id].widest_edge_id;		// Widest node ID in the source node
			string dst_id_widest_edge = nodes[destination_node_id].widest_edge_id;	// Widest node ID in the destination node

			if (src_node_type == NodeType.Intersection || src_node_type == NodeType.Continuation) {
				float aux_width = edges[ src_id_widest_edge ].width /2;
				fixed_length += aux_width; // Displacement in the direction of the edge direction vector
				e.length -= aux_width;
			}
			/*else if (src_node_type == NodeType.Limit) {
				Do nothing, roads beginning at the center of the limit nodes
			}*/

			if (dst_node_type == NodeType.Intersection || dst_node_type == NodeType.Continuation) {
				float aux_width = edges[ dst_id_widest_edge ].width /2;
				fixed_length -= aux_width; // Displacement in the opposite direction of the edge direction vector
				e.length -= aux_width;
			}
			/*else if (dst_node_type == NodeType.Limit) {
				Do nothing, roads beginning at the center of the limit nodes
			}*/
			
			// Divide in half length-adjusted to match the margins
			fixed_length = fixed_length / 2;

			if (fixed_length < 0) {
				fixed_length = Mathf.Abs(fixed_length);
				polar_angle = (polar_angle + 180) % 360;
			}

			// Calculate the adjustment position vector
			e.fixed_position_vector = MyMathClass.PolarToCartesian (fixed_length, polar_angle);
			// Calculate the adjusted position
			Node src_node = nodes[source_node_id];
			Node dst_node = nodes[destination_node_id];
			e.fixed_position = new Vector3( (dst_node.x + src_node.x)/2, 0, (dst_node.y + src_node.y)/2);
			e.fixed_position.x += e.fixed_position_vector.x;
			e.fixed_position.z += e.fixed_position_vector.y;

			// Update edge
			edges[e.id] = e;
		}
	}

	/**
	 * @brief Draw the node with id "node_id" in the 3D environment
	 * @param[in] node_id Identifier of the node to draw
	 */
	private static void drawNode (string node_id) {

		Node n = nodes[node_id];
		Vector3 pos = new Vector3 (n.x, 0, n.y);

		if (n.node_type == NodeType.Limit) {  // DRAW LIMIT NODE
			Edge e = edges[getLimitEdge(n.id)];

			float width = (e.lane_num*Constants.lane_width) + 2*Constants.lane_width; // To protrude from both sides

			GameObject aux_road = GameObject.CreatePrimitive(PrimitiveType.Cube);
			aux_road.name = node_id;
			aux_road.tag = Constants.Tag_Node_Limit;
			aux_road.GetComponent<Renderer>().material = black_material;
			pos.y += (Constants.limit_height/2);
			aux_road.transform.localScale = new Vector3(width,Constants.limit_height,Constants.limit_depth);
			aux_road.transform.rotation = Quaternion.AngleAxis(MyMathClass.RotationAngle(new Vector2 (0,1),e.direction),Vector3.down); // Vector (0,1) is the orientation of the limit node
			aux_road.transform.position = pos;
			// Place the node in the roads layer
			MyUtilitiesClass.MoveToLayer(aux_road.transform,LayerMask.NameToLayer(Constants.Layer_Roads));
		}
		else if (n.node_type == NodeType.Continuation) {  // DRAW CONTINUATION NODE
			GameObject aux_road = new GameObject();
			aux_road.name = node_id;
			aux_road.tag = Constants.Tag_Node_Continuation;
			float edge_width = nodeWidth(n.id);
			
			// Get the identifiers of the edges involved with the continuation node
			string edgeID1, edgeID2;
			RoadMap.getContinuationEdges (n.id, out edgeID1, out edgeID2);
			
			// Choose the edge whose x coordinate is less
			string selected_edge = edgeID1;
			string non_selected_edge = edgeID2;
			
			if (edges[edgeID2].fixed_position.x < edges[edgeID1].fixed_position.x) {
				selected_edge = edgeID2;
				non_selected_edge = edgeID1;
			}
			
			// Calculate angle and side of the turn
			TurnSide side;
			float angle_between_edges = nodeAngle(n.id, selected_edge, non_selected_edge, out side);
			// Create the continuation node
			CreateContinuationNode(node_id, aux_road, edge_width, edge_width, angle_between_edges, side, selected_edge);
			
			Vector2 edge_direction = new Vector2(nodes[node_id].x - edges[selected_edge].fixed_position.x, nodes[node_id].y - edges[selected_edge].fixed_position.z);
			edge_direction.Normalize();
			float rotation_degrees = Vector2.Angle(new Vector2(0,1),edge_direction); // Vector (0,1) is the orientation of the continuation node
			
			if (side == TurnSide.Left) {
				rotation_degrees = -rotation_degrees;
			}
			
			aux_road.transform.rotation = Quaternion.AngleAxis (rotation_degrees, Vector3.up);
			aux_road.transform.position = pos;
			// Place the node in the roads layer
			MyUtilitiesClass.MoveToLayer(aux_road.transform,LayerMask.NameToLayer(Constants.Layer_Roads));
		}
		else if (n.node_type == NodeType.Intersection) {  // DRAW INTERSECTION NODE
		
			GameObject road_prefab = Resources.Load("Prefabs/Road", typeof(GameObject)) as GameObject;
			
			if (road_prefab == null) {
				Debug.Log ("road_prefab is null");
			}
			else {
				pos.y = -0.05f + Constants.platform_Y_position;
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
				// Place the node in the roads layer
				MyUtilitiesClass.MoveToLayer(aux_road.transform,LayerMask.NameToLayer(Constants.Layer_Roads));
			}
		}
		else {
			Debug.Log("Trying to draw invalid node type");
		}
	} // End drawNode

	/**
	 * @brief Returns the width of the edges that reach the continuation node passed as an argument (both have the same width)
	 * @param[in] node_id Continuation node ID
	 * @return Width searched
	 */
	private static float nodeWidth (string node_id) {

		// Since the width of both edges is the same, as we find the first search stops

		foreach (KeyValuePair<string, Edge> edge in edges) {

			if (edge.Value.source_id == node_id || edge.Value.destination_id == node_id) {
				return edge.Value.width;
			}
		}
		return -1f;
	}

	/**
	 * @brief Calculate the angle that occurs between the edges reaching the continuation node passed as an argument.
	 * Also calculates the turn side of the turn.
	 * @param[in] node_id Continuation node ID
	 * @param[in] ref_edge_id Edge identifier of the referency edge
	 * @param[in] edge2_id Identifier of other edge
	 * @param[out] side Indicates if the turn is to the left or to the right
	 * @return The angle calculated in degrees [0,180]
	 * @post After the execution of this method, the value of side will haven been set
	 */
	private static float nodeAngle (string node_id, string ref_edge_id, string edge2_id, out TurnSide side) {
	
		// Position of the node in 2D and 3D
		Vector2 node_pos_2D = new Vector2(nodes[node_id].x,    nodes[node_id].y);
		
		// Position of the reference edge in 2D and 3D
		Vector2 ref_edge_pos_2D = new Vector2(edges[ref_edge_id].fixed_position.x, edges[ref_edge_id].fixed_position.z);
		
		// Position of the other edge in 2D and 3D
		Vector2 oth_edge_pos_2D = new Vector2(edges[edge2_id].fixed_position.x, edges[edge2_id].fixed_position.z);
		
		// Calculate two vectors originating from the position of the node and vertex in the position of the edges
		Vector2 vector_1 = MyMathClass.orientationVector(node_pos_2D, ref_edge_pos_2D);
		Vector2 vector_2 = MyMathClass.orientationVector(node_pos_2D, oth_edge_pos_2D);
		
		// Normalize that vectors
		vector_1.Normalize();
		vector_2.Normalize();
		
		// Calculate the smallest angle between the two vectors
		float angle_deg = Vector2.Angle(vector_1, vector_2); //[0,180]
		
		/* Calculate side:
			Get normalized vector with orientation node position -> reference edge position (vector_1)
			Get perpendicular vectors of it at left and right (left_perp, right_perp)
			Get coordinates of the ends of both vectors
			Get middle point of the positions of the edges (MP)
			If the end of left_perp is closer to MP than the end of right_perp
				The turn is to the right
			Otherwise
				The turn is to the left
		*/
		
		Vector2 left_perp = MyMathClass.getLeftPerpendicular(vector_1);
		Vector2 right_perp = MyMathClass.getRightPerpendicular(vector_1);
		
		Vector2 left_point = node_pos_2D + left_perp;
		Vector2 right_point = node_pos_2D + right_perp;
		
		Vector2 MP = MyMathClass.middlePoint(ref_edge_pos_2D, oth_edge_pos_2D);
		
		float distance_left_MP = MyMathClass.Distance(left_point, MP);
		float distance_right_MP = MyMathClass.Distance(right_point, MP);
		
		if (distance_left_MP < distance_right_MP) {
			side = TurnSide.Right;
		}
		else {
			side = TurnSide.Left;
		}
		
		return angle_deg;
	}

	/**
	 * @brief Draw the edge with id "edge_id" in the 3D environment
	 * @param[in] edge_id Edge ID to draw
	 * @pre Before running this method should be run once prepareEdges method
	 */
	private static void drawEdge (string edge_id)
	{
		Edge e = edges[edge_id];
		
		GameObject edge_root = new GameObject();
		edge_root.name = edge_id;
		edge_root.tag = Constants.Tag_Edge;
		
		GameObject topology = new GameObject();
		topology.name = Constants.Name_Topological_Objects;
		topology.transform.SetParent(edge_root.transform);
		
		#region Platform
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.name = Constants.Name_Platform;
		platform.transform.SetParent(topology.transform);
		platform.transform.localScale = new Vector3(e.width, Constants.road_thickness, e.length);
		platform.GetComponent<Renderer>().material.color = Color.gray;
		platform.GetComponent<Renderer>().material = asphalt_material;
		platform.GetComponent<Renderer>().material.mainTextureScale = new Vector2(platform.transform.localScale.x, platform.transform.localScale.z);
		Vector3 platform_position = new Vector3(0,(-Constants.road_thickness/2) + Constants.platform_Y_position,0);
		platform.transform.position = platform_position;
		#endregion
		
		// Road markings
		float lines_Y_pos = (-Constants.line_thickness/2) + Constants.markings_Y_position;

		#region Hard shoulder lines
		float hard_shoulder_d = (e.width/2) - Constants.hard_shoulder_width - (Constants.line_width/2); // Displacement from the center of the road
		draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, new Vector3(-hard_shoulder_d,lines_Y_pos,0), Constants.Line_Name_Hard_Shoulder, topology);
		draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, new Vector3( hard_shoulder_d,lines_Y_pos,0), Constants.Line_Name_Hard_Shoulder, topology);
		#endregion
		
		// Lane number on source-destination direction
		int lane_num_src_des = (e.src_des == Constants.String_No_Lane) ? 0 : e.src_des.Length;
		// Lane number on destination-source direction
		int lane_num_des_src = (e.des_src == Constants.String_No_Lane) ? 0 : e.des_src.Length;
		
		float half_lane_width = Constants.lane_width/2;
		float half_length = e.length/2;
		
		#region Center lines
		if (lane_num_src_des > 0 && lane_num_des_src > 0)
		{
			// If both directions have lanes
			int lane_diff = 0; // Same number of lanes in each direction
			
			if (lane_num_src_des != lane_num_des_src) // Different number of lanes in each direction
			{
				lane_diff = lane_num_src_des - lane_num_des_src;
			}
			float half_center_lines_separation = Constants.center_lines_separation/2;
			float center_line_common_calc = - (lane_diff * half_lane_width);
			draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, new Vector3(center_line_common_calc - half_center_lines_separation,lines_Y_pos,0), Constants.Line_Name_Center, topology);
			draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, new Vector3(center_line_common_calc + half_center_lines_separation,lines_Y_pos,0), Constants.Line_Name_Center, topology);
		}
		#endregion

		#region Lane lines
		float markings_d = (e.length / 2) - 4f;
		GameObject source_start_points = new GameObject();
		source_start_points.transform.SetParent(edge_root.transform);
		source_start_points.name = Constants.Name_Source_Start_Points;
		source_start_points.tag = Constants.Tag_Lane_Start_Point_Group;
		
		GameObject destination_start_points = new GameObject();
		destination_start_points.transform.SetParent(edge_root.transform);
		destination_start_points.name = Constants.Name_Destination_Start_Points;
		destination_start_points.tag = Constants.Tag_Lane_Start_Point_Group;
		
		// Paint as many lines as lanes are in each direction except one 
		// and put as many start lane as lanes are.
		
		for (int i=0; i < lane_num_src_des || i < lane_num_des_src; i++)
		{
			float lane_d = (Constants.lane_width + Constants.line_width) * (i+1);
			
			if (i < lane_num_src_des)
			{
				char  src_des_lane_type = e.src_des[i];
				float src_des_posX = + hard_shoulder_d - lane_d;
				
				if (i < lane_num_src_des-1)
				{
					draw_lane_line (src_des_lane_type, e.length, new Vector3(src_des_posX, lines_Y_pos, 0), topology);
				}
					
				setLaneStartPoint (src_des_lane_type, new Vector3 (src_des_posX + half_lane_width, 0, - half_length), source_start_points);
				
				Vector2 src_des_marking_pos = new Vector2(src_des_posX + half_lane_width, - markings_d);
				DrawRoad.lane_markings (src_des_lane_type, src_des_marking_pos, true, topology);
			}
			
			if (i < lane_num_des_src)
			{
				char  des_src_lane_type = e.des_src[i];
				float des_src_posX = - hard_shoulder_d + lane_d;
				
				if (i < lane_num_des_src-1)
				{
					draw_lane_line (des_src_lane_type, e.length, new Vector3(des_src_posX, lines_Y_pos, 0), topology);
				}
				
				setLaneStartPoint (des_src_lane_type, new Vector3 (des_src_posX - half_lane_width, 0, + half_length), destination_start_points);
				
				Vector2 des_src_marking_pos = new Vector2(des_src_posX - half_lane_width, + markings_d);
				DrawRoad.lane_markings (des_src_lane_type, des_src_marking_pos, false, topology);
			}
		}
		#endregion
		
		#region Detention lines
		float detention_line_dZ = half_length - (Constants.public_transport_line_width/2);
		
		if (nodes[e.destination_id].node_type != NodeType.Continuation && nodes[e.destination_id].node_type != NodeType.Limit && lane_num_src_des > 0)
		{
			float detention_line_posX = (+hard_shoulder_d) - ((lane_num_src_des * (Constants.lane_width + Constants.line_width))/2);
			draw_continuous_line (MyUtilitiesClass.detentionLineWidth(e.src_des), 
									Constants.line_thickness, 
									Constants.public_transport_line_width, 
									new Vector3(detention_line_posX, lines_Y_pos, + detention_line_dZ), 
									Constants.Line_Name_Detention, 
									topology);
		}
		
		if (nodes[e.source_id].node_type != NodeType.Continuation && nodes[e.source_id].node_type != NodeType.Limit && lane_num_des_src > 0)
		{
			float detention_line_posX = (-hard_shoulder_d) + ((lane_num_des_src * (Constants.lane_width + Constants.line_width))/2);
			draw_continuous_line (MyUtilitiesClass.detentionLineWidth(e.des_src), 
									Constants.line_thickness, 
									Constants.public_transport_line_width, 
									new Vector3(detention_line_posX, lines_Y_pos, - detention_line_dZ), 
									Constants.Line_Name_Detention, 
									topology);
		}
		#endregion

		// End road markings

		edge_root.transform.rotation = Quaternion.AngleAxis(MyMathClass.RotationAngle(new Vector2 (0,1),e.direction),Vector3.down);  // Vector (0,1) is the orientation of the newly drawn edge
		edge_root.transform.position = e.fixed_position;
		// Place the edge in the roads layer
		MyUtilitiesClass.MoveToLayer(edge_root.transform,LayerMask.NameToLayer(Constants.Layer_Roads));
	} // End drawEdge
	
	/**
	 * @brief Sets a LaneStart object at the specified position
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] position Position where the object will be placed
	 * @param[in] parent Parent object to which the object will join
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
				Debug.Log("Bus/HOV lane start point not designed yet");
				break;
			default:
				Debug.Log("Trying to draw invalid type of lane");
				break;
		}
	}

	/**
	 * @brief Draw a lane line by type aligned with the Z axis
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] length Line length
	 * @param[in] position Center line position
	 * @param[in] parent Parent object to which the line will join
	 */
	private static void draw_lane_line (char lane_type, float length, Vector3 position, GameObject parent) {

		Vector3 position1 = new Vector3(position.x, position.y, position.z - (length/2));
		Vector3 position2 = new Vector3(position.x, position.y, position.z + (length/2));
		
		draw_lane_line (lane_type, position1, position2, parent);
	}
	
	/**
	 * @brief Draw a lane line by type aligned with the Z axis
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Position of the other end of the line
	 * @param[in] parent Parent object to which the line will join
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
				Debug.Log("Bus/HOV not designed yet");
				break;
			default:
				Debug.Log("Trying to draw invalid type of lane");
				break;
			}
	}
	
	/**
	 * @brief Draw a curved lane line by type
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Control point of the line
	 * @param[in] position3 Position of the other end of the line
	 * @param[in] parent Parent object to which the line will join
	 */
	private static void draw_curved_lane_line (char lane_type, Vector3 position1, Vector3 position2, Vector3 position3, GameObject parent) {
		
		switch (lane_type) {
		case Constants.Char_Public_Lane:
			draw_continuous_curved_line (Constants.public_transport_line_width, Constants.line_thickness, position1, position2, position3, Constants.Line_Name_Public_Transport_Lane, parent);
			break;
		case Constants.Char_Normal_Lane:
			draw_discontinuous_curved_line (Constants.line_width, Constants.line_thickness, position1, position2, position3, Constants.Line_Name_Normal_Lane, parent);
			break;
		case 'A':
			Debug.Log("Parking not designed yet");
			break;
		case 'V':
			Debug.Log("Bus/HOV not designed yet");
			break;
		default:
			Debug.Log("Trying to draw invalid type of lane");
			break;
		}
	}

	/**
	 * @brief Draw a continuous white line aligned with the Z axis
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] length Length of the line
	 * @param[in] position Center position of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	private static void draw_continuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent) {
		
		Vector3 position1 = new Vector3 (position.x, position.y, position.z - (length/2));
		Vector3 position2 = new Vector3 (position.x, position.y, position.z + (length/2));
		
		draw_continuous_line (width, height, position1, position2, name, parent);
	}
	
	/**
	 * @brief Draw a continuous white line between the positions position1 and position2
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Position of the other end of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	private static void draw_continuous_line (float width, float height, Vector3 position1, Vector3 position2,string name, GameObject parent) {
		GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
		line.name = name;
		line.transform.localScale = new Vector3(width, height, MyMathClass.Distance(position1,position2));
		line.transform.position = MyMathClass.middlePoint(position1,position2);
		line.transform.rotation = Quaternion.LookRotation(MyMathClass.orientationVector(position1,position2));
		line.GetComponent<Renderer>().material.color = Color.white;
		line.GetComponent<Renderer>().material = white_asphalt_material;
		line.GetComponent<Renderer>().material.mainTextureScale = new Vector2(line.transform.localScale.x,line.transform.localScale.z);
		line.transform.parent = parent.transform;
	}
	
	/**
	 * @brief Draw a continuous curved white line between the positions position1 and position3, passing throught position2
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Control point of the line
	 * @param[in] position3 Position of the other end of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	private static void draw_continuous_curved_line (float width, float height, Vector3 position1, Vector3 position2, Vector3 position3, string name, GameObject parent) {
		GameObject continuous_curved_line = new GameObject();
		continuous_curved_line.name = name;
		continuous_curved_line.transform.parent = parent.transform;
		
		Vector3 start = position1;
		Vector3 end;
		
		for (int i=1; i<=10; i++) {
			end = MyMathClass.CalculateBezierPoint((float)i/10,position1,position2,position2,position3);
		
			GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
			line.name = name;
			line.transform.localScale = new Vector3(width, height, MyMathClass.Distance(start,end));
			line.transform.position = MyMathClass.middlePoint(start,end);
			line.transform.rotation = Quaternion.LookRotation(MyMathClass.orientationVector(start,end));
			line.GetComponent<Renderer>().material.color = Color.white;
			line.GetComponent<Renderer>().material = white_asphalt_material;
			line.GetComponent<Renderer>().material.mainTextureScale = new Vector2(line.transform.localScale.x,line.transform.localScale.z);
			line.transform.parent = continuous_curved_line.transform;
			
			start = end;
		}
	}

	/**
	 * @brief Draw a discontinuous white line aligned with the Z axis
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] length Length of the line
	 * @param[in] position Center position of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	private static void draw_discontinuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent) {

		Vector3 position1 = new Vector3 (position.x, position.y, position.z - (length/2));
		Vector3 position2 = new Vector3 (position.x, position.y, position.z + (length/2));
		
		draw_discontinuous_line (width, height, position1, position2, name, parent);
	}
	
	/**
	 * @brief Draw a discontinuous white line between the positions position1 and position2
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Position of the other end of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	private static void draw_discontinuous_line (float width, float height, Vector3 position1, Vector3 position2, string name, GameObject new_parent) {
	
		GameObject discontinuous_line = new GameObject ();
		discontinuous_line.name = name;
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
		
		pos_aux.z -= Constants.discontinuous_line_length * ((float)piece_num - 1);
		
		for (int i=0; i < piece_num; i++) {
			
			GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
			line.name = name;
			line.transform.localScale = new Vector3(width, height, Constants.discontinuous_line_length);
			line.transform.position = pos_aux;
			line.GetComponent<Renderer>().material.color = Color.white;
			line.GetComponent<Renderer>().material = white_asphalt_material;
			line.GetComponent<Renderer>().material.mainTextureScale = new Vector2(line.transform.localScale.x,line.transform.localScale.z);
			line.transform.parent = discontinuous_line.transform;
			
			pos_aux.z += Constants.discontinuous_line_length * 2;
		}
		discontinuous_line.transform.rotation = Quaternion.LookRotation(MyMathClass.orientationVector(position1,position2));
		discontinuous_line.AddComponent<BoxCollider>();
		discontinuous_line.GetComponent<BoxCollider>().size = new Vector3(width, height, length);
	}
	
	/**
	 * @brief Draw a discontinuous curved white line between the positions position1 and position3, passing throught position2
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Control point of the line
	 * @param[in] position3 Position of the other end of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	private static void draw_discontinuous_curved_line (float width, float height, Vector3 position1, Vector3 position2, Vector3 position3, string name, GameObject parent) {
		GameObject continuous_curved_line = new GameObject();
		continuous_curved_line.name = name;
		continuous_curved_line.transform.parent = parent.transform;
		
		float curve_length = MyMathClass.CalculateBezierLength(position1,position2,position2,position3);
		int num_segments_posible = (int)(curve_length / Constants.discontinuous_line_length);
		
		if (num_segments_posible % 2 == 0) {
			num_segments_posible--;
		}
		
		float margin_length = (curve_length - (num_segments_posible * Constants.discontinuous_line_length) ) / 2;
		
		float prev_dist = margin_length;
		float next_dist = prev_dist + Constants.discontinuous_line_length;
		
		while (next_dist < curve_length) {
			Vector3 prev = MyMathClass.CalculateBezierPointWithDistance(position1,position2,position2,position3,prev_dist);
			Vector3 next = MyMathClass.CalculateBezierPointWithDistance(position1,position2,position2,position3,next_dist);
			GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
			line.name = name;
			line.transform.localScale = new Vector3(width, height, MyMathClass.Distance(prev,next));
			line.transform.position = MyMathClass.middlePoint(prev,next);
			line.transform.rotation = Quaternion.LookRotation(MyMathClass.orientationVector(prev,next));
			line.GetComponent<Renderer>().material.color = Color.white;
			line.GetComponent<Renderer>().material = white_asphalt_material;
			line.GetComponent<Renderer>().material.mainTextureScale = new Vector2(line.transform.localScale.x,line.transform.localScale.z);
			line.transform.parent = continuous_curved_line.transform;
			
			prev_dist = next_dist + Constants.discontinuous_line_length;
			next_dist = prev_dist + Constants.discontinuous_line_length;
		}
	}

	/**
	 * @brief Calculate the total number of lanes of the edge whose identifier is passed as an argument
	 * @param[in] edge_id Edge ID
	 * @return The total number of lanes of the edge
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
	 * @brief Creates a mesh for the nodes of type continuation.
	 * @param[in] node_id ID of the node that is being created
	 * @param[in] node The GameObject that is being created
	 * @param[in] radius Radius of the circle circumscribed by the edges
	 * @param[in] width Width of the edges
	 * @param[in] angle Lower angle formed by edges [0,180]
	 * @param[in] side Left if the turn is to the left, Right if the turn is to the right
	 * @param[in] ref_edge_id Edge identifier of the referency edge used to draw road markings
	 */
	private static void CreateContinuationNode (string node_id, GameObject node, float radius, 
												float edge_width, float angle, TurnSide side, 
												string ref_edge_id) {
		
		GameObject topology = new GameObject();
		topology.name = Constants.Name_Topological_Objects;
		topology.transform.SetParent(node.transform);
		
		Vector2 road_center_point = new Vector2(0, -((radius * 0.5f) + 0.1f));
		Vector2 road_center_point_rotated;
		
		if (side == TurnSide.Right) {
			road_center_point_rotated = MyMathClass.rotatePoint(road_center_point, angle);
		}
		else {
			road_center_point_rotated = MyMathClass.rotatePoint(road_center_point, -angle);
		}
		
		Vector3 start_point = new Vector3(road_center_point.x,0,road_center_point.y);
		Vector3 control_point = new Vector3(0,0,0);
		Vector3 end_point = new Vector3(road_center_point_rotated.x,0,road_center_point_rotated.y);
		
		float rotation_angle = angle;
		
		if (side == TurnSide.Left) {
			rotation_angle = -angle;
		}
		
		BezierMesh (topology, Constants.road_thickness, edge_width, start_point, control_point, end_point, rotation_angle);
		
		// Attention: Following similar procedure of BezierMesh
		// Road markings
		
		float half_width = edge_width/2;
		float half_line_thickness = Constants.line_thickness/2;
		float half_line_width = Constants.line_width/2;
		float y_position_lines = -half_line_thickness + Constants.markings_Y_position;
		
		// Hard shoulders
		Vector2 LP = new Vector2 (start_point.x - (half_width - Constants.hard_shoulder_width - half_line_width), start_point.z);
		Vector2 RP = new Vector2 (start_point.x + (half_width - Constants.hard_shoulder_width - half_line_width), start_point.z);
		
		/*	Rotate angle degrees the points left and right.
			Due to the equal distance to the center of the imaginary lines start and end, rotate the left point give us
			the corresponding rotated point for the right point and the same applies to the right point. */
		
		Vector2 LPR = MyMathClass.rotatePoint(RP, rotation_angle);
		Vector2 RPR = MyMathClass.rotatePoint(LP, rotation_angle);
		
		Vector2 start_point_2D = new Vector2(start_point.x,start_point.z);
		Vector2 end_point_2D = new Vector2(end_point.x,end_point.z);
		Vector2 ref_edge_direction = MyMathClass.orientationVector(new Vector2(0,0), start_point_2D);
		Vector2 oth_edge_direction = MyMathClass.orientationVector(new Vector2(0,0), end_point_2D);
		
		Vector2 LCB_2D = MyMathClass.intersectionPoint(LP,ref_edge_direction,LPR,oth_edge_direction);
		Vector2 RCB_2D = MyMathClass.intersectionPoint(RP,ref_edge_direction,RPR,oth_edge_direction);
		
		Vector3 LP_3D  = new Vector3(LP.x,     y_position_lines, LP.y);
		Vector3 RP_3D  = new Vector3(RP.x,     y_position_lines, RP.y);
		Vector3 LPR_3D = new Vector3(LPR.x,    y_position_lines, LPR.y);
		Vector3 RPR_3D = new Vector3(RPR.x,    y_position_lines, RPR.y);
		Vector3 LCB_3D = new Vector3(LCB_2D.x, y_position_lines, LCB_2D.y);
		Vector3 RCB_3D = new Vector3(RCB_2D.x, y_position_lines, RCB_2D.y);
		
		draw_continuous_curved_line(Constants.line_width,Constants.line_thickness,LP_3D,LCB_3D,LPR_3D,Constants.Line_Name_Hard_Shoulder,topology);
		draw_continuous_curved_line(Constants.line_width,Constants.line_thickness,RP_3D,RCB_3D,RPR_3D,Constants.Line_Name_Hard_Shoulder,topology);
		
		// Center lines
		Vector2 aux_vector,aux_vector_rotated;
		Edge e = edges[ref_edge_id];
		Vector2 center_point = new Vector2(start_point.x,start_point.z);
		
		if (e.src_des != Constants.String_No_Lane && e.des_src != Constants.String_No_Lane) { // If both directions have lanes
			
			int lane_diff = 0; // Same number of lanes in each direction
			
			if (e.src_des.Length != e.des_src.Length) { // Different number of lanes in each direction
				lane_diff = e.src_des.Length - e.des_src.Length;
			}
			
			if (edges[ref_edge_id].source_id == node_id) {
				center_point.x =  (lane_diff * (Constants.lane_width/2));
			}
			else {
				center_point.x = -(lane_diff * (Constants.lane_width/2));
			}
			
			LP = new Vector2 (center_point.x - (Constants.center_lines_separation/2), center_point.y);
			RP = new Vector2 (center_point.x + (Constants.center_lines_separation/2), center_point.y);
			
			aux_vector = MyMathClass.orientationVector(road_center_point,LP);
			aux_vector_rotated = MyMathClass.rotatePoint(aux_vector, rotation_angle);
			LPR = road_center_point_rotated - aux_vector_rotated;
			
			aux_vector = MyMathClass.orientationVector(road_center_point,RP);
			aux_vector_rotated = MyMathClass.rotatePoint(aux_vector, rotation_angle);
			RPR = road_center_point_rotated - aux_vector_rotated;
			
			LCB_2D = MyMathClass.intersectionPoint(LP,ref_edge_direction,LPR,oth_edge_direction);
			RCB_2D = MyMathClass.intersectionPoint(RP,ref_edge_direction,RPR,oth_edge_direction);
			
			LP_3D  = new Vector3(LP.x,     y_position_lines, LP.y);
			RP_3D  = new Vector3(RP.x,     y_position_lines, RP.y);
			LPR_3D = new Vector3(LPR.x,    y_position_lines, LPR.y);
			RPR_3D = new Vector3(RPR.x,    y_position_lines, RPR.y);
			LCB_3D = new Vector3(LCB_2D.x, y_position_lines, LCB_2D.y);
			RCB_3D = new Vector3(RCB_2D.x, y_position_lines, RCB_2D.y);
			
			draw_continuous_curved_line(Constants.line_width,Constants.line_thickness,LP_3D,LCB_3D,LPR_3D,Constants.Line_Name_Center,topology);
			draw_continuous_curved_line(Constants.line_width,Constants.line_thickness,RP_3D,RCB_3D,RPR_3D,Constants.Line_Name_Center,topology);
		}
		
		// Lane lines
		// Paint as many lines as lanes are in each direction except one 
		// and put as many start lane as lanes have
		Vector2 P, PR, PCB;
		Vector3 P_3D, PR_3D, PCB_3D;
		
		if (e.src_des != Constants.String_No_Lane) {
			
			for (int i=0; i<e.src_des.Length-1; i++) {
				char lane_type = e.src_des[i];
				
				if (edges[ref_edge_id].source_id == node_id) {
					P = new Vector2(-((e.width / 2) - Constants.hard_shoulder_width) + ((Constants.lane_width + Constants.line_width) * (i+1)), start_point.z);
				}
				else {
					P = new Vector2(((e.width / 2) - Constants.hard_shoulder_width) - ((Constants.lane_width + Constants.line_width) * (i+1)), start_point.z);
				}
				aux_vector = MyMathClass.orientationVector(road_center_point,P);
				aux_vector_rotated = MyMathClass.rotatePoint(aux_vector, rotation_angle);
				PR = road_center_point_rotated - aux_vector_rotated;
				PCB = MyMathClass.intersectionPoint(P,ref_edge_direction,PR,oth_edge_direction);
				
				P_3D = new Vector3(P.x,y_position_lines,P.y);
				PR_3D = new Vector3(PR.x,y_position_lines,PR.y);
				PCB_3D = new Vector3(PCB.x,y_position_lines,PCB.y);
				
				draw_curved_lane_line(lane_type, P_3D, PCB_3D, PR_3D, topology);
			}
		}
		
		if (e.des_src != Constants.String_No_Lane) {
			
			for (int i=0; i<e.des_src.Length-1; i++) {
				char lane_type = e.des_src[i];
				
				if (edges[ref_edge_id].source_id == node_id) {
					P = new Vector2(((e.width / 2) - Constants.hard_shoulder_width) - ((Constants.lane_width + Constants.line_width) * (i+1)), start_point.z);
				}
				else {
					P = new Vector2(-((e.width / 2) - Constants.hard_shoulder_width) + ((Constants.lane_width + Constants.line_width) * (i+1)), start_point.z);
				}
				aux_vector = MyMathClass.orientationVector(road_center_point,P);
				aux_vector_rotated = MyMathClass.rotatePoint(aux_vector, rotation_angle);
				PR = road_center_point_rotated - aux_vector_rotated;
				PCB = MyMathClass.intersectionPoint(P,ref_edge_direction,PR,oth_edge_direction);
				
				P_3D = new Vector3(P.x,y_position_lines,P.y);
				PR_3D = new Vector3(PR.x,y_position_lines,PR.y);
				PCB_3D = new Vector3(PCB.x,y_position_lines,PCB.y);
				
				draw_curved_lane_line(lane_type, P_3D, PCB_3D, PR_3D, topology);
			}
		}
		// End road markings
	} // CreateContinuationNode
	
	/**
	 * @brief Create a thin Mesh based on a Bezier curve and rectangular sections
	 * @param[in] obj The gameobject
	 * @param[in] thick The thick of the mesh sections
	 * @param[in] width The width of the mesh sections
	 * @param[in] start_point The initial point for the Bezier curve
	 * @param[in] control_point Control point of the curve
	 * @param[in] end_point The last point for the Bezier curve
	 * @param[in] rotation_angle The angle in degrees [-180,180] of the edges involved in this turn
	 */
	private static void BezierMesh (GameObject obj, float thick, float width, Vector3 start_point, 
	                                Vector3 control_point, Vector3 end_point, float rotation_angle) {
		/*
			The object will be created following this steps:
				- Starting from a imaginary line parallel to the X axis, the mesh will turn left or right.
				  The Bezier curve who leads the turn starts at <start_point>, and ends at <end_point>.
				  <control_point> is the control point of the Bezier curve.
				- To do the turn we need 4 points for each section of the turn. Each section will be a deformed cube 
				  with its vertex in the 4 points with thickness of Constants.road_thickness.
				- To obtain these points we draw 2 imaginary Bezier curves wich will define the profile of the turn.
				  These curves starts in start_point.x - half_width <LP> and start_point.x + half_width <RP> and ends 
				  in their equivalents points at the end imaginary line of the turn (<LPR> and <RPR>).
				- The control point for these Bezier curves will be calculated as follow:
					* We take the start and end points of the turn and calculate two orientation vectors wich 
					corresponds to the edges (origin: (0,0), destination: central position of the edge).
					* Now, we calculate the intersection point of the straights who pass throught the points LP and
					LPR (for the left Bezier curve) and follow the direction of the previous calculated vectors. That 
					point will be the control point for that Bezier curve. The right Bezier curve it's calculated
					the same way.
		*/
		GameObject platform = new GameObject();
		platform.name = Constants.Name_Platform;
		platform.transform.SetParent(obj.transform);
		
		float top_y = Constants.platform_Y_position;
		float bottom_y = top_y - Constants.road_thickness;
		float half_width = width/2;
		
		Vector2 LP = new Vector2 (start_point.x - half_width, start_point.z); // Left point
		Vector2 RP = new Vector2 (start_point.x + half_width, start_point.z); // Right point
		
		/*	Rotate angle degrees the points left and right.
			Due to the equal distance to the center of the imaginary lines start and end, rotate the left point give us
			the corresponding rotated point for the right point and the same applies to the right point. */
		
		Vector2 LPR = MyMathClass.rotatePoint(RP, rotation_angle); // Left point rotated
		Vector2 RPR = MyMathClass.rotatePoint(LP, rotation_angle); // Right point rotated
		
		// Calculate control points for the Bezier curves
		Vector2 start_point_2D = new Vector2(start_point.x,start_point.z);
		Vector2 end_point_2D = new Vector2(end_point.x,end_point.z);
		Vector2 ref_edge_direction = MyMathClass.orientationVector(new Vector2(0,0), start_point_2D);
		Vector2 oth_edge_direction = MyMathClass.orientationVector(new Vector2(0,0), end_point_2D);
		
		Vector2 LCB_2D = MyMathClass.intersectionPoint(LP,ref_edge_direction,LPR,oth_edge_direction);
		Vector2 RCB_2D = MyMathClass.intersectionPoint(RP,ref_edge_direction,RPR,oth_edge_direction);
		
		// Create the turn sections
		for (int i=0; i<10; i++) {
			Vector2 point0 = MyMathClass.CalculateBezierPoint((float)i/10    ,LP,LCB_2D,LCB_2D,LPR);
			Vector2 point1 = MyMathClass.CalculateBezierPoint((float)i/10    ,RP,RCB_2D,RCB_2D,RPR);
			Vector2 point2 = MyMathClass.CalculateBezierPoint((float)(i+1)/10,RP,RCB_2D,RCB_2D,RPR);
			Vector2 point3 = MyMathClass.CalculateBezierPoint((float)(i+1)/10,LP,LCB_2D,LCB_2D,LPR);
			
			Vector3[] vertex_array = new Vector3[8];
			vertex_array[0] = new Vector3(point3.x, bottom_y, point3.y);
			vertex_array[1] = new Vector3(point2.x, bottom_y, point2.y);
			vertex_array[2] = new Vector3(point1.x, bottom_y, point1.y);
			vertex_array[3] = new Vector3(point0.x, bottom_y, point0.y);
			vertex_array[4] = new Vector3(point3.x, top_y   , point3.y);
			vertex_array[5] = new Vector3(point2.x, top_y   , point2.y);
			vertex_array[6] = new Vector3(point1.x, top_y   , point1.y);
			vertex_array[7] = new Vector3(point0.x, top_y   , point0.y);
			eightMesh(platform,vertex_array);
		}
	} // End BezierMesh
	
	/**
	 * @brief Create a mesh with 8 vertex which seems a deformed box. The algorithm has been obtained from
	 * http://wiki.unity3d.com/index.php/ProceduralPrimitives and has been adapted to the needs of this application
	 * @param[in] obj The gameobject
	 * @param[in] vertex_array The array with 8 Vector3 with the positions of all vertex. The vertex of the bottom face
	 * are p0,p1,p2,p3 and the vertex of the top face are p7,p6,p5,p4
	 */
	private static void eightMesh (GameObject obj, Vector3[] vertex_array) {
		GameObject go = new GameObject();
		go.name = Constants.Name_Turn_Section;
		go.transform.SetParent(obj.transform);
		go.AddComponent< BoxCollider >();
		go.AddComponent< MeshRenderer >();
		go.GetComponent<Renderer>().material = asphalt_material;
		MeshFilter filter = go.AddComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		mesh.Clear();
		
		#region Vertices
		Vector3 p0 = new Vector3(vertex_array[0].x, vertex_array[0].y, vertex_array[0].z);
		Vector3 p1 = new Vector3(vertex_array[1].x, vertex_array[1].y, vertex_array[1].z);
		Vector3 p2 = new Vector3(vertex_array[2].x, vertex_array[2].y, vertex_array[2].z);
		Vector3 p3 = new Vector3(vertex_array[3].x, vertex_array[3].y, vertex_array[3].z);	
		Vector3 p4 = new Vector3(vertex_array[4].x, vertex_array[4].y, vertex_array[4].z);
		Vector3 p5 = new Vector3(vertex_array[5].x, vertex_array[5].y, vertex_array[5].z);
		Vector3 p6 = new Vector3(vertex_array[6].x, vertex_array[6].y, vertex_array[6].z);
		Vector3 p7 = new Vector3(vertex_array[7].x, vertex_array[7].y, vertex_array[7].z);
		
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
	} // End eightMesh
}
