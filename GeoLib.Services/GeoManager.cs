using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoLib.Contracts;
using GeoLib.Data;
using System.ServiceModel;
namespace GeoLib.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
    public class GeoManager:IGeoService

    {
        public ZipCodeData GetZipInfo(string zip)
        {
            //throw new ApplicationException("Zip code not found"); 
            //throw new FaultException("Zip code not found");
            //ApplicationException app = new ApplicationException("Zip not found");
            //throw new FaultException<ApplicationException>(app, "just another message");
            ZipCodeData zipCodeData = null;
            IZipCodeRepository zipCodeRepository = new ZipCodeRepository();
            ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zip);
            if (zipCodeEntity != null)
            {
                zipCodeData = new ZipCodeData()
                {
                    City=zipCodeEntity.City,
                    State=zipCodeEntity.State.Abbreviation,
                    ZipCode=zipCodeEntity.Zip
                };
            }
            return zipCodeData;
        }

        public IEnumerable<string> GetStates(bool primayOnly)
        {
            List<string> statesData = new List<string>();
            IStateRepository stateRepository = new StateRepository();
            IEnumerable<State> stateEntity= stateRepository.Get(primayOnly);
            if (stateEntity != null)
            {
                foreach (State item in stateEntity)
                {
                    statesData.Add(item.Abbreviation);
                }
            }
            return statesData;
        }

        public IEnumerable<ZipCodeData> GetZips(string state)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();
            IZipCodeRepository zipCodeRepository = new ZipCodeRepository();
            IEnumerable<ZipCode> zipCodeEntity = zipCodeRepository.GetByState(state);
            if (zipCodeEntity != null)
            {
                foreach (ZipCode item in zipCodeEntity)
                {
                    zipCodeData.Add(new ZipCodeData() {City= item.City,State= item.State.Abbreviation ,ZipCode= item.Zip });
                }
            }
            return zipCodeData;
        }

        public IEnumerable<ZipCodeData> GetZips(string zip, int range)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();
            IZipCodeRepository zipCodeRepository = new ZipCodeRepository();
            ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zip);
            IEnumerable<ZipCode> zips = zipCodeRepository.GetZipsForRange(zipCodeEntity, range);
            if (zipCodeEntity != null)
            {
                foreach (ZipCode item in zips)
                {
                    zipCodeData.Add(new ZipCodeData() {City= item.City,State= item.State.Abbreviation,ZipCode= item.Zip });
                }
            }
            return zipCodeData;
        }
        public void UpdateZipCity(string zip, string city)
        {
            IZipCodeRepository zipCodeRepository = new ZipCodeRepository();
            ZipCode entity = zipCodeRepository.GetByZip(zip);
            if (entity != null)
            {
                entity.City = city;
                zipCodeRepository.UpdateZipCity(city, zip);
            }
        }
        //this method is for transaction
        //if one fails, fail all
        [OperationBehavior(TransactionScopeRequired=true)]//if there is an existing transaction, setting this true will join both the tran. if false, it will not join
            //if operation succeeds, the operation will vote to commit , else rollback
            //with this setting no tran needed at DB side
        public void UpdateZipCityBatch(IEnumerable<ZipCityData> batchData)
        {
            IZipCodeRepository zipCodeRepository = new ZipCodeRepository();
            int counter = 0;
            foreach (ZipCityData  item in batchData)
            {
                counter++;
                if (counter == 2)
                    throw new FaultException("sec zip cannot be updated");
                zipCodeRepository.UpdateZipCity(item.CityNew, item.ZipCodeNew);
            }

        }
        
    }
}
