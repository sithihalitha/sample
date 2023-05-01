using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorldBankData
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataCatalogController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public DataCatalogController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://api.worldbank.org/v2/datacatalog?format=json&q={query}");
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into a dynamic object
                dynamic responseData = JsonConvert.DeserializeObject(responseContent);

                // Filter the data based on the display titles containing the keywords
                var filteredData = responseData[1].Where(d => d.name.ToString().Contains("wind") && d.name.ToString().Contains("energy"));

                return Ok(filteredData);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(400, ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServer(500, ex.Message);
            }
        }
    }
}
