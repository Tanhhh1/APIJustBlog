namespace Application.Exceptions
{
    public class ForgeException : Exception
    {
        public ForgeException() : base()
        {
        }

        public ForgeException(string message) : base(message)
        {
        }
    }
}
