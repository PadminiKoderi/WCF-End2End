using GeoLib.Client.Contracts;
using GeoLib.Contracts;
using GeoLib.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeoLib.Client
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGetZipCode_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtZipCode.Text))
            {
                GeoClient proxy = new GeoClient("tcpEP"); //passing the endpoint name in case of more than one endpoint
                try
                {
                    ZipCodeData zipcodeData = proxy.GetZipInfo(txtZipCode.Text);
                    if (zipcodeData != null)
                    {
                        txtCityInfo.Text = zipcodeData.City;
                        txtStateInfo.Text = zipcodeData.State;
                    }
                    proxy.Close();
                }
                catch (FaultException<ExceptionDetail> fex)
                {
                    txtCityInfo.Text = "type " + fex.GetType().Name + " message " + fex.Detail.Message + " state " + proxy.State.ToString();
                    //exception will be type FaultException`1 message Error getting data. proxy state Faulted for unhandled exception meaning oly the includeexceptioninfaults is turned on
                }
                catch (FaultException<ApplicationException> fex)
                {
                    txtCityInfo.Text = "type " + fex.GetType().Name + " message " + fex.Detail.Message + " state " + proxy.State.ToString();
                    //exception will be type FaultException`1 message Error getting data. proxy state Faulted for unhandled exception meaning oly the includeexceptioninfaults is turned on state should be opened. but it is not working
                }
                catch (FaultException fex)
                {
                    txtCityInfo.Text = "type " + fex.GetType().Name + " message " + fex.Message + " state " + proxy.State.ToString();
                    //exception will be type FaultException`1 message Error getting data. proxy state Faulted for unhandled exception meaning oly the includeexceptioninfaults is turned on state should be opened. but it is not working
                }
                catch (Exception ex)
                {
                    txtCityInfo.Text = "type " + ex.GetType().Name + " message " + ex.Message + " state " + proxy.State.ToString();
                    //exception will be type FaultException`1 message Error getting data. proxy state Faulted for unhandled exception meaning oly the includeexceptioninfaults is turned on
                }
                
            }
        }

        protected void btnGetState_Click(object sender, EventArgs e)
        {
            Binding binding=new NetTcpBinding();
            EndpointAddress address=new EndpointAddress("net.tcp://localhost:8009/GeoService");//passing abc instead of using it from config
            
            if (!String.IsNullOrEmpty(txtState.Text))
            {
                GeoClient proxy = new GeoClient(binding,address);
                IEnumerable<ZipCodeData> zipCodeData = proxy.GetZips(txtState.Text);
                if (zipCodeData != null)
                {
                    lbStateData.DataSource = zipCodeData;
                    lbStateData.DataTextField = "CITY";
                    lbStateData.DataValueField = "ZIPCODE";
                    lbStateData.DataBind();
                }

                proxy.Close();
            }
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            //we can call the wcf service via proxy or without creation of a proxy
            // we can achieve this via channel factory which is kind of virtual proxy that is created by channel factory

            ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>(""); //IMessageService is the client service contract
            //empty quote has to be provided there because it is a bug in the WCF. It will not be able to find the endpoint if we leave it empty
            //If there is more than one endpoin then pass the end point name here

            IMessageService proxy = factory.CreateChannel(); //this reduces the effort of creating a proxy class

            //proxy.ShowMessage(txtMessage.Text);

            proxy.ShowMsg(txtMessage.Text);
             
            factory.Close();

        }
    }
}