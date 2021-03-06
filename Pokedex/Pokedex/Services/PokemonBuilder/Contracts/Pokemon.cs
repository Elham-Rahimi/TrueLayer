namespace Pokedex.Services.PokemonBuilder
{
    public class Pokemon
    {

        public string Name { get; set; }
        public string? Habitat { get; set; }
        public bool? IsLegendary { get; set; }
        public string? Description { get; set; }

        public bool IsYoda()
        {
            return Habitat == "cave" || (IsLegendary.HasValue && IsLegendary.Value);
        }
    

    }
}
