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

public enum NodeType : byte {Intersection, Limit, Continuation, Unknown};
public enum IntersectionType : byte {Normal, Roundabout, Unknown};
public enum TransportType: byte {Public, Private, PublicAndPrivate, Unknown};
public enum DirectionType: byte {Source_Destination, Destination_Source};
public enum TurnSide : byte {Left, Right};

public struct Node
{
	public string id;
	public NodeType node_type;
	public float x;
	public float y;
	public IntersectionType intersection_type;
	public string widest_edge_id;
	public bool two_ways; // Indicates whether two directions (true) or one (false).
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
	public Vector2 fixed_position_vector; // Position adjustment vector
	public Vector3 fixed_position; // Position already set
}

public static class Constants : object {

	// Paths
	public const string maps_path = "/StreamingAssets/Maps/";
	
	// Names
	public const string Line_Name_Hard_Shoulder = "Hard shoulder line";
	public const string Line_Name_Public_Transport_Lane = "Public transport lane line";
	public const string Line_Name_Normal_Lane = "Normal lane line";
	public const string Line_Name_Center = "Center line";
	public const string Line_Name_Detention = "Detention line";
	public const string Line_Name_Continuous = "Continuous line";
	public const string Line_Name_Continuous_Curved = "Continuous curved line";
	public const string Line_Name_Discontinuous_Curved = "Discontinuous curved line";
	public const string Line_Name_Discontinuous = "Discontinuous line";
	
	public const string Lane_Name_Public = "Public Lane";
	public const string Lane_Name_Normal = "Normal Lane";
	
	public const string Name_Ground = "Ground";
	
	public const string Name_Source_Start_Points = "Source Start Points";
	public const string Name_Destination_Start_Points = "Destination Start Points";
	
	public const string Name_Turn_Section = "Turn Section";
	
	// Tags
	public const string Tag_Node_Limit = "Limit_node";
	public const string Tag_Node_Continuation = "Continuation_node";
	public const string Tag_Node_Intersection = "Intersection_node";
	public const string Tag_Edge = "Edge";
	public const string Tag_Unknown = "Unknown";
	public const string Tag_Lane_Start_Point = "LaneStartPoint";
	public const string Tag_Lane_Start_Point_Group = "LaneStartPointGroup";
	public const string Tag_Vehicle = "Vehicle";
	public const string Tag_Ground = "Ground";
	
	// Layers
	public const string Layer_Roads = "Roads";
	public const string Layer_Vehicles = "Vehicles";
	
	// Data strings
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
	
	// Measures
	public const float lane_width = 3f;
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
	
	public const float urban_speed_limit = 13.8f; // Meters per second (50 Km/h)
	
	// Other measures
	public const float bezier_precision = 400f;
}
