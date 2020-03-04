using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Venta
{
    public int idVenta { get; set;}
    [Required]
    public int idPersona { get; set; }
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

    public ICollection<DetalleVenta> detalles { get; set; }
    public Usuario usuario { get; set; }
    public Persona persona { get; set; }
}