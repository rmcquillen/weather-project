using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InterviewProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace InterviewProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private HttpClient httpClient;

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly AppSettings _appSettings;

        private const string CITY_SEARCH_API = "City Search";

        private const string FIVE_DAYS_FORECAST_API = "5 Days of Daily Forecasts";

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _logger = logger;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpGet()]
        public async Task<FiveDayForecast> Get()
        {
            return await GetByLocation(_appSettings.DefaultCity);
        }

        [HttpGet("{location}")]
        public async Task<FiveDayForecast> GetByLocation(string location)
        {
            LocationKey locationKey;
            FiveDayForecast fiveDayForecast;

            using (httpClient = new HttpClient())
            {
                // In order to retrieve the five day forecast, a unique location key is needed for the AccuWeather API
                locationKey = await GetLocationKey(location);
                if (string.IsNullOrEmpty(locationKey.Key) == false)
                {
                    fiveDayForecast = await GetFiveDayForecast(locationKey.Key);
                    fiveDayForecast.Location = $"{locationKey.EnglishName}, {locationKey.AdministrativeArea.EnglishName}";
                }
                else
                {
                    fiveDayForecast = new FiveDayForecast();
                }
            }

            return fiveDayForecast;
        }

        /// <summary>
        /// Method <c>GetLocationKey</c> calls AccuWeather's City Search API 
        /// <see href="https://developer.accuweather.com/accuweather-locations-api/apis/get/locations/v1/cities/search"/>
        /// </summary>
        /// <param name="location">The location being searched</param>
        /// <returns>A unique "location key" that can be passed to other AccuWeather APIs (for example, to retrieve a forecast for that location)
        /// OR an empty string if lookup is not successful
        /// </returns>
        private async Task<LocationKey> GetLocationKey(string location)
        {
            using var response = await httpClient.GetAsync("http://dataservice.accuweather.com/locations/v1/cities/search?apikey=" + _appSettings.AccuWeatherDevAPIKey + "&q=" + location);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                try
                {
                    /* AccuWeather City Search API returns information for all cities that match the given location
                       The most likely match is returned as the first result, so FirstOrDefault is taken from the deserialized object
                    */
                    var locationKey = JsonConvert.DeserializeObject<List<LocationKey>>(apiResponse).FirstOrDefault();

                    if (locationKey != null)
                    {
                        return locationKey;
                    }
                    else
                    {
                        _logger.LogError("No location key was returned by AccuWeather City Search API for location: {location}", location);

                        return new LocationKey();
                    }
                }
                catch (JsonException e)
                {
                    HandleDeserializationException(e, apiResponse, CITY_SEARCH_API);

                    return new LocationKey();
                }
            }
            else
            {
                HandleInvalidResponse(response, CITY_SEARCH_API);

                return new LocationKey();
            }
        }

        /// <summary>
        /// Method <c>GetFiveDayForecast</c> calls AccuWeather's 5 Days of Daily Forecasts API
        /// <see href="https://developer.accuweather.com/accuweather-forecast-api/apis/get/forecasts/v1/daily/5day/%7BlocationKey%7D"/>
        /// </summary>
        /// <param name="locationKey">The unique location key being forecasted</param>
        /// <returns>A deserialized object representing a five day forecast</returns>
        private async Task<FiveDayForecast> GetFiveDayForecast(string locationKey)
        {
            using var response = await httpClient.GetAsync("http://dataservice.accuweather.com/forecasts/v1/daily/5day/" + locationKey + "?metric=true&apikey=" + _appSettings.AccuWeatherDevAPIKey);

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                try
                {
                    return JsonConvert.DeserializeObject<FiveDayForecast>(apiResponse);
                }
                catch (JsonException e)
                {
                    HandleDeserializationException(e, apiResponse, FIVE_DAYS_FORECAST_API);

                    return null;
                }
            }
            else
            {
                HandleInvalidResponse(response, FIVE_DAYS_FORECAST_API);

                return null;
            }
        }

        private void HandleInvalidResponse(HttpResponseMessage response, string apiName)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    _logger.LogError($"AccuWeather API key is not authorized to call {apiName} API", apiName);
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    _logger.LogError($"Error occurred during call to AccuWeather API. API key may be exceeding service limits.\nStatus Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                    break;
                default:
                    _logger.LogError($"Error occurred during call to AccuWeather {apiName} API\nStatus Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                    break;
            }
        }

        private void HandleDeserializationException(JsonException e, string apiResponse, string apiName)
        {
            _logger.LogError($"Error occurred during deserialization of AccuWeather {apiName} API\nError: {e.Message}\nAPI Response: {apiResponse}");
        }
    }
}