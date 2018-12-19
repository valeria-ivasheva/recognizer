using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecognitionApp
{
    /// <summary>
    /// Класс, вписывает жест в квадрат
    /// </summary>
    public static class SingleForm
    {
        /// <summary>
        /// Перемещает фигуру максимально близко к началу координат, но оставляет в 1 четверти
        /// </summary>
        /// <param name="points"> Список точек для перемещения</param>
        /// <param name="length"> Длина прямоугольника, описанного около фигуры</param>
        /// <param name="width"> Ширина прямоугольника, описанного около фигуры</param>
        /// <returns> Полученный список перемещенных точек</returns>
        public static List<Point> ChangeCoord(List<Point> points, out double length, out double width)
        {
            var listX = points.ConvertAll(new Converter<Point, double>((Point point) => point.X));
            var listY = points.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
            var maxX = listX.Max();
            var minX = listX.Min();
            var maxY = listY.Max();
            var minY = listY.Min();
            length = maxX - minX;
            width = maxY - minY;
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Point(points[i].X - minX, points[i].Y - minY);
            }
            return points;
        }

        /// <summary>
        /// Вписать жест в квадрат
        /// </summary>
        /// <param name="points"> Список точек жеста</param>
        /// <returns> Список точек, вписанных в квадрат</returns>
        public static List<Point> FitIntoSquare(List<Point> points)
        {
            points = ChangeCoord(points, out double length, out double width);
            var listX = points.ConvertAll(new Converter<Point, double>((Point point) => point.X));
            var listY = points.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
            if (width < length)
            {
                listX = RevertMax(listX, length);
                listY = RevertMax(listY, width);
                listY = RevertMin(listY, length / width);
            }
            else
            {
                listX = RevertMax(listX, length);
                listX = RevertMin(listX, width / length);
                listY = RevertMax(listY, width);
            }
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Point(listX[i], listY[i]);
            }
            return points;
        }

       private static List<double> RevertMax(List<double> list, double max)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i] * 100 / max;
            }
            return list;
        }

        private static List<double> RevertMin(List<double> list, double coefficient)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i] / coefficient;
            }
            return list;
        }
    }
}
