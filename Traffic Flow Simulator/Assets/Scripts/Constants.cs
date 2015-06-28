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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NodeType		: byte {Intersection, Limit, Continuation};
public enum IntersectionType: byte {Normal, Roundabout};
public enum TransportType	: byte {Public, Private, PublicAndPrivate};
public enum DirectionType	: byte {Source_Destination, Destination_Source};
public enum TurnSide		: byte {Left, Right};
public enum GuideNodeType 	: byte {Lane_start, Lane_end, OnLane};

/*
 * @brief Saves the information related to a node.
 */
public struct Node
{
	public string 			id;					// Identifier of the node.
	public NodeType 		node_type;			// Type of the node (Intersection, Limit, Continuation).
	public float 			x;					// Position of the node on the X axis.
	public float 			y;					// Position of the node on the Z axis.
	public IntersectionType intersection_type;	// Only for Intersection Nodes: Type of intersection (Normal, Roundabout).
	public string 			widest_edge_id;		// Identifier of the widest edge which touches the node.
	public string			reference_edge_id;	// Reference edge identifier, for continuation node type only.
	public string			other_edge_id;		// The other edge identifier, for continuation node type only.
}

/*
 * @brief Saves the information related to an edge.
 */
public struct Edge
{
	public string 	id;							// Identifier of the edge.
	public string 	source_id;					// Identifier of the source node.
	public string 	destination_id;				// Identifier of the destination node.
	public string 	name;						// Name of the street represented by the edge.
	public string 	src_des;					// Lane codes of the edge on source-destination direction.
	public string 	des_src;					// Lane codes of the edge on destination-source direction.
	public float 	length;						// Lenght of the edge.
	public float 	width;						// Total width of the edge.
	public int 		lane_num;					// Total number of lanes on the edge.
	public Vector2 	direction;					// Vector paralel to source-destination direction.
	public Vector2 	fixed_position_vector;		// Position adjustment vector.
	public Vector3 	fixed_position;				// Position already set.
}

/*
 * @brief Saves the information related to the allowed directions for a lane on an intersection.
 */
public struct AllowedDirections
{
	public string lane_id;
	public List<string> direction_ids;
}

public static class Constants : object {

	#region Paths
	public const string maps_path = "/StreamingAssets/Maps/";
	#endregion
	
	#region Names
	public const string Line_Name_Hard_Shoulder = "Hard shoulder line";
	public const string Line_Name_Public_Transport_Lane = "Public transport lane line";
	public const string Line_Name_Normal_Lane = "Normal lane line";
	public const string Line_Name_Center = "Center line";
	public const string Line_Name_Detention = "Detention line";
	
	public const string Lane_Name_Public = "Public Lane";
	public const string Lane_Name_Normal = "Normal Lane";
	
	public const string Name_Ground = "Ground";
	public const string Name_Platform = "Platform";
	
	public const string Name_Source_Start_Points 		= "Source Start Points";
	public const string Name_Destination_Start_Points 	= "Destination Start Points";
	public const string Name_Source_End_Points 			= "Source End Points";
	public const string Name_Destination_End_Points 	= "Destination End Points";
	public const string Name_OnLane_Points				= "On Lane Points";
	
	public const string Name_Turn_Section  = "Turn Section";
	public const string Name_Turn_Platform = "Turn Platform";
	
	public const string Name_Topological_Objects = "Topological objects";
	#endregion
	
	#region Tags
	public const string Tag_Node_Limit = "Limit_node";
	public const string Tag_Node_Continuation = "Continuation_node";
	public const string Tag_Node_Intersection = "Intersection_node";
	public const string Tag_Edge = "Edge";
	public const string Tag_Unknown = "Unknown";
	public const string Tag_Lane_Start_Point 		= "LaneStartPoint";
	public const string Tag_Lane_Start_Point_Group 	= "LaneStartPointGroup";
	public const string Tag_Lane_End_Point 			= "LaneEndPoint";
	public const string Tag_Lane_End_Point_Group 	= "LaneEndPointGroup";
	public const string Tag_Vehicle = "Vehicle";
	public const string Tag_Ground = "Ground";
	#endregion
	
	#region Layers
	public const string Layer_Roads = "Roads";
	public const string Layer_Vehicles = "Vehicles";
	#endregion
	
	#region Data strings
	public const string String_No_Lane = "0";
	public const string String_Normal_Lane = "N";
	public const string String_Public_Lane = "P";
	public const string String_Unknown = "Unknown";
	
	public const char Char_Normal_Lane = 'N';
	public const char Char_Public_Lane = 'P';
	
	public const string String_graphml_ext = "graphml";
	public const string String_topology_ext = "topology";
	
	public const string xml_graphml_key_node_type = "node_type";
	public const string xml_graphml_key_pos_x = "pos_x";
	public const string xml_graphml_key_pos_y = "pos_y";
	public const string xml_graphml_key_intersection_type = "crossing_type";
	public const string xml_graphml_key_road_name = "road_name";
	public const string xml_graphml_key_src_des = "src_des";
	public const string xml_graphml_key_des_src = "des_src";
	#endregion
	
	#region Road measures
	public const float lane_width = 3.5f;
	public const float line_width = 0.1f;
	public const float public_transport_line_width = 0.3f;
	public const float hard_shoulder_width = 1f;
	public const float limit_height = 10f;
	public const float limit_depth = 3f;
	public const float road_thickness = 0.1f;
	public const float line_thickness = 0.01f;
	public const float center_lines_separation = 0.2f;
	public const float discontinuous_line_length = 1.3f;
	public const float discontinuous_line_min_margin = 0.3f;
	public const float cont_nodes_lines_extra_length = 0.05f;
	public const float grass_ground_padding = 100f;
	
	public const float ground_Y_position 	= -0.1f;
	public const float platform_Y_position 	= -0.004f;
	public const float markings_Y_position 	= -0.001f;
	public const float vehicles_Y_position 	= 0f;
	
	public const float nameSign_Y_position 	= 4f;
	#endregion
	
	#region Speed measures
	public const float urban_speed_limit = 13.8f; // Meters per second (50 Km/h)
	//public const float urban_speed_limit = 1.38f; // Meters per second (10 Km/h)
	#endregion
	
	#region Precision measures
	public const float bezier_precision = 400f;
	public const float Guide_Node_padding = 0.5f;
	public const float infinite = 999999999;
	#endregion
}
