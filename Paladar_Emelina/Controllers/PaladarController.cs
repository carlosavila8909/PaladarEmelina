using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Paladar_Emelina.Models;
using System.Web.Security;
using System.IO;

namespace Paladar_Emelina.Controllers
{
    public class PaladarController : Controller
    {
        PaladarEntities ctx = new PaladarEntities();

        [HttpPost]
        public ActionResult Set_Idioma(string lenguaje)
        {
            Utiles.Adicionar_Cookie_Idioma(lenguaje);
            return Json("OK");
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Utiles.Establecer_Idioma();
            base.OnActionExecuting(filterContext);
        }
        // GET: Paladar
        public ActionResult Mostrar()
        {
            return View(Utiles.Obtener_Config_Horarios(ctx));
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult Negocio()
        {
            return View();
        }

        public ActionResult Chefs()
        {
            return View();
        }

        public ActionResult Galeria()
        {
            return View();
        }

        public ActionResult Contactenos()
        {
            if (Session["msg"] != null)
            {
                ViewBag.msg = "OK";
                Session.Remove("msg");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contactenos([Bind(Include = "nombre_apellidos,email,mensaje")] Contactenos_Model contact_model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Utiles.Enviar_Email_Contactenos(contact_model, ctx);
                    Session["msg"] = "OK";
                    return RedirectToAction("Contactenos");
                }
            }
            catch (Exception)
            {
                ViewBag.msg = "Error";
            }
            return View(contact_model);
        }

        public ActionResult Plato(int id)
        {
            try
            {
                DirectoryInfo[] carpetas = new DirectoryInfo(Server.MapPath("~/Content/Menu")).GetDirectories();
                for (int i = 0; i < carpetas.Length; i++)
                {
                    DirectoryInfo[] menus = carpetas[i].GetDirectories();
                    for (int j = 0; j < menus.Length; j++)
                    {
                        if (int.Parse(menus[j].Name) == id)
                        {
                            ViewBag.Categoria = carpetas[i].Name;
                            ViewBag.Carpeta = menus[j].Name;
                            return View(Utiles.Obtener_Datos_XML(Server.MapPath(String.Format("~/Content/Menu/{0}/{1}/datos.xml", carpetas[i].Name, menus[j].Name))));
                        }
                    }
                }
                return RedirectToAction("Mostrar");
            }
            catch (Exception)
            {
                return RedirectToAction("Mostrar");
            }
        }
        
        public ActionResult Reservacion()
        {
            if (Session["msg"] != null)
            {
                ViewBag.msg = "OK";
                Session.Remove("msg");
            }
            return View(new ReservacionModel { nombre_apellidos = "", fecha = DateTime.Now.ToString("dd/MM/yyyy"), hora = "11:00 AM", cant_acompannantes = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reservacion([Bind(Include = "nombre_apellidos,cant_acompannantes,fecha,hora")] ReservacionModel reservacionModel)
        {
            try
            {
                string[] arr_fecha = reservacionModel.fecha.Split('/');
                List<int> hora_militar = Utiles.Obtener_Hora_Militar(reservacionModel.hora);

                TimeSpan horario_minimo_reserva = new TimeSpan(11, 0, 0);
                TimeSpan horario_maximo_reserva = new TimeSpan(21, 0, 0);
                DateTime fecha_reservacion = new DateTime(int.Parse(arr_fecha[2]), int.Parse(arr_fecha[1]), int.Parse(arr_fecha[0]), hora_militar[0], hora_militar[1], 0);

                TimeSpan horario_reserva = new TimeSpan(hora_militar[0], hora_militar[1], 0);
                if (horario_reserva < horario_minimo_reserva || horario_reserva > horario_maximo_reserva)
                    ModelState.AddModelError("hora", Resources.PaladarHostal.Error_horario_reservacion);

                if (fecha_reservacion < DateTime.Now)
                    ModelState.AddModelError("fecha", Resources.PaladarHostal.Error_fecha_reservacion);

                if (ModelState.IsValid)
                {
                    Models.Reservacion nueva_reservacion = new Models.Reservacion { nombre_apellidos = reservacionModel.nombre_apellidos, cant_acompannantes = reservacionModel.cant_acompannantes, dia = fecha_reservacion.Day, mes = fecha_reservacion.Month, anno = fecha_reservacion.Year, hora = hora_militar[0], minuto = hora_militar[1] };
                    ctx.Entry(nueva_reservacion).State = System.Data.Entity.EntityState.Added;
                    ctx.SaveChanges();

                    Session["msg"] = "OK";
                    return RedirectToAction("Reservacion");
                }
            }
            catch (Exception)
            { }
            return View(reservacionModel);
        }
        
        public ActionResult Comentario()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comentario([Bind(Include = "id_comentario,nombre_ap,comentario")] Comentario comentari)
        {
            if (comentari.id_comentario == 0)
            {
                if (ModelState.ContainsKey("id_comentario"))
                    ModelState.Remove("id_comentario");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comentari.dia = DateTime.Now.Day;
                    comentari.mes = DateTime.Now.Month;
                    comentari.anno = DateTime.Now.Year;
                    comentari.tipo = 1;
                    ctx.Entry(comentari).State = System.Data.Entity.EntityState.Added;
                    ctx.SaveChanges();
                }
                catch (Exception)
                { }
            }
            return RedirectToAction("Comentario");
        }

        public ActionResult Comentar()
        {
            return View();
        }

        public ActionResult Autenticacion(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View("~/Views/Shared/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Autenticacion([Bind(Include = "email,password")] LoginModel login_model)
        {
            ViewBag.ReturnUrl = Request.Form["ReturnUrl"];
            if (ModelState.IsValid)
            {
                string password_hash = Utiles.Encriptar(login_model.password, 10);
                var administrador = ctx.Administrador.FirstOrDefault(c => c.email == login_model.email && c.password == password_hash);
                if (administrador == null)
                    ModelState.AddModelError("usuario", "Las credenciales no son válidas.");
                else
                {
                    if (!User.Identity.IsAuthenticated)
                        FormsAuthentication.SetAuthCookie("Administrador", false);

                    return Redirect(ViewBag.ReturnUrl == "" || ViewBag.ReturnUrl == null ? Url.Action("Configuracion", "Administracion") : ViewBag.ReturnUrl);
                }
            }
            return View("~/Views/Shared/Login.cshtml", login_model);
        }

        public ActionResult CerrarSesion()
        {
            if (User.Identity.IsAuthenticated)
                FormsAuthentication.SignOut();

            return Redirect(Url.Action("Mostrar", "Paladar"));
        }

        public ActionResult CambiarPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarPassword([Bind(Include = "pass_anterior,nuevo_pass,confirmar_pass")] Gestion_Password gestion_password)
        {
            if (ModelState.IsValid)
            {
                string password_hash = Utiles.Encriptar(gestion_password.nuevo_pass, 10);
                try
                {
                    ctx.Administrador.FirstOrDefault().password = password_hash;
                    ctx.SaveChanges();
                }
                catch (Exception)
                { }
                return Redirect(Url.Action("Configuracion", "Administracion"));
            }
            return View(gestion_password);
        }

        [HttpPost]
        public JsonResult Validar_Password_Anterior([Bind(Include = "pass_anterior")] string pass_anterior)
        {
            try
            {
                string password_hash = Utiles.Encriptar(pass_anterior, 10);
                var administrador = ctx.Administrador.FirstOrDefault(c => c.password == password_hash);
                return Json(administrador != null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadFileGaleria()
        {
            HttpPostedFileBase fichero = Request.Files[0];
            fichero.SaveAs(String.Format("{0}/{1}", Server.MapPath("~/Content/Galeria"), fichero.FileName));
            return Json(new { Message = fichero.FileName });
        }

        [HttpPost]
        public JsonResult EliminarImagenGaleria(string dir_imagen)
        {
            try
            {
                System.IO.File.Delete(Server.MapPath(String.Format("~/Content/Galeria/{0}", dir_imagen)));
                return Json("OK");
            }
            catch (Exception)
            {
                return Json("Error");
            }
        }

        [HttpPost]
        public ActionResult ImagenesGaleria_Result()
        {
            return PartialView("_ImagenesGaleriaPartial");
        }

        public ActionResult Ubicacion()
        {
            return View();
        }
    }
}