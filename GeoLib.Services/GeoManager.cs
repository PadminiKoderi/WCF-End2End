using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoLib.Contracts;
using GeoLib.Data;
using System.ServiceModel;
using System.Transactions;
using System.Windows.Forms;
using System.Security.Principal;
using System.Threading;
using System.Security.Permissions;
namespace GeoLib.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
    //[ServiceBehavior(ReleaseServiceInstanceOnTransactionComplete=false)] //wen we use transactions, it becomes per call. to make it per session, this setting is needed
    //[ServiceBehavior(ReleaseServiceInstanceOnTransactionComplete = false,InstanceContextMode=InstanceContextMode.PerCall])//per call client manual transaction
    //[ServiceBehavior(ReleaseServiceInstanceOnTransactionComplete = false,InstanceContextMode=InstanceContextMode.PerSession])//per session client manual transaction
    public class GeoManager:IGeoService,IGeoAdminService

    {
        #region identitynotes
        //client identity is passed to the service
        //once in servive there are 3 types of identity - host - identity service host is running under which will be used so the caller's identity is not used here; primary - identity of the client is passed and is available only after the service constructor ; windows - same as primary in case windows authentication is used else empty 

        //string hostIdentity = WindowsIdentity.GetCurrent().Name;
        //string primaryIdentity = ServiceSecurityContext.Current.PrimaryIdentity.Name;
        //string windowsIdentity = ServiceSecurityContext.Current.WindowsIdentity.Name;
        //string threadIdentity = Thread.CurrentPrincipal.Identity.Name;//this will get the value of primary identity after the service constructor is hit

        //Intranet security
        //netTcpBinding to be used ; default security mode is transport ; message protection level is none, signed, encrypted & signed(default) ; client credential type - how client will authenticate to the service - none, certificate, windows ; primaryidentity is used

        //Internet security - service level
        //wshttpBinding ; message security mode ; client credential type - how client will authenticate to the service - none, certificate, windows,username (manual) ; must use certificate ; negotiate security essentials settings

        //Internet security - client level
        //PeerTrust - client will have certi (without private key) or ChainTrust - client must have encoded public key 
        #endregion
        public ZipCodeData GetZipInfo(string zip)
        {
            //throw new ApplicationException("Zip code not found"); 
            //throw new FaultException("Zip code not found");
            //ApplicationException app = new ApplicationException("Zip not found");
            //throw new FaultException<ApplicationException>(app, "just another message");


            //string hostIdentity = WindowsIdentity.GetCurrent().Name; //ITLINFOSYS\Padmini_Koderi
            //string primaryIdentity = ServiceSecurityContext.Current.PrimaryIdentity.Name; //this will break becoz security mode is none for IGeoService
            //string windowsIdentity = ServiceSecurityContext.Current.WindowsIdentity.Name; //this will break becoz security mode is none for IGeoService
            //string threadIdentity = Thread.CurrentPrincipal.Identity.Name;

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
            IEnumerable<ZipCode> zips = null;// zipCodeRepository.GetZipsForRange(zipCodeEntity, range);
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
        //the below is afor automatic transaction
        [OperationBehavior(TransactionScopeRequired=true)]//if there is an existing transaction, setting this true will join both the tran. if false, it will not join
            //if operation succeeds, the operation will vote to commit , else rollback
            //with this setting no tran needed at DB side

        //the below is for manual transaction
        //[OperationBehavior(TransactionScopeRequired=false)]

        [PrincipalPermission(SecurityAction.Demand,Role="Administrators")] //this allows oly users belonging to windows admin grp to do this operation
        //if role is not mentioned, it will oly allow authenticated users to use this and will not be specific to roles
        public void UpdateZipCityBatch(IEnumerable<ZipCityData> batchData)
        {
            IZipCodeRepository zipCodeRepository = new ZipCodeRepository();

            string hostIdentity = WindowsIdentity.GetCurrent().Name; //ITLINFOSYS\Padmini_Koderi
            string primaryIdentity = ServiceSecurityContext.Current.PrimaryIdentity.Name; //ITLINFOSYS\Padmini_Koderi //this is becoz in this case both client and service are on the same machine
            string windowsIdentity = ServiceSecurityContext.Current.WindowsIdentity.Name; //ITLINFOSYS\Padmini_Koderi
            string threadIdentity = Thread.CurrentPrincipal.Identity.Name; //ITLINFOSYS\Padmini_Koderi
            bool isAdmin = Thread.CurrentPrincipal.IsInRole("admin"); //this can also be used to chk for roles
            #region autotransaction
            int counter = 0;
            foreach (ZipCityData item in batchData)
            {
                counter++;
                if (counter == 2)
                    throw new FaultException("sec zip cannot be updated");
                zipCodeRepository.UpdateZipCity(item.CityNew, item.ZipCodeNew);
            }
            #endregion
            #region manualtransaction
            //using (TransactionScope scope = new TransactionScope())
            //{
            //    int counter = 0;
            //    foreach (ZipCityData item in batchData)
            //    {
            //        counter++;
            //        if (counter == 2)
            //            throw new FaultException("sec zip cannot be updated");
            //        zipCodeRepository.UpdateZipCity(item.CityNew, item.ZipCodeNew);
            //    }
            //    scope.Complete(); //this will perform voting and manually commit if the operation is success else rollback
            //}

            #endregion
        }

        public void RequestResponseCall()
        {

            MessageBox.Show("Request response call in the service");

        }
        public void OneWayCall()
        {
            MessageBox.Show("one way call in the service");
        }
        


    }
}
