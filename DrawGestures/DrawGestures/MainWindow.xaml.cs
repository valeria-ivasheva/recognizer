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

namespace DrawGestures
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mainViewModel = new MainViewModel(this);
            DataContext = mainViewModel;
        }

        private void inkCanvasMouseMove(object sender, MouseEventArgs e)
        {
            (DataContext as MainViewModel).MouseMove(sender, e);
        }

        private void inkCanvasPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel).PreviewMouseLeftButtonDown(sender, e);
        }

        private void inkCanvasPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel).PreviewMouseLeftButtonUp();
        }

        private void ComboBoxSelected(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as MainViewModel).ComboBoxSelected(sender, e);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as MainViewModel).BeforeClose();
        }
    }
}
