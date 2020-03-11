using System;
using System.Collections.Generic;

public class VentaPDF
{
    /*Propiedades del cliente*/
    public string cliente { get; set; }
    public string num_documento { get; set; }
    public string direccion { get; set; }
    public string telefono { get; set; }
    public string email { get; set; }
    
    /*Propiedades de la venta*/
    public string usuario { get; set; }
    public string tipo_comprobante { get; set; }
    public string serie_comprobante { get; set; }
    public string num_comprobante { get; set; }
    public DateTime fecha_hora { get; set; }
    public decimal impuesto { get; set; }
    public decimal total { get; set; }
    public string estado { get; set; }

    public IEnumerable<DetalleVentaPDF> detalles { get; set; }
}
