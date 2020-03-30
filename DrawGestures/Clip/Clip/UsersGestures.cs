using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Clip
{    
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
        public string IdealPath;

        [XmlAttribute("name")]
        public string Name;

        [XmlArray("UserPaths")]
        [XmlArrayItem("UserPath", typeof(UserPath))]
        public List<UserPath> UserPath;

        public Gestures()
        {
            UserPath = new List<UserPath>();
        }

        public Gestures(string name, string idealPath, List<UserPath> userPaths)
        {
            this.Name = name;
            this.IdealPath = idealPath;
            this.UserPath = userPaths;
        }
    }

    [Serializable()]
    [XmlRoot("GesturesCollection")]
    public class GesturesCollection
    {
        [XmlArray("UsersGestures")]
        [XmlArrayItem("Gesture", typeof(Gestures))]
        public List<Gestures> Gesture;

        public GesturesCollection()
        {
            Gesture = new List<Gestures>();
        }

        public GesturesCollection(List<Gestures> gestures)
        {
            this.Gesture = gestures;
        }
    }
}