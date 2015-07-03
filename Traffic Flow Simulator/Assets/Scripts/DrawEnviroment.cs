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

public static class DrawEnviroment
{
	/**
	 * @brief Draw the grass floor.
	 */
	public static void Ground ()
	{
		Material grass_material = Resources.Load ("Materials/Grass", typeof(Material)) as Material;
		
		GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
		ground.name = Constants.Name_Ground;
		ground.tag = Constants.Tag_Ground;
		ground.layer = LayerMask.NameToLayer(Constants.Layer_Roads);
		// Ground scale is divided by 10 because measurements of the plane are 10x10 in Unity
		ground.transform.localScale = new Vector3((RoadMap.max_x-RoadMap.min_x)/10, 1, (RoadMap.max_z-RoadMap.min_z)/10);
		ground.GetComponent<Renderer>().material = grass_material;
		ground.GetComponent<Renderer>().material.mainTextureScale = new Vector2(ground.transform.localScale.x, ground.transform.localScale.z);
		
		Vector3 ground_position = new Vector3((RoadMap.max_x+RoadMap.min_x)/2,Constants.ground_Y_position,(RoadMap.max_z+RoadMap.min_z)/2);
		ground.transform.position = ground_position;
	}
}
