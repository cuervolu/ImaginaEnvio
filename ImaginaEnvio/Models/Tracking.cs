using System;

namespace ImaginaEnvio.Models
{
    public class Tracking
    {
        public int idEnvio { get; set; }
        public string EstadoEnvio { get; set; }
        public DateTime FechaHora { get; set; }
        public string Descripcion { get; set; }

        public Tracking() { }

    }
}