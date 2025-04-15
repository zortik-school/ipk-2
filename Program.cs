namespace IPK_2;

class Program
{
    static void Main(string[] args)
    {
        Parallel.Invoke(
            () => new Sender().Start(),
            () => new Receiver().Start()
        );
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}