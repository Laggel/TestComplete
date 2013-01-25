using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestComplete.Models
{
    
    public class DashboardModel
    {
        public bool alreadyQueued { get; set; }

        public List<RecursoModel> Recursos { get; set; }

        public List<UserProfile> Usuarios { get; set; }
    }
}