using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Recognition
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
        public static List<List<Point>> ChangeCoord(List<List<Point>> points, out double length, out double width)
        {
            var temp = points.SelectMany(array => array).ToList();
            var listX = temp.ConvertAll(new Converter<Point, double>((Point point) => point.X));
            var listY = temp.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
            var maxX = listX.Max();
            var minX = listX.Min();
            var maxY = listY.Max();
            var minY = listY.Min();
            length = maxX - minX;
            width = maxY - minY;
            for (int i = 0; i < points.Count; i++)
                for (int j = 0; j < points[i].Count; j++)
                {
                    {
                        points[i][j] = new Point(points[i][j].X - minX, points[i][j].Y - minY);
                    }
                }
            return points;
        }

        /// <summary>
        /// Вписать жест в квадрат
        /// </summary>
        /// <param name="points"> Список точек жеста</param>
        /// <returns> Список точек, вписанных в квадрат</returns>
        public static List<List<Point>> FitIntoSquare(List<List<Point>> points)
        {
            var result = new List<List<Point>>();
            points = ChangeCoord(points, out double length, out double width);
            for (int i = 0; i < points.Count; i++)
            {
                var pointsStroke = points[i];
                var listX = pointsStroke.ConvertAll(new Converter<Point, double>((Point point) => point.X));
                var listY = pointsStroke.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
                var newListY = RevertMax(listY, width);
                var newListX = RevertMax(listX, length);
                if (width > length)
                {
                    newListX = RevertMin(listX, listY, newListY);
                    //listY = RevertMin(listY, length / width);
                    //listX = RevertMax(listX, length);
                }
                else
                {
                    newListY = RevertMin(listY, listX, newListX);
                    //listX = RevertMax(listX, length);
                    //listX = RevertMin(listX, width / length);
                    //listY = RevertMax(listY, width);
                }
                listX = newListX;
                listY = newListY;
                var tempStroke = new List<Point>();
                for (int j = 0; j < pointsStroke.Count; j++)
                {
                    tempStroke.Add(new Point(newListX[j], newListY[j]));
                }
                result.Add(tempStroke);
            }
            return result;
        }

        private static List<double> RevertMax(List<double> list, double max)
        {
            var result = new List<double>();
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(list[i] * 100 / max);
            }
            return result;
        }

        private static List<double> RevertMin(List<double> listToRemake, List<double> oldListToSee, List<double> newListToSee)
        {
            var result = new List<double>();
            for (int i = 0; i < listToRemake.Count; i++)
            {
                var temp = oldListToSee[i] == 0 ? 0 : listToRemake[i] * newListToSee[i] / oldListToSee[i];
                result.Add(temp);
            }
            return result;
        }
    }
}
