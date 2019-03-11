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

namespace NetworkTester
{
    class PingTest : BaseThread
    {
        private readonly string Server;
        private readonly Ping Pinger;
        private readonly int Pings, Delay, TimeOut;
        private readonly List<long> TimeStamps;
        private readonly List<PingReply> Responses;
        private readonly CancellationTokenSource Source;
        private readonly CancellationToken Token;
        private readonly TaskFactory Tasks;

        public override void RunThread()
        {
            Tasks.StartNew(() => {
                if (InitTest())
                {
                    RunTest();
                    SaveToExcel();
                    Finished();
                }
            }, Token);
        }

        public PingTest(string server, int pings, int delay, int timeOut)
        {
            Pinger = new Ping();
            Responses = new List<PingReply>();
            TimeStamps = new List<long>();
            Source = new CancellationTokenSource();
            Tasks = new TaskFactory(Token);
            Token = Source.Token;
            Kill = false;
            Server = server;
            Pings = pings;
            Delay = delay;
            TimeOut = timeOut;
        }

        private bool InitTest()
        {
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Reset();
                timer.Start();
                Console.WriteLine("Pinging " + Server);
                PingReply r = Pinger.Send(Server);
                while (r == null)
                {
                    if(timer.ElapsedMilliseconds > TimeOut)
                    {
                        Console.WriteLine("Server Timed Out! Aborting Test.");
                        return false;
                    }
                    else if (Kill)
                    {
                        Source.Cancel();
                        return false;
                    }
                }
                Console.WriteLine("Initial server ping: OK");
                Console.WriteLine("Response Time: {0}ms", r.RoundtripTime);
            }
            catch (PingException e)
            {
                Console.WriteLine(e.ToString());
            }
            return true;
        }

        private void RunTest()
        {
            Stopwatch timer = new Stopwatch();
            Stopwatch totalTime = new Stopwatch();
            Console.WriteLine("Running Ping Test");
            int count = 0;
            timer.Reset();
            totalTime.Reset();
            while (count < Pings)
            {
                timer.Restart();
                if (Kill)
                {
                    Source.Cancel();
                    return;
                }
                timer.Start();
                PingReply r = Pinger.Send(Server);
                while (r == null && timer.ElapsedMilliseconds < TimeOut) { }
                if(timer.ElapsedMilliseconds < TimeOut)
                {
                    Console.WriteLine("Response Time: {0}ms", r.RoundtripTime);
                }
                else
                {
                    if(r.Status != IPStatus.Success)
                    {
                        Console.WriteLine(r.Status);
                    }
                    else
                    {
                        Console.WriteLine("Ping Timed Out!");
                    }
                }
                Responses.Add(r);
                TimeStamps.Add(totalTime.ElapsedMilliseconds);
                Thread.Sleep(Delay);
                count++;
            }
        }

        private void SaveToExcel()
        {
            FileInfo file = new FileInfo(@"C:\Users\brandon.h\Documents\test.xlsx");
            Console.WriteLine("Saving Data Do Not Exit!");
            using (ExcelPackage p = new ExcelPackage(file))
            {
                ExcelWorksheet ws = p.Workbook.Worksheets.Add("Data");
                ws.Cells[1, 1].Value = "Ping Results: ";
                ws.Cells[2, 1].Value = "Target Server: " + Server;
                ws.Cells[3, 1].Value = "Test Pings: " + Pings.ToString() + " Delay: " + Delay.ToString() + "ms";
                ws.Cells[4, 1].Value = "Round Trip Time (ms): ";
                ws.Cells[4, 2].Value = "Elapsed Time (ms): "; 
                for (int i = 0; i < Responses.Count; i++)
                {
                    ws.Cells[4 + i, 1].Value = Responses[i].RoundtripTime.ToString();
                    ws.Cells[4 + i, 2].Value = TimeStamps[i].ToString();
                }
                p.Save();
            }
        }
    }
}
