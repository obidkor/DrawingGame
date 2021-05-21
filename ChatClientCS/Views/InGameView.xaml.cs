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

namespace ChatClientCS.Views
{
    /// <summary>
    /// InGameView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InGameView : UserControl
    {
        public InGameView()
        {
            InitializeComponent();
        }

        private void Black_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Color = Colors.Black;
        }

        private void Red_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Color = Colors.Red;
        }

        private void Blue_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Color = Colors.Blue;
        }

        private void Green_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Color = Colors.Green;
        }

        private void Yellow_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Color = Colors.Yellow;
        }

        private void White_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Color = Colors.White;
        }

        private void Thin_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Height = 3;
            MainCanvas.DefaultDrawingAttributes.Width = 3;
        }

        private void Middle_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Height = 5;
            MainCanvas.DefaultDrawingAttributes.Width = 5;
        }

        private void Thick_OnChecked(object sender, RoutedEventArgs e)
        {
            MainCanvas.DefaultDrawingAttributes.Height = 10;
            MainCanvas.DefaultDrawingAttributes.Width = 10;
        }

        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            MainCanvas.Strokes.Clear();
        }

        private void MainSound_Ended(object sender, RoutedEventArgs e)
        {
            MainBackground.Play();
        }
    }
}
