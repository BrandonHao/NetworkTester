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
        private BaseThread PingingThread, PacketThread;

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
                    {
                        PingingThread = new PingTest(ServerBox.Text, 1, 5, 3000);
                        PacketThread = new PacketTest(ServerBox.Text, 10, 500, 3000);
                        
                    }
                    else
                    {
                        PingingThread = new PingTest(HintText.Text, 1, 5, 3000);
                        PacketThread = new PacketTest(HintText.Text, 10, 500, 3000);
                    }
                    PingingThread.Done += ThreadDone;
                    PacketThread.Done += ThreadDone;
                    PingingThread.Start();
                    break;
                    
                case "ExitButton":
                    if(PingingThread != null && PingingThread.IsAlive)
                    {
                        PingingThread.Kill = true;
                    }
                    if (PacketThread != null && PacketThread.IsAlive)
                    {
                        PacketThread.Kill = true;
                    }
                    while (PingingThread.IsAlive) { }
                    Application.Current.Shutdown();
                    break;
            }
        }

        public void ThreadDone(object sender, EventArgs e)
        {
            if(((string)sender).Equals("ping"))
                PacketThread.Start();
        }
    }
}
