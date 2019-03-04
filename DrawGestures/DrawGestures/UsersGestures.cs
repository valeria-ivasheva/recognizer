using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DrawGestures
{
    //class UsersGestures
    //{
    //}
    [Serializable()]
    public class UserPath
    {
        [XmlAttribute("path")]
        public string Path { get; set; }

        public UserPath()
        {
        }

        public UserPath(string path)
        {
            Path = path;
        }
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
        public List<UserPath> UserPath { get; set; }

        public Gestures()
        {
            UserPath = new List<UserPath>();
        }
    }

    [Serializable()]
    [XmlRoot("GesturesCollection")]
    public class GesturesCollection
    {
        [XmlArray("UsersGestures")]
        [XmlArrayItem("Gesture", typeof(Gestures))]
        public List<Gestures> Gesture { get; set; }

        public GesturesCollection()
        {
            Gesture = new List<Gestures>();
        }
    }
}
