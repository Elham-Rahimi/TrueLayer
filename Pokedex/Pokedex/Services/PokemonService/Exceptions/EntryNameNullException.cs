using Pokedex.Exceptions;
using System.Net;

namespace Pokedex.Services.PokemonService.Exceptions
{
    public class EntryNameNullException : ApplicationException
    {
        public EntryNameNullException() :
            base((int)HttpStatusCode.UnprocessableEntity, "service entry name can't be null or empty.")
        {
        }
    }
}
