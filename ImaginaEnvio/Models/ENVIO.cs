//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImaginaEnvio.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ENVIO
    {
        public long ID_ENVIO { get; set; }
        public Nullable<System.DateTime> FECHA_ENVIO { get; set; }
        public long DIRECCION_ID { get; set; }
        public long PEDIDO_ID { get; set; }
        public long REPARTIDOR_ID { get; set; }
    
        public virtual DIRECCION DIRECCION { get; set; }
        public virtual PEDIDO PEDIDO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
