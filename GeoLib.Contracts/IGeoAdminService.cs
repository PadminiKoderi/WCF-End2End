using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
namespace GeoLib.Contracts
{
    [ServiceContract]
    public interface IGeoAdminService
    {
        
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)] //this setting is needed, if the transaction is starting at client level. As a best practice, always set this
        void UpdateZipCityBatch(IEnumerable<ZipCityData> cityData);
    }
}
