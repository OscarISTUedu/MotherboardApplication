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

namespace ЭВМ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender.ToString()== "System.Windows.Controls.Button: Поломка №1")
                Trace.WriteLine("Поломка №1");
            if (sender.ToString() == "System.Windows.Controls.Button: О приложении")
                Trace.WriteLine("О приложении");
            //Trace.WriteLine(sender.ToString());


        }


    }
}