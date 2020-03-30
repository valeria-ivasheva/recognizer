using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecognitionApp
{
    /// <summary>
    /// Класс, создающий ключ для жеста
    /// </summary>
    public static class KeyGestureConstruction
    {
        private static List<Point> gesture;
        private static List<int> key;

        /// <summary>
        /// Находит ключ для жеста
        /// </summary>
        /// <param name="gesturePoints"> Точки жеста</param>
        /// <returns> Ключ</returns>
        public static List<int> GetKeyForGesture(List<Point> gesturePoints)
        {
            gesture = gesturePoints;
            key = new List<int>();
            BuildKey();
            var keyTemp = new List<int>();
            for (int i = 0; i < key.Count; i++)
            {
                if (!keyTemp.Contains(key[i]))
                {
                    keyTemp.Add(key[i]);
                }
            }
            key = keyTemp;
            return key;
        }

        public static List<int> GetKeyForGestureWithIdentifyPoints(List<Point> gesturePoints)
        {
            gesture = gesturePoints;
            key = new List<int>();
            BuildKeyIdentify();
            return key;
        }

        private static void BuildKeyIdentify()
        {
            gesture = SingleForm.ChangeCoord(gesture, out double length, out double width);
            var lengthMinRectangle = length / 8;
            var widthMinRectangle = width / 8;
            for (int i = 0; i < gesture.Count; i++)
            {
                int tempA = (int)Math.Truncate(gesture[i].X / lengthMinRectangle);
                tempA = (tempA != 8) ? tempA : 7;
                int tempB = (int)Math.Truncate(gesture[i].Y / widthMinRectangle);
                tempB = (tempB != 8) ? tempB : 7;
                key.Insert(i, tempA + 8 * tempB);
            }
        }

        private static void BuildKey()
        {
            gesture = SingleForm.ChangeCoord(gesture, out double length, out double width);
            var lengthMinRectangle = length / 8;
            var widthMinRectangle = width / 8;
            var epsilon = (lengthMinRectangle < widthMinRectangle) ? lengthMinRectangle : widthMinRectangle;
            AddPoint(epsilon);
            for (int i = 0; i < gesture.Count; i++)
            {
                int tempA = (int) Math.Truncate(gesture[i].X / lengthMinRectangle);
                tempA = (tempA != 8) ? tempA : 7;
                int tempB = (int) Math.Truncate(gesture[i].Y / widthMinRectangle);
                tempB = (tempB != 8) ? tempB : 7;
                key.Insert(i, tempA + 8 * tempB);
            }
        }

        private static void AddPoint(double epsilon)
        {
            var count = gesture.Count;
            var result = new List<Point>();
            for (int i = 0; i < count - 1; i++)
            {
                int countTemp = (int)Math.Truncate(Distance.EuclideanDistance(gesture[i], gesture[i + 1]) / epsilon);
                result.Add(gesture[i]);
                for (int j = 0; j < countTemp; j++)
                {
                    Point point = FindPoint(gesture[i], gesture[i + 1], epsilon * (j + 1));
                    result.Add(point);
                }
            }
            result.Add(gesture[count - 1]);
            gesture = result;
        }

        private static Point FindPoint(Point start, Point end, double distance)
        {
            if (end.Y == start.Y)
            {
                return new Point(start.X + distance, start.Y);
            }
            if (start.X == end.X)
            {
                return new Point(start.X, start.Y + distance);
            }
            var coef = (end.X - start.X) / (end.Y - start.Y);
            var pointY = start.Y + distance / (Math.Sqrt(1 + coef * coef));
            if (!(pointY > start.Y && pointY < end.Y || pointY < start.Y && pointY > end.Y))
            {
                if (pointY > start.Y)
                {
                    pointY = start.Y - (pointY - start.Y) * 2;
                }
                else
                {
                    pointY = start.Y + (start.Y - pointY) * 2;
                }
            }
            var pointX = coef * (pointY - start.Y);
            return new Point(pointX, pointY);
        }
    }
}
