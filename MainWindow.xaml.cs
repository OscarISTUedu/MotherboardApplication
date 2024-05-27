
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Media;
using System.Printing;
using System.Printing.Interop;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
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
        string[] brokes = { "Не исправен южный мост", "Неисправность RTC", "Неисправность микросхемы BIOS", "Короткое замыкание на линии 5V",
        "Короткое замыкание на линии 3.3V","Короткое замыкание на линии 12V","Неисправность слотов ОЗУ","Неисправность слотов PCI-E"};
        string broke;
        List<int> arr_of_brokes = new List<int>();
        int num;
        bool flag=true;
        int menu_num;


        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            string soundFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Kz_sound.wav"); // Замените на путь к вашему аудиофайлу
            soundPlayer = new SoundPlayer(soundFilePath);
            ToolsPages.Visibility = Visibility.Hidden;
            MotherBoardImage.Visibility = Visibility.Hidden;
            while (arr_of_brokes.Count!=8)
            {
                flag = true;
                num = rand.Next(1,9);
                for (int i = 0; i < arr_of_brokes.Count; i++)
                {
                    if (arr_of_brokes[i]== num)
                    {
                        flag = false;
                        break;
                    }                                                                                
                }
                if (flag) { arr_of_brokes.Add(num); }
            }
            Trace.WriteLine(arr_of_brokes[0]);
            Trace.WriteLine(arr_of_brokes[1]);
            Trace.WriteLine(arr_of_brokes[2]);
            Trace.WriteLine(arr_of_brokes[3]);
            Trace.WriteLine(arr_of_brokes[4]);
            Trace.WriteLine(arr_of_brokes[5]);
            Trace.WriteLine(arr_of_brokes[6]);
            Trace.WriteLine(arr_of_brokes[7]);
            int startmargine_left = -253;//генерация квадратиков для тестеров
            int startmargine_top = -363;
            int k = 0;
            for (int j = 0; j < 25; j++)
            {
                for (int i = 0; i < 18; i++)
                {
                    Rectangle myRectangle = new Rectangle
                    {
                        Width = 10,
                        Height = 10,
                        Stroke = Brushes.Black,
                        Fill = Brushes.Black,
                        StrokeThickness = 2
                    };
                    myRectangle.Margin = new Thickness(startmargine_left, startmargine_top, 0, 0);
                    string rectName = "Rect_memory" + k;
                    myRectangle.Name = rectName;
                    memory.RegisterName(rectName, myRectangle);
                    memory.Children.Add(myRectangle);
                    startmargine_left += 30;
                    k++;
                }
                startmargine_top += 30;
                startmargine_left = -253;
            }
            startmargine_left = -253;
            startmargine_top = -363;
            k = 0;
            for (int j = 0; j < 25; j++)
            {
                for (int i = 0; i < 18; i++)
                {
                    Rectangle myRectangle = new Rectangle
                    {
                        Width = 10,
                        Height = 10,
                        Stroke = Brushes.Black,
                        Fill = Brushes.Black,
                        StrokeThickness = 2
                    };
                    myRectangle.Margin = new Thickness(startmargine_left, startmargine_top, 0, 0);
                    string rectName = "Rect_pci" + k;
                    myRectangle.Name = rectName;
                    pci_e.RegisterName(rectName, myRectangle);
                    pci_e.Children.Add(myRectangle);
                    startmargine_left += 30;
                    k++;
                }
                startmargine_top += 30;
                startmargine_left = -253;
            }
        }

        private void StopGenering(object sender, EventArgs e)
        {
            CompositionTarget.Rendering -= update; 
            DownVoltageText.Text = "0,000";
            volt_text.Text = "Падение напряжение(мВ)";
            isGenering = false;
            Array.Clear(compare,0 ,compare.Length);
            GndButton.Opacity = 0;
            RtcButton.Opacity = 0;
            BIOSButton.Opacity = 0;
            GndUsb1_1.Opacity = 0;
            GndUsb1_2.Opacity = 0;
            Usb1_1.Opacity = 0;
            Usb1_2.Opacity = 0;
            Usb1_3.Opacity = 0;
            Usb1_4.Opacity = 0;
            V33_1.Opacity = 0;
            V5_1.Opacity = 0;
            V12_1.Opacity = 0;  
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
            MagnifierBrush10.Viewbox = viewboxRect;
            MagnifierBrush11.Viewbox = viewboxRect;
            MagnifierBrush12.Viewbox = viewboxRect;
            MagnifierBrush13.Viewbox = viewboxRect;
            MagnifierBrush14.Viewbox = viewboxRect;
            MagnifierBrush15.Viewbox = viewboxRect;
            MagnifierBrush16.Viewbox = viewboxRect;
            MagnifierBrush17.Viewbox = viewboxRect;
            MagnifierBrush18.Viewbox = viewboxRect;

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

            MagnifierCircle10.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle10.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle11.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle11.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle12.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle12.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle13.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle13.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle14.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle14.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle15.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle15.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle16.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle16.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle17.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle17.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);

            MagnifierCircle18.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle18.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);
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
                if (compare[0] == null|| compare[1] == null) { return; }
                if (compare[0].isGND || compare[1].isGND) //если мерим м/у gnd и элементом
                {
                    volt_text.Text = "Падение напряжение(мВ)";//было
                    if (((float)Convert.ToDouble(compare[1].V) - (float)Convert.ToDouble(compare[0].V)==0))
                        { DownVoltageText.Text = "0,000"; }
                    else
                    {
                        DownVoltageText.Text = "" + ((float)Convert.ToDouble(compare[1].V) - (float)Convert.ToDouble(compare[0].V));
                    }

                    if (((float)Convert.ToDouble(compare[0].V)>0)||((float)Convert.ToDouble(compare[1].V)>0))//если измеряем в вольтах
                    {
                        volt_text.Text= "Падение напряжение(В)";//стало
                    }
                }
                else 
                {
                    DownVoltageText.Text = "???";
                }
                AorusB450.refresh(current_mode);
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
                                case "3.3B":
                                    compare[0] = AorusB450.line_3_3B;
                                    break;
                                case "5B":
                                    compare[0] = AorusB450.line_5B;
                                    break;
                                case "12B":
                                    compare[0] = AorusB450.line_12B;
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
                                if ((compare[0].isRTC || compare[1].isRTC) && (compare[0].isGND || compare[1].isGND))//если там есть rtc
                                {
                                    if (current_mode == 2)
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
                                case "3.3B":
                                    compare[1] = AorusB450.line_3_3B;
                                    break;
                                case "5B":
                                    compare[1] = AorusB450.line_5B;
                                    break;
                                case "12B":
                                    compare[1] = AorusB450.line_12B;
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
                                case "3.3B":
                                    compare[0] = AorusB450.line_3_3B;
                                    break;
                                case "5B":
                                    compare[0] = AorusB450.line_5B;
                                    break;
                                case "12B":
                                    compare[0] = AorusB450.line_12B;
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
                                case "3.3B":
                                    compare[1] = AorusB450.line_3_3B;
                                    break;
                                case "5B":
                                    compare[1] = AorusB450.line_5B;
                                    break;
                                case "12B":
                                    compare[1] = AorusB450.line_12B;
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
        }

        /*private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop(); // Остановка звука по завершении воспроизведения
        }*/

        private void Menu_Click (object sender, RoutedEventArgs e)//меню выбора неисправности
        { 
            string Source = (string)((Button)e.OriginalSource).Content;
            string NumberString = Source.Substring(Source.Length-1);//образка названия кнопки, для выделения числа
            menu_num = int.Parse(NumberString);//преобразование в int
            current_mode = arr_of_brokes[menu_num-1];
            broke = brokes[current_mode - 1];
            current_page = "Осцилограф";
            AorusB450 = new MotherBoard(current_mode);
            lose.Visibility = Visibility.Hidden;
            answer.Text = answer.Text.Substring(0,23);
            answer.Visibility = Visibility.Hidden;
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
                        line_3_3B.Fill("3,3000" + rnd.Next(1, 10));
                        line_5B.Fill("5,0000" + rnd.Next(1, 10));
                        line_12B.Fill("12,0000" + rnd.Next(1, 10));
                        break;
                    case 2://rtc не работает//график не синусоидальный
                        usb.Fill(usb.V.Substring(0, 3) + rnd.Next(10, 100));
                        line_3_3B.Fill("3,3000" + rnd.Next(1, 10));
                        line_5B.Fill("5,0000" + rnd.Next(1, 10));
                        line_12B.Fill("12,0000" + rnd.Next(1, 10));
                        break;
                    case 3://bios//график не синусоидальный
                        usb.Fill(usb.V.Substring(0, 3) + rnd.Next(10, 100));
                        line_3_3B.Fill("3,3000" + rnd.Next(1, 10));
                        line_5B.Fill("5,0000" + rnd.Next(1, 10));
                        line_12B.Fill("12,0000" + rnd.Next(1, 10));
                        break;
                    case 4://5V кз
                        usb.Fill(usb.V.Substring(0, 3) + rnd.Next(10, 100));
                        line_3_3B.Fill("3,3000" + rnd.Next(1, 10));
                        line_12B.Fill("12,0000" + rnd.Next(1, 10));
                        break;
                    case 5://3.3V кз
                        usb.Fill(usb.V.Substring(0, 3) + rnd.Next(10, 100));
                        line_5B.Fill("5,0000" + rnd.Next(1, 10));
                        line_12B.Fill("12,0000" + rnd.Next(1, 10));
                        break;
                    case 6://12V кз
                        usb.Fill(usb.V.Substring(0, 3) + rnd.Next(10, 100));
                        line_3_3B.Fill("3,3000" + rnd.Next(1, 10));
                        line_5B.Fill("5,0000" + rnd.Next(1, 10));
                        break;
                    case 7://ОЗУ не работает
                        usb.Fill(usb.V.Substring(0, 3) + rnd.Next(10, 100));
                        line_3_3B.Fill("3,3000" + rnd.Next(1, 10));
                        line_5B.Fill("5,0000" + rnd.Next(1, 10));
                        line_12B.Fill("12,0000" + rnd.Next(1, 10));
                        break;
                    case 8://pci e тестер
                        usb.Fill(usb.V.Substring(0, 3) + rnd.Next(10, 100));
                        line_3_3B.Fill("3,3000" + rnd.Next(1, 10));
                        line_5B.Fill("5,0000" + rnd.Next(1, 10));
                        line_12B.Fill("12,0000" + rnd.Next(1, 10));
                        break;
                }


            }
            public MotherBoard(int branching)
            {
                //линии питания +12В,+3.3В,+5В
                //in,out bios
                rnd = new Random();

                usb = new Elem();
                gnd = new Elem(true);
                bios= new Elem(false,true);
                rtc = new Elem(false,false,true);
                line_12B = new Elem();
                line_3_3B = new Elem();
                line_5B = new Elem();

                usb.Fill("0," + rnd.Next(4, 7) + rnd.Next(10, 100));
                line_12B.Fill("12,0000" + rnd.Next(1, 10));
                line_3_3B.Fill("3,3000" + rnd.Next(1, 10));
                line_5B.Fill("5,0000" + rnd.Next(1, 10));
                gnd.Fill("0,000");
                rtc.Fill("0,000");
                bios.Fill("0,000");
                switch (branching)
                {          //норма от 0.450мВ до 0.7мВ
                    case 1://usb сломан
                        usb.Fill("0,9"+rnd.Next(10,100));
                        break;
                    case 2://rtc не работает//график не синусоидальный
                        break;
                    case 3://bios//график не синусоидальный
                        break;
                    case 4://5V кз
                        line_5B.isGND = true;
                        line_5B.Fill("0");
                        break;
                    case 5://3.3V кз
                        line_3_3B.isGND = true;
                        line_3_3B.Fill("0");
                        break;
                    case 6://12V кз
                        line_12B.isGND = true;
                        line_12B.Fill("0");
                        break;
                    case 7://ОЗУ не работает
                        break;
                    case 8://pci e тестер
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
                DownVoltageText.Text = "0,000";
                volt_text.Text = "Падение напряжение(мВ)";
                isGenering = false;
                Array.Clear(compare, 0, compare.Length);
                GndButton.Opacity = 0;
                RtcButton.Opacity = 0;
                BIOSButton.Opacity = 0;
                GndUsb1_1.Opacity = 0;
                GndUsb1_2.Opacity = 0;
                Usb1_1.Opacity = 0;
                Usb1_2.Opacity = 0;
                Usb1_3.Opacity = 0;
                Usb1_4.Opacity = 0;
                V33_1.Opacity = 0;
                V5_1.Opacity = 0;
                V12_1.Opacity = 0;
                Good_synk.Visibility = Visibility.Hidden;
                Bad_synk.Visibility = Visibility.Hidden;
                IsKz.Fill = Brushes.LightBlue;
                Plus.IsHitTestVisible = true;
                Minus.IsHitTestVisible = true;
                Plus_o.IsHitTestVisible = true;
                Minus_o.IsHitTestVisible = true;
                Minus_o.IsChecked = false;
                Plus_o.IsChecked = false;
                Minus.IsChecked = false;
                Plus.IsChecked = false;


                current_page = $"{selectedTab.Header}";
                MEM1.Opacity = 0;
                MEM1.IsEnabled = true;
                MEM2.Opacity = 0;
                MEM2.IsEnabled = true;
                MEM3.Opacity = 0;
                MEM3.IsEnabled = true;
                MEM4.Opacity = 0;
                MEM4.IsEnabled = true;
                PCI_E1.Opacity = 0;
                PCI_E1.IsEnabled = true;
                PCI_E2.Opacity = 0;
                PCI_E2.IsEnabled = true;


                for (int j = 0; j < 407; j++)
                {
                    Rectangle Rect_memory = (Rectangle)FindName("Rect_memory" + j);
                    Rectangle Rect_pci = (Rectangle)FindName("Rect_pci" + j);
                    Rect_memory.Fill = Brushes.Black;
                    Rect_pci.Fill = Brushes.Black;
                }

            }
        }

        private void MEM_Click(object sender, RoutedEventArgs e)
        {
            if (current_page == "Другое")
            {
                rand = new Random();
                int k = 0;
                int pos = 0;
                Button button = sender as Button;
                if (button == null) { return; }
                button.Opacity = 1;
                button.IsEnabled = false;
                for (int j=0; j<407;j++)
                {
                    Rectangle rectangle = (Rectangle)FindName("Rect_memory" + j);
                    rectangle.Fill = Brushes.Black;
                }
                if (current_mode == 7)
                {

                    k = rand.Next(2, 200);
                    for (int i = 0; i < k; i++)
                    {
                        pos = rand.Next(409);
                        Rectangle rectangle = (Rectangle)FindName("Rect_memory" + pos);
                        rectangle.Fill = Brushes.Red;
                    }
                }
            }
        }

        private void PCI_E1_Click(object sender, RoutedEventArgs e)
        {
            if (current_page == "Другое")
            {
                rand = new Random();
                int k = 0;
                int pos = 0;
                Button button = sender as Button;
                if (button == null) { return; }
                button.Opacity = 1;
                button.IsEnabled = false;
                for (int j = 0; j < 407; j++)
                {
                    Rectangle rectangle = (Rectangle)FindName("Rect_pci" + j);
                    rectangle.Fill = Brushes.Black;
                }
                if (current_mode == 8)
                {

                    k = rand.Next(2, 200);
                    for (int i = 0; i < k; i++)
                    {
                        pos = rand.Next(409);
                        Rectangle rectangle = (Rectangle)FindName("Rect_pci" + pos);
                        rectangle.Fill = Brushes.Red;
                    }
                }
            }
        }

        private void Button_confirm(object sender, RoutedEventArgs e)
        {
            if (results.SelectedIndex!=-1)
            {
                if (results.SelectedIndex+1==current_mode)
                {
                    win.Visibility = Visibility.Visible;
                    Button item = (Button)Menu.Items[menu_num-1];
                    item.Content = broke;
                    item.IsEnabled = false;
                }
                else 
                {
                    lose.Visibility= Visibility.Visible;
                    answer.Text += broke;
                    answer.Visibility = Visibility.Visible;
                }
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            change_playground(false);
            GndButton.Opacity = 0;
            RtcButton.Opacity = 0;
            BIOSButton.Opacity = 0;
            GndUsb1_1.Opacity = 0;
            GndUsb1_2.Opacity = 0;
            Usb1_1.Opacity = 0;
            Usb1_2.Opacity = 0;
            Usb1_3.Opacity = 0;
            Usb1_4.Opacity = 0;
            V33_1.Opacity = 0;
            V5_1.Opacity = 0;
            V12_1.Opacity = 0;
            MagnifierPanel.Visibility = Visibility.Hidden;
            Good_synk.Visibility = Visibility.Hidden;
            Bad_synk.Visibility = Visibility.Hidden;
            results.SelectedIndex = -1;  
            isGenering = false;
            isZoomed = false;
            IsKz.Fill = Brushes.LightBlue;
            Zoomer.Background = Brushes.LightGray;
            Zoomer_o.Background = Brushes.LightGray;
            Begin.Visibility = Visibility.Visible;
            About.Visibility = Visibility.Visible;
            ToolsPages.SelectedIndex= 0;
            win.Visibility = Visibility.Hidden;
        }
    }



}

public class Elem
    {
        public bool isGND=false;
        public bool isBIOS=false;
        public bool isRTC=false;
        public string V;//напряжение

        public Elem(bool GND)
        {
        V = ""; isGND = GND;
        }

        public Elem(bool GND, bool bios)
        {
        V = ""; isGND = GND; isBIOS= bios;
        }
        public Elem(bool GND, bool bios,bool rtc)
        {
        V = ""; isGND = GND; isBIOS = bios; isRTC = rtc;
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






