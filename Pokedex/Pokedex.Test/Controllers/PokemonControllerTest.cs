using Microsoft.AspNetCore.Mvc;
using Moq;
using Pokedex.Controllers;
using Pokedex.Exceptions;
using Pokedex.Models;
using Pokedex.Services.ApiClient.Exceptions;
using Pokedex.Services.PokemonService;
using Pokedex.Services.PokemonService.Contracts;
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
        public async Task GIVEN_Valid_name_WHEN_Called_THEN_Return_Success()
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
        public async Task GIVEN_Inalid_name_WHEN_Called_THEN_Throw_Exception()
        {
            //Arrange
            var name = "noname";
            _mockPokemonService
                .Setup(x => x.GetAsync(name))
                .Throws<ApiClientNotFoundException>();

            //Act

            //Assert
            await Assert.ThrowsAsync<ApiClientNotFoundException>(() =>
                        _pokemonController.Get(name));
        }

        [Fact]
        public async Task GIVEN_null_response_WHEN_Called_THEN_Throw_Exception()
        {
            //Arrange
            var name = "pidgeot";
            _mockPokemonService
                .Setup(x => x.GetAsync(name))
                .Throws<NullResponseException>();

            //Act

            //Assert
            await Assert.ThrowsAsync<NullResponseException>(() =>
                        _pokemonController.Get(name));
        }

    }
}
