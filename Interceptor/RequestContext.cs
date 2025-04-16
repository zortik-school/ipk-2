using System.Net.Sockets;

namespace IPK_2.Interceptor;

public record RequestContext(string[] Cmd, NetworkStream Stream, Dictionary<string, object> Meta);