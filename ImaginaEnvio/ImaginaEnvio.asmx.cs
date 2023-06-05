using ImaginaEnvio.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;

namespace ImaginaEnvio
{
    /// <summary>
    /// Descripción breve de ImaginaEnvio
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class ImaginaEnvio : System.Web.Services.WebService
    {


        public class ErrorDetails
        {
            public HttpStatusCode StatusCode { get; set; }
            public string Message { get; set; }
            public string ExceptionMessage { get; set; }
            public string StackTrace { get; set; }
        }


        [WebMethod]
        public string GenerateShipments(DateTime fecha_envio, long direccion_id, long pedido_id, long repartidor_id)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    // Obtener el Pedido, MétodoPago y Usuario correspondientes a los IDs proporcionados
                    PEDIDO pedido = dbContext.PEDIDO.FirstOrDefault(p => p.ID_PEDIDO == pedido_id);
                    USUARIO repartidor = dbContext.USUARIO.FirstOrDefault(u => u.ID == repartidor_id && u.TIPO_USUARIO == "Repartidor");
                    DIRECCION direccion = dbContext.DIRECCION.FirstOrDefault(d => d.ID_DIRECCION == direccion_id);

                    if (pedido != null && repartidor != null && direccion != null)
                    {
                        ENVIO envio = new ENVIO();
                        envio.FECHA_ENVIO = fecha_envio;
                        envio.DIRECCION = direccion;
                        envio.PEDIDO = pedido;
                        envio.REPARTIDOR_ID = repartidor_id;

                        //Guardar envio

                        dbContext.ENVIO.Add(envio);
                        dbContext.SaveChanges();

                        //Verificar si se creo el envío
                        if (envio.ID_ENVIO > 0)
                        {
                            //Modificar Estado Pedido
                            pedido.ESTADO_PEDIDO = "En Preparación";
                            dbContext.SaveChanges();

                            // Crear una estructura XML para la respuesta SOAP
                            XmlDocument responseXml = new XmlDocument();
                            XmlElement rootElement = responseXml.CreateElement("GenerateShipments");
                            XmlElement statusElement = responseXml.CreateElement("Status");
                            statusElement.InnerText = "Success";
                            rootElement.AppendChild(statusElement);
                            responseXml.AppendChild(rootElement);

                            // Convertir la estructura XML en una cadena para su retorno
                            return responseXml.OuterXml;
                        }
                        else
                        {
                            ErrorDetails errorDetails = new ErrorDetails
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                Message = "Se produjo un error al crear el envio.",
                            };

                            XmlSerializer serializer = new XmlSerializer(typeof(ErrorDetails));
                            using (StringWriter writer = new StringWriter())
                            {
                                serializer.Serialize(writer, errorDetails);
                                return writer.ToString();
                            }
                        }
                    }
                    else
                    {
                        ErrorDetails errorDetails = new ErrorDetails
                        {
                            StatusCode = HttpStatusCode.Forbidden,
                            Message = "Se produjo un error al crear el envio. El usuario no tiene los permisos necesarios para ser repartidor",
                        };

                        XmlSerializer serializer = new XmlSerializer(typeof(ErrorDetails));
                        using (StringWriter writer = new StringWriter())
                        {
                            serializer.Serialize(writer, errorDetails);
                            return writer.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDetails errorDetails = new ErrorDetails
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Se produjo un error al crear la transacción.",
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                // Serializar el objeto de error en XML
                XmlSerializer serializer = new XmlSerializer(typeof(ErrorDetails));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, errorDetails);
                    return writer.ToString();
                }

            }
        }

        public string ModifyShipment(long id_envio, string state)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    ENVIO envio = dbContext.ENVIO.FirstOrDefault(e => e.ID_ENVIO == id_envio);
                    if (envio != null)
                    {
                        switch (state)
                        {
                            case "En Preparación":
                                envio.PEDIDO.ESTADO_PEDIDO = "En Preparación";
                                break;

                            case "En Ruta":
                                envio.PEDIDO.ESTADO_PEDIDO = "En Ruta";
                                break;

                            case "Entregado":
                                envio.PEDIDO.ESTADO_PEDIDO = "Entregado";
                                break;

                            case "Cancelado":
                                envio.PEDIDO.ESTADO_PEDIDO = "Cancelado";
                                break;

                            default:
                                envio.PEDIDO.ESTADO_PEDIDO = envio.PEDIDO.ESTADO_PEDIDO;
                                break;
                        }

                        dbContext.SaveChanges();

                        // Crear una estructura XML para la respuesta SOAP
                        XmlDocument responseXml = new XmlDocument();
                        XmlElement rootElement = responseXml.CreateElement("ModifyShipment");
                        XmlElement statusElement = responseXml.CreateElement("Status");
                        statusElement.InnerText = "Estado modificado";
                        rootElement.AppendChild(statusElement);
                        responseXml.AppendChild(rootElement);

                        // Convertir la estructura XML en una cadena para su retorno
                        return responseXml.OuterXml;

                    }
                    else
                    {
                        ErrorDetails errorDetails = new ErrorDetails
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Message = "Se produjo un error al modificar el estado.",
                        };

                        XmlSerializer serializer = new XmlSerializer(typeof(ErrorDetails));
                        using (StringWriter writer = new StringWriter())
                        {
                            serializer.Serialize(writer, errorDetails);
                            return writer.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDetails errorDetails = new ErrorDetails
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Se produjo un error al crear la transacción.",
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                // Serializar el objeto de error en XML
                XmlSerializer serializer = new XmlSerializer(typeof(ErrorDetails));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, errorDetails);
                    return writer.ToString();
                }
            }
        }
    }
}
