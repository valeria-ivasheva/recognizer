using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Recognition
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public string ThisIsGesture
        {
            get => thisIsGesture;
            set
            {
                thisIsGesture = value;
                NotifyPropertyChanged("ThisIsGesture");
            }
        }
        private MainWindow window;
        private string thisIsGesture;
        private bool isDrawing = false;
        private IRecognizer recognizer = new Recognizers.HungarianRecognizer();
        private RecognitionMouse recognition;

        public MainViewModel(MainWindow window)
        {
            this.window = window;
            string path = "IdealGestures.xml";// "IdealGestures.xml";//"MultistrokeIdealGestures.xml";
            recognition = new RecognitionMouse(path, recognizer);               
        }

        /// <summary>
        /// Команда сброса
        /// </summary>
        public ICommand ResetCommand => new DelegateCommand(Reset);

        /// <summary>
        /// Команда старта распознавания
        /// </summary>
        public ICommand StartCommand => new DelegateCommand(Start);


        private void Reset()
        {
            window.inkCanvas.Children.Clear();
            window.inkCanvas.Strokes.Clear();
            arrayPoint.Clear();
            ThisIsGesture = "";
        }

        private void Start()
        {
            var result = recognition.RecognizeGesturesWithName(arrayPoint);
            //var point = recognizer.GetCharacteristicPoints();
            //window.inkCanvas.Strokes.Clear();
            //Draw(point);
            //if (point == null)
            //{
            //    return;
            //}
            ThisIsGesture = result;
            arrayPoint.Clear();
        }

        private PathFigure picture;

        private void Draw(List<Point> points)
        {
            if (points == null)
            {
                return;
            }
            picture = new PathFigure() { StartPoint = points[0] };
            var currentPath = new System.Windows.Shapes.Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Data = new PathGeometry() { Figures = { picture } }
            };
            window.inkCanvas.Children.Add(currentPath);
            for (int i = 1; i < points.Count; i++)
            {
                picture.Segments.Add(new LineSegment(points[i], true));
            }
            picture = null;
        }

        private List<List<Point>> arrayPoint = new List<List<Point>>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing)
            {
                return;
            }
            AddPointInList(new Point(e.GetPosition(window).X, e.GetPosition(window).Y));
        }

        private void AddPointInList(Point point)
        {
            var height = window.inkCanvas.ActualHeight;
            var width = window.inkCanvas.ActualWidth;
            if (point.X >= 0 && point.X <= width && point.Y >= 0 && point.Y <= height)
            {
                arrayPoint.Last().Add(point);
            }
        }

        public void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(window.inkCanvas);
            isDrawing = true;
            arrayPoint.Add(new List<Point>());
            AddPointInList(new Point(e.GetPosition(window).X, e.GetPosition(window).Y));
        }

        public void PreviewMouseLeftButtonUp()
        {
            isDrawing = false;
            Mouse.Capture(null);
        }
    }
}