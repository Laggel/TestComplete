using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace TestComplete.Models
{
    public class Queue
    {
        [Key, Column(Order = 0)]
        public int QueueId { get; set; }
         
        public int UserId { get; set; }
        public virtual UserProfile User { get; set; }

        public DateTime FechaEntrada { get; set; }

        public DateTime? FechaSalida { get; set; }

        public bool Estado { get; set; }
    }
}