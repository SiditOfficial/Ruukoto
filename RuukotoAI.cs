using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Discord.WebSocket;
using Sidit.Randomizer;

namespace RuukotoBot
{
    public partial class Ruukoto
    {
        private SqliteConnection _sqliteConnection;
        private void OpenSqliteConnection()
        {
            _sqliteConnection = new SqliteConnection("Filename=D:/SQLite/Ruukoto.db");
            _sqliteConnection.Open();
        }

        private async Task Check2(SocketMessage msg)
        {
            if (msg.Content.StartsWith('%')) return;
            if (msg.Channel.Id != 1061316882034065518) return;
            if (msg.Source != Discord.MessageSource.User) return;

            var command = _sqliteConnection.CreateCommand();
            command.CommandText = "select Key, Value from Dictionary";
            using var reader = command.ExecuteReader();

            string msgContent = msg.Content.ToLower().Replace(" ", "");

            float bestResult = 0f;
            string output = "Абоба";
            while (reader.Read())
            {
                string rootKey = reader.GetString(0).ToLower();
                if (!rootKey.Contains('`'))
                {
                    CheckKey(rootKey);
                }
                else
                {
                    string[] keys = rootKey.Split('`');
                    foreach (string key in keys)
                    {
                        CheckKey(key);
                    }
                }
                
                void CheckKey(string key)
                {
                    float result = StringFunc(key, msgContent);
                    if (result > bestResult)
                    {
                        bestResult = result;
                        output = reader.GetString(1);
                        if (output.Contains('`'))
                        {
                            output = Randomizer.GetRandomFrom(reader.GetString(1).Split('`'));
                        }
                    }
                }
            }

            await msg.RespondAsync(output);
        }

        private float StringFunc(string first, string second)
        {
            int secLen = second.Length;
            int similarCharsCount = 0;
            for (int a = 0; a < first.Length; a++)
            {
                for (int b = 0; b < second.Length;)
                {
                    if (first[a] == second[b])
                    {
                        second = second.Remove(b, 1);
                        similarCharsCount++;

                        if (second.Length == 0)
                            return GetResult();
                    }
                    else b++;
                }
            }

            return GetResult();

            float GetResult() => similarCharsCount / (float)(first.Length + secLen - similarCharsCount);
        }
    }
}
