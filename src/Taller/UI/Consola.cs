using System.Globalization;

namespace Taller.UI;

public static class Consola
{
    public static string LeerTexto(string mensaje)
    {
        while (true)
        {
            string texto = LeerTextoOpcional(mensaje);
            if (texto.Length > 0)
                return texto;
            Console.WriteLine("El texto no puede estar vacío.");
        }
    }

    public static string LeerTextoOpcional(string mensaje)
    {
        Console.Write(mensaje);
        string? texto = Console.ReadLine();
        if (texto == null)
            throw new EndOfStreamException("Se cerró la entrada de la consola.");
        return texto.Trim();
    }

    public static int LeerEntero(string mensaje)
    {
        while (true)
        {
            if (int.TryParse(LeerTextoOpcional(mensaje), out int valor))
                return valor;
            Console.WriteLine("Ingresa un número entero válido.");
        }
    }

    public static decimal LeerDecimal(string mensaje)
    {
        while (true)
        {
            if (decimal.TryParse(LeerTextoOpcional(mensaje),
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.CurrentCulture, out decimal valor))
                return valor;
            Console.WriteLine("Ingresa un decimal válido usando el separador " +
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + ".");
        }
    }

    public static void Pausa()
    {
        Console.Write("Presiona Enter para continuar...");
        Console.ReadLine();
    }

    public static void Titulo(string texto)
    {
        Console.WriteLine();
        Console.WriteLine("=== " + texto + " ===");
    }
}
