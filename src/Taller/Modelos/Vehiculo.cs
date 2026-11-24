namespace Taller.Modelos;

public class Vehiculo
{
    public int IdVehiculo { get; set; }
    public string Placa { get; set; } = "";
    public string Marca { get; set; } = "";
    public string Modelo { get; set; } = "";
    public int Anio { get; set; }
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; } = "";
}
