using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RuukotoBot
{
    partial class Ruukoto
    {
        [Command("топ")]
        public async Task<string> Baza(string[] args)
        {
            string target = "moneu";
            string targetName = "тохорублей";
            if (args.Length != 0)
            {
                if (args[0] == "кредитов")
                {
                    target = "credits";
                    targetName = "кредитов";
                }
            }
            using var command = _sqliteConnection.CreateCommand();
            command.CommandText = $"SELECT * FROM Users ORDER BY {target} DESC";
            using var reader = command.ExecuteReader();
            string output = "";

            Dictionary<ulong, double> top = new Dictionary<ulong, double>();
            while (reader.Read())
            {
                top.Add((ulong)reader.GetInt64(0), double.Parse(reader[target].ToString()));
            }
            int i = 1;
            foreach (var us in top.OrderByDescending(x => x.Value))
            {
                var user = await Client.GetUserAsync(us.Key);
                output += $"{i} место {Convert(us.Value)} {targetName} **({user.Username})**\n";
                if (++i > 10) break;
            }
            return output;
        }
    }
}
