using Microsoft.AspNetCore.Mvc;
using Pokedex.Models;
using System.Threading.Tasks;

namespace Pokedex.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PokemonResult>> Get([FromQuery] string PokemonName)
        {
            return Ok(new PokemonResult());
        }
    }
}
