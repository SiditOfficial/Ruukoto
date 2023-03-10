using System;
using Microsoft.Data.Sqlite;

namespace RuukotoBot
{
    public class UserInfo : TableInfo
    {
        public UserInfo(ulong id, SqliteConnection connection) : base(connection, "Users", $"WHERE id = {id}")
        {
            if (Get("id") != null) return;
            using var command = _connection.CreateCommand();
            command.CommandText = $"INSERT INTO Users(id) VALUES ({id})";
            command.ExecuteNonQuery();
        }

        public double Credits
        {
            get => double.Parse((string)Get("credits"));
            set => Set($"credits = '{value}'");
        }

        public double Moneu
        {
            get => double.Parse((string)Get("moneu"));
            set => Set($"moneu = '{value}'");
        }

        public int JobID
        {
            get => (int)(long)Get("jobId");
            set => Set($"jobId = {value}");
        }

        public JobInfo Job
        {
            get => new JobInfo(JobID, _connection);
        }

        public DateTime LastSalary
        {
            get => DateTime.Parse((string)Get("lastsalary"));
            set => Set($"lastsalary = '{value}'");
        }
    }
}
