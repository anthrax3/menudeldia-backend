using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MenuDelDia.API.Resources;

namespace MenuDelDia.API.Models.Site
{
    public class LocationApiModel
    {
        public LocationApiModel()
        {
            Days = new List<DaysApiModel>();
        }

        public Guid Id { get; set; }

        public string Identifier { get; set; }

        public string Zone { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Features { get; set; }

        public bool Delivery { get; set; }

        public LatLong Location { get; set; }

        public IList<DaysApiModel> Days { get; set; }
    }

    public class LatLong
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DaysApiModel
    {
        public DaysApiModel(DayOfWeek dayOfWeek)
        {
            DayOfWeek = dayOfWeek;
            From = "00";
            To = "00";
            Open = false;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: Name = MessagesResources.DayOfWeekMonday; break;
                case DayOfWeek.Tuesday: Name = MessagesResources.DayOfWeekTuesday; break;
                case DayOfWeek.Wednesday: Name = MessagesResources.DayOfWeekWednesday; break;
                case DayOfWeek.Thursday: Name = MessagesResources.DayOfWeekThursday; break;
                case DayOfWeek.Friday: Name = MessagesResources.DayOfWeekFriday; break;
                case DayOfWeek.Saturday: Name = MessagesResources.DayOfWeekSaturday; break;
                case DayOfWeek.Sunday: Name = MessagesResources.DayOfWeekSunday; break;
            }
        }

        public DayOfWeek DayOfWeek { get; set; }
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool Open { get; set; }
    }
}
