using System.Net;

namespace Pokedex.Exceptions
{
    public class ApiClientNotFoundException : ApplicationException
    {
        public ApiClientNotFoundException() :
            base((int)HttpStatusCode.NotFound, "client doesn't respond properly.")
        {
        }
    }
}
