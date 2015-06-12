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
	
	// Prefabs
	private static GameObject straight_arrow_prefab;
	private static GameObject bus_taxi_markings_prefab;
	
	/**
	 * @brief Initializes the DrawRoad class.
	 */
	static DrawRoad ()
	{
		white_asphalt_material 	= Resources.Load ("Materials/White_asphalt"					, typeof(Material	)) as Material;
		asphalt_material 		= Resources.Load ("Materials/Asphalt"						, typeof(Material	)) as Material;
		straight_arrow_prefab 	= Resources.Load ("Prefabs/RoadMarkings/straight_arrow"		, typeof(GameObject	)) as GameObject;
		bus_taxi_markings_prefab= Resources.Load ("Prefabs/RoadMarkings/taxi_bus_markings"	, typeof(GameObject	)) as GameObject;
	}
	
	/**
	 * @brief Draw a lane line by type aligned with the Z axis
	 * @param[in] lane_type Lane type (P: Public transportation, N: Normal, A: Parking, V: Bus/HOV)
	 * @param[in] length Line length
	 * @param[in] position Center line position
	 * @param[in] parent Parent object to which the line will join
	 */
	public static void lane_line (char lane_type, float length, Vector3 position, GameObject parent) {
		
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
	public static void lane_line (char lane_type, Vector3 position1, Vector3 position2, GameObject parent) {
		
		switch (lane_type) {
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
	public static void curved_lane_line (char lane_type, Vector3 position1, Vector3 position2, Vector3 position3, GameObject parent) {
		
		switch (lane_type) {
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
	public static void continuous_curved_line (float width, float height, Vector3 position1, Vector3 position2, Vector3 position3, string name, GameObject parent) {
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
	 * @brief Draw a discontinuous curved white line between the positions position1 and position3, passing throught position2
	 * @param[in] width Width of the line
	 * @param[in] height Thickness of the line
	 * @param[in] position1 Position of one end of the line
	 * @param[in] position2 Control point of the line
	 * @param[in] position3 Position of the other end of the line
	 * @param[in] name Name for the object
	 * @param[in] parent Parent object to which the line will join
	 */
	public static void discontinuous_curved_line (float width, float height, Vector3 position1, Vector3 position2, Vector3 position3, string name, GameObject parent) {
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
}
