using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.WindowsHost.Contracts
{
    //this service contract is used to illustrate contract equivalence. 
    //The service is created here to prevent creation of one more project
    //Contract equivalence means - client and service will have different set of contracts
    //If the contracts are different, they should exist in the same namespace. If they do not we can achieve it via 2 methods
    //Use the namespace attribute



    //In this case the service contract used by service is this one which resides in the host
    // The service contract used by client resides in the proxy project

    //If the contracts are different, they should exist in the same namespace. If they do not we can achieve it via 2 methods
    //Use the namespace attribute

    // IMessageService is the service contract that is invoked by the client and the message passed by the client is written to the host window instead of sending back to the client

    
    [ServiceContract(Namespace = "https://app.pluralsight.com/library/courses/wcf-end-to-end/table-of-contents")]
    public interface IMessageService
    {
        [OperationContract]
        void ShowMessage(string message);
    }
}
