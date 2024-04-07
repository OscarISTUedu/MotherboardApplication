
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.AccessControl;
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
    using static Tools;
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


        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("1");
        }


        private void Menu_Click (object sender, RoutedEventArgs e) 
        { 
            string Source = (string)((Button)e.OriginalSource).Content;
            string NumberString = Source.Substring(Source.Length-1);//образка названия кнопки, для выделения числа
            int Number = int.Parse(NumberString);//преобразование в int
            switch(Number)
            {
                case 1://КЗ
                    Menu.Visibility = Visibility.Hidden;
                    Begin.Visibility = Visibility.Hidden;
                    About.Visibility = Visibility.Hidden;
                    MotherBoardImage.Visibility = Visibility.Visible;
                    ToolsPages.Visibility = Visibility.Visible;
                   // if (Multimeter.IsSelected)
                     //   Trace.WriteLine("1");
                    //else 
                    //{ 
                     //   Trace.WriteLine(Multimeter.IsSelected);
                    //}
                    break;
                case 2://часы не работают - не работает южный порт
                    //осцилограма не показывает//график не синусоидальный//Частота не 32768Гц
                    Menu.Visibility = Visibility.Hidden;
                    Begin.Visibility = Visibility.Hidden;
                    About.Visibility = Visibility.Hidden;
                    MotherBoardImage.Visibility = Visibility.Visible;
                    ToolsPages.Visibility = Visibility.Visible;



                    break;


            }
        }



    }
    public class MotherBoard
    {
        RTC rtc;
        MotherBoard(int branching)
        {
            Init(this, branching);

        }
    }


    public class RTC
    {
        int V;//напряжение
        int R;//сопротивление
        int A;//сила тока
    }


    //public class 
   
    public static class Tools //класс для метода инициализации
    {
        public static void Init(MotherBoard board,int branching)//инициализация MotherBoard в
        {
            switch (branching) 
            {
                case 1:



                    break;
                case 2:////часы не работают - не работает южный порт
                    //осцилограма не показывает//график не синусоидальный//Частота не 32768Гц



                    break;
            
            
            
            
            
            }
        }

     
    }


}
