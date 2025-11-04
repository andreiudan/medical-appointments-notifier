using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MedicalAppointmentsNotifier.Data.Repositories
{
    public class Repository<TModel> : IRepository<TModel> where TModel : BaseEntity
    {
        protected readonly DbContext context;

        public Repository(MedicalAppointmentsContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TModel> AddAsync(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            await context.Set<TModel>().AddAsync(model).ConfigureAwait(false);

            await context.SaveChangesAsync().ConfigureAwait(false);

            return model;
        }

        public async Task<bool> DeleteAsync(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            await Task.Run(() => context.Set<TModel>().Remove(model));
            await context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        public async Task<bool> DeleteAsync(Guid modelId)
        {
            var model = await context.Set<TModel>().FindAsync(modelId).ConfigureAwait(false);
            if (model == null)
            {
                return false;
            }

            context.Set<TModel>().Remove(model);
            await context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        public async Task<List<TModel>> GetAllAsync()
        {
            return await Task.Run(() => context.Set<TModel>().AsNoTracking().IgnoreAutoIncludes().ToList()).ConfigureAwait(false);
        }

        public async Task<TModel> FindAsync(Expression<Func<TModel, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return await context.Set<TModel>().AsNoTracking().FirstOrDefaultAsync(predicate).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TModel>> FindAllAsync(Expression<Func<TModel, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return await context.Set<TModel>().AsNoTracking().Where(predicate).ToListAsync().ConfigureAwait(false);
        }

        public async Task<bool> UpdateAsync(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var original = await context.Set<TModel>().FindAsync(model.Id).ConfigureAwait(false);
            if (original == null)
            {
                return false;
            }

            context.Entry(original).CurrentValues.SetValues(model);

            await context.SaveChangesAsync().ConfigureAwait(false);
            
            return true;
        }
    }
}
