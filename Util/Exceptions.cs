namespace IPK_2.Util;

public class Exceptions
{
    public static void RunCatching(Action action)
    {
        try
        {
            action();
        } catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}