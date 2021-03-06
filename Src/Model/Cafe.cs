﻿using System.Collections.Generic;
using System.Linq;

namespace CoffeeClientPrototype.Model
{
    public class Cafe
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double CoffeeRating { get; set; }
        
        public double AtmosphereRating { get; set; }

        public int NumberOfVotes { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public IEnumerable<Photo> Photos { get; set; }

        public Cafe()
        {
            this.Photos = Enumerable.Empty<Photo>();
        }

        public override string ToString()
        {
            return string.Format("Cafe {0} ({1})", this.Id, this.Name);
        }
    }
}
