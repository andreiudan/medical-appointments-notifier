using System.Linq.Expressions;

namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IRepository<TModel> where TModel : class
    {
        public Task<TModel> AddAsync(TModel model);

        public Task<bool> UpdateAsync(TModel model);

        public Task<bool> DeleteAsync(TModel model);

        public Task<bool> DeleteAsync(Guid modelId);

        public Task<TModel> FindAsync(Expression<Func<TModel, bool>> predicate);

        public Task<IEnumerable<TModel>> FindAllAsync(Expression<Func<TModel, bool>> predicate);

        public Task<List<TModel>> GetAllAsync();
    }
}
