using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Recognition.Recognizers
{
    public class GreedyAlgRecognizer : IRecognizer
    {
        private int countPoints = 30;
        private PerfectGesturesClass perfectGestures;
        private List<List<Point>> keyIdealGestures;
        private List<Point> keyUsersGestures;

        public GreedyAlgRecognizer()
        {
            keyIdealGestures = new List<List<Point>>();
            keyUsersGestures = new List<Point>();
        }

        public double GetDistance(object gestureKey)
        {
            var tempGesture = keyIdealGestures[(int)gestureKey].ToArray();
            var tempUserGesture = keyUsersGestures.ToArray();
            var dist = GreedyCloudMatch(tempGesture, tempUserGesture);
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
                //newPoints = SingleForm.TranslatePoints(nowPoints, countPoints);
                //newPoints = SingleForm.TranslateGesture(nowPoints, countPoints);
                newPoints = FindNeedPoints(nowPoints); //SingleForm.Resample(nowPoints, countPoints);
                keyIdealGestures.Add(newPoints);
            }
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
            //newPoints = SingleForm.TranslatePoints(nowPoints, countPoints);
            //newPoints = SingleForm.TranslateGesture(nowPoints, countPoints);
            newPoints = FindNeedPoints(nowPoints);
            keyUsersGestures = newPoints;
        }

        private double GreedyCloudMatch(Point[] points1, Point[] points2)
        {
            int n = points1.Length; 
            float eps = 0.5f;       
            int step = (int)Math.Floor(Math.Pow(n, 1.0f - eps));
            double minDistance = float.MaxValue;
            for (int i = 0; i < n; i += step)
            {
                var dist1 = CloudDistance(points1, points2, i);  
                var dist2 = CloudDistance(points2, points1, i);   
                minDistance = Math.Min(minDistance, Math.Min(dist1, dist2));
            }
            return minDistance;
        }

        /// <summary>
        /// Computes the distance between two point clouds by performing a minimum-distance greedy matching
        /// starting with point startIndex
        /// </summary>
        /// <param name="points1"></param>
        /// <param name="points2"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private double CloudDistance(Point[] points1, Point[] points2, int startIndex)
        {
            int n = points1.Length;       // the two clouds should have the same number of points by now
            bool[] matched = new bool[n]; // matched[i] signals whether point i from the 2nd cloud has been already matched
            Array.Clear(matched, 0, n);   // no points are matched at the beginning

            double sum = 0;  // computes the sum of distances between matched points (i.e., the distance between the two clouds)
            int i = startIndex;
            do
            {
                int index = -1;
                double minDistance = double.MaxValue;
                for (int j = 0; j < n; j++)
                    if (!matched[j])
                    {
                        var dist = Math.Pow(Distance.EuclideanDistance(points1[i], points2[j]), 2);  // use squared Euclidean distance to save some processing time
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            index = j;
                        }
                    }
                matched[index] = true; // point index from the 2nd cloud is matched to point i from the 1st cloud
                double weight = 1.0f - ((i - startIndex + n) % n) / (1.0f * n);
                sum += weight * minDistance; // weight each distance with a confidence coefficient that decreases from 1 to 0
                i = (i + 1) % n;
            } while (i != startIndex);
            return sum;
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
