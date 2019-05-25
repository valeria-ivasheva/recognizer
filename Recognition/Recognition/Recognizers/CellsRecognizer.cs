using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Recognition.Recognizers
{
    public class CellsRecognizer : IRecognizer
    {
        private double minMovement = 20;
        private double minPoint = -1000;
        private double maxRelation = 8;
        private int hightSize = 30;
        private int widthSize = 30;
        private PerfectGesturesClass perfectGestures;
        private List<List<double>> keyIdealGestures;
        private List<double> keyUsersGestures;
        private int gridSize = 100;

        public CellsRecognizer()
        {
            keyIdealGestures = new List<List<double>>();
            keyUsersGestures = new List<double>();
        }

        public double GetDistance(object gestureKey)
        {
            double norm = 0;
            double sum = 0;
            for (int i = 0; i < gridSize * gridSize; i++)
            {
                sum += Math.Abs(keyUsersGestures[i] - keyIdealGestures[(int)gestureKey][i]);
                norm = Math.Max(norm, Math.Abs(keyUsersGestures[i] - keyIdealGestures[(int)gestureKey][i]));
            }
            //return norm / (gridSize * gridSize);
            return norm + sum / (gridSize * gridSize);
        }

        public void InitialKeyForIdealGestures(PerfectGesturesClass perfectGestures)
        {
            this.perfectGestures = perfectGestures;
            foreach (var nowGestures in perfectGestures.IdealGestures)
            {
                var temp = GetKey(nowGestures.Points);
                keyIdealGestures.Add(temp);
            }
        }

        public void SetKey(List<List<Point>> points)
        {
            keyUsersGestures = GetKey(points);
        }

        private List<double> GetKey(List<List<Point>> path)
        {
            var key = GetKeyTemp(path, hightSize, widthSize);
            var finalKey = new double[gridSize * gridSize];
            for (int i = 0; i < gridSize * gridSize; i++)
            {
                finalKey[i] = gridSize;
            }
            if (key.Count == 0)
                return finalKey.ToList();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    double dist = Math.Abs(key[0].X - i) + Math.Abs(key[0].Y - j);
                    foreach (var pos in key)
                    {
                        dist = Math.Min(dist, Math.Abs(pos.X - i) + Math.Abs(pos.Y - j));
                    }
                    finalKey[i * gridSize + j] = dist;
                }
            }
            return finalKey.ToList();
        }
        private List<Point> GetKeyTemp(List<List<Point>> path, int heightSize, int widthSize)
        {
            List<Point> key = new List<Point>();
            if (path.Count == 0)
                return key;
            var temp = path.SelectMany(array => array).ToList();
            var listX = temp.ConvertAll(new Converter<Point, double>((Point point) => point.X));
            var listY = temp.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
            var maxX = listX.Max();
            var minX = listX.Min();
            var maxY = listY.Max();
            var minY = listY.Min();
            var width = maxY - minY;
            var length = maxX - minX;
            if (length < minMovement && width < minMovement)
                return key;
            foreach (var stroke in path)
            {
                Point previous = new Point(minPoint, minPoint);
                Point last = new Point(0, 0);
                foreach (var point in stroke)
                {
                    if ((width) * maxRelation < length)
                    {
                        last.X = (point.X - minX) * widthSize / (length);
                    }
                    else if ((length) * maxRelation < width)
                    {
                        last.Y = (point.Y - maxY) * heightSize / (width);
                    }
                    else
                    {
                        last.X = (int)((point.X - minX) * widthSize / (length));
                        last.Y = (int)((point.Y - minY) * heightSize / (width));
                    }
                    if (last.X == widthSize)
                        last.X--;
                    if (last.Y == heightSize)
                        last.Y--;
                    if (previous.X != minPoint || previous.Y != minPoint)
                    {
                        key = RasterizeSegment(previous, last, key);
                    }
                    previous = last;
                }
            }
            return key;
        }

        private List<Point> RasterizeSegment(Point pos1, Point pos2, List<Point> segment)
        {
            if (segment.Count != 0 && pos1 == segment[0])
                segment.RemoveAt(segment.Count - 1);
            if (pos1 == pos2)
            {
                segment.Add(pos1);
                return segment;
            }
            var x = pos1.X;
            var y = pos1.Y;
            var deltaX = Math.Abs(pos2.X - x);
            var deltaY = Math.Abs(pos2.Y - y);
            int signX = Math.Sign(pos2.X - x);
            int signY = Math.Sign(pos2.Y - y);
            bool isChanged = false;
            if (deltaY > deltaX)
            {
                var c = deltaX;
                deltaX = deltaY;
                deltaY = c;
                isChanged = true;
            }
            var e = 2 * deltaY - deltaX;
            for (int i = 0; i < deltaX; i++)
            {
                segment.Add(new Point(x, y));
                while (e >= 0)
                {
                    if (isChanged)
                        x += signX;
                    else
                        y += signY;
                    e -= 2 * deltaX;
                }
                if (isChanged)
                    y += signY;
                else
                    x += signX;
                e += 2 * deltaY;
            }
            return segment;
        }
    }
}
