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

namespace NetworkTester
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
            Button source = (Button)sender;
            switch(source.Name)
            {
                case "StartButton":
                    if (ServerBox.Text != null && !ServerBox.Text.Equals(""))
                        new PingTest(ServerBox.Text, 0, 0);
                    else
                        new PingTest(HintText.Text, 0, 0);
                    break;

                case "SaveButton":
                    break;

                case "ExitButton":
                    Application.Current.Shutdown();
                    break;
            }
        }
    }
}
