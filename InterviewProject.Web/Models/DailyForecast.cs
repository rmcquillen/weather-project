using System;

namespace InterviewProject.Models
{
    public class DailyForecast
    {
        public DateTime Date { get; set; }

        public string DateString => Date.ToString("dddd, MMMM d");

        public Temperature Temperature { get; set; }

        public Day Day { get; set; }
    }
}