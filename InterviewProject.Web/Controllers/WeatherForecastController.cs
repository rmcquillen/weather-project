using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InterviewProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace InterviewProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private HttpClient httpClient;

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly AppSettings _appSettings;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _logger = logger;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpGet("{location?}")]
        public async Task<FiveDayForecast> GetByLocation(string location = "Cleveland")
        {
            string locationKey;
            FiveDayForecast fiveDayForecast;

            using (httpClient = new HttpClient())
            {
                // In order to retrieve the five day forecast, a unique location key is needed for the AccuWeather API
                locationKey = await GetLocationKey(location);
                fiveDayForecast = await GetFiveDayForecast(locationKey);
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
        private async Task<string> GetLocationKey(string location)
        {
            using var response = await httpClient.GetAsync("http://dataservice.accuweather.com/locations/v1/cities/search?apikey=" + _appSettings.AccuWeatherDevAPIKey + "&q=" + location);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogError("AccuWeather API key is not authorized to call City Search API");

                return string.Empty;
            }
            else if (response.IsSuccessStatusCode)
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
                        return locationKey.Key;
                    }
                    else
                    {
                        _logger.LogError("No location key was returned by AccuWeather City Search API for location: {location}", location);

                        return string.Empty;
                    }
                }
                catch (JsonException e)
                {
                    _logger.LogError("Error occurred during deserialization of AccuWeather city search\nError: {errorMessage}\nAPI Response: {response}",
                        e.Message, apiResponse);

                    return string.Empty;
                }
            }
            else
            {
                _logger.LogError("Error occurred during call to AccuWeather City Search API\nStatus Code: {statusCode}, Reason: {reason}",
                    response.StatusCode, response.ReasonPhrase);

                return string.Empty;
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
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogError("AccuWeather API key is not authorized to call 5 Days of Daily Forecasts API");

                return null;
            }
            else if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                try
                {
                    return JsonConvert.DeserializeObject<FiveDayForecast>(apiResponse);
                }
                catch (JsonException e)
                {
                    _logger.LogError("Error occurred during deserialization of AccuWeather 5 Days of Daily Forecasts API call\nError: {errorMessage}, Response: {response}",
                        e.Message, apiResponse);

                    return null;
                }
            }
            else
            {
                _logger.LogError("Error occurred during call to AccuWeather 5 Days of Daily Forecasts API\nStatus Code: {statusCode}, Reason: {reason}",
                    response.StatusCode, response.ReasonPhrase);

                return null;
            }
        }
    }
}