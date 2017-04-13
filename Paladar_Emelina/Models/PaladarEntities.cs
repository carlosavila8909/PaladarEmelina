using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using System.Web.Mvc;
using System.Globalization;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

namespace Paladar_Emelina.Models
{
    public class PaladarEntities : DbContext
    {
        public PaladarEntities()
        : base("name=DefaultConnection")
        {
        }
        
        public DbSet<Horario> Horario { get; set; }
        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<Administrador> Administrador { get; set; }
        public DbSet<Reservacion> Reservacion { get; set; }
        public DbSet<Reservacion_Hostal> Reservacion_Hostal { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class ReservacionModel
    {
        [Required(ErrorMessageResourceName = "Required_nombre_apellidos", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string nombre_apellidos { get; set; }

        [Required(ErrorMessageResourceName = "Required_acompannantes", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public int cant_acompannantes { get; set; }

        [Required(ErrorMessageResourceName = "Required_fecha", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string fecha { get; set; }

        [Required(ErrorMessageResourceName = "Required_hora", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string hora { get; set; }
    }

    public class Reservacion_Hostal
    {
        [Key]
        public int id_reservacion { get; set; }
        public string nombre_apellidos { get; set; }
        public string email { get; set; }
        public string pais { get; set; }
        public string habitaciones { get; set; }
        public int dia_entrada { get; set; }
        public int mes_entrada { get; set; }
        public int anno_entrada { get; set; }
        public int dia_salida { get; set; }
        public int mes_salida { get; set; }
        public int anno_salida { get; set; }
        public int cant_acompannantes { get; set; }
        public string mensaje_adicional { get; set; }
        public string password { get; set; }
    }         

    public class Reservacion_Hostal_Model
    {
        [Required(ErrorMessageResourceName = "Required_nombre_apellidos", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string nombre_apellidos { get; set; }

        [EmailAddress(ErrorMessageResourceName = "Error_Email", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        [Required(ErrorMessageResourceName = "Required_email", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string email { get; set; }

        [Required(ErrorMessageResourceName = "Required_fecha_entrada", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string fecha_entrada { get; set; }

        [Required(ErrorMessageResourceName = "Required_fecha_salida", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string fecha_salida { get; set; }

        [Required(ErrorMessageResourceName = "Required_pais", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string pais { get; set; }

        public string habitaciones { get; set; }
        public int cant_acompannantes { get; set; }
        public string mensaje_adicional { get; set; }
    }

    public class Reservacion
    {
        [Key]
        public int id_reservacion { get; set; }
        public string nombre_apellidos { get; set; }
        public int cant_acompannantes { get; set; }
        public int dia { get; set; }
        public int mes { get; set; }
        public int anno { get; set; }
        public int hora { get; set; }
        public int minuto { get; set; }
    }

    public class Horario
    {
        [Key]
        public int id_horario { get; set; }
        public string dia { get; set; }
        public string hora_entrada { get; set; }
        public string hora_salida { get; set; }
    }

    public class Administrador
    {
        [Key]
        public string email { get; set; }
        public string password { get; set; }
        public string host_internet { get; set; }
    }

    public class Comentario
    {
        [Key]
        public int id_comentario { get; set; }

        [Required(ErrorMessageResourceName = "Required_nombre_apellidos", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string nombre_ap { get; set; }

        [Required(ErrorMessageResourceName = "Required_comentario", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string comentario { get; set; }
        public int dia { get; set; }
        public int mes { get; set; }
        public int anno { get; set; }
        public int tipo { get; set; }
    }

    public class Contactenos_Model
    {
        [Required(ErrorMessageResourceName = "Required_nombre_apellidos", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string nombre_apellidos { get; set; }

        [EmailAddress(ErrorMessageResourceName = "Error_Email", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        [Required(ErrorMessageResourceName = "Required_email", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string email { get; set; }

        [Required(ErrorMessageResourceName = "Required_mensaje_contactenos", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string mensaje { get; set; }
    }

    public class Datos_Menu
    {
        public string nombre_plato { get; set; }
        public string precio { get; set; }
    }

    public class Datos_Chefs
    {
        public string nombre_apellidos { get; set; }
        public string cargo { get; set; }
    }

    public class Utiles
    {
        private static List<Comentario> Obtener_Comentarios_Error()
        {
            return new List<Comentario>() { new Comentario { id_comentario = 1, comentario = "Comida excelente!!! Uno de los mejores paladares de Trinidad", dia = 24, mes = 2, anno = 2017 }, new Comentario { id_comentario = 2, comentario = "Los chefs son una maravilla, me encantó la comida", dia = 23, mes = 2, anno = 2017 } };
        }

        public static string Arreglar_Precio(string precio)
        {
            if (precio.Contains("."))
            {
                string[] arr_precio = precio.Split('.');
                if (arr_precio[1].Length == 1)
                    precio += "0";
            }
            else
                precio += ".00";

            return precio;
        }

        public static string Obtener_Mes(int mes)
        {
            switch (mes)
            {
                case 1: return Resources.PaladarHostal.ResourceManager.GetString("Enero");
                case 2: return Resources.PaladarHostal.ResourceManager.GetString("Febrero");
                case 3: return Resources.PaladarHostal.ResourceManager.GetString("Marzo");
                case 4: return Resources.PaladarHostal.ResourceManager.GetString("Abril");
                case 5: return Resources.PaladarHostal.ResourceManager.GetString("Mayo");
                case 6: return Resources.PaladarHostal.ResourceManager.GetString("Junio");
                case 7: return Resources.PaladarHostal.ResourceManager.GetString("Julio");
                case 8: return Resources.PaladarHostal.ResourceManager.GetString("Agosto");
                case 9: return Resources.PaladarHostal.ResourceManager.GetString("Septiembre");
                case 10: return Resources.PaladarHostal.ResourceManager.GetString("Octubre");
                case 11: return Resources.PaladarHostal.ResourceManager.GetString("Noviembre");
                default: return Resources.PaladarHostal.ResourceManager.GetString("Diciembre");
            }
        }

        public static string Arreglar_Fecha(int diaF, int mesF, int annoF)
        {
            string mes = Obtener_Mes(mesF);
            string dia = diaF.ToString().Length == 1 ? "0" + diaF.ToString() : diaF.ToString();
            string anno = annoF.ToString().Length == 1 ? "0" + annoF.ToString() : annoF.ToString();
            return String.Format("{0} {1} {2}", dia, mes, anno);
        }

        public static string Arreglar_Ceros_Fecha(int diaF, int mesF, int annoF)
        {
            string dia = diaF.ToString().Length == 1 ? "0" + diaF.ToString() : diaF.ToString();
            string mes = mesF.ToString().Length == 1 ? "0" + mesF.ToString() : mesF.ToString();
            string anno = annoF.ToString().Length == 1 ? "0" + annoF.ToString() : annoF.ToString();
            return String.Format("{0}/{1}/{2}", dia, mes, anno);
        }

        public static Config_Horarios Obtener_Config_Horarios(PaladarEntities ctx)
        {
            Config_Horarios config_hor = null;
            try
            {
                config_hor = new Config_Horarios { horarios = ctx.Horario.Select(c => c).ToList(), comentarios = ctx.Comentario.OrderByDescending(c => c.anno).ThenByDescending(c => c.mes).ThenByDescending(c => c.dia).ToList() };
            }
            catch (Exception)
            {
                config_hor = new Config_Horarios { horarios = new List<Horario>(), comentarios = Obtener_Comentarios_Error() };
            }
            return config_hor;
        }

        public static List<string> Obtener_Dir_Imagenes(string direccion)
        {
            List<string> dir_imagenes = new List<string>();
            string[] imgs = Directory.GetFiles(HttpContext.Current.Server.MapPath(direccion));
            foreach (var item in imgs)
            {
                FileInfo finfo = new FileInfo(item);
                dir_imagenes.Add(finfo.Name);
            }
            return dir_imagenes;
        }

        public static TimeSpan Obtener_Hora(string hora_convertir)
        {
            string[] arr = hora_convertir.Split(' ');
            string[] arr_hora = arr[0].Split(':');

            int hora = int.Parse(arr_hora[0]);
            if (arr[1] == "PM")
                hora += 12;

            return new TimeSpan(hora, int.Parse(arr_hora[1]), 0);
        }

        public static List<Horario_Aux> Agrupar_Horarios(List<Horario> horarios)
        {
            List<Horario_Aux> nuevos_horarios = new List<Horario_Aux>();
            if (horarios.Count > 0)
            {
                Horario horario_inicio = horarios[0];
                Horario horario_fin = null;
                for (int i = 1; i < horarios.Count; i++)
                {
                    if (horarios[i].hora_entrada == horario_inicio.hora_entrada && horarios[i].hora_salida == horario_inicio.hora_salida)
                        horario_fin = horarios[i];
                    else
                    {
                        Adicionar_Horario_Aux(horario_inicio, horario_fin, nuevos_horarios, horarios[i]);
                        horario_inicio = horarios[i];
                        horario_fin = null;
                    }

                    if (i == horarios.Count - 1)
                        Adicionar_Horario_Aux(horario_inicio, horario_fin, nuevos_horarios, horarios[i]);
                }
            }
            return nuevos_horarios;
        }

        private static void Adicionar_Horario_Aux(Horario horario_inicio, Horario horario_fin, List<Horario_Aux> nuevos_horarios, Horario horario_actual)
        {
            string rango_dias = horario_fin == null ? Resources.PaladarHostal.ResourceManager.GetString(horario_inicio.dia) : String.Format("{0} - {1}", Resources.PaladarHostal.ResourceManager.GetString(horario_inicio.dia), Resources.PaladarHostal.ResourceManager.GetString(horario_fin.dia));
            nuevos_horarios.Add(new Horario_Aux { rango_dias = rango_dias, horario = horario_inicio.hora_entrada == "Cerrado" ? Resources.PaladarHostal.ResourceManager.GetString("Cerrado") : String.Format("{0} - {1}", horario_inicio.hora_entrada, horario_inicio.hora_salida) });
        }

        public static int Cant_Comentarios()
        {
            using (PaladarEntities ctx = new PaladarEntities())
            {
                try
                {
                    return ctx.Comentario.Count();
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public static List<int> Obtener_Hora_Militar(string hora)
        {
            List<int> hora_militar = new List<int>();
            string[] arr_hora = hora.Split(' ');
            string[] arr_hora_m = arr_hora[0].Split(':');
            int hora_final = arr_hora[1] == "PM" ? arr_hora_m[0] == "12" ? 12 : int.Parse(arr_hora_m[0]) + 12 : arr_hora_m[0] == "12" ? 0 : int.Parse(arr_hora_m[0]);
            hora_militar.Add(hora_final);
            hora_militar.Add(int.Parse(arr_hora_m[1]));
            return hora_militar;
        }

        public static string Arreglar_Hora(int hora, int minuto)
        {
            string am_pm = "";
            string hora_final = "";
            string minuto_final = minuto.ToString();

            if (hora >= 0 && hora < 12)
            {
                hora_final = hora == 0 ? "12" : hora.ToString();
                am_pm = "AM";
            }
            else
            {
                hora_final = hora == 12 ? "12" : (hora - 12).ToString();
                am_pm = "PM";
            }

            if (hora_final.Length == 1)
                hora_final = "0" + hora_final;

            if (minuto_final.Length == 1)
                minuto_final = "0" + minuto_final;

            return String.Format("{0}:{1} {2}", hora_final, minuto_final, am_pm);
        }

        public static void Idioma(string idioma)
        {
            CultureInfo ci = CultureInfo.GetCultureInfo(idioma);
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
        }

        public static void Adicionar_Cookie_Idioma(string lenguaje)
        {
            var nueva_cookie = new HttpCookie("lang", lenguaje);
            nueva_cookie.Expires = DateTime.Now.AddDays(7);
            HttpContext.Current.Response.Cookies.Add(nueva_cookie);
        }

        public static HttpCookie Obtener_Cookie_Idioma()
        {
            var cookie_lang = HttpContext.Current.Request.Cookies["lang"];
            if (cookie_lang != null)
                return cookie_lang;
            else
            {
                Adicionar_Cookie_Idioma("es");
                return HttpContext.Current.Request.Cookies["lang"];
            }
        }

        public static void Establecer_Idioma()
        {
            try
            {
                var cookie_lang = Obtener_Cookie_Idioma();
                Idioma(cookie_lang != null ? cookie_lang.Value : "es");
            }
            catch (Exception)
            {
                 Idioma("es");
            }
        }

        public static Datos_Menu Obtener_Datos_XML(string direccion)
        {
            XElement doc = XElement.Load(direccion);
            var descendants = doc.Descendants();
            return new Datos_Menu { nombre_plato = descendants.ElementAtOrDefault(0).Value, precio = descendants.ElementAtOrDefault(1).Value };
        }

        public static Datos_Chefs Obtener_Datos_Chef(string direccion)
        {
            XElement doc = XElement.Load(direccion);
            var descendants = doc.Descendants();
            return new Datos_Chefs { nombre_apellidos = descendants.ElementAtOrDefault(0).Value, cargo = descendants.ElementAtOrDefault(1).Value };
        }

        public static string Encriptar(string password, int iteraciones)
        {
            string salted = password + "{890!@#$%^WXYZabcdef}";
            SHA512Managed _objSha512 = new SHA512Managed();
            string hash1 = "";

            try
            {
                hash1 = Convert.ToBase64String(new SHA512Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(salted)));
                for (int i = 1; i < iteraciones; i++)
                    hash1 = Convert.ToBase64String(new SHA512Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash1 + salted)));
            }
            finally { _objSha512.Clear(); }
            return hash1;
        }

        public static void Enviar_Email(MailMessage msg)
        {
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential("carlos.avila8909@gmail.com", "skuareenix", "smtp.gmail.com");
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Send(msg);

            //Smtp ATI
            /*SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential("carlos", "Skuareenix.0");
            client.Host = "172.19.62.9";
            client.Port = 25;
            client.Send(msg);*/
        }

        public static void Enviar_Email_Reservacion_Cliente(int id_reservacion, string password, string nombre_apellidos, string email_to, DateTime fecha_entrada, DateTime fecha_salida, PaladarEntities ctx)
        {
            try
            {
                string direccion_url = String.Format("{0}://{1}/{3}/Hostal/Detalles/{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, id_reservacion, ctx.Administrador.FirstOrDefault().host_internet);
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("carlos.avila8909@gmail.com");
                msg.To.Add(email_to);
                msg.Subject = Resources.PaladarHostal.Titulo;
                msg.Body = String.Format("{0} {1}, {2} {3} {13} {4}. {5} {6} \n{7}: \n{8}: {9}\n{10}: {11}\n{12}", Resources.PaladarHostal.Saludos, nombre_apellidos, Resources.PaladarHostal.Satisface_recibirlo, Arreglar_Fecha(fecha_entrada.Day, fecha_entrada.Month, fecha_entrada.Year), Arreglar_Fecha(fecha_salida.Day, fecha_salida.Month, fecha_salida.Year), Resources.PaladarHostal.Cancelar_reservacion, direccion_url, Resources.PaladarHostal.Credenciales, Resources.PaladarHostal.Correo, email_to, Resources.PaladarHostal.Contrasenna, password, Resources.PaladarHostal.Gracias_preferencia, Resources.PaladarHostal.Hasta_el);
                msg.Priority = MailPriority.High;
                Enviar_Email(msg);
            }
            catch (Exception)
            { }

            //Correo desde ATI
            /*try
            {
                string direccion_url = String.Format("{0}://{1}/{3}/Hostal/Detalles/{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, id_reservacion, ctx.Administrador.FirstOrDefault().host_internet);
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("carlos@atiss.une.cu");//Este debe ser de Gmail
                msg.To.Add(email_to);
                msg.Subject = Resources.PaladarHostal.Titulo;
                msg.Body = String.Format("{0} {1}, {2} {3} {13} {4}. {5} {6} \n{7}: \n{8}: {9}\n{10}: {11}\n{12}", Resources.PaladarHostal.Saludos, nombre_apellidos, Resources.PaladarHostal.Satisface_recibirlo, Arreglar_Fecha(fecha_entrada.Day, fecha_entrada.Month, fecha_entrada.Year), Arreglar_Fecha(fecha_salida.Day, fecha_salida.Month, fecha_salida.Year), Resources.PaladarHostal.Cancelar_reservacion, direccion_url, Resources.PaladarHostal.Credenciales, Resources.PaladarHostal.Correo, email_to, Resources.PaladarHostal.Contrasenna, password, Resources.PaladarHostal.Gracias_preferencia, Resources.PaladarHostal.Hasta_el);
                msg.Priority = MailPriority.High;
                Enviar_Email(msg);
            }
            catch (Exception)
            { }*/
        }

        public static void Enviar_Email_Cancelacion_Cliente(Reservacion_Hostal reservacion_hostal, PaladarEntities ctx)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("carlos.avila8909@gmail.com");
                msg.To.Add(reservacion_hostal.email);
                msg.Subject = Resources.PaladarHostal.Titulo;
                msg.Priority = MailPriority.High;
                msg.Body = String.Format("{7}\n{9}: {0}\n{10}: {1}\n{11}: {8}\n{12}: {2}\n{13}: {3}\n{14}: {4}\n{15}: {5}\n{16}: {6}", reservacion_hostal.nombre_apellidos, reservacion_hostal.email, Obtener_Habitaciones(reservacion_hostal.habitaciones), Arreglar_Ceros_Fecha(reservacion_hostal.dia_entrada, reservacion_hostal.mes_entrada, reservacion_hostal.anno_entrada), Arreglar_Ceros_Fecha(reservacion_hostal.dia_salida, reservacion_hostal.mes_salida, reservacion_hostal.anno_salida), reservacion_hostal.cant_acompannantes, reservacion_hostal.mensaje_adicional == null || reservacion_hostal.mensaje_adicional.Trim() == "" ? "-" : reservacion_hostal.mensaje_adicional, Resources.PaladarHostal.Cancelar_reservacion_cliente, reservacion_hostal.pais, Resources.PaladarHostal.Nombre_apellidos, Resources.PaladarHostal.Correo, Resources.PaladarHostal.Pais, Resources.PaladarHostal.Habitaciones, Resources.PaladarHostal.Fecha_entrada, Resources.PaladarHostal.Fecha_salida, Resources.PaladarHostal.Acompannantes, Resources.PaladarHostal.Mensaje_adicional);
                Enviar_Email(msg);
            }
            catch (Exception)
            { }
        }

        public static void Enviar_Email_Cancelacion_Reservacion_Administrador(Reservacion_Hostal reservacion_hostal, string introduccion, PaladarEntities ctx)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("carlos.avila8909@gmail.com");
                msg.To.Add(ctx.Administrador.FirstOrDefault().email);
                msg.Subject = Resources.PaladarHostal.Titulo;
                msg.Priority = MailPriority.High;
                msg.Body = String.Format("{7}:\nNombre y Apellidos: {0}.\nCorreo electrónico: {1}\nPaís: {8}\nHabitaciones: {2}\nFecha de entrada: {3}\nFecha de salida: {4}\nAcompañantes: {5}\nMensaje adicional: {6}", reservacion_hostal.nombre_apellidos, reservacion_hostal.email, Obtener_Habitaciones(reservacion_hostal.habitaciones), Arreglar_Ceros_Fecha(reservacion_hostal.dia_entrada, reservacion_hostal.mes_entrada, reservacion_hostal.anno_entrada), Arreglar_Ceros_Fecha(reservacion_hostal.dia_salida, reservacion_hostal.mes_salida, reservacion_hostal.anno_salida), reservacion_hostal.cant_acompannantes, reservacion_hostal.mensaje_adicional == null || reservacion_hostal.mensaje_adicional.Trim() == "" ? "-" : reservacion_hostal.mensaje_adicional, introduccion, reservacion_hostal.pais);
                Enviar_Email(msg);
            }
            catch (Exception)
            { }
        }

        public static void Enviar_Email_Contactenos(Contactenos_Model contact_model, PaladarEntities ctx)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("carlos.avila8909@gmail.com");
            msg.To.Add(ctx.Administrador.FirstOrDefault().email);
            msg.Priority = MailPriority.High;
            msg.Subject = String.Format("Mensaje de cliente desde {0}", Resources.PaladarHostal.Titulo);
            msg.Body = String.Format("{0} le ha enviado un mensaje a través del sitio web {1}, usted puede responderle a la dirección de correo {2}.\nEl mensaje es el siguiente: {3}", contact_model.nombre_apellidos, Resources.PaladarHostal.Titulo, contact_model.email, contact_model.mensaje);
            Enviar_Email(msg);
        }

        public static string Obtener_Habitaciones(string habitaciones)
        {
            string habs = "";
            string[] arr_habitaciones = habitaciones.Split('|');
            for (int i = 0; i < arr_habitaciones.Length - 1; i++)
                habs += arr_habitaciones[i] + ", ";

            return habs.Substring(0, habs.Length - 2);
        }

        public static string Obtener_Email_Negocio()
        {
            try
            {
                return new PaladarEntities().Administrador.FirstOrDefault().email;
            }
            catch (Exception)
            {
                return "emelina@nauta.cu";
            }
        }
    }

    public class Config_Horarios
    {
        public List<Horario> horarios { get; set; }
        public List<Comentario> comentarios { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "El campo Correo electrónico es obligatorio.")]
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "El correo electrónico entrado no es válido.")]
        public string email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
        public string password { get; set; }
    }

    public class Reservacion_Login_Model
    {
        public int id_reservacion { get; set; }
        public Reservacion_Hostal_Model reservacion_hostal { get; set; }
        public Login_Reservacion_Hostal login_reservacion { get; set; }
        public int? id_eliminar_reservacion { get; set; }
    }

    public class Login_Reservacion_Hostal
    {
        [EmailAddress(ErrorMessageResourceName = "Error_Email", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        [Required(ErrorMessageResourceName = "Required_email", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceName = "Required_password", ErrorMessageResourceType = typeof(Resources.PaladarHostal))]
        public string password { get; set; }
    }

    public class Horario_Aux
    {
        public string rango_dias { get; set; }
        public string horario { get; set; }
    }

    public class Gestion_Password
    {
        [Required(ErrorMessage = "Tiene que escribir la contraseña anterior.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña anterior")]
        [Remote("Validar_Password_Anterior", "Paladar", ErrorMessage = "La contraseña anterior no es correcta.", HttpMethod = "POST")]
        public string pass_anterior { get; set; }

        [Required(ErrorMessage = "Tiene que escribir la nueva contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("confirmar_pass", ErrorMessage = "La contraseña nueva y su confirmación no coinciden.")]
        public string nuevo_pass { get; set; }

        [Required(ErrorMessage = "Tiene que escribir la confirmación de la nueva contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        public string confirmar_pass { get; set; }
    }
}