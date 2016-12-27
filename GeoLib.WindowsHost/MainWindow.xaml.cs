using GeoLib.Contracts;
using GeoLib.Services;
using GeoLib.WindowsHost.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeoLib.WindowsHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    //In Process services are services where both the host and client is the same application
    //So what happens is both host and client tries to use the same thread and hence a deadlock situation occurs
    //In usual scenario , client (on another proj/machine) calls the service (on another proj/machine) so no issue here
    //Here the host and service is the same
    //on click of BtnSendMessgeWPF , service is invoked and a message has to be send that is displayed back in the txtMessage
    //Now since service is also the same proj, this service uses the same thread that the button used and ends up in deadlock situation

    public partial class MainWindow : Window
    {
        public static MainWindow mainUI { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            mainUI = this;
            _syncContext = SynchronizationContext.Current;
            
        }
        
        ServiceHost _messageHost = new ServiceHost(typeof(MessageManager));
        ServiceHost _serviceHost = new ServiceHost(typeof(GeoManager));
        SynchronizationContext _syncContext = null;
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            
            _serviceHost.Open();
            _messageHost.Open();
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _serviceHost.Close();
            _messageHost.Close();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        public void ShowMessage(string message)
        {
            //txtMessage.Content = message; // since label is in the main UI, it will be in the main thread. 
            //Now service is in background thread. so service cannot call this since both are different threads. 
            //This is solved via marshalling
            //Marshalling will wrap the label to execute in main thread so the call of this method is background thread

            int threadId = Thread.CurrentThread.ManagedThreadId;
            SendOrPostCallback callback=new SendOrPostCallback(arg=>{
                txtMessage.Text = message + Environment.NewLine +
                    "marshalled from thread " + threadId +
                    " to thread " + Thread.CurrentThread.ManagedThreadId.ToString();
            });

            _syncContext.Post(callback, null); //we can pass any value here to be used inside callback

        }

        private void BtnSendMessgeWPF_Click(object sender, RoutedEventArgs e)
        {

            //the below code will result in deadlock , since the host and service are accessing the same thread. To avoid this the service should always run on a worker thread and should not disturb the main thread
            #region deadlocksituation

            //we can call the wcf service via proxy or without creation of a proxy
            // we can achieve this via channel factory which is kind of virtual proxy that is created by channel factory

            //ChannelFactory<GeoLib.WindowsHost.Contracts.IMessageService> factory = new ChannelFactory<GeoLib.WindowsHost.Contracts.IMessageService>("inprocessEP"); //IMessageService is the client service contract
            ////empty quote has to be provided there because it is a bug in the WCF. It will not be able to find the endpoint if we leave it empty
            ////If there is more than one endpoin then pass the end point name here

            //GeoLib.WindowsHost.Contracts.IMessageService proxy = factory.CreateChannel(); //this reduces the effort of creating a proxy class

            //proxy.ShowMessage("sample text");
                    

            //factory.Close();

            #endregion

            //the above issue is solved by enclosing it in a thread
            //this way UI also will never be blocked

            Thread thr = new Thread(() =>
            {
                ChannelFactory<GeoLib.WindowsHost.Contracts.IMessageService> factory = new ChannelFactory<GeoLib.WindowsHost.Contracts.IMessageService>("inprocessEP"); //IMessageService is the client service contract
                //empty quote has to be provided there because it is a bug in the WCF. It will not be able to find the endpoint if we leave it empty
                //If there is more than one endpoin then pass the end point name here

                GeoLib.WindowsHost.Contracts.IMessageService proxy = factory.CreateChannel(); //this reduces the effort of creating a proxy class

                proxy.ShowMessage("sample text");


                factory.Close();
            });
            thr.IsBackground = true;
            thr.Start(); 
        }
    }
}
