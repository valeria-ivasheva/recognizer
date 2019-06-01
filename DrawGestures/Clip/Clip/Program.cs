using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Clip
{
    class Program
    {
        private static GesturesCollection gesturesCollection;
        private static GesturesCollection GesturesCollectionClip = new GesturesCollection();
        private struct Key
        {
            public string Name;
            public string Path;
            public Key(string Name, string Path)
            {
                this.Name = Name;
                this.Path = Path;
            }
        }

        static void Main(string[] args)
        {
            string pathToXml = @"C:\Users\ACER\Downloads\курсач\МногоштриховыеДругие";
            string pathToSave = "UnlikeMultistrokeGestures.xml";            
            var usersPathGestures = new Dictionary<string, List<UserPath>>();
            var KeyList = new List<Key>();
            var files = Directory.GetFiles(pathToXml);
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine(files[i]);
                using (StreamReader reader = new StreamReader(files[i]))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GesturesCollection));
                    gesturesCollection = (GesturesCollection)serializer.Deserialize(reader);
                }
                foreach(var gestures in gesturesCollection.Gesture)
                {
                    if (!usersPathGestures.ContainsKey(gestures.Name))
                    {
                        KeyList.Add(new Key(gestures.Name, ""));
                        usersPathGestures.Add(gestures.Name, new List<UserPath>());
                    }

                    var tempUserPath = usersPathGestures[gestures.Name].Concat(gestures.UserPath).ToList();
                    usersPathGestures[gestures.Name] = tempUserPath;
                }
            }
            var gesturesUser = new List<Gestures>();
            for (int i = 0; i < KeyList.Count; i++)
            {
                var userPaths = usersPathGestures[KeyList[i].Name];
                var temp = new Gestures(KeyList[i].Name, KeyList[i].Path, userPaths);
                gesturesUser.Add(temp);
            }
            var gesturesCol = new GesturesCollection(gesturesUser);
            XmlSerializer serializerForWriter = new XmlSerializer(typeof(GesturesCollection));
            using (StreamWriter writer = new StreamWriter(pathToSave, true))
            {
                serializerForWriter.Serialize(writer, gesturesCol);
            }
        }
    }
}
