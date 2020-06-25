using System;
namespace Controllers
{
        public class SensorInterface
        {
            public string SensorId { get; set; }
            public DateTime? OccurDate { get; set; }
            public string Category { get; set; }
            public string Value { get; set; }
        }
}