using System.Threading;

namespace Semaphore_Task2_1
{
    class Program
    {
        static void Main(string[] args)
        {
            const int numProducers = 3;
            const int numConsumers = 3;
            ProducerConsumer pc = new ProducerConsumer();
            List<Thread> producers = new List<Thread>();
            List<Thread> consumers = new List<Thread>();
            CancellationTokenSource cts = new CancellationTokenSource();
            
            Console.WriteLine("Starting Producers-Consumers Semaphore Algorithm with " + numProducers + 
                              " producers and " + numConsumers + " consumers.");
            Console.WriteLine("Press any key to start/abort.");
            Console.ReadKey();

            for (int i = 0; i < numProducers; i++)
            {
                producers.Add(new Thread(() => pc.Producer(cts.Token)));
                producers.Last().Start();
            }

            for (int i = 0; i < numConsumers; i++)
            {
                consumers.Add(new Thread(() => pc.Consumer(cts.Token)));
                consumers.Last().Start();
            }

            Console.ReadKey();
            cts.Cancel();
            producers.AsParallel().ForAll(el => el.Join());
            consumers.AsParallel().ForAll(el => el.Join());
            cts.Dispose();
        }
    }
}
