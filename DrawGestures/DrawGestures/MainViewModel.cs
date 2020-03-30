using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace DrawGestures
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string figure;
        private MainWindow window;
        private bool isDrawing = false;
        private string pathToSave;
        private List<List<Point>> arrayPoint = new List<List<Point>>();
        private int index;
        private int indexUP;
        private GesturesCollection gestures;

        public MainViewModel(MainWindow mainWindow)
        {
            window = mainWindow;
            pathToSave = @"MultiGestures.xml";
            index = -1;
            index = -1;
            gestures = new GesturesCollection();
        }

        /// <summary>
        /// Команда сброса
        /// </summary>
        public ICommand ResetCommand => new DelegateCommand(Reset);

        /// <summary>
        /// Команда старта распознавания
        /// </summary>
        public ICommand SaveCommand => new DelegateCommand(Save);


        private void Reset()
        {
            window.inkCanvas.Children.Clear();
            window.inkCanvas.Strokes.Clear();
            arrayPoint.Clear();
            index = -1;
        }

        private void Save()
        {
            indexUP++;
            var temp = new Gestures();
            temp.Name = figure;
            var userPath = ListPointToString();
            temp.UserPath.Add(new UserPath(userPath));
            gestures.Gesture.Add(temp);
            Reset();
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
            index++;
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

        public void ComboBoxSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            //if (index == -1)
            //{
            //    figure = selectedItem.Content.ToString();
            //}
            figure = selectedItem.Content.ToString();
        }

        public void BeforeClose()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GesturesCollection));
            using (StreamWriter writer = new StreamWriter(pathToSave, true))
            {
                serializer.Serialize(writer, gestures);
            }
        }

        private const string comma = ", ";
        private const string pointDelimeter = " : ";
        private const string pathDelimeter = " | ";

        private string ListPointToString()
        {
            var result = "";
            foreach (var listPoint in arrayPoint)
            {
                foreach (var point in listPoint)
                {
                    result = result + $"{(int)point.X}" + comma + $"{(int)point.Y}" + pointDelimeter;
                }
                result += pathDelimeter;
            }
            return result;
        }
    }
}
