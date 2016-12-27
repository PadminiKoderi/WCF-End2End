using GeoLib.WindowsHost.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.WindowsHost.Services
{
    [ServiceBehavior(UseSynchronizationContext = false)] //this will make the service run on a background thread
    public class MessageManager:IMessageService
    {
        
        public void ShowMessage(string message)
        {
            MainWindow.mainUI.ShowMessage(message);
        }
    }
}
