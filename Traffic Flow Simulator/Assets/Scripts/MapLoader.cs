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

public static class MapLoader {

	public static void LoadMap (string nombre_fichero_mapa) {
		
		// Preparar nombre del fichero del mapa a cargar
		string full_path = Application.dataPath + "/Resources/Maps/" + nombre_fichero_mapa + ".topology.graphml";

		Debug.Log ("MapLoaderRoutine will start with full path: "+full_path);

		MapLoaderRoutine(full_path);
		
		Debug.Log("MapLoaderRoutine has finished");
	}
	
	private static void MapLoaderRoutine (string nombre_fichero_completo) {
		
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
		
		int j = 0;
		string attribute = "";
		bool parsing_edges = false;
		
		XmlReader reader = XmlReader.Create(nombre_fichero_completo);
		
		while (reader.Read()) {
			Debug.Log("reader.Read(): "+reader.Name);
			// Detectar los elementos de inicio como por ejemplo <key> pero no </key>
			if (reader.IsStartElement()) {
			
				// Obtener el nombre del elemento y elegir en funcion del mismo
				switch (reader.Name) {
					case "key":
						attribute = reader["id"];
						
						if (attribute != null) {
							id = attribute;
						}
						break;
						
					case "default":
						if (id != null) {
							if (reader.Read()) {
								switch (id) {
									case "node_type":
										node_type_default = (NodeType) XmlConvert.ToByte(reader.Value.Trim());
										break;
									case "intersection_type":
										intersection_type_default = (IntersectionType) XmlConvert.ToByte(reader.Value.Trim());
										break;
									case "pos_x":
										x_default = (float) XmlConvert.ToDouble(reader.Value.Trim());
										break;
									case "pos_y":
										y_default = (float) XmlConvert.ToDouble(reader.Value.Trim());
										break;
									case "name":
										name_default = reader.Value.Trim();
										break;
									case "src_des":
										src_des_default = reader.Value.Trim();
										break;
									case "des_src":
										des_src_default = reader.Value.Trim();
										break;
								} // switch (id)
								id = null;
							}
						}
						break;
						
					case "graph":
						id = null;
						break;
						
					case "node":
						if (id == null) { // Primer nodo
							attribute = reader["id"];
							
							if (attribute != null) {
								id = attribute;
							}
						}
						else { // Resto de nodos
							// Guardar nodo anterior
							SaveNode(id,node_type_value,x_value,y_value,intersection_type_value);
							
							// Poner valores default
							node_type_value = node_type_default;
							x_value = x_default;
							y_value = y_default;
							intersection_type_value = intersection_type_default;
							
							// Leer id del siguiente nodo
							attribute = reader["id"];
							
							if (attribute != null) {
								id = attribute;
							}
						}
						break;
						
					case "edge":
						if (id == null && !parsing_edges) { // Primer arco
							parsing_edges = true;
							// Guardar ultimo nodo
							SaveNode(id,node_type_value,x_value,y_value,intersection_type_value);
							
							// Poner valores default
							source_id_value = "";
							destination_id_value = "";
							name_value = "";
							src_des_value = "";
							des_src_value = "";
							
							// Leer primer arco
							attribute = reader["id"];
							
							if (attribute != null) {
								id = attribute;
							}
							
							attribute = reader["source"];
							
							if (attribute != null) {
								source_id_value = attribute;
							}
							
							attribute = reader["target"];
							
							if (attribute != null) {
								destination_id_value = attribute;
							}
						}
						else if (parsing_edges) { // Resto de arcos
							// Guardar arco anterior
							SaveEdge(id,source_id_value,destination_id_value,name_value,src_des_value,des_src_value);
							
							// Poner valores defautl
							source_id_value = "";
							destination_id_value = "";
							name_value = "";
							src_des_value = "";
							des_src_value = "";
							
							// Leer id del siguiente nodo
							attribute = reader["id"];
							
							if (attribute != null) {
								id = attribute;
							}
						}
						break;
						
					case "data":
						if (id != null) {
							attribute = reader["key"];
							
							if (attribute != null) {
								key = attribute;
								
								if (reader.Read()) {
									
									switch (key) {
									case "node_type":
										node_type_value = (NodeType) XmlConvert.ToByte(reader.Value.Trim());
										break;
									case "intersection_type":
										intersection_type_value = (IntersectionType) XmlConvert.ToByte(reader.Value.Trim());
										break;
									case "pos_x":
										x_value = (float) XmlConvert.ToDouble(reader.Value.Trim());
										break;
									case "pos_y":
										y_value = (float) XmlConvert.ToDouble(reader.Value.Trim());
										break;
									case "name":
										name_value = reader.Value.Trim();
										break;
									case "src_des":
										src_des_value = reader.Value.Trim();
										break;
									case "des_src":
										des_src_value = reader.Value.Trim();
										break;
									}
								}
							}
						}
						break;
				} // switch (reader.Name)
				
			} // if (reader.IsStartElement())
			
			j++;
		} // while (reader.Read())
		
		// Guardar ultimo arco
		SaveEdge(id,source_id_value,destination_id_value,name_value,src_des_value,des_src_value);
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
