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

namespace RecognitionApp
{
    /// <summary>
    /// Класс идеальных жестов
    /// </summary>
    public class PerfectGesturesClass
    {
        private readonly string pathToPerfectGestureDescription;

        /// <summary>
        /// Список иделаьных жестов
        /// </summary>
        public List<IdealGesture> IdealGestures { get; private set; }

        public PerfectGesturesClass(string pathToPerfectGestureDescription)
        {
            this.pathToPerfectGestureDescription = pathToPerfectGestureDescription;
            IdealGestures = new List<IdealGesture>();
            IdealGestureLoad();
            AddedPointIdealGestures();
            for (int i = 0; i < IdealGestures.Count; i++)
            {
                IdealGestures[i].FitIntoSquare();
                IdealGestures[i].SetKeyIdeal();
            }
        }

        /// <summary>
        /// Находит индекс идеального жеста, содержащего максимальное количество точек 
        /// </summary>
        /// <returns> Индекс идеального жеста, содержащего максимальное количество точек </returns>
        public int MaxCountIdealGestures()
        {
            return IdealGestures[MaxCountPointsIndexGesture()].Points.Count;
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
        
        public List<Point> AdopterPoints(string str)
        {
            var result = new List<Point>();
            var separator = new string[] { " : ", "\t" };
            var strArray = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strArray.Length; i++)
            {
                var strTemp = new string[]{", "};
                var coord = strArray[i].Split(strTemp, StringSplitOptions.RemoveEmptyEntries);
                int x;
                int y;
                Int32.TryParse(coord[0], out x);
                Int32.TryParse(coord[1], out y);
                result.Add(new Point(x, y));
            }
            return result;
        }

        private int MaxCountPointsIndexGesture()
        {
            var maxCount = IdealGestures[0].Points.Count;
            var index = 0;
            for (int i = 1; i < IdealGestures.Count; i++)
            { 
                if (maxCount < IdealGestures[i].Points.Count)
                {
                    maxCount = IdealGestures[i].Points.Count;
                    index = i;
                }
            }
            return index;
        }

        private void AddedPointIdealGestures()
        {
            var count = IdealGestures[MaxCountPointsIndexGesture()].Points.Count;
            for (int i = 0; i < IdealGestures.Count; i++)
            {
                if (IdealGestures[i].Points.Count() < count)
                {
                    AddedPointGestures(i, count - IdealGestures[i].Points.Count);
                }
            }
        }

        private void AddedPointGestures(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var indexSegment = FindIndexMax(index);
                var newPoint = new Point((IdealGestures[index].Points[indexSegment].X + IdealGestures[index].Points[indexSegment + 1].X) / 2, (IdealGestures[index].Points[indexSegment].Y + IdealGestures[index].Points[indexSegment + 1].Y) / 2);
                IdealGestures[index].Points.Insert(indexSegment + 1, newPoint);
            }
        }

        private int FindIndexMax(int index)
        {
            var indexResult = 0;
            double maxLength = 0;
            var pointsGesture = IdealGestures[index].Points;
            for (int i = 0; i < pointsGesture.Count - 1; i++)
            {
                var tempLength = Distance.EuclideanDistance(pointsGesture[i], pointsGesture[i + 1]);
                if (tempLength > maxLength)
                {
                    maxLength = tempLength;
                    indexResult = i;
                }
            }
            return indexResult;
        }
    }
}

