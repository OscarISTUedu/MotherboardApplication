
using System;
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
    using static Tools;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random();
        MotherBoard AorusB450;
        Elem [] compare = new Elem[2];
        private double _factor = 0.5;//масштаб
        bool isZoomed = false;
        bool isGenering = false;
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
        }

        private void StopGenering(object sender, EventArgs e)
        {
            CompositionTarget.Rendering -= update;
            isGenering = false;
            Array.Clear(compare,0 ,compare.Length);
            DownVoltageText.Text = "0,000";
            GndButton.Opacity= 0;
            RtcButton.Opacity= 0;
            GndUsb1_1.Opacity= 0;
            GndUsb1_2.Opacity= 0;
            Usb1_1.Opacity= 0;
            Usb1_2.Opacity= 0;
            Usb1_3.Opacity= 0;
            Usb1_4.Opacity= 0;
            Plus.IsHitTestVisible = true;
            Minus.IsHitTestVisible = true;
        }

        private void Zoom(object sender, RoutedEventArgs e)
        {
            if ((Plus.IsChecked == false)&&(Minus.IsChecked == false)&&(GetNumOfElem(compare)==0))
            {
                if (isZoomed) 
                {
                    Zoomer.Background = Brushes.LightGray;
                }
                else { Zoomer.Background = Brushes.DarkGray; }
                isZoomed=!isZoomed;
            }
            /*if (isZoomed) MagnifierPanel.Visibility= Visibility.Visible;*/
        }

        private void ContentPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isZoomed)
                return;
            Point center = e.GetPosition(MotherBoardImage);
            double length = MagnifierCircle.ActualWidth * _factor;
            double radius = length / 2;
            Rect viewboxRect = new Rect(center.X - radius, center.Y - radius, length, length);
            MagnifierBrush.Viewbox = viewboxRect;
            MagnifierCircle.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);
        }

        private void ContentPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            string source;
            if (!isZoomed)
                return;
            if (isZoomed) MagnifierPanel.Visibility = Visibility.Visible;
        }

        private void ContentPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            string source;
            if (!isZoomed)
                return;
            MagnifierPanel.Visibility = Visibility.Hidden;
        }


        private void update(object sender, EventArgs e)
        {
            if ((AorusB450 != null) &&(rand.Next(0,100)>95)) //compare[0] - это минус,compare[1] - плюс.От плюса отнимаем минус compare[1]-compare[0]
            {
                isGenering = true;
                if (compare[0].isGND || compare[1].isGND) //если мерим м/у gnd и элементом
                {
                    DownVoltageText.Text = "" + ((float)Convert.ToDouble(compare[1].V) - (float)Convert.ToDouble(compare[0].V));
                }
                else 
                {
                    DownVoltageText.Text = "???";
                }
                AorusB450.refresh(1);
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

        private void Rtc_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)Minus.IsChecked)//обработка выделения GND. compare[0] - это минус,compare[1] - плюс
            {
                isZoomed = false;
                Zoomer.Background = Brushes.LightGray;
                MagnifierPanel.Visibility = Visibility.Hidden;
                RtcButton.Opacity = 1;
                Minus.IsChecked = false;
                Plus.IsChecked = false;
                compare[0] = AorusB450.rtc;
                if (GetNumOfElem(compare) == 2)
                {
                    if (compare[0] != compare[1])
                    {
                        CompositionTarget.Rendering += update;
                        Plus.IsHitTestVisible = false;
                        Minus.IsHitTestVisible = false;
                        return;
                    }
                    Array.Clear(compare, 1, compare.Length);
                }
            }
            else if ((bool)Plus.IsChecked)//compare[0] - это минус,compare[1] - плюс
            {
                isZoomed = false;
                Zoomer.Background = Brushes.LightGray;
                MagnifierPanel.Visibility = Visibility.Hidden;
                RtcButton.Opacity = 1;
                Minus.IsChecked = false;
                Plus.IsChecked = false;
                compare[1] = AorusB450.rtc;
                if (GetNumOfElem(compare) == 2)
                {
                    if (compare[0] != compare[1])
                    {
                        CompositionTarget.Rendering += update;
                        Plus.IsHitTestVisible = false;
                        Minus.IsHitTestVisible = false;
                        return;
                    }
                    Array.Clear(compare, 1, compare.Length);
                }
            }
        }


        private void Anyclick(object sender, RoutedEventArgs e)
        {
            if ((bool)Minus.IsChecked)//обработка выделения GND. compare[0] - это минус,compare[1] - плюс
            {
                Button button = sender as Button;
                if (button!= null)
                {
                    try
                    {
                        string text = button.Content.ToString(); 
                        button.Opacity= 1;
                        isZoomed = false;
                        Zoomer.Background = Brushes.LightGray;
                        MagnifierPanel.Visibility = Visibility.Hidden;
                        Minus.IsChecked = false;
                        Plus.IsChecked = false;
                        switch (text)
                        {
                            case "GND":
                                compare[0] = AorusB450.gnd;
                                break;
                            case "RTC":
                                compare[0] = AorusB450.rtc;
                                break;
                            case "USB":
                                compare[0] =AorusB450.usb;
                                break;
                        }
                        if (GetNumOfElem(compare) == 2)
                        {
                            if (compare[0] != compare[1])
                            {
                                CompositionTarget.Rendering += update;
                                Plus.IsHitTestVisible = false;
                                Minus.IsHitTestVisible = false;
                                Trace.WriteLine(GetNumOfElem(compare));
                                return;
                            }
                            Array.Clear(compare, 0, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
            else if ((bool)Plus.IsChecked)//compare[0] - это минус,compare[1] - плюс
            {
                Button button = sender as Button;
                if (button != null)
                {
                    try
                    {
                        string text = button.Content.ToString();
                        button.Opacity = 1;
                        isZoomed = false;
                        Zoomer.Background = Brushes.LightGray;
                        MagnifierPanel.Visibility = Visibility.Hidden;
                        Minus.IsChecked = false;
                        Plus.IsChecked = false;
                        Trace.WriteLine("before");
                        Trace.WriteLine(GetNumOfElem(compare));
                        switch (text)
                        {
                            case "GND":
                                compare[1] = AorusB450.gnd;
                                break;
                            case "RTC":
                                compare[1] = AorusB450.rtc;
                                break;
                            case "USB":
                                compare[1] = AorusB450.usb;
                                break;
                        }
                        Trace.WriteLine("after");
                        Trace.WriteLine(GetNumOfElem(compare));
                        if (GetNumOfElem(compare) == 2)
                        {
                            if (compare[0] != compare[1])
                            {
                                CompositionTarget.Rendering += update;
                                Plus.IsHitTestVisible = false;
                                Minus.IsHitTestVisible = false;
                                return;
                            }
                            Array.Clear(compare, 1, compare.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
        }




        private void GndClick(object sender, RoutedEventArgs e)
        {
            
            if ((bool)Minus.IsChecked)//обработка выделения GND. compare[0] - это минус,compare[1] - плюс
            {
                isZoomed = false;
                Zoomer.Background = Brushes.LightGray;
                MagnifierPanel.Visibility = Visibility.Hidden;
                GndButton.Opacity = 1;
                Minus.IsChecked = false;
                Plus.IsChecked= false;
                compare[0]=AorusB450.gnd;   
                if (GetNumOfElem(compare) == 2) 
                {
                    if (compare[0] != compare[1])
                    {
                        CompositionTarget.Rendering += update;
                        Plus.IsHitTestVisible = false;
                        Minus.IsHitTestVisible = false;
                        return;
                    }
                    Array.Clear(compare, 1, compare.Length);
                }
            }
            else if ((bool)Plus.IsChecked)//compare[0] - это минус,compare[1] - плюс
            {
                isZoomed = false;
                Zoomer.Background = Brushes.LightGray;
                MagnifierPanel.Visibility = Visibility.Hidden;
                GndButton.Opacity = 1;
                Minus.IsChecked = false;
                Plus.IsChecked = false;
                compare[1] = AorusB450.gnd;
                if (GetNumOfElem(compare) == 2)
                {
                    if (compare[0] != compare[1])
                    {
                        CompositionTarget.Rendering += update;
                        Plus.IsHitTestVisible = false;
                        Minus.IsHitTestVisible = false;
                        return;
                    }
                    Array.Clear(compare, 1, compare.Length);
                }
            }
        }

        private void UsbClick(object sender, RoutedEventArgs e)//compare[0] - это минус,compare[1] - плюс
        {

            if ((bool)Plus.IsChecked)//compare[0] - это минус,compare[1] - плюс
            {
                isZoomed = false;
                Zoomer.Background = Brushes.LightGray;
                MagnifierPanel.Visibility = Visibility.Hidden;
                /*UsbButton.Opacity = 1;*/
                Minus.IsChecked = false;
                Plus.IsChecked = false;
                compare[1] = AorusB450.usb;
                if (GetNumOfElem(compare) == 2) 
                {
                    if (compare[0] != compare[1])
                    {
                        CompositionTarget.Rendering += update;
                        Plus.IsHitTestVisible = false;
                        Minus.IsHitTestVisible = false;
                        return;
                    }
                    Array.Clear(compare, 1, compare.Length);
                }
            }
            else if ((bool)Minus.IsChecked)//compare[0] - это минус,compare[1] - плюс
            {
                isZoomed = false;
                Zoomer.Background = Brushes.LightGray;
                MagnifierPanel.Visibility = Visibility.Hidden;
                /*UsbButton.Opacity = 1;*/
                Minus.IsChecked = false;
                Plus.IsChecked = false;
                compare[0] = AorusB450.usb;
                if (GetNumOfElem(compare) == 2)
                {
                    if (compare[0] != compare[1])
                    {
                        CompositionTarget.Rendering += update;
                        Plus.IsHitTestVisible = false;
                        Minus.IsHitTestVisible = false;
                        return;
                    }
                    Array.Clear(compare, 1, compare.Length);
                }
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
            isZoomed = false;
            Zoomer.Background = Brushes.LightGray;
            MagnifierPanel.Visibility = Visibility.Hidden;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
           Plus.IsChecked = false;
            isZoomed = false;
            Zoomer.Background = Brushes.LightGray;
            MagnifierPanel.Visibility = Visibility.Hidden;
        }


        public class MotherBoard
        {
            public int W;//мощность блока питания
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
                        gnd.Fill("0", "0", "0");
                        rtc.Fill("0", "0", "0");
                        /*Trace.WriteLine(usb.R);*/
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
                //линии питания +12В,+3.3В,+5В
                //in,out bios
                rnd = new Random();

                rtc = new Elem();
                usb = new Elem();
                gnd = new Elem(true);
                switch (branching)
                {          //от 0.450мВ до 0.7мВ
                    case 1://всё исправно
                        usb.Fill("0,"+rnd.Next(4,7)+rnd.Next(10,100), "1", "0," + 5);//v,r,a
                        gnd.Fill("0", "0", "0");
                        rtc.Fill("0", "0", "0");
                        /*rtc.Fill();*/
                        break;
                    case 2:////часы не работают - не работает южный порт
                           //осцилограма не показывает//график не синусоидальный//Частота не 32768Гц
                        break;
                }
            }

        }

        private void RtcButton_Click(object sender, RoutedEventArgs e)
        {

        }


    }


}

public class Elem
    {
        public bool isGND=false;
        public string V;//напряжение
        public string R;//сопротивление
        public string A;//сила тока
        public Elem(bool GND)
        {
        V = ""; R=""; A=""; isGND = GND;
        }

        public Elem()
        {
            V = ""; R = ""; A = "";
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

        public static int GetNumOfElem(Elem[] arr)
        {
        int k = 0;
            for (int i = 0; i < arr.Length;i++)
            {
            if (arr[i] != null) k++;
            }
        return k;
        }

        /*public static */
    }






