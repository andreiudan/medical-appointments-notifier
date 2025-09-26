namespace MedicalAppointmentsNotifier.Core.Services
{
    internal class NameCorrector
    {
        public string CorrectName(string name)
        {
            name = name.ToLower().Trim();

            for (int i = 0; i < name.Length; i++)
            {
                if (!char.IsLetter(name[i]) && !char.IsWhiteSpace(name[i]) && !name[i].Equals('-'))
                {
                    throw new ArgumentException("User name can only contain letters and spaces.");
                }

                if (char.IsWhiteSpace(name[i]) && name[i].Equals('-') && i + 1 < name.Length)
                {
                    if (char.IsLetter(name[i + 1]))
                    {
                        name = name.Remove(i + 1, 1).Insert(i + 1, char.ToUpper(name[i + 1]).ToString());
                    }
                }
            }

            return name;
        }
    }
}
