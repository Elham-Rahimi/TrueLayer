using Pokedex.Exceptions;
using System.Net;

namespace Pokedex.Services.PokemonBuilder.Exceptions
{
    public class PokemonNullNameException : ApplicationException
    {
        public PokemonNullNameException() :
            base((int)HttpStatusCode.UnprocessableEntity, "pokemon name can't be null or empty.")
        {
        }
    }
}
