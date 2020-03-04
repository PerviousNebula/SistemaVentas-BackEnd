using System.Collections.Generic;
using System.Text;

public class TemplateGenerator
{
    public static string GetArticulosHTMLString(List<ArticuloPDF> articulos)
    {
        var sb = new StringBuilder();
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='header'><h3 class='text-themecolor'>Artículos</h3></div>
                            <table class='display nowrap table table-striped table-bordered dataTable' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
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
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
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
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='header'><h3 class='text-themecolor'>Categorías</h3></div>
                            <table class='display nowrap table table-striped table-bordered dataTable' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
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
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
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
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='header'><h3 class='text-themecolor'>Ingresos</h3></div>
                            <table class='display nowrap table table-striped table-bordered dataTable' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
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
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                                <td>{5}</td>
                                <td>{6}</td>
                                <td>{7}</td>
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
        sb.Append(@"
                    <html>
                        <head>
                        </head>
                        <body>
                            <div class='header'><h3 class='text-themecolor'>Proveedores</h3></div>
                            <table class='display nowrap table table-striped table-bordered dataTable' cellspacing='0' width='100%' role='grid' aria-describedby='example23_info' style='width: 100%;'>
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
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                </tr>", prov.nombre, prov.direccion, prov.telefono, prov.email);
        }

        sb.Append(@"            </tbody>
                            </table>
                        </body>
                    </html>");

        return sb.ToString();
    }

}