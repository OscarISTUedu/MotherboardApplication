
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
        List<Button> compare = new List<Button>();
        //массив выделенных объектов

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


        private void GndClick(object sender, RoutedEventArgs e)//обработка всех областей
        {

            if ((bool)Minus.IsChecked  /*и массив не пустой*/ )//обработка выделения GND
            {
                GndButton.Opacity = 1;
                Minus.IsChecked = false;
                /*compare.Add(GndButton);*/
                //добавил в массив объект
                //запуски функции 
            }
        }

        private void UsbClick(object sender, RoutedEventArgs e)//обработка всех областей
        {

            if ((bool)Plus.IsChecked)//обработка выделения GND
            {
                UsbButton.Opacity = 1;
                Plus.IsChecked = false;
                compare.Add(GndButton);
                /*sender.*/
            }
        }

        public void Tab_Click(object sender, RoutedEventArgs e)
        {
            string Source = "";
            try 
            {
            Source = (string)((TextBlock)e.OriginalSource).Text;
            }
            catch (Exception ex)
            {
                return;
            }
            if (Source == "Мультиметр")
            {

            }
            else if (Source == "Осцилограф")
            {

            }

            Trace.WriteLine(Source);

            }


        private void Menu_Click (object sender, RoutedEventArgs e)//меню выбора неисправности
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
                    


                    /*  if (Multimeter.IsSelected)
                        Trace.WriteLine("1");*/
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

        /*public string Test ()
        {
            return "213";

        }*/


        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            Minus.IsChecked = false;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            Plus.IsChecked = false;
        }
    public class MotherBoard
    {
        RTC rtc;
        GND gnd;
        USB usb;
        MotherBoard(int branching)
        {
            rtc= new RTC();
            gnd = new GND();
            usb = new USB();
            switch (branching)
            {                      //от 0.450мВ до 0.7мВ
                case 1://кз по USB
                    usb.Fill(NextFloat(0.45f,0.7f), NextFloat(10f,8f), NextFloat(0.3f,0.9f));//v,r,a
                    /*MainWindow.VoltageText*/
                            /*VoltageText.Text = "1,23";*/

                        /*NumbersVoltage.IsChecked = false;*/

                        break;
                case 2:////часы не работают - не работает южный порт
                    //осцилограма не показывает//график не синусоидальный//Частота не 32768Гц



                    break;







            }

        }
    }

    }


    public class RTC
    {
        float V;//напряжение
        float R;//сопротивление
        float A;//сила тока
    }

    public class USB
    {
        float V;//напряжение
        float R;//сопротивление
        float A;//сила тока
        public USB()
        {
            V = 0;//напряжение
            R = 0;//сопротивление
            A = 0;//сила тока
        }
        public void Fill(float v, float r, float a)
        {
            V = v;//напряжение
            R = r;//сопротивление
            A = a;//сила тока
        }


    }
    public class GND
    {
        float V;//напряжение
        float R;//сопротивление
        float A;//сила тока
        public GND()
        {
            V = 0;//напряжение
            R = 0;//сопротивление
            A = 0;//сила тока
        }
    }

    public static class Tools //класс для метода инициализации
    {
        public static float NextFloat(float min, float max)
        {
            Random random = new Random();
            double range;
            double sample;
            double scaled;
            range  = (double)max - (double)min;
            sample = random.NextDouble();
            scaled = (sample * range) + min;
            return (float)scaled;
        }
    }


}
