using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecognitionApp
{
    /// <summary>
    /// Упрощение методом Дугласа-Пекера 
    /// </summary>
    public static class DouglasPeuckerSimplification
    {
        private static List<Point> points;
        private static List<int> result;
        private static double epsilon;
        private static int countPoint;

        /// <summary>
        /// Упрощение списка точек методом Дугласа-Пекера
        /// </summary>
        /// <param name="points"> Список точек, который сокращается</param>
        /// <param name="count"> Сколько точек нужно</param>
        /// <returns></returns>
        public static List<Point> DouglasPeucker(List<Point> points, int count)
        {
            DouglasPeuckerSimplification.points = points;
            DouglasPeuckerSimplification.result = new List<int>();
            DouglasPeuckerSimplification.countPoint = count;
            epsilon = DouglasPeuckerSimplification.CalculateEpsilon(40);
            FindResultPoint();
            if (countPoint != result.Count)
            {
                var countDifference = Math.Abs(countPoint - result.Count);
                if (countPoint > result.Count)
                {
                    AddPoint(countDifference);
                }
                else
                {
                    DeletePoint(countDifference);
                }
            }
            var resultPoint = ResultPoint();
            return resultPoint;
        }

        private static void AddPoint(int count)
        {
            while (countPoint > result.Count && epsilon >= 1)
            {
                epsilon = epsilon / 2;
                result = new List<int>();
                FindResultPoint();
            }
            if (result.Count == countPoint)
            {
                return;
            }
            if (countPoint > result.Count)
            {
                var countTemp = result.Count - 1;
                for (int i = 0; i < countTemp; i++)
                {
                    var temp = result[i] + result[i + 1] / 2;
                    if (temp < points.Count)
                    {
                        result.Add(temp);
                    }
                }
            }
            DeletePoint(result.Count - countPoint);
        }

        private static void DeletePoint(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var delIndex = FindIndexMin() + 1;
                result.RemoveAt(delIndex);
            }
        }

        private static void FindResultPoint()
        {
            result.Add(0);
            FindPoint(0, points.Count - 1);
            result.Add(points.Count - 1);
            result.Sort();
        }

        private static List<Point> ResultPoint()
        {
            var newPoint = new List<Point>();
            foreach (var i in result)
            {
                newPoint.Add(points[i]);
            }
            return newPoint;
        }

        private static void FindPoint(int first, int last)
        {
            if (first == last - 1)
            {
                return;
            }
            double distMax = -1;
            int index = -1;
            for (int i = first + 1; i < last; i++)
            {
                double dist = 0;
                if (points[first] != points[last])
                {
                    dist = Distance.PerpendicularDistance(points[i], points[first], points[last]);
                }
                else
                {
                    dist = Distance.EuclideanDistance(points[i], points[first]);
                }
                if (dist > distMax)
                {
                    index = i;
                    distMax = dist;
                }
            }
            if (distMax > epsilon)
            {
                result.Add(index);
                FindPoint(first, index);
                FindPoint(index, last);
            }
        }

        private static double CalculateEpsilon(int temp)
        {
            var listX = points.ConvertAll(new Converter<Point, double>((Point point) => point.X));
            var listY = points.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
            var maxX = listX.Max();
            var minX = listX.Min();
            var maxY = listY.Max();
            var minY = listY.Min();
            var epsilon = Math.Min(maxX - minX, maxY - minY) / temp;
            return epsilon;
        }

        private static int FindIndexMin()
        {
            var indexResult = 0;
            double minLength = Int32.MaxValue;
            for (int i = 0; i < result.Count - 2; i++)
            {
                var tempLength = Distance.EuclideanDistance(points[result[i]], points[result[i + 2]]);
                if (tempLength < minLength)
                {
                    minLength = tempLength;
                    indexResult = i;
                }
            }
            return indexResult;
        }
    }
}
