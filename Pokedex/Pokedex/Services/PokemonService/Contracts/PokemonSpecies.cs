using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pokedex.Services.PokemonService.Contracts
{
    public class PokemonSpecies
    {
        [JsonProperty("flavor_text_entries")]
        public List<FlavorTextEntry> FlavorTextEntries { get; set; }


        [JsonProperty("is_legendary")]
        public bool? IsLegendary { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("habitat")]
        public Habitat Habitat { get; set; }
    }

    public class Habitat
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Language
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Version
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class FlavorTextEntry
    {
        [JsonProperty("flavor_text")]
        public string FlavorText { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }
    }
}
