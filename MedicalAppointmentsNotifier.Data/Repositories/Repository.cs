using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MedicalAppointmentsNotifier.Data.Repositories
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        protected readonly DbContext context;

        public Repository(MedicalAppointmentsContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TModel> AddAsync(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var entry = await context.Set<TModel>().AddAsync(model);
            TModel addedModel =entry.Entity;

            await context.SaveChangesAsync();

            return addedModel;
        }

        public async Task<bool> DeleteAsync(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            await Task.Run(() => context.Set<TModel>().Remove(model));

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<TModel>> GetAllAsync()
        {
            return await Task.Run(() => context.Set<TModel>().IgnoreAutoIncludes().ToList());
        }

        public async Task<TModel> FindAsync(Expression<Func<TModel, bool>> predicate)
        {
            if(predicate == null)
            {
                throw new ArgumentException("The id cannot be empty.", nameof(predicate));
            }

            return await context.Set<TModel>().FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> UpdateAsync(TModel model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            try
            {
                if(context.Entry(model).State == EntityState.Detached)
                {
                    context.Set<TModel>().Attach(model);
                }

                context.Entry(model).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                return false;
            }

            await context.SaveChangesAsync();

            return true;
        }
    }
}
