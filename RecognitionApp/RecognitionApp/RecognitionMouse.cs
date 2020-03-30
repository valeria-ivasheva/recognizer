using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecognitionApp
{
    /// <summary>
    /// Класс, распознающий жест
    /// </summary>
    public class RecognitionMouse
    {
        private List<Point> points;
        private PerfectGesturesClass PGC;
        private int count;

        public RecognitionMouse(List<Point> points)
        {
            this.points = points;
            string path = @"C:\Users\ACER\source\repos\RecognitionApp\RecognitionApp\IdealGestures.xml";
            PGC = new PerfectGesturesClass(path);
            count = PGC.MaxCountIdealGestures();
        }

        /// <summary>
        /// Находит идеальный жест ( из точек упрощенных)
        /// </summary>
        /// <returns> Индекс жеста из списка идеальных жестов</returns>
        public int WhatIsItIndex()
        {
            points = SingleForm.FitIntoSquare(points);
            double minDist = Int32.MaxValue;
            int index = -1;
            for (int i = 0; i < PGC.IdealGestures.Count; i++)
            {
                var temp = Distance.DistanceBetweenGesture(PGC.IdealGestures[i].Points, points);
                if (minDist > temp)
                {
                    index = i;
                    minDist = temp;
                }
            }
            return index;
        }

        public string WhatIsItName()
        {
            var index = WhatIsItIndex();
            if (index == -1)
            {
                return "";
            }
            return PGC.IdealGestures[index].Name;
        }

        /// <summary>
        /// Находит идеальный жест с помощью ключа
        /// </summary>
        /// <returns> Индекс жеста из списка идеальных жестов</returns>
        public int WhatIsItWithKey()
        {
            var keyUsersGesture = KeyGestureConstruction.GetKeyForGesture(points);
            double minDist = Int32.MaxValue;
            int index = -1;
            for (int i = 0; i < PGC.IdealGestures.Count; i++)
            {
                var temp = Distance.LevenshteinDistance(PGC.IdealGestures[i].Key, keyUsersGesture);
                if (minDist > temp)
                {
                    index = i;
                    minDist = temp;
                }
            }
            return index;
        }

        public int WhatIsItWithIdealKey()
        {
            if (points.Count == 0)
            {
                return -1;
            }
            double minDist = Int32.MaxValue;
            int index = -1;
            var temp = new IdentifyCharacteristicsPoints(points, count);
            var newListPoints = temp.GetIdentifyCharacteristicsPoints();
            var keyIdealGesture = KeyGestureConstruction.GetKeyForGestureWithIdentifyPoints(newListPoints);
            for (int i = 0; i < PGC.IdealGestures.Count; i++)
            {
                var tempDist = Distance.LevenshteinDistance(PGC.IdealGestures[i].KeyIdeal, keyIdealGesture);
                if (minDist > tempDist)
                {
                    index = i;
                    minDist = tempDist;
                }
            }
            return index;
        }

        /// <summary>
        /// Применить алгоритм Дугласа-Пекера к жесту
        /// </summary>
        /// <returns> Упрощенный список</returns>
        public List<Point> DouglasPeucker()
        {
            if (points.Count == 0)
            {
                return null;
            }
            var newListPoints = DouglasPeuckerSimplification.DouglasPeucker(points, count);
            points = newListPoints;
            return newListPoints;
        }

        /// <summary>
        /// Найти характерные точки жеста
        /// </summary>
        /// <returns> Список характерных точек</returns>
        public List<Point> IdentifyChar()
        {
            if (points.Count == 0)
            {
                return null;
            }
            var temp = new IdentifyCharacteristicsPoints(points, count);
            var newListPoints = temp.GetIdentifyCharacteristicsPoints();
            points = newListPoints;
            return newListPoints;
        }
    }
}
