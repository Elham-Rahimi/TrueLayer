using Pokedex.Exceptions;
using System.Net;

namespace Pokedex.Services.PokemonService.Exceptions
{
    public class PokemonNameNullException : ApplicationException
    {
        public PokemonNameNullException() :
            base((int)HttpStatusCode.UnprocessableEntity, "pokemon name can't be null or empty.")
        {
        }
    }
}
