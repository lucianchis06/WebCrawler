using System;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler.Database.Models
{
    public class Ad
    {
        public Ad()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Url { get; set; }

        public string PhotoUrl { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Fuel { get; set; }

        public string Engine { get; set; }

        public string Gearbox { get; set; }

        public string Year { get; set; }

        public string Km { get; set; }

        public string VIN { get; set; }

        public string Country { get; set; }

        public string Color { get; set; }

        public string First { get; set; }

        public string Accident { get; set; }

        public string Description { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public string Address { get; set; }

        public string OwnBy { get; set; }

        public string Owner { get; set; }

        public string OwnerUrl { get; set; }

        public string Body { get; set; }

        public string Condition { get; set; }

        public string Date { get; set; }

        public string UniqueIdentifier { get; set; }

        public string Price { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Power { get; set; }

        public string CO2 { get; set; }

        public string Emissions { get; set; }

        public string Transmission { get; set; }

        public string Doors { get; set; }

        public string AddedOnline { get; set; }
    }
}
