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
using System.Transactions;
using System.Windows.Forms;
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
                    //exception will be type FaultException<applicationexception> message Error getting data. proxy state opened 
                }
                catch (FaultException fex)
                {
                    txtCityInfo.Text = "type " + fex.GetType().Name + " message " + fex.Message + " state " + proxy.State.ToString();
                    //exception will be type FaultException`1 message Error getting data. proxy state opened
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
            System.ServiceModel.Channels.Binding binding=new NetTcpBinding();
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

        protected void btnUpdateCityBatch_Click(object sender, EventArgs e)
        {
            List<ZipCityData> cityBatch = new List<ZipCityData>(){
                new ZipCityData(){ZipCodeNew="00501",CityNew="Dallas"},
                new ZipCityData(){ZipCodeNew="00602", CityNew="Irving"}
            };
            #region percallclientmanualtransaction
            //try
            //{
            //    GeoClient proxy = new GeoClient("tcpEP");
            //    using (TransactionScope scope = new TransactionScope())
            //    {
                    
            //        proxy.UpdateZipCityBatch(cityBatch);
            //        scope.Complete();
            //    }
            //    proxy.Close();
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            #endregion
            #region persessionclientmanualtransaction
            //try
            //{
            //    GeoClient proxy = new GeoClient("tcpEP");
            //    using (TransactionScope scope = new TransactionScope())
            //    {

            //        proxy.UpdateZipCityBatch(cityBatch);
            //        proxy.Close();
            //        scope.Complete();
            //    }
            //    
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            #endregion
            try
            {
                GeoAdminClient proxy = new GeoAdminClient("tcpEP");
                proxy.UpdateZipCityBatch(cityBatch);
                proxy.Close();
                //if an error is thrown here, the update will still be rolled back because propagation of transaction is turned on the service -> TransactionFlowOption.Allowed
                //if an error is thrown here ,the update will happen succesfully, if the transaction flow is disallowed on the service -> TransactionFlowOption.NotAllowed

            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        #region OperationNotes
        //Request - response call - call goes to the service, does the operation and comes back to the client. In the below eg. first service message will be printed then client message
        //One way call - fire and forget - shul be of type void - isoneway=true in operationcontract - no soap response so no fault handling - when it is a transport session, proxy closes, client will be blocked untill call complete
        //in the below eg, both the client first and the service message will happen simultaneously and then after the service is closed, client second message will come as transport session blocks the client
        //Duplex call - service calls back the client - call back only with transport session(tcp,icp, wshttp with security n reliability on)
        //no code for this . refer the site
        #endregion
        protected void btnOneWayCall_Click(object sender, EventArgs e)
        {
            #region requestresponsecall
            GeoClient proxy = new GeoClient("tcpEP");
            proxy.RequestResponseCall();
            txtCityInfo.Text = "Request response call in the client";
            proxy.Close();
            #endregion

            //#region onewaycall
            //GeoClient proxy = new GeoClient("tcpEP");
            //proxy.OneWayCall();
            ////txtCityInfo.Text = "One way call in the client";
            //MessageBox.Show("One way call in the client");
            //proxy.Close();
            //MessageBox.Show("Transport session blocked client");//this wil be displayed only after servce message is closed
            //#endregion
        }
    }
}