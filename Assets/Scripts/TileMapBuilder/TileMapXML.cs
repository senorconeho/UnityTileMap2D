using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

[XmlRoot("Map")]
public class MapContainer{

	[XmlAttribute("name")] public string name;
	[XmlArray("Layers")][XmlArrayItem("Layer")] public LayerContainer[] layers;

	public static MapContainer Load(string path) {

		var serializer = new XmlSerializer(typeof(MapContainer));

		using(var stream = new FileStream(path, FileMode.Open)) {

			return serializer.Deserialize(stream) as MapContainer;
		}
	}

	public static MapContainer LoadFromText(string text) {

		var serializer = new XmlSerializer(typeof(MapContainer));
		return serializer.Deserialize(new StringReader(text)) as MapContainer;
	}

	public string SaveToText(MapContainer mapInfo) {

		var serializer = new XmlSerializer(typeof(MapContainer));

		StringWriter textWriter = new StringWriter();
		serializer.Serialize(textWriter, mapInfo);

		return textWriter.ToString();
	}
}

[XmlRoot("layer")]
public class LayerContainer {

	[XmlAttribute] public string name;
	[XmlAttribute] public int order;
	[XmlArray("Tiles")][XmlArrayItem("Line")] public TileLineContainer[] tiles;
}

[XmlRoot("Line")]
public class TileLineContainer {

	[XmlAttribute("text")] public string text;
}

