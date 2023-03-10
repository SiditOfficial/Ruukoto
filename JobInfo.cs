using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

namespace RuukotoBot
{
    public abstract class TableInfo
    {
        protected readonly SqliteConnection _connection;
        protected readonly string _tableName;
        protected readonly string _where;

        protected TableInfo(SqliteConnection connection, string tableName, string where)
        {
            _connection = connection;
            _tableName = tableName;
            _where = where;
        }

        protected T Get<T>(string name) => (T)Get(name);
        protected virtual object Get(string name)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT {name} FROM {_tableName} " + _where;
            return command.ExecuteScalar();
        }

        protected virtual void Set(string nameValue)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = $"UPDATE {_tableName} SET {nameValue} " + _where;
            command.ExecuteNonQuery();
        }
    }

    public class JobInfo : TableInfo
    {
        public JobInfo(int id, SqliteConnection connection)
            : base(connection, "Jobs", $"WHERE id = {id}") { }

        public long OrgID
        {
            get => Get<long>("orgId");
        }

        public OrganizationInfo Organization
        {
            get => new OrganizationInfo(OrgID, _connection);
        }

        public string Name
        {
            get => Get<string>("name");
            set => Set($"name='{value}'");
        }

        public double Wages
        {
            get => Get<double>("wages");
            set => Set($"wages={value}");
        }


    }

    public class OrganizationInfo : TableInfo
    {
        public OrganizationInfo(long id, SqliteConnection connection)
            : base(connection, "Organizations", $"WHERE id = {id}") { }

        public ulong OwnerID
        {
            get => (ulong)Get<long>("employerid");
        }

        public string Name
        {
            get => Get<string>("name");
        }

        public string Description
        {
            get => Get<string>("description");
        }

        public double Balance
        {
            get => double.Parse(Get<string>("balance"));
            set => Set($"balance='{value}'");
        }
    }
}
