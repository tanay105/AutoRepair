using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRepairPhone
{
    class shopsdata
    {
        public class Metadata
        {
            public string uri { get; set; }
        }

        public class Result
        {
            public Metadata __metadata { get; set; }
            public string ID { get; set; }
            public string Name { get; set; }
            public string AddressLine { get; set; }
            public string PostalCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string StoreType { get; set; }
            public string Confidence { get; set; }
            public string Locality { get; set; }
            public string AdminDistrict { get; set; }
            public string CountryRegion { get; set; }
            public double __Distance { get; set; }
        }

        public class D
        {
            public string __copyright { get; set; }
            public List<Result> results { get; set; }
        }

        public class restObj
        {
            public D d { get; set; }
        }

    }
}
