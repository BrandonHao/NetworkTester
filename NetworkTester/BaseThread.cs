using System;
using System.Threading;

namespace NetworkTester
{
    abstract class BaseThread
    {
        private readonly Thread _thread;
        public bool Kill;
        public event EventHandler Done;

        protected BaseThread()
        {
            _thread = new Thread(new ThreadStart(RunThread));
        }

        public void Start() => _thread.Start();
        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;
        public void Sleep(int millis) => Thread.Sleep(millis);

        public abstract void RunThread();
        public void Finished(string type)
        {
            Console.WriteLine("Test Complete!");
            Done(type, new EventArgs());
        }
    }
}
