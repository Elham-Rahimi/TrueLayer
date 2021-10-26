using Microsoft.AspNetCore.Mvc;
using Pokedex.Exceptions;
using Pokedex.Models;
using Pokedex.Services.PokemonService;
using System.Threading.Tasks;

namespace Pokedex.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<PokemonResult>> Get([FromRoute] string name)
        {
            var result = await _pokemonService.GetAsync(name);

            return Ok(new PokemonResult()
            {
                Name = result.Name,
                Description = result.Description,
                Habitat = result.Habitat,
                IsLegendary = result.IsLegendary
            });
        }
    }
}
