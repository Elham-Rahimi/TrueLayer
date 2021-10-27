using Pokedex.Services.PokemonService.Contracts;

namespace Pokedex.Services.PokemonBuilder
{
    public interface IPokemonBuilder
    {
        Pokemon Build();
        PokemonBuilder Init(PokemonSpecies species);
        PokemonBuilder WithName();
        PokemonBuilder WithHabitat();
        PokemonBuilder WithIsLegendary();
        PokemonBuilder WithDescription(string languageKey);
    }
}
