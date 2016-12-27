using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Contracts
{
    [DataContract]
    public class ZipCityData
    {
        [DataMember]
        public string CityNew { get; set; }
        
        [DataMember]
        public string ZipCodeNew { get; set; }
    }
}