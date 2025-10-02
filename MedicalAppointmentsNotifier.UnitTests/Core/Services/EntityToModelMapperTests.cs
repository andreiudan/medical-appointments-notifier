using MedicalAppointmentsNotifier.Core.Services;

namespace MedicalAppointmentsNotifier.UnitTests.Core.Services
{
    [TestFixture]
    public class EntityToModelMapperTests
    {
        private EntityToModelMapper mapper;

        [SetUp]
        public void SetUp()
        {
            mapper = new EntityToModelMapper();
        }
    }
}
