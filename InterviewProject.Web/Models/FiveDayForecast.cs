using System;
using System.Collections.Generic;

namespace InterviewProject.Models
{
    public class FiveDayForecast
    {
        public Headline Headline { get; set; }

        public List<DailyForecast> DailyForecasts { get; set; }

        public string Location { get; set; }
    }
}
