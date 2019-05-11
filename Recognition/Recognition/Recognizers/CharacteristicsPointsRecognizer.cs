using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Recognition.Recognizers
{
    public class CharacteristicsPointsRecognizer : IRecognizer
    {
        private PerfectGesturesClass perfectGestures;
        private List<List<Point>> keyIdealGestures;
        private List<Point> keyUsersGestures; 
        private int countResult;

        public CharacteristicsPointsRecognizer()
        {
            keyIdealGestures = new List<List<Point>>();
            keyUsersGestures = new List<Point>();
        }

        public double GetDistance(object gestureKey)
        {
            var dist = Distance.DistanceBetweenGesture(keyUsersGestures, keyIdealGestures[(int)gestureKey]);
            return dist;
        }

        public void InitialKeyForIdealGestures(PerfectGesturesClass perfectGestures)
        {
            this.perfectGestures = perfectGestures;
            for (int i = 0; i < perfectGestures.IdealGestures.Count; i++)
            {
                keyIdealGestures.Add(GetOnestrokeGesture(perfectGestures.IdealGestures[i].Points));
            }
            var ind = MaxCountPointsIndexGesture();
            var maxCountGestures = keyIdealGestures[ind].Count;
            countResult = maxCountGestures * 2;
            for (int i = 0; i < perfectGestures.IdealGestures.Count; i++)
            {
                keyIdealGestures[i] = AddPoint(countResult, keyIdealGestures[i]);
                keyIdealGestures[i] = KeyForGesture(keyIdealGestures[i]);
            }
        }

        public void SetKey(List<List<Point>> points)
        {
            var inscribedGestures = GetOnestrokeGesture(points);
            keyUsersGestures = KeyForGesture(inscribedGestures);
        }

        private List<Point> KeyForGesture(List<Point> points)
        {
            var result = new List<Point>();
            var lastCharPnt = points[0];
            result.Add(lastCharPnt);
            double minDist = 10;
            for (int i = 1; i < points.Count - 1; i++)
            {
                var angle = ChangeAngle(lastCharPnt, points[i], points[i + 1]);
                bool cond = (angle > Math.PI / 9) && Distance.EuclideanDistance(lastCharPnt, points[i]) > minDist;
                if (cond)
                {
                    lastCharPnt = points[i];
                    result.Add(lastCharPnt);
                }
            }
            result.Add(points.Last());
            if (result.Count == countResult)
            {
                return result;
            }
            var count = Math.Abs(result.Count - countResult);
            if (result.Count < countResult)
            {
                result = AddPoint(count, result);
            }
            else
            {
                result = RemovePoint(count, result);
            }
            return result;
        }

        private List<Point> GetOnestrokeGesture(List<List<Point>> gesture)
        {
            var result = new List<Point>();
            gesture = SingleForm.FitIntoSquare(gesture);
            foreach (var strokeGesture in gesture)
            {
                result = result.Concat(strokeGesture).ToList();
            }
            return result;
        }

        private int MaxCountPointsIndexGesture()
        {
            var maxCount = keyIdealGestures[0].Count;
            var index = 0;
            for (int i = 1; i < keyIdealGestures.Count; i++)
            {
                if (maxCount < keyIdealGestures[i].Count)
                {
                    maxCount = keyIdealGestures[i].Count;
                    index = i;
                }
            }
            return index;
        }
        
        private List<Point> RemovePoint(int count, List<Point> points)
        {
            for (int i = 0; i < count; i++)
            {
                var delIndex = FindIndexMin(points) + 1;
                points.RemoveAt(delIndex);
            }
            return points;
        }

        private int FindIndexMin(List<Point> points)
        {
            var indexResult = 0;
            double minLength = int.MaxValue;
            for (int i = 0; i < points.Count - 2; i++)
            {
                var tempLength = Distance.EuclideanDistance(points[i], points[i + 2]);
                if (tempLength < minLength)
                {
                    minLength = tempLength;
                    indexResult = i;
                }
            }
            return indexResult;
        }

        private List<Point> AddPoint(int count, List<Point> points)
        {
            for (int i = 0; i < count; i++)
            {
                var indexSegment = FindIndexMax(points);
                var newPoint = new Point((points[indexSegment].X + points[indexSegment + 1].X) / 2, (points[indexSegment].Y + points[indexSegment + 1].Y) / 2);
                points.Insert(indexSegment + 1, newPoint);
            }
            return points;
        }

        private double ChangeAngle(Point a, Point b, Point c)
        {
            double xSegmentAB = a.X - b.X;
            double ySegmentAB = a.Y - b.Y;
            double xSegmentBC = b.X - c.X;
            double ySegmentBC = b.Y - c.Y;
            double lengthAB = Distance.EuclideanDistance(a, b);
            double lengthBC = Distance.EuclideanDistance(b, c);
            return Math.Acos(Math.Abs(xSegmentAB * xSegmentBC + ySegmentAB * ySegmentBC) / (lengthAB * lengthBC));
        }

        private int FindIndexMax(List<Point> points)
        {
            var indexResult = 0;
            double maxLength = 0;
            var pointsGesture = points;
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
