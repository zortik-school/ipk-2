using System.Net.Sockets;

namespace IPK_2.Interceptor;

public interface ICommandLineInterceptor
{
    
    /**
     * Intercepts the command line arguments and sends something to the server.
     *
     * @param args The command line arguments.
     * @param sendMessageFunction The function to send a message to the server.
     * @param stream The network stream to send the message to.
     *
     * @return true if the client should wait for a response from the server, false otherwise
     */
    bool Intercept(string[] args, Action<string> sendMessageFunction, NetworkStream stream);
    
    /**
     * Checks if the interceptor is applicable for the given command line arguments.
     *
     * @param args The command line arguments.
     *
     * @return true if the interceptor is applicable, false otherwise
     */
    bool IsApplicable(string[] args);
}