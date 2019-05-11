using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
                      
namespace Recognition
{
    public class PerfectGesturesClass
    {
        private readonly string pathToPerfectGestureDescription;

        /// <summary>
        /// Список идеальных жестов
        /// </summary>
        public List<IdealGesture> IdealGestures { get; private set; }

        public PerfectGesturesClass(string pathToPerfectGestureDescription)
        {
            this.pathToPerfectGestureDescription = pathToPerfectGestureDescription;
            IdealGestures = new List<IdealGesture>();
            IdealGestureLoad();
            ///AddedPointIdealGestures();
            //for (int i = 0; i < IdealGestures.Count; i++)
            //{
            //    IdealGestures[i].FitIntoSquare();
            //}
        }

        private void AddedPointIdealGestures()
        {
            throw new NotImplementedException();
        }

        private GesturesCollection Deserialize(string path)
        {
            //string path = @"C:\Users\ACER\source\repos\RecognitionApp\RecognitionApp\UserGestures.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(GesturesCollection));

            StreamReader reader = new StreamReader(path);
            var gesturesCollection = (GesturesCollection)serializer.Deserialize(reader);
            reader.Close();
            return gesturesCollection;
        }

        private void IdealGestureLoad()
        {
            var gestureCollection = Deserialize(pathToPerfectGestureDescription);
            foreach (var gesture in gestureCollection.Gesture)
            {
                var points = AdopterPoints(gesture.IdealPath);
                IdealGestures.Add(new IdealGesture(gesture.Name, points));
            }
        }

        public List<List<Point>> AdopterPoints(string str)
        {
            var result = new List<List<Point>>();
            var strSep = new string[] { " | " };
            var strArrayStroke = str.Split(strSep, StringSplitOptions.RemoveEmptyEntries);
            foreach (var stroke in strArrayStroke)
            {
                var strokePoint = new List<Point>();
                var separator = new string[] { " : ", "\t" };
                var strArray = stroke.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strArray.Length; i++)
                {
                    var strTemp = new string[] { ", " };
                    var coord = strArray[i].Split(strTemp, StringSplitOptions.RemoveEmptyEntries);
                    Int32.TryParse(coord[0], out int x);
                    Int32.TryParse(coord[1], out int y);
                    strokePoint.Add(new Point(x, y));
                }
                result.Add(strokePoint);
            }
            return result;
        }
               
        public class IdealGesture
        {
            public IdealGesture(string name, List<List<Point>> points)
            {
                Name = name;
                Points = points;
            }

            public string Name { get; private set; }
            public List<List<Point>> Points { get; private set; }
            public List<List<Point>> Key { get; set; }

            //internal void FitIntoSquare()
            //{
            //    Points = SingleForm.FitIntoSquare(Points);
            //}
        }
    }
}