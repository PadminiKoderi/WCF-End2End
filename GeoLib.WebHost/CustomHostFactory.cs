using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;

namespace GeoLib.WebHost
{
    //To programatically configure end point and stuff , we need access to the service host. 
    //This is done by overriding the service host factory and injecting this as the factory into web.config
    public class CustomHostFactory:ServiceHostFactory
    {
        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHost host = new ServiceHost(serviceType, baseAddresses);
           // host.Open();

            return host;
        }
    }

    //Note
    /*
     * IIS can use only http binding. To use other bindings we should use WAS. To use WAS we need IIS
     * This system does not have WAS and hence there is no code for the same
     * Also the applicationhost.config has to be changed for the same. The script to modify this is available in MSDN.
     */
}