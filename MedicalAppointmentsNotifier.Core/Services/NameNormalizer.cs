using MedicalAppointmentsNotifier.Domain.Interfaces;
using System.Text;

namespace MedicalAppointmentsNotifier.Core.Services
{
    internal class NameNormalizer : INameNormalizer
    {
        public string Normalize(string name)
        {
            name = name.ToLower().Trim();

            StringBuilder sb = new StringBuilder(name);

            for (int i = 0; i < name.Length; i++)
            {
                if(i == 0)
                {
                    sb.Replace(name[i], char.ToUpper(name[i]), i, 1);
                }

                if ((char.IsWhiteSpace(name[i]) || name[i].Equals('-')) && i + 1 < name.Length)
                {
                    if (char.IsLetter(name[i + 1]))
                    {
                        sb.Replace(name[i + 1], char.ToUpper(name[i + 1]), i + 1, 1);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
