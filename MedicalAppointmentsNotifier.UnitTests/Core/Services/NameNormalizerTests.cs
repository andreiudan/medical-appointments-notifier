using MedicalAppointmentsNotifier.Core.Services;

namespace MedicalAppointmentsNotifier.UnitTests.Core.Services
{
    [TestFixture]
    public class NameNormalizerTests
    {
        private NameNormalizer _nameNormalizer;

        [SetUp]
        public void SetUp()
        {
            _nameNormalizer = new NameNormalizer();
        }

        [TestCase("Name")]
        [TestCase("name")]
        [TestCase(" namE ")]
        public void Normalize_OneGivenName_ReturnNormalizedName(string name)
        {
            //Arrange
            const string ExpectedName = "Name";

            //Act
            string result = _nameNormalizer.Normalize(name);

            //Assert
            Assert.That(result, Is.EqualTo(ExpectedName));
        }

        [TestCase("Name Name")]
        [TestCase("name name")]
        [TestCase(" namE nAme")]
        public void Normalize_TwoGivenNameWithWhitespace_ReturnNormalizedName(string name)
        {
            //Arrange
            const string ExpectedName = "Name Name";

            //Act
            string result = _nameNormalizer.Normalize(name);

            //Assert
            Assert.That(result, Is.EqualTo(ExpectedName));
        }

        [TestCase("Name-Name")]
        [TestCase("name-name")]
        [TestCase(" namE-nAme ")]
        public void Normalize_TwoGivenNameWithHyphen_ReturnNormalizedName(string name)
        {
            //Arrange
            const string ExpectedName = "Name-Name";

            //Act
            string result = _nameNormalizer.Normalize(name);

            //Assert
            Assert.That(result, Is.EqualTo(ExpectedName));
        }
    }
}
