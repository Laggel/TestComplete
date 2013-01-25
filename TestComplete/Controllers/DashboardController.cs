using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestComplete.Models;

namespace TestComplete.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private UsersContext db = new UsersContext();
        private static DashboardModel dashboard = new DashboardModel();
        private static bool alreadyQueued = false;

        private List<RecursoModel> loadRecursos()
        {
            var recursos = (from r in db.Recursos
                            select new RecursoModel() { RecursoId = r.RecursoId, Descripcion = r.Descripcion }
                           ).ToList();

            foreach (var recurso in recursos)
            {
                recurso.Estado = !(from r in db.RecursoUsuarios
                                   where r.RecursoId == recurso.RecursoId
                                      && !r.Estado
                                   select r).Any();

                if (!recurso.Estado)
                {
                    recurso.Usuario = db.RecursoUsuarios.Where(r => r.RecursoId == recurso.RecursoId
                                                               && !r.Estado)
                                                               .FirstOrDefault().User.UserName;
                                                               
                }
            }

            return recursos;

        }

        private List<UserProfile> loadQueue()
        {
            return (from r in db.Queues
                            where !r.Estado
                            select r.User
                            ).ToList();

        }

        private void Assign()
        {

            var test = (from ru in db.RecursoUsuarios
                        where !ru.Estado
                        select ru.RecursoId);

            //!(from ru in db.RecursoUsuarios
            //                         where ru.RecursoId == r.RecursoId
            //                         && !ru.Estado
            //                         select ru).Any()
            var recursos = (from r in db.Recursos
                            where !test.Contains(r.RecursoId)
                            select r);

            var queue = (from q in db.Queues
                         where !q.Estado
                         select q)
                         .OrderBy(q=>q.FechaEntrada);

            if (queue != null)
            {
                var lista = queue.ToList();

                int i = 0;
                foreach (var r in recursos)
                {
                    if (lista.Count() != i)
                    {
                        var usuario = lista[i];
                        var recursoUsuario = new RecursoUsuario()
                        {
                            UserId = usuario.UserId,
                            RecursoId = r.RecursoId,
                            FechaEntrada = DateTime.Now,
                            Estado = false
                        };
                        i++;
                        db.RecursoUsuarios.Add(recursoUsuario);

                        usuario.Estado = true;
                    }
                }
                db.SaveChanges();
            }
        }

        public bool isQueue()
        {
            var userId = db.UserProfiles.Where(r => r.UserName == User.Identity.Name).FirstOrDefault().UserId;

            return (from r in db.Queues
                             where !r.Estado
                                && r.UserId == userId
                             select r).Any()
                           ||
                        (from r in db.RecursoUsuarios
                         where !r.Estado
                            && r.UserId == userId
                         select r).Any()
                           ;
        }



        public ActionResult Esperar()
        {
            var userId = db.UserProfiles.Where(r => r.UserName == User.Identity.Name).FirstOrDefault().UserId;

            alreadyQueued = isQueue();

            if (!alreadyQueued)
            {

                var usuario = new Queue()
                {
                    FechaEntrada = DateTime.Now,
                    Estado = false
                };
                usuario.UserId = userId;
                db.Queues.Add(usuario);
                db.SaveChanges();
                Assign();
            }
            
            return RedirectToAction("Index");
        }


        public ActionResult Liberar()
        {
            var userId = db.UserProfiles.Where(r => r.UserName == User.Identity.Name).FirstOrDefault().UserId;

            var queue = (from r in db.Queues
                             where !r.Estado
                                && r.UserId == userId
                             select r).SingleOrDefault();
            
            if (queue != null)            
                queue.Estado = true;
            
            var rec = (from r in db.RecursoUsuarios
                         where !r.Estado
                            && r.UserId == userId
                         select r)
                           ;

            if (rec != null)
            {
                foreach (var r in rec)
                {
                    r.Estado = true;
                }
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            if (alreadyQueued)
            {
                ModelState.AddModelError(string.Empty, "Usted ya esta en lista de espera o usando un usuario.");
                alreadyQueued = false;
            }
            
            dashboard.Recursos = loadRecursos();
            dashboard.Usuarios = loadQueue();
            dashboard.alreadyQueued = isQueue();

            return View(dashboard);
        }

        //
        // GET: /Dashboard/Details/5

        public ActionResult Details(int id = 0)
        {
            Recurso recurso = db.Recursos.Find(id);
            if (recurso == null)
            {
                return HttpNotFound();
            }
            return View(recurso);
        }

        //
        // GET: /Dashboard/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Dashboard/Create

        [HttpPost]
        public ActionResult Create(Recurso recurso)
        {
            if (ModelState.IsValid)
            {
                db.Recursos.Add(recurso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(recurso);
        }

        //
        // GET: /Dashboard/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Recurso recurso = db.Recursos.Find(id);
            if (recurso == null)
            {
                return HttpNotFound();
            }
            return View(recurso);
        }

        //
        // POST: /Dashboard/Edit/5

        [HttpPost]
        public ActionResult Edit(Recurso recurso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recurso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recurso);
        }

        //
        // GET: /Dashboard/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Recurso recurso = db.Recursos.Find(id);
            if (recurso == null)
            {
                return HttpNotFound();
            }
            return View(recurso);
        }

        //
        // POST: /Dashboard/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Recurso recurso = db.Recursos.Find(id);
            db.Recursos.Remove(recurso);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}