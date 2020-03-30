using System;
using System.Collections.Generic;

public class BuscarViewModel
{
    public List<CategoriaViewModel> categorias { get; set; }
    public List<ArticuloViewModel> articulos { get; set; }
    public List<IngresoViewModel> ingresos { get; set; }
    public List<PersonaViewModel> proveedores { get; set; }
    public List<VentaViewModel> ventas { get; set; }
    public List<PersonaViewModel> clientes { get; set; }
    public List<UsuarioViewModel> usuarios { get; set; }
    public Boolean sinResultados { get; set; }

    public BuscarViewModel()
    {
        categorias = new List<CategoriaViewModel>();
        articulos = new List<ArticuloViewModel>();
        ingresos = new List<IngresoViewModel>();
        proveedores = new List<PersonaViewModel>();
        ventas = new List<VentaViewModel>();
        clientes = new List<PersonaViewModel>();
        usuarios = new List<UsuarioViewModel>();
    }
}
