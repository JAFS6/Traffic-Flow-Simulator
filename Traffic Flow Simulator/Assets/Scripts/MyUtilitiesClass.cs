﻿/*
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

public static class MyUtilitiesClass : object
{
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
}
