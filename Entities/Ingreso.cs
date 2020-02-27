using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Ingreso
{
    public int idIngreso { get; set;}
    [Required]
    public int idProveedor { get; set; }
    [Required]
    public int idUsuario { get; set; }
    [Required]
    public string tipo_comprobante { get; set; }
    public string serie_comprobante { get; set; }
    [Required]
    public string num_comprobante { get; set; }
    [Required]
    public DateTime fecha_hora { get; set; }
    [Required]
    public decimal impuesto { get; set; }
    [Required]
    public decimal total { get; set; }
    [Required]
    public string estado { get; set; }

    public ICollection<DetalleIngreso> detalles { get; set; }
    public Usuario usuario { get; set; }
    public Persona proveedor { get; set; }
    
}