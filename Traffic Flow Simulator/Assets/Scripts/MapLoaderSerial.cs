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
using System.Xml.Serialization;

namespace MapLoaderSerial
{	
	[XmlRoot(ElementName = "graphml", Namespace = "http://graphml.graphdrawing.org/xmlns")]
	public class xml_Graphml
	{	
		[XmlElement(ElementName = "key")]
		public xml_MapKey[] xml_Keys { get; set; }
		
		[XmlElement(ElementName = "graph")]
		public xml_Graph[] xml_Graphs { get; set; }
	}
	
	public class xml_MapKey
	{
		[XmlAttribute(AttributeName = "id")]
		public string ID { get; set; }
		
		[XmlElement(ElementName = "default")]
		public xml_Default[] xml_Defaults { get; set; }
	}
	
	public class xml_Default
	{
		[XmlText(Type = typeof(string))]
		public string Value { get; set; }
	}
	
	public class xml_Graph
	{
		[XmlAttribute(AttributeName = "id")]
		public string ID { get; set; }
		
		[XmlElement(ElementName = "node")]
		public xml_Node[] xml_Nodes { get; set; }
		
		[XmlElement(ElementName = "edge")]
		public xml_Edge[] xml_Edges { get; set; }
	}
	
	public class xml_Node
	{
		[XmlAttribute(AttributeName = "id")]
		public string ID { get; set; }
		
		[XmlElement(ElementName = "data")]
		public xml_Node_data[] xml_Node_datas { get; set; }
	}
	
	public class xml_Node_data
	{
		[XmlAttribute(AttributeName = "key")]
		public string key { get; set; }
		
		[XmlText(Type = typeof(string))]
		public string Value { get; set; }
	}
	
	public class xml_Edge
	{
		[XmlAttribute(AttributeName = "id")]
		public string ID { get; set; }
		
		[XmlAttribute(AttributeName = "source")]
		public string src_ID { get; set; }
		
		[XmlAttribute(AttributeName = "target")]
		public string des_ID { get; set; }
		
		[XmlElement(ElementName = "data")]
		public xml_Edge_data[] xml_Edge_datas { get; set; }
	}
	
	public class xml_Edge_data
	{
		[XmlAttribute(AttributeName = "key")]
		public string key { get; set; }
		
		[XmlText(Type = typeof(string))]
		public string Value { get; set; }
	}
	
} // namespace MapLoaderSerial
