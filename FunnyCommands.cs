using Sidit.Randomizer;
using System.Threading.Tasks;

namespace RuukotoBot
{
    public partial class Ruukoto
    {
        [Command("правда", "правда?")]
        public string TrueOrFalse()
        {
            return Randomizer.ChooseRandom("Правда", "Неправда");
        }

        [Command("как", "как?")]
        public string Kak()
        {
            return Randomizer.ChooseRandom("Хуняй", "Кринж", "Норм", "Ахуенно", "Это база");
        }

        
    }
}
