using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RuukotoBot
{
    public partial class Ruukoto
    {
        private readonly Type userInfoType = typeof(UserInfo);
        private readonly Type userType = typeof(SocketGuildUser);
        private readonly Type msgType = typeof(SocketMessage);
        private readonly Type argsType = typeof(string[]);


        private CommandData[] commands;
        private readonly List<object> parameters = new List<object>();
        protected override async Task OnDiscordCommand(SocketMessage msg, string cmdName, string[] args)
        {
            var user = _guild.GetUser(msg.Author.Id);
            
            foreach (var com in commands)
            {
                var command = com.attribute;
                var method = com.method;

                if (command.Names.Contains(cmdName))
                {
                    if (command.ForAdmin && !user.GuildPermissions.Administrator)
                    {
                        await msg.RespondAsync("Команда только для админов");
                        return;
                    }

                    parameters.Clear();
                    foreach (var parameter in method.GetParameters().OrderBy(x => x.Position))
                    {
                        var parameterType = parameter.ParameterType;
                        if (parameterType == userType)
                            parameters.Add(user);

                        if (parameterType == userInfoType)
                            parameters.Add(GetUserInfo(user.Id));

                        if (parameterType == msgType)
                            parameters.Add(msg);

                        else if (parameterType == argsType)
                            parameters.Add(args);
                    }

                    object result = method.Invoke(this, parameters.ToArray());
                    parameters.Clear();

                    if (result is Task<string> stringTask)
                        result = await stringTask;

                    if (result is Task<object> objectTask)
                        result = await objectTask;

                    if (result is Task task) { await task; result = null; }

                    if (result == null) return;
                    if (!(result is string output)) output = result.ToString();

                    await msg.RespondAsync(output);
                    return;
                }
            }
            string helpText = help;
            if (user.GuildPermissions.Administrator) helpText += "\n" + adminHelp;
            await msg.RespondAsync(helpText);

        }

        public class CommandData
        {
            public readonly MethodInfo method;
            public readonly CommandAttribute attribute;

            public CommandData(MethodInfo method, CommandAttribute attribute)
            {
                this.method = method;
                this.attribute = attribute;
            }
        }
    }
}
