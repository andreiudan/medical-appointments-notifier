namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IRepository<TModel> where TModel : class
    {
        public TModel Add(TModel model);

        public TModel Update(TModel model);

        public bool Delete(TModel model);

        public TModel GetById(Guid id);

        public Task<List<TModel>> GetAll();
    }
}
