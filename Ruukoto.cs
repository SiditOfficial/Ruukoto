using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using Sidit.Randomizer;

namespace RuukotoBot
{
    public partial class Ruukoto : DiscordBot
    {
        private SocketGuild _guild;

        public string help;
        public string adminHelp;

        protected async override Task OnStart()
        {
            await base.OnStart();

            OpenSqliteConnection();

            Client.MessageReceived += Check2;

            List<CommandData> commandDatas = new List<CommandData>();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            foreach (var method in GetType().GetMethods(flags))
            {
                var command = method.GetCustomAttribute<CommandAttribute>();
                if (command != null)
                {
                    commandDatas.Add(new CommandData(method, command));
                }
            }
            commands = commandDatas.ToArray();

            help = "Доступные команды:\n";
            foreach (var com in commands.Where(x => !x.attribute.ForAdmin))
            {
                help += "(" + string.Join(", ", com.attribute.Names) + ")";
                help += " ";
            }

            adminHelp = "Админ команды:\n";
            foreach (var com in commands.Where(x => x.attribute.ForAdmin))
            {
                adminHelp += "(" + string.Join(", ", com.attribute.Names) + ")";
                adminHelp += " ";
            }


            _guild = Client.GetGuild(925678799062966313);
            while (_guild == null)
            {
                _guild = Client.GetGuild(925678799062966313);
                await Task.Delay(1000);
                Console.WriteLine("Не удалось получить гильдию, ожидане");
            }
            _currentChanel = _guild.GetTextChannel(965306450396196884);

            emote = await _guild.GetEmoteAsync(962689997952155698);

            Console.WriteLine("Всё ок");
            StartConsoleCommands();
        }
        private GuildEmote emote;


        

        

        private SocketTextChannel _currentChanel;
        protected override async Task OnConsoleCommand(string cmdName, string[] args)
        {
            ulong ParseArg(int index) => ulong.Parse(args[index]);
            switch (cmdName)
            {
                case "move":
                    _currentChanel = _guild.GetTextChannel(ParseArg(0));
                    break;
                case "res":
                    var msg = await _currentChanel.GetMessageAsync(ParseArg(0));
                    await msg.RespondAsync(string.Join(" ", args.Skip(1)).FirstCharToUpper());
                    break;
            }
        }
        protected override async Task DefaultConsoleCommand(string input)
        {
            await _currentChanel.SendMessageAsync(input);
        }

        private async Task SendInChanel(ulong chatID ,string content)
        {
            await _guild.GetTextChannel(chatID).SendMessageAsync(content);
        }
    }
}
