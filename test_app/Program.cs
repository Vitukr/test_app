using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test_app
{
    class Program
    {
        //private CancellationTokenSource tokenSource = new CancellationTokenSource();
        //private List<data> container = new List<data>();

        static void Main(string[] args)
        {
            List<data> container = new List<data>();
            if (args.Length == 2)
            {
                // Test for errors
                bool error = false;
                int x;
                if (!int.TryParse(args[0], out x))
                {
                    Console.WriteLine("Can not convert x to int");
                    error = true;
                }
                if (x < 2 || x > 64)
                {
                    Console.WriteLine("x argument is out of range");
                    error = true;
                }
                int y;
                if (!int.TryParse(args[1], out y))
                {
                    Console.WriteLine("Can not convert y to int");
                    error = true;
                }
                if (y < 0)
                {
                    Console.WriteLine("y argument must be positive");
                    error = true;
                }

                List<Thread> thread = new List<Thread>();
                // Run application
                if (!error)
                {
                    for (int i = 0; i < x; i++)
                    {
                        parametr_data p = new parametr_data();
                        p.i = i;
                        p.y = y;
                        p.container = container;
                        Thread thread_n = new Thread(new ParameterizedThreadStart(doThreadWork));
                        thread.Add(thread_n);
                        thread_n.Start(p);
                    }                   
                    
                    ConsoleKeyInfo cki;
                    while (true)
                    {
                        cki = Console.ReadKey();
                        if (cki.Key == ConsoleKey.Enter)
                        {
                            for (int i = 0; i < x; i++)
                            {
                                thread[i].Abort();
                            }
                            result(container, x);
                            Console.ReadLine();
                            break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Usage: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName + " x y");
            }
        }

        static private void doThreadWork(object d)
        {
            if (d is parametr_data)
            {
                parametr_data p = (parametr_data)d;
                Random random = new Random((int)DateTime.Now.Ticks);
                while (true)
                {
                    int n = random.Next(5000);
                    lock (p.container)
                    {
                        if (p.container.Count > 0)
                        {
                            if (p.container[p.container.Count - 1].thread != p.i || p.container.Count > p.y)
                            {
                                Console.WriteLine("Remove from thread: " + p.container[p.container.Count - 1].thread.ToString() + " index: " + (p.container.Count - 1).ToString());
                                p.container.RemoveAt(p.container.Count - 1);
                            }
                        }
                        data d_n = new data();
                        d_n.thread = p.i;
                        d_n.number = n;
                        p.container.Add(d_n);                        
                    }
                    Console.WriteLine("Thread: " + p.i.ToString() + " number: " + n.ToString());
                    Thread.Sleep(n);
                }
            }
        }
        
        static private void result(List<data> container, int x)
        {
            List<int> r = new List<int>();
            for (int i = 0; i < x; i++)
            {
                r.Add(0);
            }
            for (int i = 0; i < container.Count; i++)
            {
                r[container[i].thread] += 1;
            }
            for (int i = 0; i < x; i++)
            {
                Console.WriteLine("Thread: " + i.ToString() + " has: " + r[i].ToString() + " element(s)");
            }
            Console.WriteLine("In container : " + container.Count.ToString() + " element(s)");
        }
    }

    public struct data
    {
        public int thread;
        public int number;
    }

    public struct parametr_data
    {
        public int i;
        public int y;
        public List<data> container;
    }
}
