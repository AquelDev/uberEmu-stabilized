using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Uber.Storage
{
    internal sealed class DatabaseClient : IDisposable
    {
        public DatabaseManager Manager;
        public MySqlConnection Connection;
        public MySqlCommand Command;

        public DatabaseClient(DatabaseManager _Manager)
        {
            Manager = _Manager;
            Connection = new MySqlConnection(Manager.ConnectionString);
            Command = Connection.CreateCommand();
            Connection.Open();
        }

        public void Dispose()
        {
            Connection.Close();
            Command.Dispose();
            Connection.Dispose();
        }

        public void AddParamWithValue(string sParam, object val)
        {
            Command.Parameters.AddWithValue(sParam, val);
        }

        public void ExecuteQuery(string sQuery)
        {
            try
            {
                Command.CommandText = sQuery;
                Command.ExecuteScalar();
                Command.CommandText = null;
            }
            catch (Exception e) { UberEnvironment.GetLogging().WriteLine(e + "\n (" + sQuery + ")"); }
        }

        public bool findsResult(string sQuery)
        {
            bool Found = false;
            try
            {
                Command.CommandText = sQuery;
                MySqlDataReader dReader = Command.ExecuteReader();
                Found = dReader.HasRows;
                dReader.Close();

            }
            catch (Exception e) { UberEnvironment.GetLogging().WriteLine(e + "\n (" + sQuery + ")"); }
            return Found;
        }

        public DataSet ReadDataSet(string Query)
        {
            DataSet DataSet = new DataSet();
            Command.CommandText = Query;

            using (MySqlDataAdapter Adapter = new MySqlDataAdapter(Command))
            {
                Adapter.Fill(DataSet);
            }

            Command.CommandText = null;
            return DataSet;
        }

        public DataTable ReadDataTable(string Query)
        {
            DataTable DataTable = new DataTable();
            Command.CommandText = Query;

            using (MySqlDataAdapter Adapter = new MySqlDataAdapter(Command))
            {
                Adapter.Fill(DataTable);
            }

            Command.CommandText = null;
            return DataTable;
        }

        public DataRow ReadDataRow(string Query)
        {
            DataTable DataTable = ReadDataTable(Query);

            if (DataTable != null && DataTable.Rows.Count > 0)
            {
                return DataTable.Rows[0];
            }

            return null;
        }

        public string ReadString(string Query)
        {
            Command.CommandText = Query;
            string result = Command.ExecuteScalar().ToString();
            Command.CommandText = null;
            return result;
        }

        public Int32 ReadInt32(string Query)
        {
            Command.CommandText = Query;
            Int32 result = Int32.Parse(Command.ExecuteScalar().ToString());
            Command.CommandText = null;
            return result;
        }
    }
}
