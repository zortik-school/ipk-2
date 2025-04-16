using System.CommandLine;
using System.CommandLine.Invocation;

namespace IPK_2.Command;

public interface ICommand
{
    ICommandHandler GetHandler();
    
    Option[] GetOptions() => [];

    Argument[] GetArguments() => [];

    string GetDescription() => "";

    public System.CommandLine.Command Build()
    {
        RootCommand cmd = new RootCommand(GetDescription());
        
        foreach (Option option in GetOptions())
        {
            cmd.AddOption(option);
        }
        
        foreach (Argument argument in GetArguments())
        {
            cmd.AddArgument(argument);
        }
        
        cmd.Handler = GetHandler();

        return cmd;
    }
}