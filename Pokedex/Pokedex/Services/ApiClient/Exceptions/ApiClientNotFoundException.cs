using Pokedex.Exceptions;
using System.Net;

namespace Pokedex.Services.ApiClient.Exceptions
{
    public class ApiClientNotFoundException : ApplicationException
    {
        public ApiClientNotFoundException() :
            base((int)HttpStatusCode.NotFound, "client doesn't respond properly.")
        {
        }
    }
}
