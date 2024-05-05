
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Printing;
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
    using static System.Runtime.InteropServices.JavaScript.JSType;
    using static Tools;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random();
        MotherBoard AorusB450;
        List<Button> compare = new List<Button>();
        //массив выделенных объектов
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StopGenering(object sender, EventArgs e)
        {
            CompositionTarget.Rendering -= update;
            compare.Clear();
            VoltageText.Text = "0,000";
            AmperText.Text = "0,000";
            ResistText.Text = "0,000";
            GndButton.Opacity= 0;
            UsbButton.Opacity= 0;
            Plus.IsHitTestVisible = true;
            Minus.IsHitTestVisible = true;
            //можно трогатть чек боксы
        }

        private void update(object sender, EventArgs e)
        {
            if ((AorusB450 != null) &&(rand.Next(0,100)>95))
            {
                AorusB450.refresh(1);
                VoltageText.Text = AorusB450.usb.V;
                AmperText.Text = AorusB450.usb.A;
                ResistText.Text = AorusB450.usb.R;
                /*string roundedNumber = number.ToString("N3");*/
            }
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
                compare.Add(GndButton);
                if ((compare.Count == 2) && (compare[0] != compare[1])) 
                {
                    CompositionTarget.Rendering += update;
                    Plus.IsHitTestVisible = false;
                    Minus.IsHitTestVisible = false;
                }
                else if ((compare.Count == 2) && (compare[0] == compare[1])) 
                {
                    compare.RemoveAt(1);
                }

                Trace.WriteLine(compare.Count);
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
                compare.Add(UsbButton);
                if ((compare.Count == 2) && (compare[0] != compare[1]))
                {
                    CompositionTarget.Rendering += update;
                    Plus.IsHitTestVisible = false;
                    Minus.IsHitTestVisible = false;
                }
                else if ((compare.Count == 2) && (compare[0] == compare[1]))
                {
                    compare.RemoveAt(1);
                }
                Trace.WriteLine(compare.Count);
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
            switch (Number)
            {
                case 1://всё исправно
                    Menu.Visibility = Visibility.Hidden;
                    Begin.Visibility = Visibility.Hidden;
                    About.Visibility = Visibility.Hidden;
                    MotherBoardImage.Visibility = Visibility.Visible;
                    ToolsPages.Visibility = Visibility.Visible;
                    AorusB450 = new MotherBoard(1);


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
            private Random rnd;
            public Elem rtc;
            public Elem gnd;
            public Elem usb;

            public void refresh(int branching)
            {
                switch (branching)
                {                      //от 0.450мВ до 0.7мВ
                    case 1://всё исправно
                        usb.Fill(usb.V.Substring(0,3) + rnd.Next(10, 100), (""+((float)Convert.ToDouble(usb.V)* Math.Pow(10,3) / ((float)Convert.ToDouble(usb.A)*Math.Pow(10,3)))).Substring(0,4), "0," + 5);//v,r,a
                        Trace.WriteLine(usb.R);
                        /*Trace.WriteLine(((float)Convert.ToDouble(usb.V) * Math.Pow(10, -3) / (float)Convert.ToDouble(usb.A)));
                        Trace.WriteLine(usb.R);*/
                        break;
                    case 2:////часы не работают - не работает южный порт
                           //осцилограма не показывает//график не синусоидальный//Частота не 32768Гц
                        break;
                }

            }
            public MotherBoard(int branching)
            {
                rnd = new Random();
                rtc = new Elem();
                gnd = new Elem();
                usb = new Elem();
                switch (branching)
                {          //от 0.450мВ до 0.7мВ
                    case 1://всё исправно
                        usb.Fill("0,"+rnd.Next(4,7)+rnd.Next(10,100), "1", "0," + 5);//v,r,a
                        /*MainWindow.VoltageText*/
                        /*NumbersVoltage.IsChecked = false;*/
                        break;
                    case 2:////часы не работают - не работает южный порт
                           //осцилограма не показывает//график не синусоидальный//Частота не 32768Гц
                        break;
                }
            }


        }

    }

}

    public class Elem
    {
        public string V;//напряжение
        public string R;//сопротивление
        public string A;//сила тока
        public Elem()
        {
        V = ""; R=""; A="";
        }
        public void Fill(string v, string r, string a)
        {
            V = v;//напряжение
            R = r;//сопротивление
            A = a;//сила тока
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



