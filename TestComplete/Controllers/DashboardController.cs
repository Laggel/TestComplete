using SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
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

                recurso.Usuario = (from r in db.RecursoUsuarios
                                   where r.RecursoId == recurso.RecursoId
                                      && !r.Estado
                                   select r.User.UserName).FirstOrDefault();

                recurso.Estado = recurso.Usuario == null;
                     

                //if (!recurso.Estado)
                //{
                //    recurso.Usuario = db.RecursoUsuarios.Where(r => r.RecursoId == recurso.RecursoId
                //                                               && !r.Estado)
                //                                               .FirstOrDefault().User.UserName;
                                                               
                //}
            }

            return recursos;

        }

        private List<UserProfile> loadQueue()
        {   
            return db.Queues.Where(r => !r.Estado).Select(r => r.User).ToList();
        }

        private void Assign(int userId)
        {
            var test = (from ru in db.RecursoUsuarios
                        where !ru.Estado
                        select ru.RecursoId);

            var recursos = (from r in db.Recursos
                            where !test.Contains(r.RecursoId)
                            select r).ToList();

            var queue = (from q in db.Queues
                         where !q.Estado
                         select q)
                         .OrderBy(q=>q.FechaEntrada);

            if (queue != null)
            {
                var lista = queue.ToList();

                foreach (var r in recursos)
                {
                    for (var i = 0; i < lista.Count(); i++)
                    {
                        var usuario = lista[i];

                        if ((usuario.RecursoId == r.RecursoId //Is the resource specified
                            || usuario.RecursoId == null) //Has not specified any particular resource
                            && (!usuario.Estado) //Has not yet be assigned
                            )
                        {
                            var recursoUsuario = new RecursoUsuario()
                            {
                                UserId = usuario.UserId,
                                RecursoId = r.RecursoId,
                                FechaEntrada = DateTime.Now,
                                Estado = false
                            };

                            db.RecursoUsuarios.Add(recursoUsuario);

                            usuario.Estado = true;
                            usuario.FechaSalida = DateTime.Now;
                            db.SaveChanges();

                            if (usuario.UserId != userId)
                                sendMail(usuario.User.UserName);
                        }
                        
                    }
                }
                db.SaveChanges();
            }
        }

        private UserProfile GetCurrentUser()
        {
            return db.UserProfiles.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
        }

        private bool isQueue()
        {
            var userId = GetCurrentUser().UserId;

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

        private void sendMail(string email)
        {
            var smtp = new SmtpClient("smtp.gmail.com");
            smtp.UseDefaultCredentials = false;
            var credentials = new System.Net.NetworkCredential("delacruzpaulino@gmail.com", "Laggel008");
            smtp.Credentials = credentials;
            smtp.EnableSsl = true;
            smtp.Port = 587;

            var message = new System.Net.Mail.MailMessage();
            message.From = new MailAddress("TestComplete@SimetricaConsulting.com");
            message.To.Add(email);
            message.Subject = "Disponibilidad de Test Complete";
            message.Body = "Saludos \n\n"
                          + "Notificamos que existe un usuario libre en Test Complete y ha sido asignado a Usted.\n\n"
                          + "En caso de que ya no necesite utilizarlo favor dar clic aqui:"
                          + @"http://listadeespera.apphb.com/Dashboard/Liberar";

            smtp.Send(message);
        }

        public ActionResult Esperar(int recursoId = 0)
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
                
                if (recursoId != 0)
                    usuario.RecursoId = recursoId;

                usuario.UserId = userId;
                db.Queues.Add(usuario);
                db.SaveChanges();
                Assign(userId);
            }

            Broadcast();
            return RedirectToAction("Index");
        }

        public ActionResult Usar(int RecursoId)
        {
            alreadyQueued = isQueue();

            if (!alreadyQueued)
            {
                var recursoUsuario = new RecursoUsuario()
                {
                    UserId = GetCurrentUser().UserId,
                    RecursoId = RecursoId,
                    FechaEntrada = DateTime.Now,
                    Estado = false
                };

                db.RecursoUsuarios.Add(recursoUsuario);
                db.SaveChanges();
            }

            Broadcast();
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
            {
                queue.Estado = true;
                queue.FechaSalida = DateTime.Now;
            }
            
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
                    r.FechaSalida = DateTime.Now;
                }
            }

            db.SaveChanges();
            Assign(0);
            Broadcast();
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

        public void Broadcast()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<PresentationHub>();
            context.Clients.showSlide("hello");
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