using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Globalization;

namespace RuukotoBot
{
    public partial class Ruukoto
    {
        public string Convert(double d)
        {
            string output = d.ToString("N0", CultureInfo.InvariantCulture).Replace(",", " ");
            if (output.StartsWith('-')) output = output.Replace("-", "минус ");
            if (output.Contains("Infinity")) output = output.Replace("Infinity", "дохуя");
            return output;
        }

        [Command("статус", "стс")]
        public string GetStatus(UserInfo userInfo)
        {
            return $"{Convert(userInfo.Credits)} кредитов";
        }

        [Command("баланс", "бал", "бл")]
        public string GetMoneu(UserInfo userInfo)
        {
            return $"{Convert(userInfo.Moneu)} тохорублей";
        }

        [Command("зарплата", "зп")]
        public string GetSalary(UserInfo userInfo)
        {
            var job = userInfo.Job;
            var org = job.Organization;

            double hoursWorkTime = (DateTime.Now - userInfo.LastSalary).TotalHours;
            double salary = userInfo.Job.Wages * hoursWorkTime;

            if (org.Balance < salary)
                return "У вашей организации недостаточно средств чтоб оплатить ваш труд.";

            org.Balance -= salary;
            userInfo.Moneu += salary;
            userInfo.LastSalary = DateTime.Now;

            return $"Ты получил(а) зарплату {Convert(salary)} тохорублей\nЗа {Convert(hoursWorkTime)} часов работы";
        }

        [Command("работа")]
        public string Job(UserInfo userInfo)
        {
            var job = userInfo.Job;
            return $"Место работы: {job.Organization.Name}\n" +
                   $"Должность: {job.Name}\n" +
                   $"Зарплата {Convert(job.Wages)} тохорублей в час";
        }

        [Command("организация", "орг")]
        public string Org(UserInfo userInfo, string[] args)
        {
            var org = userInfo.Job.Organization;

            if (args.Length != 0)
            {
                if (args[0] == "взнос")
                {
                    if (!ParseDouble(args[1], out double sum)) return "Цыфры нормально введи";
                    sum = Math.Abs(sum);
                    if (userInfo.Moneu < sum) return "Недостаточно средств";
                    userInfo.Moneu -= sum;
                    org.Balance += sum;
                    return $"Казна пополнена на {sum} тохорублей";
                }
            }
            return $"Организация: {org.Name}\n" +
                   $"Описание: {org.Description}\n" +
                   $"Баланс: {Convert(org.Balance)} тохорублей";
        }

        [Command(true, "назначить")]
        public string Nanat(SocketMessage msg, string[] args)
        {
            if (args.Length == 0) return "А где?";
            if (!int.TryParse(args[0], out int jobId)) return "Чё с цифрами";

            string output = "";
            foreach (var user in msg.MentionedUsers)
            {
                var userInfo = GetUserInfo(user.Id);
                userInfo.JobID = jobId;
                userInfo.LastSalary = DateTime.Now;
                output += $"{user.Mention} назначен как {userInfo.Job.Name}\n";
            }
            return output;
        }

        [Command("уволиться")]
        public string ToQuit(UserInfo userInfo)
        {
            if (userInfo.JobID == 0) return "Тебе неоткудо увольняться";
            userInfo.JobID = 0;
            return "Ты уволился";
        }

        [Command("передать")]
        public string Pay(SocketMessage msg, UserInfo userInfo, string[] args)
        {
            if (args.Length == 0) return "Введите сумму";
            if (!ParseDouble(args[0], out double sum)) return "Цыфры нормально введи";
            sum = Math.Abs(sum);
            if (userInfo.Moneu < sum) return "Недостаточно средств";
            foreach (var user in msg.MentionedUsers)
            {
                var targetUser = GetUserInfo(user.Id);
                userInfo.Moneu -= sum + 15;
                targetUser.Moneu += sum;
                return $"{msg.Author.Mention} передал {user.Mention} {Convert(sum)} тохорублей";
            }
            return "Пинганите кого нибудь";
        }

        public bool ParseDouble(string str, out double result)
        {
            return double.TryParse(str.Replace('.', ','), out result);
        }

        [Command(true, "дать")]
        public async Task<string> AddCredits(SocketMessage msg, string[] args)
        {
            if (!ParseDouble(args[0], out double add)) return "Цыфры нормально введи";
            foreach (var user in msg.MentionedUsers)
            {
                var userInfo = GetUserInfo(user.Id);
                double newCredits = userInfo.Credits + add;
                if (double.IsNaN(newCredits)) newCredits = 0;

                userInfo.Credits = newCredits;
                await msg.RespondAsync($"{user.Mention} получил(а) {add} кредитов!");
            }
            return null;
        }


        private UserInfo GetUserInfo(ulong id)
        {
            return new UserInfo(id, _sqliteConnection);
        }
    }
}
