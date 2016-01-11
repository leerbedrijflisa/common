using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Lisa.Common.Sql
{
    public sealed class Gateway : IDisposable
    {
        public Gateway(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public object SelectSingle(string query, object parameters = null)
        {
            return SelectMany(query, parameters).FirstOrDefault();
        }

        public IEnumerable<object> SelectMany(string query, object parameters = null)
        {
            var command = CreateCommand(query, parameters);
            using (var reader = command.ExecuteReader())
            {
                var dataProvider = new SqlDataProvider(reader);
                var mapper = new ObjectMapper();
                return mapper.Many(dataProvider);
            }
        }

        public object Insert(string query, object parameters)
        {
            var command = CreateCommand(query, parameters);
            command.ExecuteNonQuery();

            command = CreateCommand("select @@identity");
            return command.ExecuteScalar();
        }

        public void Update(string query, object parameters)
        {
            var command = CreateCommand(query, parameters);
            command.ExecuteNonQuery();
        }

        public void Delete(string query, object parameters)
        {
            var command = CreateCommand(query, parameters);
            command.ExecuteNonQuery();
        }

        public void ProcessTransaction(Action transaction)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Already processing a transaction.");
            }

            using (_transaction = _connection.BeginTransaction())
            {

                try
                {
                    transaction();
                    _transaction.Commit();
                }
                catch
                {
                    _transaction.Rollback();
                    throw;
                }
            }

            _transaction = null;
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        private SqlCommand CreateCommand(string query, object parameters = null)
        {
            var commandText = QueryBuilder.Build(query, parameters);
            return new SqlCommand(commandText, _connection, _transaction);
        }

        private SqlConnection _connection;
        private SqlTransaction _transaction;
    }
}