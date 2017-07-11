namespace Heartcatch
{
    public class Exception : System.Exception
    {
        public Exception(string reason) : base(reason)
        {
        }

        public Exception(string reason, Exception interlanlException) : base(reason, interlanlException)
        {
        }
    }

    public class LoadingException : Exception
    {
        public LoadingException(string message) : base(message)
        {
        }
    }
}