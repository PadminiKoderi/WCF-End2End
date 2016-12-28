using GeoLib.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.Proxies
{
    //Proxies are tied to the  service contracts. Each proxy has one to one mapping to the service contract
    //Also proxy should implement all the methods in the interface
    //But instead of we defining the methods again (which is incorrect), we have implemented ClientBase that contains channels that will do the same
    public class GeoAdminClient : ClientBase<IGeoAdminService>, IGeoAdminService
    {
        //Parameterized constructor is needed to supply the endpoint name in case of multiple end points
        //Since the base class(ClientBase) has already the parameterized constructor, we can just inherit it
        public GeoAdminClient(string endPointName) : base(endPointName) { }

        //This constructor helps to use enpoint without web.config but by supplying via code
        //Since it is already tied to the service contract, the contract is not there in the constructor
        public GeoAdminClient(Binding binding, EndpointAddress address) : base(binding, address) { }
        
        public void UpdateZipCityBatch(IEnumerable<ZipCityData> cityData)
        {
            Channel.UpdateZipCityBatch(cityData);
        }
        
    }
}
