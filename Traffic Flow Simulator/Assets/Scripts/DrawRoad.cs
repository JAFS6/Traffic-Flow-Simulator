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
using System.IO;
using System.Collections.Generic;

/**
 * @class Class with methods to create GameObjects to draw de RoadMap
 */
public static class DrawRoad
{
	// Materials
	private static Material white_asphalt_material;
	private static Material asphalt_material;
	private static Material black_material;
	
	// Prefabs
	private static GameObject straight_arrow_prefab;
	private static GameObject bus_taxi_markings_prefab;
	
	/**
	 * @brief Initializes the DrawRoad class.
	 */
	static DrawRoad ()
	{
		white_asphalt_material 		= Resources.Load ("Materials/White_asphalt"					, typeof(Material	)) as Material;
		asphalt_material 			= Resources.Load ("Materials/Asphalt"						, typeof(Material	)) as Material;
		black_material 				= Resources.Load ("Materials/Simple_Black"					, typeof(Material	)) as Material;
		straight_arrow_prefab 		= Resources.Load ("Prefabs/RoadMarkings/straight_arrow"		, typeof(GameObject	)) as GameObject;
		bus_taxi_markings_prefab	= Resources.Load ("Prefabs/RoadMarkings/taxi_bus_markings"	, typeof(GameObject	)) as GameObject;
	}
	
	/**
	 * @brief Draws a node limit oriented to positive Z axis.
	 * @param[in] name The name for the object.
	 * @param[in] lane_num The number of lanes on this node.
	 * @param[in] parent Parent object to which the object will join.
	 */
	public static void nodeLimit (string name, int lane_num, GameObject parent)
	{
		float width = (lane_num*Constants.lane_width) + 2*Constants.lane_width; // To protrude from both sides
		
		GameObject aux_road = GameObject.CreatePrimitive(PrimitiveType.Cube);
		aux_road.name = name;
		aux_road.tag = Constants.Tag_Node_Limit;
		aux_road.transform.SetParent(parent.transform);
		aux_road.GetComponent<Renderer>().material = black_material;
		aux_road.transform.localScale = new Vector3(width,Constants.limit_height,Constants.limit_depth);
		Vector3 pos = new Vector3(0,(Constants.limit_height/2),0);
		aux_road.transform.position = pos;
	}
	
	/**
	 * @brief Draw a lane line by type aligned with the Z axis
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] length Line length
	 * @param[in] position Center line position
	 * @param[in] parent Parent object to which the line will join
	 */
	public static void lane_line (char lane_type, float length, Vector3 position, GameObject parent)
	{
		Vector3 position1 = new Vector3(position.x, position.y, position.z - (length/2));
		Vector3 position2 = new Vector3(position.x, position.y, position.z + (length/2));
		
		lane_line (lane_type, position1, position2, parent);
	}
	
	/**
	 * @brief Draw a lane line by type aligned with the Z axis
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Position of the other end of the line
	 * @param[in] parent Parent object to which the line will join
	 */
	public static void lane_line (char lane_type, Vector3 position1, Vector3 position2, GameObject parent)
	{
		switch (lane_type)
		{
			case Constants.Char_Public_Lane:
				continuous_line (Constants.public_transport_line_width, Constants.line_thickness, position1, position2, Constants.Line_Name_Public_Transport_Lane, parent);
				break;
			case Constants.Char_Normal_Lane:
				discontinuous_line (Constants.line_width, Constants.line_thickness, position1, position2, Constants.Line_Name_Normal_Lane, parent);
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
	public static void continuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent)
	{
		Vector3 position1 = new Vector3 (position.x, position.y, position.z - (length/2));
		Vector3 position2 = new Vector3 (position.x, position.y, position.z + (length/2));
		
		continuous_line (width, height, position1, position2, name, parent);
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
	public static void continuous_line (float width, float height, Vector3 position1, Vector3 position2,string name, GameObject parent)
	{
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
	 * @brief Draw a discontinuous white line aligned with the Z axis
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] length Length of the line
	 * @param[in] position Center position of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	public static void discontinuous_line (float width, float height, float length, Vector3 position, string name, GameObject parent)
	{
		Vector3 position1 = new Vector3 (position.x, position.y, position.z - (length/2));
		Vector3 position2 = new Vector3 (position.x, position.y, position.z + (length/2));
		
		discontinuous_line (width, height, position1, position2, name, parent);
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
	public static void discontinuous_line (float width, float height, Vector3 position1, Vector3 position2, string name, GameObject new_parent)
	{
		GameObject discontinuous_line = new GameObject ();
		discontinuous_line.name = name;
		discontinuous_line.transform.parent = new_parent.transform;
		discontinuous_line.transform.position = MyMathClass.middlePoint(position1,position2);
		float length = MyMathClass.Distance(position1,position2);
		
		int piece_num = 0;
		
		while ( (((piece_num * 2) - 1) * Constants.discontinuous_line_length) + (2 * Constants.discontinuous_line_min_margin) <= length )
		{
			piece_num++;
		}
		
		if ((((piece_num * 2) - 1) * Constants.discontinuous_line_length) + (2 * Constants.discontinuous_line_min_margin) > length)
		{
			piece_num--;
		}
		
		Vector3 pos_aux = MyMathClass.middlePoint(position1,position2);
		
		pos_aux.z -= Constants.discontinuous_line_length * ((float)piece_num - 1);
		
		for (int i=0; i < piece_num; i++)
		{
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
	 * @brief Draw a curved lane line by type
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Control point of the line
	 * @param[in] position3 Position of the other end of the line
	 * @param[in] parent Parent object to which the line will join
	 */
	public static void curved_lane_line (char lane_type, Vector3 position1, Vector3 position2, Vector3 position3, GameObject parent)
	{
		switch (lane_type)
		{
			case Constants.Char_Public_Lane:
				continuous_curved_line (Constants.public_transport_line_width, Constants.line_thickness, position1, position2, position3, Constants.Line_Name_Public_Transport_Lane, parent);
				break;
			case Constants.Char_Normal_Lane:
				discontinuous_curved_line (Constants.line_width, Constants.line_thickness, position1, position2, position3, Constants.Line_Name_Normal_Lane, parent);
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
	 * @brief Draw a continuous curved white line between the positions position1 and position3, passing throught position2
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Control point of the line
	 * @param[in] position3 Position of the other end of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	public static void continuous_curved_line (float width, float height, Vector3 position1, Vector3 position2, Vector3 position3, string name, GameObject parent)
	{
		GameObject continuous_curved_line = new GameObject();
		continuous_curved_line.name = name;
		continuous_curved_line.transform.parent = parent.transform;
		
		Vector3 start = position1;
		Vector3 end;
		
		for (int i=1; i<=10; i++)
		{
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
	 * @brief Draw a discontinuous curved white line between the positions position1 and position3, passing throught position2
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Control point of the line
	 * @param[in] position3 Position of the other end of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	public static void discontinuous_curved_line (float width, float height, Vector3 position1, Vector3 position2, Vector3 position3, string name, GameObject parent)
	{
		GameObject continuous_curved_line = new GameObject();
		continuous_curved_line.name = name;
		continuous_curved_line.transform.parent = parent.transform;
		
		float curve_length = MyMathClass.CalculateBezierLength(position1,position2,position2,position3);
		int num_segments_posible = (int)(curve_length / Constants.discontinuous_line_length);
		
		if (num_segments_posible % 2 == 0)
		{
			num_segments_posible--;
		}
		
		float margin_length = (curve_length - (num_segments_posible * Constants.discontinuous_line_length) ) / 2;
		
		float prev_dist = margin_length;
		float next_dist = prev_dist + Constants.discontinuous_line_length;
		
		while (next_dist < curve_length)
		{
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
	 * @brief Draw lane markings by the specified lane type.
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal).
	 * @param[in] pos Position of the center of the markings on the XZ plane.
	 * @param[in] positiveZ If set to@c>true</c> the object will be aligned with the positive Z axis.
	 * Otherwise it will be aligned with the negative Z axis.
	 * @param[in] parent Parent object to which the markings will join.
	 */
	public static void lane_markings (char lane_type, Vector2 pos, bool positiveZ, GameObject parent)
	{
		Vector3 marking_pos = new Vector3 (pos.x, Constants.markings_Y_position, pos.y);
		Quaternion rotation = (positiveZ) ? Quaternion.identity : Quaternion.AngleAxis(180, Vector3.up);
		
		if (lane_type == Constants.Char_Normal_Lane)
		{
			GameObject arrow = GameObject.Instantiate (straight_arrow_prefab, marking_pos, rotation) as GameObject;
			arrow.transform.SetParent(parent.transform);
		}
		else if (lane_type == Constants.Char_Public_Lane)
		{
			GameObject bus_taxi_markings = GameObject.Instantiate (bus_taxi_markings_prefab, marking_pos, rotation) as GameObject;
			bus_taxi_markings.transform.SetParent(parent.transform);
		}
		else
		{
			Debug.LogError ("Error on lane_markings: lane_type invalid");
		}
	}
	
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
	public static void BezierMesh (GameObject obj, float thick, float width, Vector3 start_point, Vector3 control_point, Vector3 end_point, float rotation_angle)
	{
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
		for (int i=0; i<10; i++)
		{
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
	}
	
	/**
	 * @brief Create a mesh with 8 vertex which seems a deformed box. The algorithm has been obtained from
	 * http://wiki.unity3d.com/index.php/ProceduralPrimitives and has been adapted to the needs of this application
	 * @param[in] obj The gameobject
	 * @param[in] vertex_array The array with 8 Vector3 with the positions of all vertex. The vertex of the bottom face
	 * are p0,p1,p2,p3 and the vertex of the top face are p7,p6,p5,p4
	 */
	private static void eightMesh (GameObject obj, Vector3[] vertex_array)
	{
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
	}
}
