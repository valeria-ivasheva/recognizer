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
        public static List<List<Point>> ChangeCoord(List<List<Point>> points)//, out double length, out double width)
        {
            var temp = points.SelectMany(array => array).ToList();
            var listX = temp.ConvertAll(new Converter<Point, double>((Point point) => point.X));
            var listY = temp.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
            var maxX = listX.Max();
            var minX = listX.Min();
            var maxY = listY.Max();
            var minY = listY.Min();
            var length = maxX - minX;
            var width = maxY - minY;
            var scale = Math.Min(length, width);
            for (int i = 0; i < points.Count; i++)
                for (int j = 0; j < points[i].Count; j++)
                {
                    {
                        points[i][j] = new Point((points[i][j].X - minX) / scale, (points[i][j].Y - minY) / scale);
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
            points = ChangeCoord(points);//, out double length, out double width);
            double width = 1, length = 1;
            for (int i = 0; i < points.Count; i++)
            {
                var pointsStroke = points[i];
                var listX = pointsStroke.ConvertAll(new Converter<Point, double>((Point point) => point.X));
                var listY = pointsStroke.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
                var newListY = new List<double>();
                var newListX = new List<double>();
                //var newListY = RevertMax(listY, width);
                //var newListX = RevertMax(listX, length);
                if (width < length)
                {
                    newListX = RevertMax(listX, length);
                    newListY = RevertMax(listY, width);
                    if (width != 0)
                    {
                        newListY = RevertMin(newListY, length / width);
                    }
                }
                else
                {
                    newListX = RevertMax(listX, length);
                    newListY = RevertMax(listY, width);
                    if (length != 0)
                    {
                        newListX = RevertMin(newListX, width / length);
                    }
                }                
                //if (width > length)
                //{
                //    newListX = RevertMin(listX, listY, newListY);
                //    //listY = RevertMin(listY, length / width);
                //    //listX = RevertMax(listX, length);
                //}
                //else
                //{
                //    newListY = RevertMin(listY, listX, newListX);
                //    //listX = RevertMax(listX, length);
                //    //listX = RevertMin(listX, width / length);
                //    //listY = RevertMax(listY, width);
                //}
                //listX = newListX;
                //listY = newListY;
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
            if (max == 0)
            {
                return list;
            }
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

        public static List<Point> TranslateGesture(List<List<Point>> points, int count)
        {
            points = ChangeCoord(points);
            points = TranslateTo(points, Centroid(points));
            var result = Resample(points, count);
            return result;
        }
             
        private static List<List<Point>> TranslateTo(List<List<Point>> points, Point p)
        {
            List<List<Point>> newPoints = new List<List<Point>>();
            for (int i = 0; i < points.Count; i++)
            {
                var tempList = new List<Point>();
                for (int j = 0; j < points[i].Count; j++)
                {
                    tempList.Add(new Point(points[i][j].X - p.X, points[i][j].Y - p.Y));
                }
                newPoints.Add(tempList);
            }
            return newPoints;
        }

        /// <summary>
        /// Computes the centroid for an array of points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static Point Centroid(List<List<Point>> points)
        {
            double cx = 0, cy = 0;
            var temp = points.SelectMany(array => array).ToList();
            var listX = temp.ConvertAll(new Converter<Point, double>((Point point) => point.X));
            var listY = temp.ConvertAll(new Converter<Point, double>((Point point) => point.Y));
            for (int i = 0; i < temp.Count; i++)
            {
                cx += listX[i];
                cy += listY[i];
            }
            return new Point(cx / temp.Count, cy / temp.Count);
        }

        /// <summary>
        /// Resamples the array of points into n equally-distanced points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static List<Point> Resample(List<List<Point>> points, int n)
        {
            List<Point> newPoints = new List<Point>();
            newPoints.Add(new Point(points[0][0].X, points[0][0].Y));
            int numPoints = 1;
            var eps = PathLength(points) / (n - 1); // computes interval length
            foreach (var stroke in points)
            {
                var pointNow = stroke[0];
                newPoints.Add(pointNow);
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
                        newPoints.Add(new Point(x, y));
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
            var temp = points[points.Count - 1].Count - 1;
            if (numPoints == n - 1) // sometimes we fall a rounding-error short of adding the last point, so add it if so
                newPoints.Add(new Point(points[points.Count - 1][temp].X, points[points.Count - 1][temp].Y));
            return newPoints;
        }

        /// <summary>
        /// Computes the path length for an array of points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static double PathLength(List<List<Point>> points)
        {
            double length = 0;
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 1; j < points[i].Count; j++)
                {
                    length += Distance.EuclideanDistance(points[i][j - 1], points[i][j]);
                }
            }
            return length;
        }

        //private static List<double> RevertMax(List<double> list, double max)
        //{
        //    var result = new List<double>();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        result.Add(list[i] * 100 / max);
        //    }
        //    return result;
        //}

        //private static List<double> RevertMin(List<double> listToRemake, List<double> oldListToSee, List<double> newListToSee)
        //{
        //    var result = new List<double>();
        //    for (int i = 0; i < listToRemake.Count; i++)
        //    {
        //        var temp = oldListToSee[i] == 0 ? 0 : listToRemake[i] * newListToSee[i] / oldListToSee[i];
        //        result.Add(temp);
        //    }
        //    return result;
        //}
    }
}
