using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecognitionApp
{
    /// <summary>
    /// Класс, значащие точки жеста
    /// </summary>
    public class IdentifyCharacteristicsPoints
    {
        private List<Point> input;
        private int countResult;
        private List<Point> result;

        public IdentifyCharacteristicsPoints(List<Point> points, int countPoints)
        {
            input = points;
            countResult = countPoints;
            result = new List<Point>();
        }

        /// <summary>
        /// Получить характеристические точки жеста
        /// </summary>
        /// <returns> Список характеристических точек</returns>
        public List<Point> GetIdentifyCharacteristicsPoints()
        {
            FindIdentifyCharacteristicsPoints(input);
            return result;
        }

        private void FindIdentifyCharacteristicsPoints(List<Point> points)
        {
            result = new List<Point>();
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
                return;
            }
            var count = Math.Abs(result.Count - countResult);
            if (result.Count < countResult)
            {
                AddPoint(count);
            }
            else
            {
                RemovePoint(count);
            }
        }

        private void RemovePoint(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var delIndex = FindIndexMin() + 1;
                result.RemoveAt(delIndex);
            }
        }

        private int FindIndexMin()
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

        private void AddPoint(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var indexSegment = FindIndexMax();
                var newPoint = new Point((result[indexSegment].X + result[indexSegment + 1].X) / 2, (result[indexSegment].Y + result[indexSegment + 1].Y) / 2);
                result.Insert(indexSegment + 1, newPoint);
            }
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

        private int FindIndexMax()
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
    }
}
