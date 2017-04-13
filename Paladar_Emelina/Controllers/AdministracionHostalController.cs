using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Paladar_Emelina.Models;

namespace Paladar_Emelina.Controllers
{
    [Authorize]
    public class AdministracionHostalController : Controller
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

        public ActionResult ConfiguracionReservacion()
        {
            try
            {
                return View(ctx.Reservacion_Hostal.Where(c=> c.anno_entrada == DateTime.Now.Year && c.mes_entrada == DateTime.Now.Month).OrderByDescending(c => c.anno_salida).ThenByDescending(c => c.mes_salida).ThenByDescending(c => c.dia_salida).ThenByDescending(c => c.anno_entrada).ThenByDescending(c => c.mes_entrada).ThenByDescending(c => c.dia_entrada).ToList());
            }
            catch (Exception)
            {
                return View(new List<Reservacion_Hostal>());
            }
        }

        public ActionResult Reservacion_Result([Bind(Include = "mes,anno")] int mes, int anno)
        {
            try
            {
                return PartialView("_ReservacionesPartial", ctx.Reservacion_Hostal.Where(c => c.anno_entrada == anno && c.mes_entrada == mes).OrderByDescending(c => c.anno_salida).ThenByDescending(c => c.mes_salida).ThenByDescending(c => c.dia_salida).ThenByDescending(c => c.anno_entrada).ThenByDescending(c => c.mes_entrada).ThenByDescending(c => c.dia_entrada).ToList());
            }
            catch (Exception)
            {
                return PartialView("_ReservacionesPartial", new List<Reservacion_Hostal>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfiguracionReservacion([Bind(Include = "id_eliminar")] int id_eliminar)
        {
            try
            {
                var reservacion_hostal = ctx.Reservacion_Hostal.First(c => c.id_reservacion == id_eliminar);
                ctx.Reservacion_Hostal.Remove(reservacion_hostal);
                Utiles.Enviar_Email_Cancelacion_Cliente(reservacion_hostal, ctx);
                ctx.SaveChanges();
            }
            catch (Exception)
            { }
            return RedirectToAction("ConfiguracionReservacion");
        }

        public ActionResult ConfiguracionComentarios()
        {
            try
            {
                return View(ctx.Comentario.Where(c => c.tipo == 2).OrderByDescending(c => c.anno).ThenByDescending(c => c.mes).ThenByDescending(c => c.dia).ToList());
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
                    return PartialView("_ComentariosPartial", ctx.Comentario.Where(c => c.tipo == 2 && c.dia == dia && c.mes == mes && c.anno == anno).ToList());
                }
                else
                    return PartialView("_ComentariosPartial", ctx.Comentario.Where(c=> c.tipo == 2).OrderByDescending(c => c.anno).ThenByDescending(c => c.mes).ThenByDescending(c => c.dia).ToList());
            }
            catch (Exception)
            {
                return PartialView("_ComentariosPartial", new List<Comentario>());
            }
        }
    }
}