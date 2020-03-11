public class ArticuloFilterModel
{
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public string codigo { get; set; }
    public int stock { get; set; }
    public decimal precio_min { get; set; }
    public decimal precio_max { get; set; }
    public int idCategoria { get; set; }
    public bool activo { get; set; }
}