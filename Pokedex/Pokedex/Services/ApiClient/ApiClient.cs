using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using Pokedex.Exceptions;
using Pokedex.Services.ApiClient.Exceptions;

namespace Pokedex.Services.ApiClient
{
    public class ApiClient : IApiClient
    {
        private readonly IRestClient _restClient;

        public ApiClient(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<T> GetAsync<T>(string baseUrl) where T : class
        {
            var response = await _restClient.ExecuteAsync(new RestRequest(baseUrl));
            if (response == null)
            {
                throw new NullResponseException();
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiClientNotFoundException();
            }
            var resultModel = JsonConvert.DeserializeObject<T>(response.Content);
            //error on jason convert
            return resultModel;
        }
    }
}
