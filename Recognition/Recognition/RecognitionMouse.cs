using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Recognition
{
    public class RecognitionMouse
    {
        private IRecognizer recognizer;
        private PerfectGesturesClass perfectGestures;

        public RecognitionMouse(string pathToPerfectGestures, IRecognizer recognizer)
        {
            SetPerfectGestures(pathToPerfectGestures);
            this.recognizer = recognizer;
            recognizer.InitialKeyForIdealGestures(perfectGestures);
        }

        private void SetPerfectGestures(string path)
        {
            perfectGestures = new PerfectGesturesClass(path);
            perfectGestures.IdealGestures.Count();
        }

        public int RecognizeGestures(List<List<Point>> points)
        {
            recognizer.SetKey(points);
            var count = perfectGestures.IdealGestures.Count;
            var distanceBetweenUsersAndIdealGestures = new List<double>();
            for (int i = 0; i < count; i++)
            {
                var distance = recognizer.GetDistance(i);/// или в  скобках указать i
                distanceBetweenUsersAndIdealGestures.Add(distance);
            }
            var index = distanceBetweenUsersAndIdealGestures.IndexOf(distanceBetweenUsersAndIdealGestures.Min());
            return index;
        }

        public string ReturnName(int index) => perfectGestures.IdealGestures[index].Name;

        public string RecognizeGesturesWithName(List<List<Point>> points)
        {
            var index = RecognizeGestures(points);
            return perfectGestures.IdealGestures[index].Name;
        }
    }
}
