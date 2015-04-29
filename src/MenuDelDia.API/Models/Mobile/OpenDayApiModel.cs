using System;

namespace MenuDelDia.API.Models.Mobile
{
    public class OpenDayApiModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public int OpenHour { get; set; }
        public int OpenMinutes { get; set; }
        public int CloseHour { get; set; }
        public int CloseMinutes { get; set; }
    }
}