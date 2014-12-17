﻿using UnityEngine;
using System.Collections;

public enum NodeType : byte {Intersection, Limit, Continuation, Unknown};
public enum IntersectionType : byte {Normal, Roundabout, Unknown};
public enum TransportType: byte {Public, Private, PublicAndPrivate, Unknown};

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

public static class Constants : object {

	// Names
	public const string Line_Name_Hard_Shoulder = "Hard shoulder line";
	public const string Line_Name_Public_Transport_Lane = "Public transport lane line";
	public const string Line_Name_Normal_Lane = "Normal lane line";
	public const string Line_Name_Center = "Center line";
	public const string Line_Name_Detention = "Detention line";
	public const string Line_Name_Continuous = "Continuous line";
	public const string Line_Name_Discontinuous = "Discontinuous line";
	
	public const string Lane_Name_Public = "Public Lane";
	public const string Lane_Name_Normal = "Normal Lane";
	
	public const string Name_Ground = "Ground";
	
	// Tags
	public const string Tag_Node_Limit = "Limit_node";
	public const string Tag_Node_Continuation = "Continuation_node";
	public const string Tag_Node_Intersection = "Intersection_node";
	public const string Tag_Edge = "Edge";
	public const string Tag_Unknown = "Unknown";
	public const string Tag_Lane_Start_Point = "LaneStartPoint";
	public const string Tag_Vehicle = "Vehicle";
	
	// Data strings
	public const string String_No_Lane = "0";
	public const string String_Normal_Lane = "N";
	public const string String_Public_Lane = "P";
	
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
	public const float discontinuous_line_length = 2f;
}