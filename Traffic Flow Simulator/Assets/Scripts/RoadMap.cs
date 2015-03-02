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
			return NodeType.Unknown;
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
		
		tt = TransportType.Unknown;
		
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
	public static void draw () {
		
		prepareEdges ();
		
		// Draw the ground
		drawGround ();
		
		// Draw the edges
		foreach (KeyValuePair<string, Edge> edge in edges){
			drawEdge (edge.Key);
		}
		
		// Draw the nodes
		foreach (KeyValuePair<string, Node> node in nodes){
			drawNode (node.Key);
		}
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
			aux_road.renderer.material = black_material;
			pos.y += (Constants.limit_height/2);
			aux_road.transform.localScale = new Vector3(width,Constants.limit_height,Constants.limit_depth);
			// Limit node vector
			Vector2 dir = new Vector2 (0,1);
			aux_road.transform.rotation = Quaternion.AngleAxis(MyMathClass.RotationAngle(dir,e.direction),Vector3.up);
			aux_road.transform.position = pos;
			// Place the node in the roads layer
			MyUtilitiesClass.MoveToLayer(aux_road.transform,LayerMask.NameToLayer(Constants.Layer_Roads));
		}
		else if (n.node_type == NodeType.Continuation) {  // DRAW CONTINUATION NODE
			GameObject aux_road = new GameObject();
			aux_road.name = node_id;
			aux_road.tag = Constants.Tag_Node_Continuation;
			float edge_width = nodeWidth(n.id);
			
			// Get the identifiers of the edges involved with the continuatino node
			string edgeID1, edgeID2;
			RoadMap.getContinuationEdges (n.id, out edgeID1, out edgeID2);
			// Choose the edge whose x coordinate is less
			string selected_edge = edgeID1;
			
			if (edges[edgeID2].fixed_position.x < edges[edgeID1].fixed_position.x) {
				selected_edge = edgeID2;
			}
			// Create the continuation node
			CreateContinuationNode(node_id, aux_road, edge_width, edge_width, nodeAngle(n.id), selected_edge);
			
			Vector2 edge_direction = edges[selected_edge].direction;
			float rotation_degrees = MyMathClass.RotationAngle(new Vector2(0,-1),edge_direction);
			aux_road.transform.rotation = Quaternion.AngleAxis (rotation_degrees, new Vector3(0,1,0));
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
	 * @param[in] node_id Continuation node ID
	 * @return The angle calculated in degrees [0,360)
	 */
	private static float nodeAngle (string node_id) {
		Vector2 edge_1 = new Vector2();
		Vector2 edge_2 = new Vector2();
		bool first_found = false;
		bool second_found = false;

		// Find the two edges that reach the continuation node and save their positions

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

		// Calculate two vectors originating from the position of the node and vertex in the position of the edges
		Vector2 vector_1 = new Vector2 (edge_1.x - nodes[node_id].x, edge_1.y - nodes[node_id].y);
		Vector2 vector_2 = new Vector2 (edge_2.x - nodes[node_id].x, edge_2.y - nodes[node_id].y);

		// Calculate the smallest angle between the two vectors
		float angle_deg = MyMathClass.RotationAngle (vector_1, vector_2); //(-360,360)

		if (angle_deg < 0) {
			angle_deg += 360f;
		}
		return angle_deg;
	}

	/**
	 * @brief Draw the edge with id "edge_id" in the 3D environment
	 * @param[in] edge_id Edge ID to draw
	 * @pre Before running this method should be run once prepareEdges method
	 */
	private static void drawEdge (string edge_id) {
		Edge e = edges[edge_id];
		
		// Platform
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.name = edge_id;
		platform.tag = Constants.Tag_Edge;
		platform.transform.localScale = new Vector3(e.width, Constants.road_thickness, e.length);
		platform.renderer.material.color = Color.gray;
		platform.renderer.material = asphalt_material;
		platform.renderer.material.mainTextureScale = new Vector2(platform.transform.localScale.x, platform.transform.localScale.z);
		
		Vector3 position;

		// Road markings

		// Hard shoulder lines
		position = new Vector3();
		position.x = platform.transform.position.x - ((e.width / 2) - Constants.hard_shoulder_width);
		position.y = platform.transform.position.y + (Constants.road_thickness/2)+(Constants.line_thickness/2);
		position.z = 0;
		draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, position, Constants.Line_Name_Hard_Shoulder, platform);

		position.x = platform.transform.position.x + ((e.width / 2) - Constants.hard_shoulder_width);
		draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, position, Constants.Line_Name_Hard_Shoulder, platform);

		// Center lines
		if (e.src_des != Constants.String_No_Lane && e.des_src != Constants.String_No_Lane) { // If both directions have lanes

			int lane_diff = 0; // Same number of lanes in each direction
			
			if (e.src_des.Length != e.des_src.Length) { // Different number of lanes in each direction
				lane_diff = e.src_des.Length - e.des_src.Length;
			}
			
			position.x = platform.transform.position.x - (Constants.center_lines_separation/2) - (lane_diff * (Constants.lane_width/2));
			draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, position, Constants.Line_Name_Center, platform);
			position.x = platform.transform.position.x + (Constants.center_lines_separation/2) - (lane_diff * (Constants.lane_width/2));
			draw_continuous_line (Constants.line_width, Constants.line_thickness, e.length, position, Constants.Line_Name_Center, platform);
		}

		// Lane lines

		Vector3 save_position = new Vector3 (position.x, position.y, position.z);

		// Paint as many lines as lanes are in each direction except one 
		// and put as many start lane as lanes have
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

			// Stop lines before intersection
			if (nodes[e.destination_id].node_type != NodeType.Continuation && nodes[e.destination_id].node_type != NodeType.Limit) {
				position.x = platform.transform.position.x + ((e.width / 2) - Constants.hard_shoulder_width) - (e.src_des.Length * (Constants.lane_width + Constants.line_width))/2;
				position.z = platform.transform.position.z + (e.length/2 - Constants.public_transport_line_width/2);
				draw_continuous_line (e.src_des.Length * (Constants.lane_width + Constants.line_width),Constants.line_thickness,Constants.public_transport_line_width,position,Constants.Line_Name_Detention,platform); // Exchanged width by length to make perpendicular line
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

			// Stop lines before intersection
			if (nodes[e.source_id].node_type != NodeType.Continuation && nodes[e.source_id].node_type != NodeType.Limit) {
				position.x = platform.transform.position.x - ((e.width / 2) - Constants.hard_shoulder_width) + (e.des_src.Length * (Constants.lane_width + Constants.line_width))/2;
				position.z = platform.transform.position.z - (e.length/2 - Constants.public_transport_line_width/2);
				draw_continuous_line (e.des_src.Length * (Constants.lane_width + Constants.line_width),Constants.line_thickness,Constants.public_transport_line_width,position,Constants.Line_Name_Detention,platform); // Intercambiado ancho por largo para hacer linea perpendicular
			}
		}

		// End road markings

		// Vector of the newly drawn edge
		Vector2 dir_pref = new Vector2 (0,1);

		platform.transform.rotation = Quaternion.AngleAxis(MyMathClass.RotationAngle(dir_pref,e.direction),Vector3.up);
		platform.transform.position = e.fixed_position;
		// Place the edge in the roads layer
		MyUtilitiesClass.MoveToLayer(platform.transform,LayerMask.NameToLayer(Constants.Layer_Roads));
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
	 * @brief Draw a lane line by type
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] length Line lenght
	 * @param[in] position Center line position
	 * @param[in] parent Parent object to which the line will join
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
				Debug.Log("Bus/HOV not designed yet");
				break;
			default:
				Debug.Log("Trying to draw invalid type of lane");
				break;
		}
	}
	
	/**
	 * @brief Draw a lane line by type
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
	 * @brief Draw a continuous white line aligned with the Z axis
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] length Lenght of the line
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
		line.renderer.material.color = Color.white;
		line.renderer.material = white_asphalt_material;
		line.renderer.material.mainTextureScale = new Vector2(line.transform.localScale.x,line.transform.localScale.z);
		line.transform.parent = parent.transform;
	}

	/**
	 * @brief Draw a discontinuous white line aligned with the Z axis
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] length Lenght of the line
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
		
		pos_aux.z -= Constants.discontinuous_line_length * ((float)piece_num - 1);
		
		for (int i=0; i < piece_num; i++) {
			
			GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
			line.name = name;
			line.transform.localScale = new Vector3(width, height, Constants.discontinuous_line_length);
			line.transform.position = pos_aux;
			line.renderer.material.color = Color.white;
			line.renderer.material = white_asphalt_material;
			line.renderer.material.mainTextureScale = new Vector2(line.transform.localScale.x,line.transform.localScale.z);
			line.transform.parent = discontinuous_line.transform;
			
			pos_aux.z += Constants.discontinuous_line_length * 2;
		}
		discontinuous_line.transform.rotation = Quaternion.LookRotation(MyMathClass.orientationVector(position1,position2));
		discontinuous_line.AddComponent<BoxCollider>();
		discontinuous_line.GetComponent<BoxCollider>().size = new Vector3(width, height, length);
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
	 * @param[in] angle Lower angle formed by edges [0,360)
	 * @param[in] ref_edge_id Edge identifier of the referency edge used to draw road markings
	 */
	private static void CreateContinuationNode (string node_id, GameObject node, float radius, float edge_width, float angle, string ref_edge_id) {
		
		node.AddComponent< BoxCollider >();
		node.AddComponent< MeshRenderer >();
		node.renderer.material = asphalt_material;
		
		float half_road_thickness = Constants.road_thickness * 0.5f;
		float half_negative_radius = -radius * 0.5f;
		float half_edge_width = edge_width * 0.5f;
		
		Vector2 left_point  = new Vector2 (-half_edge_width, half_negative_radius);
		Vector2 right_point = new Vector2 ( half_edge_width, half_negative_radius);

		// Rotate angle degrees the points left and right
		Vector2 left_point_rotated  = MyMathClass.rotatePoint(left_point , angle);
		Vector2 right_point_rotated = MyMathClass.rotatePoint(right_point, angle);
		 
		Vector3 p0 = new Vector3(  right_point_rotated.x,	-half_road_thickness,	right_point_rotated.y );
		Vector3 p1 = new Vector3(  left_point_rotated.x, 	-half_road_thickness,	left_point_rotated.y  );
		Vector3 p2 = new Vector3(  half_edge_width, 		-half_road_thickness,	half_negative_radius  );
		Vector3 p3 = new Vector3( -half_edge_width,			-half_road_thickness,	half_negative_radius  );
		Vector3 p4 = new Vector3(  right_point_rotated.x,	 half_road_thickness,	right_point_rotated.y );
		Vector3 p5 = new Vector3(  left_point_rotated.x, 	 half_road_thickness,	left_point_rotated.y  );
		Vector3 p6 = new Vector3(  half_edge_width, 		 half_road_thickness,	half_negative_radius  );
		Vector3 p7 = new Vector3( -half_edge_width,	 		 half_road_thickness,	half_negative_radius  );
		
		Vector3[] vertex_array = new Vector3[8];
		vertex_array[0] = p0;
		vertex_array[1] = p1;
		vertex_array[2] = p2;
		vertex_array[3] = p3;
		vertex_array[4] = p4;
		vertex_array[5] = p5;
		vertex_array[6] = p6;
		vertex_array[7] = p7;
		
		eightMesh(node,vertex_array);
		
		Edge e = edges[ref_edge_id];
		
		// Common calculations
		float pos_y_lines = (Constants.road_thickness/2)+(Constants.line_thickness/2);
		float pos_z_lines = -((radius * 0.5f) + 0.1f);
		Vector2 road_center_point = new Vector2(0, pos_z_lines);
		Vector2 road_center_point_rotated = MyMathClass.rotatePoint(road_center_point, angle);
		float right_hard_shoulder_pos_x = (e.width / 2) - Constants.hard_shoulder_width;
		float left_hard_shoulder_pos_x = -((e.width / 2) - Constants.hard_shoulder_width);
		float lane_w_plus_line_w = Constants.lane_width + Constants.line_width;
		
		// Check if the node is source or destionation of the referency edge
		bool is_source = true;
		
		if (node_id == e.destination_id) {
			is_source = false;
		}
		
		// Hard shoulder lines
		
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
		
		// Center lines
		
		if (nodes[node_id].two_ways) {
		
			int lane_diff = 0; // Same number of lanes in each direction
			
			if (edges[ref_edge_id].src_des.Length != edges[ref_edge_id].des_src.Length) { // Different number of lanes in each direction
				lane_diff = edges[ref_edge_id].src_des.Length - edges[ref_edge_id].des_src.Length;
			}
			
			// Calculate the center point of the center lines and its corresponding rotated
			Vector2 center_point;
			
			if (is_source) {
				center_point = new Vector2 (+ (lane_diff * (Constants.lane_width/2)), pos_z_lines);
			}
			else {
				center_point = new Vector2 (- (lane_diff * (Constants.lane_width/2)), pos_z_lines);
			}
			Vector2 center_point_rotated = MyMathClass.rotatePoint(center_point, angle);
			
			// Subtract twice the vector with origin in the point road_center_point_rotated and end in the point center_point_rotated
			// To bring the point symmetrically across the center point of the road
			Vector2 cp_cpr = new Vector2(center_point_rotated.x - road_center_point_rotated.x, center_point_rotated.y - road_center_point_rotated.y);
			center_point_rotated -= 2*cp_cpr;
			
			// Left line
			
			// Calculate the starting point of the line
			Vector2 line_begin_point = new Vector2(center_point.x - (Constants.center_lines_separation/2), center_point.y);
				
			// Calculate the same point rotated
			Vector2 line_begin_point_rotated = MyMathClass.rotatePoint(line_begin_point, angle);
			
			// Subtract twice the vector with origin in the point road_center_point_rotated and end in the point line_begin_point_rotated
			// To bring the point symmetrically across the center point of the road
			Vector2 rcpr_lbpr = new Vector2(line_begin_point_rotated.x - road_center_point_rotated.x, line_begin_point_rotated.y - road_center_point_rotated.y);
			line_begin_point_rotated -= 2*rcpr_lbpr;
			
			// Prepare the positions and draw the lines
			Vector3 pos1 = new Vector3(line_begin_point.x,			pos_y_lines, line_begin_point.y);
			Vector3 pos2 = new Vector3(line_begin_point_rotated.x,	pos_y_lines, line_begin_point_rotated.y);
			
			draw_continuous_line(Constants.line_width,Constants.line_thickness,pos1,pos2,Constants.Line_Name_Center,node);
			
			// Repeat for the right line setting "line_begin_point"
			
			line_begin_point = new Vector2(center_point.x + (Constants.center_lines_separation/2), center_point.y);
			line_begin_point_rotated = MyMathClass.rotatePoint(line_begin_point, angle);
			rcpr_lbpr = new Vector2(line_begin_point_rotated.x - road_center_point_rotated.x, line_begin_point_rotated.y - road_center_point_rotated.y);
			line_begin_point_rotated -= 2*rcpr_lbpr;
			pos1 = new Vector3(line_begin_point.x,			pos_y_lines, line_begin_point.y);
			pos2 = new Vector3(line_begin_point_rotated.x,	pos_y_lines, line_begin_point_rotated.y);
			draw_continuous_line(Constants.line_width,Constants.line_thickness,pos1,pos2,Constants.Line_Name_Center,node);
		}
		
		// Lane lines
		
		// If the direction source -> destination has lanes
		if (e.src_des != Constants.String_No_Lane) {
			
			// Paint a line for each of the lanes except the most central
			for (int i=0; i < e.src_des.Length-1; i++) {
				
				// Calculate the starting point of the line
				Vector2 line_begin_point;
				
				if (is_source) {
					line_begin_point = new Vector2(left_hard_shoulder_pos_x + (lane_w_plus_line_w * (i+1)), pos_z_lines);
				}
				else {
					line_begin_point = new Vector2(right_hard_shoulder_pos_x - (lane_w_plus_line_w * (i+1)), pos_z_lines);
				}
				
				// Calculate the same point rotated
				Vector2 line_begin_point_rotated = MyMathClass.rotatePoint(line_begin_point, angle);
				
				// Subtract twice the vector with origin in the point road_center_point_rotated and end in the point line_begin_point_rotated
				// To bring the point symmetrically across the center point of the road
				Vector2 rcpr_lbpr = new Vector2(line_begin_point_rotated.x - road_center_point_rotated.x, line_begin_point_rotated.y - road_center_point_rotated.y);
				line_begin_point_rotated -= 2*rcpr_lbpr;
				
				// Prepare the positions and draw the lines
				Vector3 pos1 = new Vector3(line_begin_point.x,			pos_y_lines, line_begin_point.y);
				Vector3 pos2 = new Vector3(line_begin_point_rotated.x,	pos_y_lines, line_begin_point_rotated.y);
				
				char lane_type = e.src_des[i];
				draw_lane_line (lane_type, pos1, pos2, node);
			}
		}
		
		// If the direction direccion destination -> source has lanes
		if (e.des_src != Constants.String_No_Lane) {
		
			// Paint a line for each of the lanes except the most central
			for (int i=0; i < e.des_src.Length-1; i++) {
				
				// Calculate the starting point of the line
				Vector2 line_begin_point;
				
				if (is_source) {
					line_begin_point = new Vector2(right_hard_shoulder_pos_x - (lane_w_plus_line_w * (i+1)), pos_z_lines);
				}
				else {
					line_begin_point = new Vector2(left_hard_shoulder_pos_x + (lane_w_plus_line_w * (i+1)), pos_z_lines);
				}
				
				// Calculate the same point rotated
				Vector2 line_begin_point_rotated = MyMathClass.rotatePoint(line_begin_point, angle);
				
				// Subtract twice the vector with origin in the point road_center_point_rotated and end in the point line_begin_point_rotated
				// To bring the point symmetrically across the center point of the road
				Vector2 rcpr_lbpr = new Vector2(line_begin_point_rotated.x - road_center_point_rotated.x, line_begin_point_rotated.y - road_center_point_rotated.y);
				line_begin_point_rotated -= 2*rcpr_lbpr;
				
				// Prepare the positions and draw the lines
				Vector3 pos1 = new Vector3(line_begin_point.x,			pos_y_lines, line_begin_point.y);
				Vector3 pos2 = new Vector3(line_begin_point_rotated.x,	pos_y_lines, line_begin_point_rotated.y);
				
				char lane_type = e.des_src[i];
				draw_lane_line (lane_type, pos1, pos2, node);
			}
		}
		// End road markings
	} // CreateContinuationNode
	
	/**
	 * @brief Create a thin Mesh based on a Bezier curve and rectangular sections
	 * @param[in] obj The gameobject
	 * @param[in] thick The thick of the mesh sections
	 * @param[in] width The width of the mesh sections
	 * @param[in] p0 The initial point for the Bezier curve
	 * @param[in] p1 One control point of the curve
	 * @param[in] p2 Other control point of the curve
	 * @param[in] p3 The last point for the Bezier curve
	 */
	private static void BezierMesh (GameObject obj, float thick, float width, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		
	} // End BezierMesh
	
	/**
	 * @brief Create a mesh with 8 vertex which seems a deformed box. The algorithm has been obtained from
	 * http://wiki.unity3d.com/index.php/ProceduralPrimitives and has been adapted to the needs of this application
	 * @param[in] obj The gameobject
	 * @param[in] vertex_array The array with 8 Vector3 with the positions of all vertex
	 */
	private static void eightMesh (GameObject obj, Vector3[] vertex_array) {
		MeshFilter filter = obj.AddComponent< MeshFilter >();
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
	
	/**
	 * @brief Draw the grass floor
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
		
		max_x += Constants.grass_ground_padding;
		max_y += Constants.grass_ground_padding;
		min_x -= Constants.grass_ground_padding;
		min_y -= Constants.grass_ground_padding;
		
		Material grass_material = Resources.Load ("Materials/Grass", typeof(Material)) as Material;
		
		GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
		ground.name = Constants.Name_Ground;
		ground.tag = Constants.Tag_Ground;
		ground.transform.localScale = new Vector3((max_x-min_x)/10, 1, (max_y-min_y)/10); // It is divided by 10 because measurements of the plane are 10x10 in Unity
		ground.renderer.material = grass_material;
		ground.renderer.material.mainTextureScale = new Vector2(ground.transform.localScale.x, ground.transform.localScale.z);
		
		Vector3 ground_position = new Vector3((max_x+min_x)/2,0,(max_y+min_y)/2);
		ground.transform.position = ground_position;
	} // End drawGround
}
