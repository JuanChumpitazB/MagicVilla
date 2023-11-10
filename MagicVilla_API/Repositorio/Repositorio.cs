using MagicVilla_API.Datos;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_API.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly AplicacionDBContext _dbContext;
        internal DbSet<T> dbset;

        public Repositorio(AplicacionDBContext dbContext)
        {
            _dbContext = dbContext;
            this.dbset = _dbContext.Set<T>();
        }

        public async Task Crear(T entidad)
        {
            await _dbContext.AddAsync(entidad);
            await Grabar();
        }

        public async Task Grabar()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true)
        {
            IQueryable<T> query = dbset.AsQueryable();
            if(!tracked) query = query.AsNoTracking();
            if(filtro != null) query = query.Where(filtro);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null)
        {
            IQueryable<T> query = dbset;
            if (filtro != null) query = query.Where(filtro);
            return await query.ToListAsync();
        }

        public async Task Remover(T entidad)
        {
            _dbContext.Remove(entidad);
            await Grabar();
        }
    }
}
