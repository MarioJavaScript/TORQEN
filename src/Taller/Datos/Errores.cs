using Microsoft.Data.Sqlite;

namespace Taller.Datos;

public static class Errores
{
    public static string Mensaje(SqliteException ex)
    {
        return ex.SqliteExtendedErrorCode switch
        {
            787 => "El registro relacionado no existe o está siendo utilizado (FOREIGN KEY).",
            2067 or 1555 => "Ya existe un registro con ese valor único (UNIQUE o PRIMARY KEY).",
            275 => "El valor no cumple las condiciones permitidas (CHECK).",
            1299 => "Falta un dato obligatorio (NOT NULL).",
            _ => "Error de base de datos: " + ex.Message
        };
    }
}
