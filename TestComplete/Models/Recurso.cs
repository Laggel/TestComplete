using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace TestComplete.Models
{
    public class Recurso
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecursoId { get; set; }
        
        public string Descripcion { get; set; }

        public virtual IQueryable<RecursoUsuario> RecursoUsuario { get; set; }

    }

    public class RecursoModel
    {
        public int RecursoId { get; set; }

        public string Descripcion { get; set; }

        public bool Estado { get; set; }

        public string Usuario { get; set; }

    }
}