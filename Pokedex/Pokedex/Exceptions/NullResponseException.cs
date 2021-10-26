using System.Net;

namespace Pokedex.Exceptions
{
    public class NullResponseException : ApplicationException
    {
        public NullResponseException() :
            base((int)HttpStatusCode.NotFound, "client respond with null.")
        {
        }
    }
}
