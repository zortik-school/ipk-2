using IPK_2.Chat;

namespace IPK_2.Interceptor;

public class RenameInterceptor(ChatService service) : CommandInterceptor("rename", 2, Int32.MaxValue)
{
    // TODO
}