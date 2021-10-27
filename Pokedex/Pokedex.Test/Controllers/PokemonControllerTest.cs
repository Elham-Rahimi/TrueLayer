using Microsoft.AspNetCore.Mvc;
using Moq;
using Pokedex.Controllers;
using Pokedex.Models;
using Pokedex.Services.PokemonBuilder;
using Pokedex.Services.PokemonService;
using System.Threading.Tasks;
using Xunit;

namespace Pokedex.Test.Controllers
{
    public class PokemonControllerTest
    {
        private readonly Mock<IPokemonService> _mockPokemonService;
        private PokemonController _pokemonController;

        public PokemonControllerTest()
        {
            _mockPokemonService = new Mock<IPokemonService>();
            _pokemonController = new PokemonController(_mockPokemonService.Object);
        }

        [Fact]
        public async Task GIVEN_Valid_Name_WHEN_Get_Called_THEN_Return_Success()
        {
            //Arrange
            var pokemon = new Pokemon()
            {
                Name= "mewtwo",
                Habitat= "rare",
                IsLegendary=true,
                Description= "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments."
            };

            _mockPokemonService
                .Setup(x => x.GetAsync(pokemon.Name))
                .Returns(Task.FromResult(pokemon));

            //Act
            var response = await _pokemonController.Get(pokemon.Name);

            //Assert
            Assert.NotNull(response.Result);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = ((OkObjectResult)response.Result).Value as PokemonResult;
            Assert.Equal(pokemon.Name, result.Name);
            Assert.Equal(pokemon.Habitat, result.Habitat);
            Assert.Equal(pokemon.IsLegendary, result.IsLegendary);
            Assert.Equal(pokemon.Description, result.Description);
        }

        [Fact]
        public async Task GIVEN_Valid_Name_WHEN_GetTranslated_Called_THEN_Return_Success()
        {
            //Arrange
            var pokemon = new Pokemon()
            {
                Name = "mewtwo",
                Habitat = "rare",
                IsLegendary = true,
                Description = "Translated"
            };

            _mockPokemonService
                .Setup(x => x.GetWithTranslationAsync(pokemon.Name))
                .Returns(Task.FromResult(pokemon));

            //Act
            var response = await _pokemonController.GetTranslated(pokemon.Name);

            //Assert
            Assert.NotNull(response.Result);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = ((OkObjectResult)response.Result).Value as PokemonResult;
            Assert.Equal(pokemon.Name, result.Name);
            Assert.Equal(pokemon.Habitat, result.Habitat);
            Assert.Equal(pokemon.IsLegendary, result.IsLegendary);
            Assert.Equal(pokemon.Description, result.Description);
        }

    }
}
