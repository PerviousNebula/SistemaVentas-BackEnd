using System.Collections.Generic;

public class DashboardModel
{
    public IEnumerable<ChartModel> ingresos { get; set; }
    public decimal totalIngresos { get; set; }
    public IEnumerable<ChartModel> ventas { get; set; }
    public decimal totalVentas { get; set; }
}