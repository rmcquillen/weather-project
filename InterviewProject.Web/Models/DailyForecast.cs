using System;

namespace InterviewProject.Models
{
    public class DailyForecast
    {
        public DateTime Date { get; set; }

        public Temperature Temperature { get; set; }

        public Day Day { get; set; }
    }
}