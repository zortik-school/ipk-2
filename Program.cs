using System.CommandLine;
using IPK_2.Command;

namespace IPK_2;

class Program
{
    static void Main(string[] args)
    {
        ICommand command = new BaseCommand();

        command.Build().Invoke(args);
    }
}