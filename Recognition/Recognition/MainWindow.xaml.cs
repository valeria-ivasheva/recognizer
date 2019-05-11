using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Recognition
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var context = new MainViewModel(this);
            DataContext = context;
        }

        private void InkCanvasMouseMove(object sender, MouseEventArgs e) => (DataContext as MainViewModel).MouseMove(sender, e);

        private void InkCanvasPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => (DataContext as MainViewModel).PreviewMouseLeftButtonDown(sender, e);

        private void InkCanvasPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => (DataContext as MainViewModel).PreviewMouseLeftButtonUp();
    }
}
