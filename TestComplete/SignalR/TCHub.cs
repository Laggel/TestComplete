using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace TestComplete
{
    [HubName("presentation")]
    public class PresentationHub
        : Hub
    {
        public void GotoSlide(int slideId)
        {
            Clients.showSlide(slideId);
        }

    }

    //public class TCHub : Hub
    //{
    //    public void GetServiceState()
    //    {
    //        Clients.updatePage();
    //    }

    //    public void UpdateServiceState()
    //    {
    //        Clients.updatePage();
    //    }

    //}

    //[HubName("swStats")]
    //public class SWStats : Hub
    //{
    //}

    //public class HealthCheck : Hub
    //{
    //    //public void getServiceState()
    //    //{
    //    //    Clients.updatePage();
    //    //}

    //    public void Distribute()
    //    {
    //        Clients.All.UpdatePage();
    //    }

    //}
}