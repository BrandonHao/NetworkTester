using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTester
{
    class PingTest
    {
        private readonly string Server;
        private readonly Ping Pinger;
        private readonly int Pings, Delay;

        public PingTest(string server, int pings, int delay)
        {
            Pinger = new Ping();
            Server = server;
            Pings = pings;
            Delay = delay;
            InitTest();
        }

        private void InitTest()
        {
            try
            {
                Console.WriteLine("Pinging " + Server);
                PingReply r = Pinger.Send(Server);
                while (r == null) { }
                Console.WriteLine("Response Time: " + r.RoundtripTime + "ms");
            }
            catch (PingException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
