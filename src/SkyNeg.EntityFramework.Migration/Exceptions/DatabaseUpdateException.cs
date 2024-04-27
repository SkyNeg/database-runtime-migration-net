namespace SkyNeg.EntityFramework.Migration.Exceptions
{
    public class DatabaseUpdateException : Exception
    {
        public DatabaseUpdateException(string? message) : base(message)
        {
        }

        public DatabaseUpdateException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
