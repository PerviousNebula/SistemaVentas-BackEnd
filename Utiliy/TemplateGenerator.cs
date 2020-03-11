using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class TemplateGenerator
{
    public static string GetArticulosHTMLString(List<ArticuloPDF> articulos)
    {
        var sb = new StringBuilder();
        string rutaLogo = Path.Combine(Directory.GetCurrentDirectory() + "/assets/images/logo.png"); 
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='row'>
                                <div class='column'>
                                    <img src='").Append(rutaLogo).Append(@"' />
                                </div>
                                <div class='column text-center mt-1'>
                                    <h4>Sistema de Inventario ADMINPRO</h4>
                                    <h4>1457 Sycamore Fork Road Sunrise, FL 33317</h4>
                                    <h4>Teléfono: 664-227-9568</h4>
                                    <h4>Email: arturo.nevarez.villa@gmail.com</h4>
                                </div>
                            </div>
                            <table class='display nowrap table table-striped table-bordered dataTable mt-2' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
                            <thead>
                                <tr role='row'>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 244px;'>Nombre</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Descripción</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Código</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Precio</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Stock</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Categoría</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Estado</th>
                                </tr>
                            </thead><tbody>");

        foreach (var art in articulos)
        {
            sb.AppendFormat(@"<tr>
                                <td class='text-center'>{0}</td>
                                <td class='text-center'>{1}</td>
                                <td class='text-center'>{2}</td>
                                <td class='text-center'>{3}</td>
                                <td class='text-center'>{4}</td>
                                <td class='text-center'>{5}</td>
                                <td class='text-center'>{6}</td>
                                </tr>", art.nombre, art.descripcion, art.codigo, art.precio_venta, art.stock, art.categoria, art.activo ? "ACTIVO" : "INACTIVO");
        }

        sb.Append(@"            </tbody>
                            </table>
                        </body>
                    </html>");

        return sb.ToString();
    }
    
    public static string GetCategoriasHTMLString(List<CategoriaPDF> categorias)
    {
        var sb = new StringBuilder();
        string rutaLogo = Path.Combine(Directory.GetCurrentDirectory() + "/assets/images/logo.png"); 
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='row'>
                                <div class='column'>
                                    <img src='").Append(rutaLogo).Append(@"' />
                                </div>
                                <div class='column text-center mt-1'>
                                    <h4>Sistema de Inventario ADMINPRO</h4>
                                    <h4>1457 Sycamore Fork Road Sunrise, FL 33317</h4>
                                    <h4>Teléfono: 664-227-9568</h4>
                                    <h4>Email: arturo.nevarez.villa@gmail.com</h4>
                                </div>
                            </div>
                            <table class='display nowrap table table-striped table-bordered dataTable mt-2' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
                            <thead>
                                <tr role='row'>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 244px;'>Nombre</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Descripción</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Estado</th>
                                </tr>
                            </thead><tbody>");

        foreach (var cat in categorias)
        {
            sb.AppendFormat(@"<tr>
                                <td class='text-center'>{0}</td>
                                <td class='text-center'>{1}</td>
                                <td class='text-center'>{2}</td>
                                </tr>", cat.nombre, cat.descripcion, cat.activo ? "ACTIVO" : "INACTIVO");
        }

        sb.Append(@"            </tbody>
                            </table>
                        </body>
                    </html>");

        return sb.ToString();
    }

    public static string GetIngresosHTMLString(List<IngresosPDF> ingresos)
    {
        var sb = new StringBuilder();
        string rutaLogo = Path.Combine(Directory.GetCurrentDirectory() + "/assets/images/logo.png"); 
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='row'>
                                <div class='column'>
                                    <img src='").Append(rutaLogo).Append(@"' />
                                </div>
                                <div class='column text-center mt-1'>
                                    <h4>Sistema de Inventario ADMINPRO</h4>
                                    <h4>1457 Sycamore Fork Road Sunrise, FL 33317</h4>
                                    <h4>Teléfono: 664-227-9568</h4>
                                    <h4>Email: arturo.nevarez.villa@gmail.com</h4>
                                </div>
                            </div>
                            <table class='display nowrap table table-striped table-bordered dataTable mt-2' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
                            <thead>
                                <tr role='row'>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 244px;'>Usuario</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Proveedor</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Tipo Comprobante</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Serie Comprobante</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>No. Comprobante</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Fecha</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Total</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Estado</th>
                                </tr>
                            </thead><tbody>");

        foreach (var ing in ingresos)
        {
            sb.AppendFormat(@"<tr>
                                <td class='text-center'>{0}</td>
                                <td class='text-center'>{1}</td>
                                <td class='text-center'>{2}</td>
                                <td class='text-center'>{3}</td>
                                <td class='text-center'>{4}</td>
                                <td class='text-center'>{5}</td>
                                <td class='text-center'>{6}</td>
                                <td class='text-center'>{7}</td>
                                </tr>", ing.usuario, ing.proveedor, ing.tipo_comprobante, ing.serie_comprobante, ing.num_comprobante, ing.fecha_hora, "$" + ing.total, ing.estado);
        }

        sb.Append(@"            </tbody>
                            </table>
                        </body>
                    </html>");

        return sb.ToString();
    }

    public static string GetProveedoresHTMLString(List<ProveedoresPDF> proveedores)
    {
        var sb = new StringBuilder();
        string rutaLogo = Path.Combine(Directory.GetCurrentDirectory() + "/assets/images/logo.png"); 
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='row'>
                                <div class='column'>
                                    <img src='").Append(rutaLogo).Append(@"' />
                                </div>
                                <div class='column text-center mt-1'>
                                    <h4>Sistema de Inventario ADMINPRO</h4>
                                    <h4>1457 Sycamore Fork Road Sunrise, FL 33317</h4>
                                    <h4>Teléfono: 664-227-9568</h4>
                                    <h4>Email: arturo.nevarez.villa@gmail.com</h4>
                                </div>
                            </div>
                            <table class='display nowrap table table-striped table-bordered dataTable mt-2' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
                            <thead>
                                <tr role='row'>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 244px;'>Nombre</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Dirección</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Teléfono</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Email</th>
                                </tr>
                            </thead><tbody>");

        foreach (var prov in proveedores)
        {
            sb.AppendFormat(@"<tr>
                                <td class='text-center'>{0}</td>
                                <td class='text-center'>{1}</td>
                                <td class='text-center'>{2}</td>
                                <td class='text-center'>{3}</td>
                                </tr>", prov.nombre, prov.direccion, prov.telefono, prov.email);
        }

        sb.Append(@"            </tbody>
                            </table>
                        </body>
                    </html>");

        return sb.ToString();
    }

    public static string GetVentasHTMLString(List<VentasPDF> ventas)
    {
        var sb = new StringBuilder();
        string rutaLogo = Path.Combine(Directory.GetCurrentDirectory() + "/assets/images/logo.png"); 
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='row'>
                                <div class='column'>
                                    <img src='").Append(rutaLogo).Append(@"' />
                                </div>
                                <div class='column text-center mt-1'>
                                    <h4>Sistema de Inventario ADMINPRO</h4>
                                    <h4>1457 Sycamore Fork Road Sunrise, FL 33317</h4>
                                    <h4>Teléfono: 664-227-9568</h4>
                                    <h4>Email: arturo.nevarez.villa@gmail.com</h4>
                                </div>
                            </div>
                            <table class='display nowrap table table-striped table-bordered dataTable mt-2' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
                            <thead>
                                <tr role='row'>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 244px;'>Usuario</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Cliente</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Tipo Comprobante</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Serie Comprobante</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>No. Comprobante</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Fecha</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Total</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Estado</th>
                                </tr>
                            </thead><tbody>");

        foreach (var ven in ventas)
        {
            sb.AppendFormat(@"<tr>
                                <td class='text-center'>{0}</td>
                                <td class='text-center'>{1}</td>
                                <td class='text-center'>{2}</td>
                                <td class='text-center'>{3}</td>
                                <td class='text-center'>{4}</td>
                                <td class='text-center'>{5}</td>
                                <td class='text-center'>{6}</td>
                                <td class='text-center'>{7}</td>
                                </tr>", ven.usuario, ven.cliente, ven.tipo_comprobante, ven.serie_comprobante, ven.num_comprobante, ven.fecha_hora, ven.total, ven.estado.ToUpper());
        }

        sb.Append(@"            </tbody>
                            </table>
                        </body>
                    </html>");

        return sb.ToString();
    }

    public static string GetVentaHTMLString(VentaPDF venta)
    {
        var sb = new StringBuilder();
        string rutaLogo = Path.Combine(Directory.GetCurrentDirectory() + "/assets/images/logo.png"); 
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='row'>
                                <div class='column'>
                                    <img src='").Append(rutaLogo).Append(@"' />
                                </div>
                                <div class='column text-center mt-1'>
                                    <h4>Sistema de Inventario ADMINPRO</h4>
                                    <h4>1457 Sycamore Fork Road Sunrise, FL 33317</h4>
                                    <h4>Teléfono: 664-227-9568</h4>
                                    <h4>Email: arturo.nevarez.villa@gmail.com</h4>
                                </div>
                                <div class='column text-right mt-1'>
                                    <h2>").Append(venta.tipo_comprobante.ToUpper()).Append(@"</h2>
                                    <h4>").Append(venta.num_comprobante.ToUpper()).Append(@" - ").Append(venta.serie_comprobante).Append(@"</h4>
                                    <h4>").Append(venta.fecha_hora.ToShortDateString()).Append(@"</h4>
                                </div>
                            </div>
                            <div class='row'>
                                <div class='column'>
                                    <h2>Sr(a). ").Append(venta.cliente).Append(@"</h2>
                                    <h4>Documento: ").Append(venta.num_documento).Append(@"</h4>
                                    <h4>Dirección: ").Append(venta.direccion).Append(@"</h4>
                                    <h4>Teléfono: ").Append(venta.telefono).Append(@"</h4>
                                    <h4>Email: ").Append(venta.email).Append(@"</h4>
                                </div>
                            </div>
                            <table class='display nowrap table table-striped table-bordered dataTable mt-2' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
                            <thead>
                                <tr role='row'>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 244px;'>Cantidad</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Artículo</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Precio Unitario</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Descuento</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Precio Total</th>
                                </tr>
                            </thead><tbody>");

        foreach (var detalle in venta.detalles)
        {
            sb.AppendFormat(@"<tr>
                                <td class='text-center'>{0}</td>
                                <td class='text-center'>{1}</td>
                                <td class='text-center'>${2}</td>
                                <td class='text-center'>${3}</td>
                                <td class='text-center'>${4}</td>
                                </tr>", detalle.cantidad, detalle.articulo, detalle.precio, detalle.descuento, detalle.precio * detalle.cantidad - detalle.descuento);
        }

        sb.Append(@"            </tbody>
                            </table>
                            <div class='bd-example float-right'>
                                <ul class='list-group'>
                                    <li class='list-group-item'>Subtotal: $").Append(Math.Round((double)venta.total * 0.82, 2)).Append(@"</li>
                                    <li class='list-group-item'>Impuesto: $").Append(Math.Round((double)venta.total * 0.18, 2)).Append(@"</li>
                                    <li class='list-group-item'>Total: $").Append(venta.total).Append(@"</li>
                                </ul>
                            </div>
                        </body>
                    </html>");

        return sb.ToString();
    }
    
    public static string GetClientesHTMLString(List<ClientesPDF> clientes)
    {
        var sb = new StringBuilder();
        string rutaLogo = Path.Combine(Directory.GetCurrentDirectory() + "/assets/images/logo.png"); 
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='row'>
                                <div class='column'>
                                    <img src='").Append(rutaLogo).Append(@"' />
                                </div>
                                <div class='column text-center mt-1'>
                                    <h4>Sistema de Inventario ADMINPRO</h4>
                                    <h4>1457 Sycamore Fork Road Sunrise, FL 33317</h4>
                                    <h4>Teléfono: 664-227-9568</h4>
                                    <h4>Email: arturo.nevarez.villa@gmail.com</h4>
                                </div>
                            </div>
                            <table class='display nowrap table table-striped table-bordered dataTable mt-2' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
                            <thead>
                                <tr role='row'>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 244px;'>Nombre</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Teléfono</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Dirección</th>
                                    <th tabindex='0' aria-controls='example23' rowspan='1' colspan='1' style='width: 359px;'>Email</th>
                                </tr>
                            </thead><tbody>");

        foreach (var c in clientes)
        {
            sb.AppendFormat(@"<tr>
                                <td class='text-center'>{0}</td>
                                <td class='text-center'>{1}</td>
                                <td class='text-center'>{2}</td>
                                <td class='text-center'>{3}</td>
                                </tr>", c.nombre, c.telefono, c.direccion, c.email);
        }

        sb.Append(@"            </tbody>
                            </table>
                        </body>
                    </html>");

        return sb.ToString();
    }

}