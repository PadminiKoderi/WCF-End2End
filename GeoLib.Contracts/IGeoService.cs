using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Contracts
{
    [ServiceContract]
    public interface IGeoService
    {
        [OperationContract]
        [FaultContract(typeof(ApplicationException))]////this will make the client to anticiapte the exception type
        ZipCodeData GetZipInfo(string zip);
        [OperationContract]
        IEnumerable<string> GetStates(bool primayOnly);
        [OperationContract(Name="GetZipsByState")] //this will help in resolving naming conflicts for lang that do not know method overloading
        IEnumerable<ZipCodeData> GetZips(string state);
        [OperationContract(Name = "GetZipsForRange")]       
        IEnumerable<ZipCodeData> GetZips(string zip, int range);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)] //this setting is needed, if the transaction is starting at client level. As a best practice, always set this
        void UpdateZipCityBatch(IEnumerable<ZipCityData> cityData);
    }
}
