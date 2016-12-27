using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GeoLib.Core;


namespace GeoLib.Data
{
    public interface IZipCodeRepository 
    {
        ZipCode GetByZip(string zip);
        IEnumerable<ZipCode> GetByState(string state);
        //IEnumerable<ZipCode> GetZipsForRange(ZipCode zip, int range);

        void UpdateZipCity(string city, string zip);
    }
}
