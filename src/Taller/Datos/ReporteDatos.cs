using Microsoft.Data.Sqlite;

namespace Taller.Datos;

public static class ReporteDatos
{
    // R1: INNER JOIN relaciona las órdenes con vehículo, cliente y mecánico.
    private const string SqlOrdenes = """
        SELECT o.id_orden, o.fecha, o.estado, v.placa, c.nombre, m.nombre
        FROM orden o
        INNER JOIN vehiculo v ON v.id_vehiculo = o.id_vehiculo
        INNER JOIN cliente c ON c.id_cliente = v.id_cliente
        INNER JOIN mecanico m ON m.id_mecanico = o.id_mecanico
        ORDER BY o.fecha, o.id_orden;
        """;

    // R2: LEFT JOIN e IS NULL encuentran clientes sin vehículos.
    private const string SqlClientesSinVehiculos = """
        SELECT c.id_cliente, c.nombre
        FROM cliente c LEFT JOIN vehiculo v ON v.id_cliente = c.id_cliente
        WHERE v.id_vehiculo IS NULL
        ORDER BY c.id_cliente;
        """;

    // R3: GROUP BY agrupa servicios y HAVING filtra su ingreso acumulado.
    private const string SqlIngresoPorServicio = """
        SELECT s.nombre, COUNT(*), SUM(os.cantidad * os.precio) AS ingreso
        FROM servicio s INNER JOIN orden_servicio os ON os.id_servicio = s.id_servicio
        GROUP BY s.id_servicio, s.nombre
        HAVING SUM(os.cantidad * os.precio) > $minimo
        ORDER BY ingreso DESC, s.nombre;
        """;

    // R4: Una subconsulta calcula el promedio para comparar los precios.
    private const string SqlServiciosSobrePromedio = """
        SELECT id_servicio, nombre, precio_base,
               (SELECT AVG(precio_base) FROM servicio) AS promedio
        FROM servicio
        WHERE precio_base > (SELECT AVG(precio_base) FROM servicio)
        ORDER BY precio_base DESC, id_servicio;
        """;

    // R5: LIKE y OR buscan coincidencias; AND exige el año mínimo.
    private const string SqlBuscarVehiculos = """
        SELECT id_vehiculo, placa, marca, modelo, anio
        FROM vehiculo
        WHERE (marca LIKE $texto OR modelo LIKE $texto OR placa LIKE $texto)
          AND anio >= $anioMinimo
        ORDER BY id_vehiculo;
        """;

    // R6: ExecuteScalar obtiene una sola suma de las órdenes terminadas.
    private const string SqlTotalFacturado = """
        SELECT SUM(os.cantidad * os.precio)
        FROM orden o INNER JOIN orden_servicio os ON os.id_orden = o.id_orden
        WHERE o.estado = $estado;
        """;

    public static List<(int IdOrden, string Fecha, string Estado, string Placa, string Cliente, string Mecanico)> ListarOrdenes()
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = SqlOrdenes;
        using var lector = comando.ExecuteReader();
        var filas = new List<(int IdOrden, string Fecha, string Estado, string Placa, string Cliente, string Mecanico)>();
        while (lector.Read())
            filas.Add((lector.GetInt32(0), lector.GetString(1), lector.GetString(2),
                lector.GetString(3), lector.GetString(4), lector.GetString(5)));
        return filas;
    }

    public static List<(int IdCliente, string Nombre)> ClientesSinVehiculos()
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = SqlClientesSinVehiculos;
        using var lector = comando.ExecuteReader();
        var filas = new List<(int IdCliente, string Nombre)>();
        while (lector.Read())
            filas.Add((lector.GetInt32(0), lector.GetString(1)));
        return filas;
    }

    public static List<(string Nombre, int Veces, decimal Ingreso)> IngresoPorServicio(decimal minimo)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = SqlIngresoPorServicio;
        comando.Parameters.Add("$minimo", SqliteType.Real).Value = Convert.ToDouble(minimo);
        using var lector = comando.ExecuteReader();
        var filas = new List<(string Nombre, int Veces, decimal Ingreso)>();
        while (lector.Read())
            filas.Add((lector.GetString(0), lector.GetInt32(1), lector.GetDecimal(2)));
        return filas;
    }

    public static List<(int IdServicio, string Nombre, decimal PrecioBase, decimal Promedio)> ServiciosSobrePromedio()
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = SqlServiciosSobrePromedio;
        using var lector = comando.ExecuteReader();
        var filas = new List<(int IdServicio, string Nombre, decimal PrecioBase, decimal Promedio)>();
        while (lector.Read())
            filas.Add((lector.GetInt32(0), lector.GetString(1), lector.GetDecimal(2), lector.GetDecimal(3)));
        return filas;
    }

    public static List<(int IdVehiculo, string Placa, string Marca, string Modelo, int Anio)> BuscarVehiculos(string texto, int anioMinimo)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = SqlBuscarVehiculos;
        comando.Parameters.AddWithValue("$texto", "%" + texto + "%");
        comando.Parameters.AddWithValue("$anioMinimo", anioMinimo);
        using var lector = comando.ExecuteReader();
        var filas = new List<(int IdVehiculo, string Placa, string Marca, string Modelo, int Anio)>();
        while (lector.Read())
            filas.Add((lector.GetInt32(0), lector.GetString(1), lector.GetString(2), lector.GetString(3), lector.GetInt32(4)));
        return filas;
    }

    public static decimal TotalFacturado()
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = SqlTotalFacturado;
        comando.Parameters.AddWithValue("$estado", "terminada");
        object? total = comando.ExecuteScalar();
        return total == null || total == DBNull.Value ? 0 : Convert.ToDecimal(total);
    }
}
