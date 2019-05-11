using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Recognition
{
    public interface IRecognizer
    {
        void InitialKeyForIdealGestures(PerfectGesturesClass perfectGestures);
        void SetKey(List<List<Point>> points);
        double GetDistance(object gestureKey);
    }
}
