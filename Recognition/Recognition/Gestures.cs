using System;
using System.Xml.Serialization;

namespace Recognition
{
    [Serializable()]
    public class UserPath
    {
        [XmlAttribute("path")]
        public string Path { get; set; }
    }

    [Serializable()]
    public class Gestures
    {
        [XmlAttribute("idealPath")]
        public string IdealPath { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray("UserPaths")]
        [XmlArrayItem("UserPath", typeof(UserPath))]
        public UserPath[] UserPath { get; set; }
    }

    [Serializable()]
    [XmlRoot("GesturesCollection")]
    public class GesturesCollection
    {
        [XmlArray("UsersGestures")]
        [XmlArrayItem("Gesture", typeof(Gestures))]
        public Gestures[] Gesture { get; set; }
    }
}
