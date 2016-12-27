using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using GeoLib.Core;
using System.Data;

namespace GeoLib.Data
{
    public class ZipCodeRepository : IZipCodeRepository
    {
        
        //public IEnumerable<ZipCode> GetZipsForRange(ZipCode zip, int range)
        //{
        //    using (GeoLibDbContext entityContext = new GeoLibDbContext())
        //    {
        //        double degrees = range / 69.047;

        //        return entityContext.ZipCodeSet
        //            .Include(e => e.State)
        //            .Where(e => (e.Latitude <= zip.Latitude + degrees && e.Latitude >= zip.Latitude - degrees) &&
        //                        (e.Longitude <= zip.Longitude + degrees && e.Longitude >= zip.Longitude - degrees))
        //            .ToFullyLoaded();
        //    }
        //}

        public ZipCode GetByZip(string zip)
        {
            DataTable zipData=GeoLibDbContext.GetOracleData("SELECT Name,Abbreviation,City,Zip from ZipCode zc inner join state st on zc.stateid=st.stateid where rownum=1 and zc.zip='"+zip+"'");            
            ZipCode zipCodeData = null;
            if (zipData.Rows.Count > 0)
            {
                foreach (DataRow item in zipData.Rows)
                {
                    zipCodeData =new ZipCode(){
                        City=item["City"].ToString(),
                        Zip=item["Zip"].ToString(),
                        State= new State()
                        {
                            Name= item["Name"].ToString(),
                            Abbreviation=item["Abbreviation"].ToString()
                        }                        
                    };                     
                }
            }
            return zipCodeData;
        }

        public IEnumerable<ZipCode> GetByState(string state)
        {
            DataTable stateData = GeoLibDbContext.GetOracleData("SELECT ZIP || ' - ' || CITY as ZIPCITY,ZIP,ABBREVIATION,NAME from ZipCode zc inner join state st on zc.stateid=st.stateid where ST.ABBREVIATION='" + state + "'");
            List<ZipCode> zipCodeList=new List<ZipCode>();            
            if (stateData.Rows.Count > 0)
            {
                foreach (DataRow item in stateData.Rows)
                {
                    zipCodeList.Add(new ZipCode()
                    {
                        City = item["ZIPCITY"].ToString(),

                        Zip = item["ZIP"].ToString(),
                        State = new State()
                        {
                            Abbreviation = item["ABBREVIATION"].ToString(),
                            Name=item["NAME"].ToString()
                        }
                    });
                }
            }
            return zipCodeList;
        }
        public void UpdateZipCity(string city, string zip)
        {
            GeoLibDbContext.UpdateOracleData("UPDATE ZIPCODE SET CITY='"+city+"' WHERE ZIP='"+zip+"'");
        }

        
    }
}
