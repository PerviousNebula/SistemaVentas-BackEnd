using System;
using System.Collections.Generic;

public class VentaViewModel
{
    public int idVenta { get; set;}
    public int idCliente { get; set; }
    public string cliente { get; set; }
    public int idUsuario { get; set; }
    public string usuario { get; set; }
    public string tipo_comprobante { get; set; }
    public string serie_comprobante { get; set; }
    public string num_comprobante { get; set; }
    public DateTime fecha_hora { get; set; }
    public decimal impuesto { get; set; }
    public decimal total { get; set; }
    public string estado { get; set; }

    public IEnumerable<DetalleVentaViewModel> detalles { get; set; }
}