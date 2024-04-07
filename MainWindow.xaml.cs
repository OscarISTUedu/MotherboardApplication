using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace ЭВМ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MotherBoard AorusB450;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Source = (string)((Button)e.OriginalSource).Content;
            if (Source == "Начать")
            {
                Menu.Visibility= Visibility.Visible;
            }
        }

        private void Menu_Click (object sender, RoutedEventArgs e) 
        { 
            string Source = (string)((Button)e.OriginalSource).Content;
            string NumberString = Source.Substring(Source.Length-1);//образка названия кнопки, для выделения числа
            int Number = int.Parse(NumberString);//преобразование в int
            switch(Number)
            {
                case 1:
                    Menu.Visibility = Visibility.Hidden;
                    Begin.Visibility = Visibility.Hidden;
                    About.Visibility = Visibility.Hidden;
                    break;


            }
        }



    }

    public class MotherBoard
    {
        MotherBoard()
        {


        }
    }
}