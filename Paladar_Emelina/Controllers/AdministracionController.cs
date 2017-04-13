using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Paladar_Emelina.Models;
using System.Web.Security;

namespace Paladar_Emelina.Controllers
{
    [Authorize]
    public class AdministracionController : Controller
    {
        PaladarEntities ctx = new PaladarEntities();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Utiles.Establecer_Idioma();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Configuracion()
        {
            return View();
        }

        public ActionResult ConfiguracionGaleria()
        {
            return View();
        }

        public ActionResult ConfiguracionHorario()
        {
            List<Horario> horarios = new List<Horario>();
            try
            {
                horarios = ctx.Horario.Select(c => c).ToList();
            }
            catch (Exception)
            { }
            return View(horarios);
        }

        //GET
        public ActionResult GestionarHorario(int id)
        {
            try
            {
                var horario = ctx.Horario.First(c => c.id_horario == id);
                if (horario.hora_entrada == "Cerrado")
                {
                    horario.hora_entrada = "9:00 AM";
                    horario.hora_salida = "10:00 PM";
                    ViewBag.estado = "Cerrado";
                }
                else
                    ViewBag.estado = "Abierto";
                return View(horario);
            }
            catch (Exception)
            {
                return RedirectToAction("ConfiguracionHorario");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GestionarHorario([Bind(Include = "id_horario,dia,hora_entrada,hora_salida,estado")] Horario horario)
        {
            if (ModelState.IsValid)
            {
                if (Request.Form["estado"] == "Abierto")
                {
                    TimeSpan hora_entrada = Utiles.Obtener_Hora(horario.hora_entrada);
                    TimeSpan hora_salida = Utiles.Obtener_Hora(horario.hora_salida);

                    if (hora_entrada > hora_salida)
                        ModelState.AddModelError("hora_entrada", "La hora de entrada no puede ser mayor que la hora de salida.");
                }
                else
                {
                    horario.hora_entrada = "Cerrado";
                    horario.hora_salida = "Cerrado";
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        ctx.Entry(horario).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                    }
                    catch (Exception)
                    { }
                    return RedirectToAction("ConfiguracionHorario");
                }
            }
            return View(horario);
        }

        public ActionResult ConfiguracionReservacion()
        {
            try
            {
                return View(ctx.Reservacion.OrderByDescending(c => c.anno).ThenByDescending(c => c.mes).ThenByDescending(c => c.dia).ThenBy(c => c.hora).ThenBy(c => c.minuto).ToList());
            }
            catch (Exception)
            {
                return View(new List<Reservacion>());
            }
        }

        [HttpPost]
        public ActionResult EliminarReservacion([Bind(Include = "id_eliminar")] int id_eliminar)
        {
            try
            {
                ctx.Reservacion.RemoveRange(ctx.Reservacion.Where(c => c.id_reservacion == id_eliminar));
                ctx.SaveChanges();
            }
            catch (Exception)
            { }
            return RedirectToAction("ConfiguracionReservacion");
        }

        [HttpPost]
        public ActionResult Reservacion_Result(string fecha)
        {
            try
            {
                if (fecha != "")
                {
                    string[] arr_fecha = fecha.Split('/');
                    int dia = int.Parse(arr_fecha[0]);
                    int mes = int.Parse(arr_fecha[1]);
                    int anno = int.Parse(arr_fecha[2]);
                    return PartialView("_ReservacionesPartial", ctx.Reservacion.Where(c => c.dia == dia && c.mes == mes && c.anno == anno).ToList());
                }
                else
                    return PartialView("_ReservacionesPartial", ctx.Reservacion.OrderByDescending(c => c.anno).ThenByDescending(c => c.mes).ThenByDescending(c => c.dia).ThenBy(c => c.hora).ThenBy(c => c.minuto).ToList());
            }
            catch (Exception)
            {
                return PartialView("_ReservacionesPartial", new List<Reservacion>());
            }
        }

        public ActionResult ConfiguracionComentarios()
        {
            try
            {
                return View(ctx.Comentario.Where(c=> c.tipo == 1).OrderByDescending(c => c.anno).ThenByDescending(c => c.mes).ThenByDescending(c => c.dia).ToList());
            }
            catch (Exception)
            {
                return View(new List<Comentario>());
            }
        }

        [HttpPost]
        public ActionResult EliminarComentario([Bind(Include = "id_eliminar")] int id_eliminar)
        {
            try
            {
                ctx.Comentario.RemoveRange(ctx.Comentario.Where(c => c.id_comentario == id_eliminar));
                ctx.SaveChanges();
            }
            catch (Exception)
            { }
            return RedirectToAction("ConfiguracionComentarios");
        }

        [HttpPost]
        public ActionResult Comentario_Result(string fecha)
        {
            try
            {
                if (fecha != "")
                {
                    string[] arr_fecha = fecha.Split('/');
                    int dia = int.Parse(arr_fecha[0]);
                    int mes = int.Parse(arr_fecha[1]);
                    int anno = int.Parse(arr_fecha[2]);
                    return PartialView("_ComentariosPartial", ctx.Comentario.Where(c => c.tipo == 1 && c.dia == dia && c.mes == mes && c.anno == anno).ToList());
                }
                else
                    return PartialView("_ComentariosPartial", ctx.Comentario.Where(c => c.tipo == 1).OrderByDescending(c => c.anno).ThenByDescending(c => c.mes).ThenByDescending(c => c.dia).ToList());
            }
            catch (Exception)
            {
                return PartialView("_ComentariosPartial", new List<Comentario>());
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
        public JsonResult EliminarImagenesGaleria(string[] arr_img)
        {
            try
            {
                foreach (var item in arr_img)
                {
                    if (System.IO.File.Exists(Server.MapPath(String.Format("~/Content/Galeria/{0}", item))))
                        System.IO.File.Delete(Server.MapPath(String.Format("~/Content/Galeria/{0}", item)));
                }
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
    }
}