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

public static class MapLoader {

	private xml_Graphml xml_graphml;

	public static void LoadMap (string mapFilename) {
		
		// Preparar nombre del fichero del mapa a cargar
		string full_path = Application.dataPath + "/Resources/Maps/" + mapFilename + ".topology.graphml";

		Debug.Log ("MapLoaderRoutine will start with full path: "+full_path);

		MapLoaderRoutine(full_path);
		
		Debug.Log("MapLoaderRoutine has finished");
	}
	
	private static void MapLoaderRoutine (string full_path) {
		
		// Variables para tomar valores por defecto
		NodeType node_type_default = NodeType.Intersection;
		IntersectionType intersection_type_default = IntersectionType.Normal;
		float x_default = 0.0f;
		float y_default = 0.0f;
		string name_default = "";
		string src_des_default = "0";
		string des_src_default = "0";
		
		// Variables para tomar valores leidos
		string id = "";
		NodeType node_type_value = NodeType.Intersection;
		IntersectionType intersection_type_value = IntersectionType.Normal;
		float x_value = 0.0f;
		float y_value = 0.0f;
		string source_id_value = "";
		string destination_id_value = "";
		string name_value = "";
		string src_des_value = "0";
		string des_src_value = "0";
		string key = "";
		
		XmlSerializer serial = new XmlSerializer(typeof(xml_Graphml));
		Stream reader = new FileStream(full_path);
		xml_graphml = (xml_Graphml)serial.Deserialize(reader);
		
		
	}
	
	private static void SaveNode (string id, NodeType node_type, float x, float y, IntersectionType intersection_type) {
		// Guardar nodo anterior
		if (node_type == NodeType.Intersection) {
			RoadMap.addNode(id,node_type,x,y,intersection_type);
		}
		else {
			RoadMap.addNode(id,node_type,x,y);
		}
	}
	
	private static void SaveEdge (string id, string source_id, string destination_id, string name, string src_des, string des_src) {
		RoadMap.addEdge (id, source_id, destination_id, name, src_des, des_src);
	}
	
	static byte[] GetBytes (string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}
}
