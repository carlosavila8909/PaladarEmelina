using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Paladar_Emelina.Models;
using System.Web.Security;

namespace Paladar_Emelina.Controllers
{
    public class HostalController : Controller
    {
        PaladarEntities ctx = new PaladarEntities();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Utiles.Establecer_Idioma();
            base.OnActionExecuting(filterContext);
        }
        public ActionResult Mostrar()
        {
            return View();
        }

        public ActionResult Ubicacion()
        {
            return View();
        }

        public ActionResult Negocio()
        {
            return View();
        }

        public ActionResult Servicios()
        {
            return View();
        }

        public ActionResult Galeria()
        {
            return View();
        }

        public ActionResult Habitacion1()
        {
            return View();
        }

        public ActionResult Habitacion2()
        {
            return View();
        }

        public ActionResult Comentario()
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
                    comentari.tipo = 2;
                    ctx.Entry(comentari).State = System.Data.Entity.EntityState.Added;
                    ctx.SaveChanges();
                }
                catch (Exception)
                { }
            }
            return RedirectToAction("Comentario");
        }

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            return Redirect(Url.Action("Mostrar", "Hostal"));
        }

        public ActionResult CambiarPassword()
        {
            return View();
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

                    return Redirect(ViewBag.ReturnUrl == "" || ViewBag.ReturnUrl == null ? Url.Action("Configuracion", "AdministracionHostal") : ViewBag.ReturnUrl);
                }
            }
            ViewBag.Uricon = "Hostal";
            return View("~/Views/Shared/Login.cshtml", login_model);
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
                return Redirect(Url.Action("Configuracion", "AdministracionHostal"));
            }
            return View(gestion_password);
        }

        public ActionResult Reservacion()
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
        public ActionResult Reservacion([Bind(Include = "fecha_entrada,fecha_salida,nombre_apellidos,email,habitaciones,cant_acompannantes,mensaje_adicional,pais")] Reservacion_Hostal_Model reservacion)
        {
            ViewBag.habitaciones = reservacion.habitaciones;
            if (reservacion.habitaciones == null || reservacion.habitaciones == "")
                ModelState.AddModelError("habitaciones", Resources.PaladarHostal.Required_habitaciones);

            string[] arr_fecha_entrada = reservacion.fecha_entrada.Split('/');
            string[] arr_fecha_salida = reservacion.fecha_salida.Split('/');

            DateTime fecha_entrada = new DateTime(int.Parse(arr_fecha_entrada[2]), int.Parse(arr_fecha_entrada[1]), int.Parse(arr_fecha_entrada[0]));
            DateTime fecha_salida = new DateTime(int.Parse(arr_fecha_salida[2]), int.Parse(arr_fecha_salida[1]), int.Parse(arr_fecha_salida[0]));

            if (fecha_entrada > fecha_salida)
                ModelState.AddModelError("fecha_entrada", Resources.PaladarHostal.Fecha_ent_may_sal);

            var fecha_entrada_aux = fecha_entrada;
            while (fecha_entrada_aux <= fecha_salida)
            {
                if (ctx.Reservacion_Hostal.Count(c => fecha_entrada_aux.Day >= c.dia_entrada && fecha_entrada_aux.Month >= c.mes_entrada && fecha_entrada_aux.Year >= c.anno_entrada && fecha_entrada_aux.Day <= c.dia_salida && fecha_entrada_aux.Month <= c.mes_salida && fecha_entrada_aux.Year <= c.anno_salida && (reservacion.habitaciones.Contains(c.habitaciones) || c.habitaciones.Contains(reservacion.habitaciones))) > 0)
                {
                    ModelState.AddModelError("fecha_entrada", Resources.PaladarHostal.Existe_reservacion_hostal);
                    break;
                }
                fecha_entrada_aux = fecha_entrada_aux.AddDays(1);
            }

            Reservacion_Hostal nueva_reservacion = null;
            try
            {
                if (ModelState.IsValid)
                {
                    string password = Utiles.Encriptar(Guid.NewGuid().ToString(), 5);
                    nueva_reservacion = ctx.Reservacion_Hostal.Add(new Reservacion_Hostal
                    {
                        nombre_apellidos = reservacion.nombre_apellidos,
                        email = reservacion.email,
                        pais = reservacion.pais,
                        habitaciones = reservacion.habitaciones,
                        dia_entrada = fecha_entrada.Day,
                        mes_entrada = fecha_entrada.Month,
                        anno_entrada = fecha_entrada.Year,
                        dia_salida = fecha_salida.Day,
                        mes_salida = fecha_salida.Month,
                        anno_salida = fecha_salida.Year,
                        cant_acompannantes = reservacion.cant_acompannantes,
                        mensaje_adicional = reservacion.mensaje_adicional,
                        password = Utiles.Encriptar(password, 5)
                    });
                    ctx.SaveChanges();
                    Utiles.Enviar_Email_Reservacion_Cliente(nueva_reservacion.id_reservacion, password, reservacion.nombre_apellidos, reservacion.email, fecha_entrada, fecha_salida, ctx);
                    Utiles.Enviar_Email_Cancelacion_Reservacion_Administrador(nueva_reservacion, "Se ha realizado una nueva reservación con los datos", ctx);
                    Session["msg"] = "OK";
                    return RedirectToAction("Reservacion");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", Resources.PaladarHostal.Error_crear_reservacion);
            }
            return View(reservacion);
        }

        public ActionResult Detalles(int? id)
        {
            if (id != null)
            {
                if (ctx.Reservacion_Hostal.Count(c => c.id_reservacion == id) > 0)
                {
                    ViewBag.id_reservacion = id;
                    ViewBag.accion = 1;
                    return View();
                }
            }
            return RedirectToAction("Mostrar");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles([Bind(Include = "login_reservacion,login_reservacion.email,login_reservacion.password,id_reservacion,id_eliminar_reservacion,reservacion_hostal,reservacion_hostal.fecha_entrada,reservacion_hostal.fecha_salida,reservacion_hostal.nombre_apellidos,reservacion_hostal.email,reservacion_hostal.habitaciones,reservacion_hostal.cant_acompannantes,reservacion_hostal.mensaje_adicional,reservacion_hostal.pais")] Reservacion_Login_Model login_reservacion_hostal)
        {
            ViewBag.id_reservacion = login_reservacion_hostal.id_reservacion;
            if (login_reservacion_hostal.login_reservacion != null)
            {
                string password = Utiles.Encriptar(login_reservacion_hostal.login_reservacion.password, 5);
                var reservacion_hostal = ctx.Reservacion_Hostal.FirstOrDefault(c => c.id_reservacion == login_reservacion_hostal.id_reservacion && c.email == login_reservacion_hostal.login_reservacion.email && c.password == password);
                if (reservacion_hostal != null)
                {
                    ViewBag.accion = 2;

                    DateTime fecha_entrada = new DateTime(reservacion_hostal.anno_entrada, reservacion_hostal.mes_entrada, reservacion_hostal.dia_entrada);
                    DateTime fecha_salida = new DateTime(reservacion_hostal.anno_salida, reservacion_hostal.mes_salida, reservacion_hostal.dia_salida);

                    login_reservacion_hostal.reservacion_hostal = new Reservacion_Hostal_Model { nombre_apellidos = reservacion_hostal.nombre_apellidos, mensaje_adicional = reservacion_hostal.mensaje_adicional, email = reservacion_hostal.email, cant_acompannantes = reservacion_hostal.cant_acompannantes, habitaciones = reservacion_hostal.habitaciones, fecha_entrada = fecha_entrada.ToString("dd/MM/yyyy"), fecha_salida = fecha_salida.ToString("dd/MM/yyyy"), pais = reservacion_hostal.pais };
                    ViewBag.habitaciones = reservacion_hostal.habitaciones;
                    return View(login_reservacion_hostal);
                }
                else
                    ModelState.AddModelError("login_reservacion.email", Resources.PaladarHostal.Credenciales_invalidas);
            }
            else if (login_reservacion_hostal.id_eliminar_reservacion != null)
            {
                try
                {
                    var reservacion_hostal = ctx.Reservacion_Hostal.FirstOrDefault(c => c.id_reservacion == login_reservacion_hostal.id_eliminar_reservacion);
                    ctx.Reservacion_Hostal.Remove(reservacion_hostal);
                    ctx.SaveChanges();
                    Utiles.Enviar_Email_Cancelacion_Reservacion_Administrador(reservacion_hostal, "Se ha cancelado la reservación con los datos", ctx);
                    ModelState.AddModelError("", Resources.PaladarHostal.Reservacion_elimino);
                    ViewBag.accion = 1;
                    return View();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", Resources.PaladarHostal.Error_eliminar_reservacion);
                }
            }
            else
            {
                ViewBag.accion = 2;
                ViewBag.habitaciones = login_reservacion_hostal.reservacion_hostal.habitaciones;

                if (login_reservacion_hostal.reservacion_hostal.habitaciones == null || login_reservacion_hostal.reservacion_hostal.habitaciones == "")
                    ModelState.AddModelError("reservacion_hostal.habitaciones", Resources.PaladarHostal.Required_habitaciones);

                string[] arr_fecha_entrada = login_reservacion_hostal.reservacion_hostal.fecha_entrada.Split('/');
                string[] arr_fecha_salida = login_reservacion_hostal.reservacion_hostal.fecha_salida.Split('/');

                DateTime fecha_entrada = new DateTime(int.Parse(arr_fecha_entrada[2]), int.Parse(arr_fecha_entrada[1]), int.Parse(arr_fecha_entrada[0]));
                DateTime fecha_salida = new DateTime(int.Parse(arr_fecha_salida[2]), int.Parse(arr_fecha_salida[1]), int.Parse(arr_fecha_salida[0]));

                if (fecha_entrada > fecha_salida)
                    ModelState.AddModelError("reservacion_hostal.fecha_entrada", Resources.PaladarHostal.Fecha_ent_may_sal);

                var fecha_entrada_aux = fecha_entrada;
                while (fecha_entrada_aux <= fecha_salida)
                {
                    if (ctx.Reservacion_Hostal.Count(c => c.id_reservacion != login_reservacion_hostal.id_reservacion && fecha_entrada_aux.Day >= c.dia_entrada && fecha_entrada_aux.Month >= c.mes_entrada && fecha_entrada_aux.Year >= c.anno_entrada && fecha_entrada_aux.Day <= c.dia_salida && fecha_entrada_aux.Month <= c.mes_salida && fecha_entrada_aux.Year <= c.anno_salida && (login_reservacion_hostal.reservacion_hostal.habitaciones.Contains(c.habitaciones) || c.habitaciones.Contains(login_reservacion_hostal.reservacion_hostal.habitaciones))) > 0)
                    {
                        ModelState.AddModelError("reservacion_hostal.fecha_entrada", Resources.PaladarHostal.Existe_reservacion_hostal);
                        break;
                    }
                    fecha_entrada_aux = fecha_entrada_aux.AddDays(1);
                }

                try
                {
                    var reservacion_hostal = ctx.Reservacion_Hostal.FirstOrDefault(c => c.id_reservacion == login_reservacion_hostal.id_reservacion);
                    string fecha_entrada_vieja = Utiles.Arreglar_Ceros_Fecha(reservacion_hostal.dia_entrada, reservacion_hostal.mes_entrada, reservacion_hostal.anno_entrada);
                    string fecha_salida_vieja = Utiles.Arreglar_Ceros_Fecha(reservacion_hostal.dia_salida, reservacion_hostal.mes_salida, reservacion_hostal.anno_salida);

                    reservacion_hostal.nombre_apellidos = login_reservacion_hostal.reservacion_hostal.nombre_apellidos;
                    reservacion_hostal.email = login_reservacion_hostal.reservacion_hostal.email;
                    reservacion_hostal.habitaciones = login_reservacion_hostal.reservacion_hostal.habitaciones;
                    reservacion_hostal.dia_entrada = fecha_entrada.Day;
                    reservacion_hostal.mes_entrada = fecha_entrada.Month;
                    reservacion_hostal.anno_entrada = fecha_entrada.Year;
                    reservacion_hostal.pais = login_reservacion_hostal.reservacion_hostal.pais;
                    reservacion_hostal.dia_salida = fecha_salida.Day;
                    reservacion_hostal.mes_salida = fecha_salida.Month;
                    reservacion_hostal.anno_salida = fecha_salida.Year;
                    reservacion_hostal.cant_acompannantes = login_reservacion_hostal.reservacion_hostal.cant_acompannantes;
                    reservacion_hostal.mensaje_adicional = login_reservacion_hostal.reservacion_hostal.mensaje_adicional;
                    ctx.SaveChanges();
                    Utiles.Enviar_Email_Cancelacion_Reservacion_Administrador(reservacion_hostal, String.Format("Se han modificado los datos de la reservación prevista para el {0} hasta el {1} con los datos", fecha_entrada_vieja, fecha_salida_vieja), ctx);
                    ViewBag.msg = "OK";
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", Resources.PaladarHostal.Error_crear_reservacion);
                }
                return View(login_reservacion_hostal);
            }
            ViewBag.accion = 1;
            return View(login_reservacion_hostal);
        }
    }
}