using HotelListing3.API.Contracts;
using HotelListing3.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing3.API.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDbContext _context;

        public GenericRepository(HotelListingDbContext context)
        {
            _context = context;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        //
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        //
        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        //
        public async Task<bool> Exist(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        //
        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        //
        public async Task<T> GetAsync(int? id)
        {
            if(id == null) return null;

            return await _context.Set<T>().FindAsync(id);
        }

        /// //////////////////////////////////////
        //////////////////////////////////////////////
        //
        public async Task UpdateAsync(T entity)
        {
            // el "Update()" pone el EntityState en modified y ya luego se puede guardar cambios
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
