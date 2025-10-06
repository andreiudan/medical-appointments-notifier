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

            await context.Set<TModel>().AddAsync(model);

            await context.SaveChangesAsync();

            return model;
        }

        public async Task<bool> DeleteAsync(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            await Task.Run(() => context.Set<TModel>().Remove(model));

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid modelId)
        {
            ArgumentNullException.ThrowIfNull(modelId);

            TModel model = await context.Set<TModel>().FindAsync(modelId);

            if(model == null)
            {
                return false;
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
            ArgumentNullException.ThrowIfNull(predicate);

            return await context.Set<TModel>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TModel>> FindAllAsync(Expression<Func<TModel, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return await context.Set<TModel>().Where(predicate).ToListAsync();
        }

        public async Task<bool> UpdateAsync(TModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            TModel originalModel = await context.Set<TModel>().FindAsync(model.Id);
            if (originalModel == null)
            {
                return false;
            }

            context.Entry(originalModel).CurrentValues.SetValues(model);

            await context.SaveChangesAsync();

            return true;
        }
    }
}
