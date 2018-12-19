using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RecognitionApp
{
    /// <summary>
    /// Идеальный жест
    /// </summary>
    public class IdealGesture
    {
        public IdealGesture(string name, List<Point> points)
        {
            Name = name;
            Points = points;
            Key = KeyGestureConstruction.GetKeyForGesture(points);
        }

        public string Name { get; private set; }
        public List<Point> Points { get; private set; }
        public List<int> Key { get; private set; }
        public List<int> KeyIdeal { get; set; }

        /// <summary>
        /// Вписать в квадрат идеальный жест
        /// </summary>
        public void FitIntoSquare()
        {
            Points = SingleForm.FitIntoSquare(Points);
        }

        public void SetKeyIdeal()
        {
            KeyIdeal = KeyGestureConstruction.GetKeyForGestureWithIdentifyPoints(Points);
        }
    }
}
