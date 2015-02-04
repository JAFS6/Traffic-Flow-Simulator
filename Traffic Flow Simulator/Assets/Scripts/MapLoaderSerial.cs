using System.Xml.Serialization;

namespace MapLoaderSerial {
	
	[XmlRoot(ElementName = "graphml", Namespace = "http://graphml.graphdrawing.org/xmlns")]
	public class xml_Graphml {
		
		[XmlElement(ElementName = "key")]
		public xml_MapKey[] xml_Keys { get; set; }
		
		[XmlElement(ElementName = "graph")]
		public xml_Graph[] xml_Graphs { get; set; }
	}
	
	public class xml_MapKey {
		
		[XmlAttribute(AttributeName = "id")]
		public string ID { get; set; }
		
		[XmlElement(ElementName = "default")]
		public xml_Default[] xml_Defaults { get; set; }
	}
	
	public class xml_Default {
		
		[XmlText(Type = typeof(string))]
		public string Value { get; set; }
	}
	
	public class xml_Graph {
		
		[XmlAttribute(AttributeName = "id")]
		public string ID { get; set; }
		
		[XmlElement(ElementName = "node")]
		public xml_Node[] xml_Nodes { get; set; }
		
		[XmlElement(ElementName = "edge")]
		public xml_Edge[] xml_Edges { get; set; }
	}
	
	public class xml_Node {
		
		[XmlAttribute(AttributeName = "id")]
		public string ID { get; set; }
		
		[XmlElement(ElementName = "data")]
		public xml_Node_data[] xml_Node_datas { get; set; }
	}
	
	public class xml_Node_data {
		
		[XmlAttribute(AttributeName = "key")]
		public string key { get; set; }
		
		[XmlText(Type = typeof(string))]
		public string Value { get; set; }
	}
	
	public class xml_Edge {
		
		[XmlAttribute(AttributeName = "id")]
		public string ID { get; set; }
		
		[XmlAttribute(AttributeName = "source")]
		public string src_ID { get; set; }
		
		[XmlAttribute(AttributeName = "target")]
		public string des_ID { get; set; }
		
		[XmlElement(ElementName = "data")]
		public xml_Edge_data[] xml_Edge_datas { get; set; }
	}
	
	public class xml_Edge_data {
		
		[XmlAttribute(AttributeName = "key")]
		public string key { get; set; }
		
		[XmlText(Type = typeof(string))]
		public string Value { get; set; }
	}
	
} // namespace MapLoaderSerial
