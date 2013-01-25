using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestComplete.Models;

namespace TestComplete.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Seed()
        {
            //new TestComplete.Migrations.Test2().Up();
            

            var db = new UsersContext();
            db.SaveChanges();
            //foreach (var recurso in db.Recursos)
            //{
            //    db.Recursos.Remove(recurso);
            //}
            //db.SaveChanges();

            db.Recursos.Add(new Recurso() { Descripcion = "Usuario 01" });
            db.Recursos.Add(new Recurso() { Descripcion = "Usuario 02" });
            db.Recursos.Add(new Recurso() { Descripcion = "Usuario 03" });
            db.SaveChanges();

            return View();
        }
    }
}
