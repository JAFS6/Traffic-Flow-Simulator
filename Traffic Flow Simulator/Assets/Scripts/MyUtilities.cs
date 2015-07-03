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

public static class MyUtilities : object
{
	/**
	 * @brief Creates a game object.
	 * @param[in] name Name for the game object.
	 * @param[in] parent Parent for the game object.
	 * @param[in] tag Tag for the game object.
	 * @return The new game object.
	 */
	public static GameObject CreateGameObject (string name, GameObject parent, string tag)
	{
		GameObject g = new GameObject();
		g.name = name;
		
		if (parent != null) g.transform.SetParent(parent.transform);
		
		if (tag != null) g.tag = tag;
		
		return g;
	}
	
	/**
	 * @brief Assigns the hierarchy whose root is passed as argument to the layer passed as argument
	 * @param[in] root Root of the hierarchy
	 * @param[in] layer Layer index
	 */
	public static void MoveToLayer(Transform root, int layer)
	{
		root.gameObject.layer = layer;
		
		foreach(Transform child in root)
		{
			MoveToLayer(child, layer);
		}
	}
	
	/**
	 * @brief Calculate the width of a detention line for the given lane codes.
	 * @param[in] lane_codes Lane codes of one direction of an edge.
	 * @return The calculated width.
	 */
	public static float detentionLineWidth (string lane_codes)
	{
		int n = lane_codes.Length;
		
		float w = Constants.lane_width * n;
		
		for (int i=0; i<n-1; i++)
		{
			w += (lane_codes[i].ToString() == Constants.String_Public_Lane) ? Constants.public_transport_line_width : Constants.line_width;
		}
		return w;
	}
	
	/**
	 * @brief Gets the first child with str in its name from the parent GameObject;
	 * @param[in] str The substring to search.
	 * @param[in] parent The parent.
	 */
	public static GameObject getGameObjectWithName (string str, GameObject parent)
	{
		foreach (Transform child in parent.transform)
		{
			if (child.gameObject.name.Contains(str))
			{
				return child.gameObject;
			}
		}
		return null;
	}
	
	/**
	 * @brief Gets the first child in hierarchy with str in its name from the parent GameObject;
	 * @param[in] str The substring to search.
	 * @param[in] parent The parent.
	 */
	public static GameObject getGameObjectWithNameInHierarchy (string str, GameObject parent)
	{
		foreach (Transform child in parent.transform)
		{
			if (child.gameObject.name.Contains(str))
			{
				return child.gameObject;
			}
			else
			{
				GameObject g = getGameObjectWithNameInHierarchy(str, child.gameObject);
				
				if (g != null)
				{
					return g;
				}
			}
		}
		return null;
	}
	
	public static void splitTurnPointID (string str, out string edgeID, out DirectionType direction, out int lane_order)
	{
		int i;
		
		if (str.Contains("_src_des_"))
		{
			i = str.IndexOf("_src_des_");
			direction = DirectionType.Source_Destination;
		}
		else
		{
			i = str.IndexOf("_des_src_");
			direction = DirectionType.Destination_Source;
		}
		edgeID = str.Substring(0,i);
		lane_order = System.Convert.ToInt32(str.Substring(i+9,str.Length-(i+9)));
	}
}
