using InterviewProject.Models;

namespace InterviewProject.Tests
{
    public class WeatherTests
    {
        /// <summary>
        /// Testing the conversion from Celcius to Fahrenheit. If the calculation ever changes, we want this test to
        /// catch that the conversion could potentially be broken or unintentionally altering part of the application.
        /// </summary>
        [Fact]
        public void TestFahrenheitCalculation()
        {
            // Arrange
            var dailyForecast1 = new DailyForecast { Temperature = new Temperature { Maximum = new Maximum { ValueC = 0 } } };
            var dailyForecast2 = new DailyForecast { Temperature = new Temperature { Maximum = new Maximum { ValueC = 100 } } };
            var dailyForecast3 = new DailyForecast { Temperature = new Temperature { Maximum = new Maximum { ValueC = 16.7 } } };

            // Assert
            Assert.True(dailyForecast1.Temperature.Maximum.ValueF == ConvertCelciusToFahrenheit(dailyForecast1.Temperature.Maximum.ValueC));
            Assert.True(dailyForecast2.Temperature.Maximum.ValueF == ConvertCelciusToFahrenheit(dailyForecast2.Temperature.Maximum.ValueC));
            Assert.True(dailyForecast3.Temperature.Maximum.ValueF == ConvertCelciusToFahrenheit(dailyForecast3.Temperature.Maximum.ValueC));
        }

        private double ConvertCelciusToFahrenheit(double degreesC)
        {
            return Math.Round(32 + (degreesC / 0.5556));
        }
    }
}