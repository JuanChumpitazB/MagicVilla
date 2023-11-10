using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Repositorio
{
    public class NumeroVillaRepositorio : Repositorio<NumeroVilla>, INumeroVillaRepositorio
    {
        private readonly AplicacionDBContext _dbContext;

        public NumeroVillaRepositorio(AplicacionDBContext dbContext) :base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<NumeroVilla> Actualizar(NumeroVilla entidad)
        {
            entidad.FechaActualizacion = DateTime.Now;
             _dbContext.NumeroVillas.Update(entidad) ;
            await _dbContext.SaveChangesAsync();
            return entidad;
        }
    }
}
