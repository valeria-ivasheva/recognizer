using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecognitionApp
{
    /// <summary>
    /// Различные расстояния
    /// </summary>
    public static class Distance
    {
        public static double EuclideanDistance(Point firstPoint, Point endPoint)
        {
            var distFirstEnd = Math.Sqrt(Math.Pow(firstPoint.X - endPoint.X, 2) + Math.Pow(firstPoint.Y - endPoint.Y, 2));
            return distFirstEnd;
        }

        public static double PerpendicularDistance(Point point, Point firstPoint, Point endPoint)
        {
            var pointLine = Math.Abs((endPoint.Y - firstPoint.Y) * point.X - (endPoint.X - firstPoint.X) * point.Y + endPoint.X * firstPoint.Y - endPoint.Y * firstPoint.X);
            var distFirstEnd = Math.Sqrt(Math.Pow(firstPoint.X - endPoint.X, 2) + Math.Pow(firstPoint.Y - endPoint.Y, 2));
            var dist = pointLine / distFirstEnd;
            return dist;
        }

        public static double DistanceBetweenGesture(List<Point> pointsIdealGesture,List<Point> pointsOfGesture)
        {
            double result = 0;
            for (int i = 0; i < pointsOfGesture.Count; i++)
            {
                result += EuclideanDistance(pointsIdealGesture[i], pointsOfGesture[i]);
            }
            return result;
        }

        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            int n = a.Count();
            int m = b.Count();
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            int curRow = 0;
            int nextRow = 1;
            var first = a.ToList();
            var second = b.ToList();
            int[][] rows = new int[][] { new int[m + 1], new int[m + 1] };
            for (int j = 0; j <= m; ++j) rows[curRow][j] = j;
            for (int i = 1; i <= n; ++i)
            {
                rows[nextRow][0] = i;
                for (int j = 1; j <= m; ++j)
                {
                    int dist1 = rows[curRow][j] + 1;
                    int dist2 = rows[nextRow][j - 1] + 1;
                    int dist3 = rows[curRow][j - 1] + (first[i - 1].Equals(second[j - 1]) ? 0 : 1);
                    rows[nextRow][j] = Math.Min(dist1, Math.Min(dist2, dist3));
                }
                if (curRow == 0)
                {
                    curRow = 1;
                    nextRow = 0;
                }
                else
                {
                    curRow = 0;
                    nextRow = 1;
                }
            }
            return rows[curRow][m];
        }
    }
}
