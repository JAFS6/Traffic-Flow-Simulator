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
