using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Recognition.Recognizers
{
    public class HungarianRecognizer : IRecognizer
    {
        private int countPoints = 30;
        private PerfectGesturesClass perfectGestures;
        private List<List<Point>> keyIdealGestures;
        private List<Point> keyUsersGestures;

        public HungarianRecognizer()
        {
            keyIdealGestures = new List<List<Point>>();
            keyUsersGestures = new List<Point>();
        }

        public double GetDistance(object gestureKey)
        {
            var listDist = new List<List<double>>();
            for (int i = 0; i < keyUsersGestures.Count; i++)
            {
                var listTemp = new List<double>();
                for (int j = 0; j < keyIdealGestures[(int)gestureKey].Count; j++)
                {
                    var distTemp = Distance.EuclideanDistance(keyUsersGestures[i], keyIdealGestures[(int)gestureKey][j]);
                    listTemp.Add(distTemp);
                }
                listDist.Add(listTemp);
            }
            var hung = new Algorithms.Hungarian(listDist);
            var index = hung.Execute().ToList();
            double dist = 0;
           for (int i = 0; i < index.Count; i++)
            {
                dist = dist + Distance.EuclideanDistance(keyUsersGestures[index[i][0]], keyIdealGestures[(int)gestureKey][index[i][1]]);
            }
            return dist;
        }

        public void InitialKeyForIdealGestures(PerfectGesturesClass perfectGestures)
        {
            this.perfectGestures = perfectGestures;
            foreach (var nowGestures in perfectGestures.IdealGestures)
            {
                var nowPoints = new List<List<Point>>();
                foreach (var stroke in nowGestures.Points)
                {
                    //var charPoints = new IdentifyCharacteristicsPoints(stroke);
                    //nowPoints.Add(charPoints.GetIdentifyCharacteristicsPoints());
                    nowPoints.Add(stroke);
                }
                var newPoints = new List<Point>();
                newPoints = FindNeedPoints(nowPoints);
                keyIdealGestures.Add(newPoints);
            }
        }

        private List<Point> FindNeedPoints(List<List<Point>> points)
        {
            var result = new List<Point>();
            points = SingleForm.FitIntoSquare(points);

            var eps = PathLength(points) / (countPoints - 1);
            foreach (var stroke in points)
            {
                var pointNow = stroke[0];
                result.Add(pointNow);
                double tempDist = 0;
                var tempPoints = new List<Point>();
                for (int i = 1; i < stroke.Count; i++)
                {
                    tempPoints.Add(stroke[i]);
                }
                //var tempPoints = stroke.GetRange(1, stroke.Count - 1);
                int index = 0;
                while (index < tempPoints.Count)
                {
                    var dist = Distance.EuclideanDistance(pointNow, tempPoints[index]);
                    if ((tempDist + dist) > eps)
                    {
                        var x = pointNow.X + ((eps - tempDist) / dist) * (tempPoints[index].X - pointNow.X);
                        var y = pointNow.Y + ((eps - tempDist) / dist) * (tempPoints[index].Y - pointNow.Y);
                        result.Add(new Point(x, y));
                        tempPoints.Insert(index, new Point(x, y));
                        tempDist = 0;
                    }
                    else
                    {
                        tempDist += dist;
                    }
                    pointNow = tempPoints[index];
                    index++;
                }
            }
            if (result.Count == countPoints)
            {
                return result;
            }
            if (result.Count > countPoints)
            {
                result = RemovePoint(result, result.Count - countPoints);
            }
            else
            {
                result = AddPoint(result, countPoints - result.Count);
            }
            return result;
        }

        private double PathLength(List<List<Point>> points)
        {
            double result = 0;
            foreach (var stroke in points)
            {
                for (int i = 1; i < stroke.Count; i++)
                {
                    result += Distance.EuclideanDistance(stroke[i - 1], stroke[i]);
                }
            }
            return result;
        }

        public void SetKey(List<List<Point>> points)
        {
            var nowPoints = new List<List<Point>>();
            foreach (var stroke in points)
            {
                var charPoints = new IdentifyCharacteristicsPoints(stroke);
                nowPoints.Add(charPoints.GetIdentifyCharacteristicsPoints());
                //nowPoints.Add(stroke);
            }
            //var epsilon = PathLength(nowPoints) / (countPoints - 1);
            var newPoints = new List<Point>();
            newPoints = FindNeedPoints(nowPoints);
            keyUsersGestures = newPoints;
        }
        
        private int FindIndexMax(List<Point> result)
        {
            var indexResult = 0;
            double maxLength = 0;
            var pointsGesture = result;
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

        private List<Point> AddPoint(List<Point> result, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var indexSegment = FindIndexMax(result);
                var newPoint = new Point((result[indexSegment].X + result[indexSegment + 1].X) / 2, (result[indexSegment].Y + result[indexSegment + 1].Y) / 2);
                result.Insert(indexSegment + 1, newPoint);
            }
            return result;
        }

        private int FindIndexMin(List<Point> result)
        {
            var indexResult = 0;
            double minLength = int.MaxValue;
            for (int i = 0; i < result.Count - 2; i++)
            {
                var tempLength = Distance.EuclideanDistance(result[i], result[i + 2]);
                if (tempLength < minLength)
                {
                    minLength = tempLength;
                    indexResult = i;
                }
            }
            return indexResult;
        }

        private List<Point> RemovePoint(List<Point> result, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var delIndex = FindIndexMin(result) + 1;
                result.RemoveAt(delIndex);
            }
            return result;
        }
    }
}
