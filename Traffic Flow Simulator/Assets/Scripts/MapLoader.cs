﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;

public class MapLoader {

	RoadMap roadMap;

	public MapLoader (ref RoadMap m) {
		roadMap = m;
	}

	public void LoadMap (string nombre_fichero_mapa) {
		// Liberar recursos en caso de que hubiese un mapa previo
		Unload();
		
		// Preparar nombre del fichero del mapa a cargar
		string full_path = Application.dataPath + "/Resources/Maps/" + nombre_fichero_mapa + ".graphml";

		Debug.Log ("MapLoaderRoutine will start with full path: "+full_path);

		MapLoaderRoutine(full_path);
	}
	
	private void Unload () {
		
	}
	
	private void MapLoaderRoutine (string nombre_fichero_completo) {
		
		// Variables para tomar valores por defecto
		NodeType node_type_default = NodeType.INTERSECTION;
		IntersectionType intersection_type_default = IntersectionType.NORMAL;
		float x_default = 0.0f;
		float y_default = 0.0f;
		string name_default = "";
		string src_des_default = "0";
		string des_src_default = "0";
		
		// Variables para tomar valores leidos
		string id = "";
		NodeType node_type_value = NodeType.INTERSECTION;
		IntersectionType intersection_type_value = IntersectionType.NORMAL;
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
			// Detectar los elementos de inicio como por ejemplo <key> pero no </key>
			if (reader.IsStartElement()) {
				// Obtener el nombre del elemento y elegir en funcion del mismo
				switch (reader.Name) {
				case "key":
					attribute = reader["id"];
					
					if (attribute != null) {
						id = attribute;
					}
					else {
						id = "";
					}
					break;
					
				case "default":
					if (id != "") {
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
							}
							id = "";
						}
					}
					break;
					
				case "graph":
					id = "";
					break;
					
				case "node":
					if (id == "") { // Primer nodo
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
					if (id == "" && !parsing_edges) { // Primer arco
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
				}
			}
			
			j++;
		} // while (reader.Read())
		
		// Guardar ultimo arco
		SaveEdge(id,source_id_value,destination_id_value,name_value,src_des_value,des_src_value);
	}
	
	private void SaveNode (string id, NodeType node_type, float x, float y, IntersectionType intersection_type) {
		// Guardar nodo anterior
		if (node_type == NodeType.INTERSECTION) {
			roadMap.addNode(id,node_type,x,y,intersection_type);
		}
		else {
			roadMap.addNode(id,node_type,x,y);
		}
	}
	
	private void SaveEdge (string id, string source_id, string destination_id, string name, string src_des, string des_src) {
		roadMap.addEdge (id, source_id, destination_id, name, src_des, des_src);
	}
	
	static byte[] GetBytes (string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}
}