using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Repositorio
{
    public class VillaRepositorio : Repositorio<Villa>, IVillaRepositorio
    {
        private readonly AplicacionDBContext _dbContext;

        public VillaRepositorio(AplicacionDBContext dbContext) :base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Villa> Actualizar(Villa entidad)
        {
            entidad.FechaActualizacion = DateTime.Now;
             _dbContext.Villas.Update(entidad) ;
            await _dbContext.SaveChangesAsync();
            return entidad;
        }
    }
}
