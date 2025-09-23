using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointmentsNotifier.Data.Repositories
{
    internal class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        protected readonly DbContext context;

        public Repository(DbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TModel Add(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return context.Set<TModel>().Add(model).Entity;
        }

        public bool Delete(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            context.Set<TModel>().Remove(model);

            return true;
        }

        public List<TModel> GetAll()
        {
            return context.Set<TModel>().ToList();
        }

        public TModel GetById(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentException("The id cannot be empty.", nameof(id));
            }

            return context.Set<TModel>().Find(id);
        }

        public TModel Update(TModel model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return context.Set<TModel>().Update(model).Entity;
        }
    }
}
