using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class IngresoActualizarModel
{
    // Propiedades maestro
    [Required]
    public int idIngreso { get; set; }
    [Required]
    public int idProveedor { get; set; }
    [Required]
    public string tipo_comprobante { get; set; }
    public string serie_comprobante { get; set; }
    [Required]
    public string num_comprobante { get; set; }
    [Required]
    public decimal impuesto { get; set; }
    [Required]
    public decimal total { get; set; }

    // Propiedades detalle
    [Required]
    public List<DetalleViewModel> detalles { get; set; }
}