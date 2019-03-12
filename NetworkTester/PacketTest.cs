using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkTester
{
    class PacketTest : BaseThread
    {
        private readonly string Server;
        private readonly int Pings, Delay, TimeOut;
        private int DroppedPackets;
        private readonly Stopwatch Timer;
        private readonly List<PingReply> Responses;
        private readonly CancellationTokenSource Source;
        private readonly CancellationToken Token;
        private readonly TaskFactory Tasks;
        private readonly List<long> TimeStamps;

        public override void RunThread()
        {
            void work()
            {
                Timer.Reset();
                Timer.Start();
                long time;
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Pinging {0}: {0} times", Server, Pings);
                    GetLoss(Server, Pings);
                    time = Timer.ElapsedMilliseconds;
                    while (Timer.ElapsedMilliseconds - time < 60000)
                    {
                        if (Kill)
                        {
                            return;
                        }
                    }
                }
                
            }
            work();
            SaveToExcel();
            if(!Kill)
                Finished("packet");
        }

        public PacketTest(string server, int pings, int delay, int timeOut)
        {
            Responses = new List<PingReply>();
            Tasks = new TaskFactory(Token);
            Source = new CancellationTokenSource();
            Token = Source.Token;
            Timer = new Stopwatch();
            TimeStamps = new List<long>();
            Timer.Reset();
            Kill = false;
            Server = server;
            Pings = pings;
            Delay = delay;
            TimeOut = timeOut;
        }

        private void GetLoss(string host, int pingAmount)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions
            {
                DontFragment = true
            };
            
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            for (int i = 0; i < pingAmount; i++)
            {
                if (Kill)
                {
                    Source.Cancel();
                    return;
                }
                TimeStamps.Add(Timer.ElapsedMilliseconds);
                PingReply reply = pingSender.Send(host, TimeOut, buffer, options);
                Console.WriteLine("Sending Packets to Server: {0}", Server);
                if (reply.Status != IPStatus.Success)
                {
                    DroppedPackets += 1;
                    Console.WriteLine("Packet Dropped! Total Packets Dropped: {0}", DroppedPackets);
                }
                Thread.Sleep(Delay);
                Responses.Add(reply);
            }
        }

        private void SaveToExcel()
        {
            string time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            time = time.Replace(":", "-");
            string path = Directory.GetCurrentDirectory() + @"\" + "data" + time + ".xlsx";
            FileInfo file = new FileInfo(path);
            Console.WriteLine("Saving Data Do Not Exit!");
            using (ExcelPackage p = new ExcelPackage(file))
            {
                ExcelWorksheet ws = p.Workbook.Worksheets.Add("Data");
                ws.Cells[1, 1].Value = "Ping Results: ";
                ws.Cells[2, 1].Value = "Target Server: " + Server;
                ws.Cells[3, 1].Value = "Test Pings: " + Pings.ToString() + " Delay: " + Delay.ToString() + "ms";
                ws.Cells[4, 1].Value = "Elapsed Time (ms): ";
                ws.Cells[4, 2].Value = "Round Trip Time (ms): ";
                ws.Cells[4, 3].Value = "Dropped Packets: " + DroppedPackets;
                ws.Cells[4, 3].Value = "Percentage of Packets Dropped: " + DroppedPackets / Pings;

                for (int i = 0; i < Responses.Count; i++)
                {
                    ws.Cells[4 + i, 1].Value = TimeStamps[i].ToString();
                    ws.Cells[4 + i, 2].Value = Responses[i].RoundtripTime.ToString();
                }
                p.Save();
            }
        }
    }
}
