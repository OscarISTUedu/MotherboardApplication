
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Media;
using System.Printing;
using System.Printing.Interop;
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
using System.Xml.Linq;
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
        private SoundPlayer soundPlayer;
        string current_page = "Осцилограф";
        int current_mode = 0;
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            string soundFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Kz_sound.wav"); // Замените на путь к вашему аудиофайлу
            soundPlayer = new SoundPlayer(soundFilePath);
            ToolsPages.Visibility = Visibility.Hidden;
            MotherBoardImage.Visibility = Visibility.Hidden;
            // Инициализируем SoundPlayer с путем к аудиофайлу
        }

        private void StopGenering(object sender, EventArgs e)
        {
            Trace.WriteLine("len.com");
            Trace.WriteLine(GetNumOfElem(compare));
            
            CompositionTarget.Rendering -= update; 
            DownVoltageText.Text = "0,000";
            //CompositionTarget.Rendering -= update;
            isGenering = false;
            Array.Clear(compare,0 ,compare.Length);
            //DownVoltageText.Text = "0,000";
            /*elements.Opacity = 0;*/
            GndButton.Opacity = 0;
            RtcButton.Opacity = 0;
            BIOSButton.Opacity = 0;
            GndUsb1_1.Opacity = 0;
            GndUsb1_2.Opacity = 0;
            Usb1_1.Opacity = 0;
            Usb1_2.Opacity = 0;
            Usb1_3.Opacity = 0;
            Usb1_4.Opacity = 0;
            Good_synk.Visibility = Visibility.Hidden;   
            Bad_synk.Visibility = Visibility.Hidden;   
            IsKz.Fill = Brushes.LightBlue;
            Plus.IsHitTestVisible = true;
            Minus.IsHitTestVisible = true;
            Plus_o.IsHitTestVisible = true;
            Minus_o.IsHitTestVisible = true;
        }

        private void Zoom(object sender, RoutedEventArgs e)
        {
                if (isZoomed) 
                {
                    Zoomer.Background = Brushes.LightGray;
                    Zoomer_o.Background = Brushes.LightGray;
                }
                else { Zoomer.Background = Brushes.DarkGray; Zoomer_o.Background = Brushes.DarkGray; }
                isZoomed=!isZoomed;
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
            MagnifierBrush1.Viewbox = viewboxRect;
            MagnifierBrush2.Viewbox = viewboxRect;
            MagnifierBrush3.Viewbox = viewboxRect;
            MagnifierBrush4.Viewbox = viewboxRect;
            MagnifierBrush5.Viewbox = viewboxRect;
            MagnifierBrush6.Viewbox = viewboxRect;
            MagnifierBrush7.Viewbox = viewboxRect;
            MagnifierBrush8.Viewbox = viewboxRect;
            MagnifierBrush9.Viewbox = viewboxRect;
            MagnifierCircle.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle1.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle1.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle2.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle2.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle3.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle3.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle4.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle4.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle5.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle5.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle6.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle6.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle7.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle7.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle8.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle8.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle9.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle9.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);
        }

        private void ContentPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isZoomed)
                return;
            if (isZoomed) MagnifierPanel.Visibility = Visibility.Visible;
        }

        private void ContentPanel_MouseLeave(object sender, MouseEventArgs e)
        {
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


        private void Anyclick(object sender, RoutedEventArgs e)
        {
            if (current_page == "Осцилограф")
            {
                if ((bool)Minus_o.IsChecked)//обработка выделения GND. compare[0] - это минус,compare[1] - плюс
                {
                    Button button = sender as Button;
                    if (button != null)
                    {
                        try
                        {
                            string text = button.Content.ToString();
                            button.Opacity = 1;
                            Minus_o.IsChecked = false;
                            Plus_o.IsChecked = false;
                            switch (text)
                            {
                                case "BIOS":
                                    compare[0] = AorusB450.bios;
                                    break;
                                case "GND":
                                    compare[0] = AorusB450.gnd;
                                    break;
                                case "RTC":
                                    compare[0] = AorusB450.rtc;
                                    break;
                                case "USB":
                                    compare[0] = AorusB450.usb;
                                    break;
                                
                            }
                            if (GetNumOfElem(compare) == 2)
                            {
                                //логика осцилографа
                                if ((compare[0].isBIOS || compare[1].isBIOS)&&(compare[0].isGND || compare[1].isGND))//если там есть биос
                                {
                                    if (current_mode==3)
                                    {
                                        Bad_synk.Visibility = Visibility.Visible;
                                    }
                                    else { Good_synk.Visibility = Visibility.Visible; }
                                }
                                Plus_o.IsHitTestVisible = false;
                                Minus_o.IsHitTestVisible = false;
                            }
                            else { Minus_o.IsHitTestVisible = false; }
                        }
                        catch (Exception ex)
                        {
                            return;
                        }
                    }
                }
                else if ((bool)Plus_o.IsChecked)//compare[0] - это минус,compare[1] - плюс
                {
                    Button button = sender as Button;
                    if (button != null)
                    {
                        try
                        {
                            string text = button.Content.ToString();
                            button.Opacity = 1;
                            Minus_o.IsChecked = false;
                            Plus_o.IsChecked = false;
                            switch (text)
                            {
                                case "BIOS":
                                    compare[1] = AorusB450.bios;
                                    break;
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
                            if (GetNumOfElem(compare) == 2)
                            {
                                //логика осцилографа
                                if ((compare[0].isBIOS || compare[1].isBIOS) && (compare[0].isGND || compare[1].isGND))//если там есть биос
                                {
                                    if (current_mode == 3)
                                    {
                                        Bad_synk.Visibility = Visibility.Visible;
                                    }
                                    else { Good_synk.Visibility = Visibility.Visible; }
                                }
                                Plus_o.IsHitTestVisible = false;
                                Minus_o.IsHitTestVisible = false;
                            }
                            else { Plus_o.IsHitTestVisible = false; }
                        }
                        catch (Exception ex)
                        {
                            return;
                        }
                    }
                }
            }  
            if (current_page== "Вольтметр")
            {
                if ((bool)Minus.IsChecked)//обработка выделения GND. compare[0] - это минус,compare[1] - плюс
                {
                    Button button = sender as Button;
                    if (button != null)
                    {
                        try
                        {
                            string text = button.Content.ToString();
                            button.Opacity = 1;
                            Minus.IsChecked = false;
                            Plus.IsChecked = false;
                            switch (text)
                            {
                                case "BIOS":
                                    compare[0] = AorusB450.bios;
                                    break;
                                case "GND":
                                    compare[0] = AorusB450.gnd;
                                    break;
                                case "RTC":
                                    compare[0] = AorusB450.rtc;
                                    break;
                                case "USB":
                                    compare[0] = AorusB450.usb;
                                    break;
                            }
                            if (GetNumOfElem(compare) == 2)
                            {
                                CompositionTarget.Rendering += update;
                                if ((compare[0].isGND) && (compare[1].isGND)) { IsKz.Fill = Brushes.Red; }
                                Plus.IsHitTestVisible = false;
                                Minus.IsHitTestVisible = false;
                            }
                            else { Minus.IsHitTestVisible = false; }
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
                            Minus.IsChecked = false;
                            Plus.IsChecked = false;
                            switch (text)
                            {
                                case "BIOS":
                                    compare[1] = AorusB450.bios;
                                    break;
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
                            if (GetNumOfElem(compare) == 2)
                            {
                                CompositionTarget.Rendering += update;
                                if ((compare[0].isGND) && (compare[1].isGND)) { IsKz.Fill = Brushes.Red; }
                                Plus.IsHitTestVisible = false;
                                Minus.IsHitTestVisible = false;
                            }
                            else { Plus.IsHitTestVisible = false; }
                        }
                        catch (Exception ex)
                        {
                            return;
                        }
                    }
                }

            }
            else {
                Trace.WriteLine("----------");
                Trace.WriteLine(current_page);
            }
            
        }

        /*private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop(); // Остановка звука по завершении воспроизведения
        }*/



        private void Menu_Click (object sender, RoutedEventArgs e)//меню выбора неисправности
        { 
            string Source = (string)((Button)e.OriginalSource).Content;
            string NumberString = Source.Substring(Source.Length-1);//образка названия кнопки, для выделения числа
            int Number = int.Parse(NumberString);//преобразование в int
            current_mode = Number;
            AorusB450 = new MotherBoard(current_mode);
            Menu.Visibility = Visibility.Hidden;
            Begin.Visibility = Visibility.Hidden;
            About.Visibility = Visibility.Hidden;
            MotherBoardImage.Visibility = Visibility.Visible;
            ToolsPages.Visibility = Visibility.Visible;
        }



        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            Minus.IsChecked = false;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
           Plus.IsChecked = false;
        }

        private void Plus_o_Click(object sender, RoutedEventArgs e)
        {
            Minus_o.IsChecked = false;

        }

        private void Minus_o_Click(object sender, RoutedEventArgs e)
        {
            Plus_o.IsChecked = false;
        }
        public class MotherBoard
        {
            public int W;//мощность блока питания
            private Random rnd;
            public Elem rtc;
            public Elem gnd;
            public Elem usb;
            public Elem line_5B;
            public Elem line_12B;
            public Elem line_3_3B;  
            public Elem bios;
            public void refresh(int branching)
            {
                switch (branching)
                {                      //норма от 0.450мВ до 0.7мВ
                    case 1://usb сломан
                        usb.Fill("0,9" + rnd.Next(10, 100));
                        break;
                    case 2:////часы не работают - не работает южный порт
                           //осцилограма не показывает//график не синусоидальный//Частота не 32768Гц
                        break;
                }
                /*Trace.WriteLine("branching:");
                Trace.WriteLine(branching);*/

            }
            public MotherBoard(int branching)
            {
                //линии питания +12В,+3.3В,+5В
                //in,out bios
                rnd = new Random();

                rtc = new Elem();
                usb = new Elem();
                gnd = new Elem(true);
                bios= new Elem(false,true);

                usb.Fill("0");
                gnd.Fill("0");
                rtc.Fill("0");
                bios.Fill("0");
                switch (branching)
                {          //норма от 0.450мВ до 0.7мВ
                    case 1://usb сломан
                        usb.Fill("0,9"+rnd.Next(10,100));
                        break;
                    case 2:////часы не работают - не работает южный порт
                           //осцилограма не показывает//график не синусоидальный//Частота не 32768Гц
                        break;
                }
            }

        }

        public void change_playground (bool isVisible)
        {
            if (isVisible) 
            {
                MotherBoardImage.Visibility = Visibility.Visible;
                ToolsPages.Visibility = Visibility.Visible;
            }
            else {
                MotherBoardImage.Visibility = Visibility.Hidden;
                ToolsPages.Visibility = Visibility.Hidden;
            }
        }

        private void ToolsPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToolsPages.SelectedItem is TabItem selectedTab)
            {
                current_page = $"{selectedTab.Header}";
            }
        }
    }



}

public class Elem
    {
        public bool isGND=false;
        public bool isBIOS=false;
        public string V;//напряжение

        public Elem(bool GND)
        {
        V = ""; isGND = GND;
        }

        public Elem(bool GND, bool bios)
        {
        V = ""; isGND = GND; isBIOS= bios;
        }

    public Elem()
        {
            V = "";
        }
        public void Fill(string v)
        {
            V = v;//напряжение
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

    }






