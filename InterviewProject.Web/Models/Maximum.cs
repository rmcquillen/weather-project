using Newtonsoft.Json;
using System;

namespace InterviewProject.Models
{
    public class Maximum
    {
        [JsonProperty("Value")]
        public double ValueC { get; set; }

        public double ValueF => Math.Round(32 + (ValueC / 0.5556));
    }
}