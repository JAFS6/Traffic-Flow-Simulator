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
using System.Xml;
using System.Xml.Serialization;
using MapLoaderSerial;

public class MapLoader {

	public void LoadMap (string mapFilename) {
		
		// Prepare filename of the map to load
		string full_path = Application.dataPath + Constants.maps_path + mapFilename + ".topology.graphml";
		MapLoaderRoutine (full_path);
		full_path = Application.dataPath + Constants.maps_path + mapFilename + ".turns.graphml";
		MapTurnsLoaderRoutine (full_path);
	}
	
	private void MapLoaderRoutine (string full_path) {
		
		// Variables to take defaults values
		NodeType node_type_default = NodeType.Intersection;
		IntersectionType intersection_type_default = IntersectionType.Normal;
		float x_default = 0.0f;
		float y_default = 0.0f;
		string name_default = "";
		string src_des_default = "0";
		string des_src_default = "0";
		
		// Serializer
		XmlSerializer serial = new XmlSerializer(typeof(xml_Graphml));
		Stream reader = new FileStream(full_path,FileMode.Open);
		xml_Graphml xml_graphml = (xml_Graphml)serial.Deserialize(reader);
		
		RoadMap.setMapName(xml_graphml.xml_Graphs[0].ID);
		
		// Process the defaults values section of the graph
		
		foreach (xml_MapKey k in xml_graphml.xml_Keys) {
			
			switch (k.ID) {
				case Constants.xml_graphml_key_node_type:
					
					if (k.xml_Defaults != null) {
						node_type_default = stringToNodeType(k.xml_Defaults[0].Value);
					}
					break;
				
				case Constants.xml_graphml_key_pos_x:
					
					if (k.xml_Defaults != null) {
						x_default = float.Parse(k.xml_Defaults[0].Value, System.Globalization.CultureInfo.InvariantCulture);
					}
					break;
					
				case Constants.xml_graphml_key_pos_y:
					
					if (k.xml_Defaults != null) {
						y_default = float.Parse(k.xml_Defaults[0].Value, System.Globalization.CultureInfo.InvariantCulture);
					}
					break;
					
				case Constants.xml_graphml_key_intersection_type:
					
					if (k.xml_Defaults != null) {
						intersection_type_default = stringToIntersectionType(k.xml_Defaults[0].Value);
					}
					break;
					
				case Constants.xml_graphml_key_road_name:
					
					if (k.xml_Defaults != null) {
						name_default = k.xml_Defaults[0].Value;
					}
					break;
					
				case Constants.xml_graphml_key_src_des:
					
					if (k.xml_Defaults != null) {
						src_des_default = k.xml_Defaults[0].Value;
					}
					break;
					
				case Constants.xml_graphml_key_des_src:
					
					if (k.xml_Defaults != null) {
						des_src_default = k.xml_Defaults[0].Value;
					}
					break;
			} // switch(k.ID)
		} // foreach (xml_MapKey k in xml_graphml.xml_Keys)
		
		// Process the nodes of the graph
		
		foreach (xml_Node n in xml_graphml.xml_Graphs[0].xml_Nodes) {
			NodeType node_type_value = node_type_default;
			IntersectionType intersection_type_value = intersection_type_default;
			float x_value = x_default;
			float y_value = y_default;
			string id = n.ID;
			
			if (n.xml_Node_datas != null) {
			
				foreach (xml_Node_data d in n.xml_Node_datas) {
					
					switch (d.key) {
						case Constants.xml_graphml_key_node_type:
							node_type_value = stringToNodeType(d.Value);
							break;
							
						case Constants.xml_graphml_key_pos_x:
							x_value = float.Parse(d.Value, System.Globalization.CultureInfo.InvariantCulture);
							break;
							
						case Constants.xml_graphml_key_pos_y:
							y_value = float.Parse(d.Value, System.Globalization.CultureInfo.InvariantCulture);
							break;
							
						case Constants.xml_graphml_key_intersection_type:
							intersection_type_value = stringToIntersectionType(d.Value);
							break;
					}
				} // foreach (xml_Node_data d in n.xml_Node_datas)
			}
			
			SaveNode (id, node_type_value, x_value, y_value, intersection_type_value);
			
		} // foreach(xml_Node n in xml_graphml.xml_Graphs[0].xml_Nodes)
		
		// Process the edges of the graph
		
		foreach (xml_Edge e in xml_graphml.xml_Graphs[0].xml_Edges) {
			string id = e.ID;
			string src_id = e.src_ID;
			string des_id = e.des_ID;
			string name_value = name_default;
			string src_des_value = src_des_default;
			string des_src_value = des_src_default;
			
			if (e.xml_Edge_datas != null) {
			
				foreach (xml_Edge_data d in e.xml_Edge_datas) {
				
					switch (d.key) {
						case Constants.xml_graphml_key_road_name:
							name_value = d.Value;
							break;
						
						case Constants.xml_graphml_key_src_des:
							src_des_value = d.Value;
							break;
							
						case Constants.xml_graphml_key_des_src:
							des_src_value = d.Value;
							break;
					}
				} // foreach (xml_Edge_data d in e.xml_Edge_datas)
			}
			
			SaveEdge (id, src_id, des_id, name_value, src_des_value, des_src_value);
			
		} // foreach(xml_Edge n in xml_graphml.xml_Graphs[0].xml_Edges)
		
	} // private void MapLoaderRoutine (string full_path)
	
	private void MapTurnsLoaderRoutine (string full_path)
	{
		// Serializer
		XmlSerializer serial = new XmlSerializer(typeof(xml_Graphml));
		Stream reader = new FileStream(full_path,FileMode.Open);
		xml_Graphml xml_graphml = (xml_Graphml)serial.Deserialize(reader);
		
		// Process the edges of the graph. The nodes are not necessary.
		
		if (xml_graphml.xml_Graphs[0].xml_Edges != null)
		{
			foreach (xml_Edge e in xml_graphml.xml_Graphs[0].xml_Edges)
			{
				string turn_start = e.src_ID;
				string turn_end = e.des_ID;
				
				SaveTurn (turn_start, turn_end);
			}
		}
	}
	
	private void SaveNode (string id, NodeType node_type, float x, float y, IntersectionType intersection_type) {
		
		if (node_type == NodeType.Intersection) {
			RoadMap.addNode(id,node_type,x,y,intersection_type);
		}
		else {
			RoadMap.addNode(id,node_type,x,y);
		}
	}
	
	private void SaveEdge (string id, string source_id, string destination_id, string name, string src_des, string des_src) {
		RoadMap.addEdge (id, source_id, destination_id, name, src_des, des_src);
	}
	
	private void SaveTurn (string turn_start, string turn_end)
	{
		RoadMap.addTurn(turn_start, turn_end);
	}
	
	private NodeType stringToNodeType (string s) {
		if (s == "0") {
			return NodeType.Intersection;
		}
		else if (s == "1") {
			return NodeType.Limit;
		}
		else if (s == "2") {
			return NodeType.Continuation;
		}
		else {
			Debug.LogError ("Error on stringToNodeType, invalid string. Returning default.");
			return NodeType.Limit; // Default return
		}
	}
	
	private IntersectionType stringToIntersectionType (string s) {
		if (s == "0") {
			return IntersectionType.Normal;
		}
		else if (s == "1") {
			return IntersectionType.Roundabout;
		}
		else {
			Debug.LogError ("Error on stringToIntersectionType, invalid string. Returning default.");
			return IntersectionType.Normal; // Default return
		}
	}
}
