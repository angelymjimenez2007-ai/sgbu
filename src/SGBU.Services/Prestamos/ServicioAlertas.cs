using SGBU.Domain.Repositorios;
using SGBU.Services.Seguridad;

namespace SGBU.Services.Prestamos;

// Alertas de vencimiento de préstamos.
public class ServicioAlertas
{
    private readonly IFabricaUnidadDeTrabajo fabrica;

    public ServicioAlertas(IFabricaUnidadDeTrabajo fabrica)
    {
        this.fabrica = fabrica;
    }

    public async Task<IReadOnlyList<string>> ObtenerAlertasAsync(Sesion sesion)
    {
        List<string> alertas = new List<string>();

        await using IUnidadDeTrabajo uow = await fabrica.CrearAsync();

        var prestamos = await uow.Prestamos.ListarActivosDetalleAsync(
            null, sesion.UsuarioId);

        foreach (PrestamoDetalle prestamo in prestamos)
        {
            int dias = (prestamo.FechaVencimiento.Date - DateTime.Today).Days;

            if (dias < 0)
            {
                alertas.Add("El préstamo de " + prestamo.Titulo +
                    " está vencido hace " + Math.Abs(dias) + " día(s).");
            }
            else if (dias == 0)
            {
                alertas.Add("El préstamo de " + prestamo.Titulo +
                    " vence hoy.");
            }
            else if (dias <= 3)
            {
                alertas.Add("El préstamo de " + prestamo.Titulo +
                    " vence en " + dias + " día(s).");
            }
        }

        return alertas;
    }
}
