using System.Threading.Tasks;

namespace Pokedex.Services.ApiClient
{
    public interface IApiClient
    {
        Task<T> GetAsync<T>(string baseUrl) where T : class;
    }
}
