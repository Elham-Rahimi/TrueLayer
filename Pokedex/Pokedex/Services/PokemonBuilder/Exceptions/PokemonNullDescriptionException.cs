using Pokedex.Exceptions;
using System.Net;

namespace Pokedex.Services.PokemonBuilder.Exceptions
{
    public class PokemonNullDescriptionException : ApplicationException
    {
        public PokemonNullDescriptionException() :
            base((int)HttpStatusCode.UnprocessableEntity, "pokemon description can't be null or empty.")
        {
        }
    }
}
