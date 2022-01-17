using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*importamos*/
using MySql.Data.MySqlClient;
/*/ para trabajar con listas */
using System.Collections.Generic; 
using System.Data.Common;
using System.Data;

namespace softPedidos.App
{
   public class DbHelper
    {

        //PROPIEDADES
        //la cadena de conexion
        private string _Connectionstring = "";

        // clase abstracta que representa  una conexion a una base de datos
        private DbConnection _Connection;
        // clase anstracta que representa  uan senteciaSql o procedimiento almacenado
        private DbCommand _Command;
        //clase anstracta que provee un conjnto de metodos para crear instancia de conexion a distintos motores de db
        private DbProviderFactory _factory = null;
        private DbProviders _provider;
        // objecto de tipo ENUM que indica como se interpretar la propiedad ComandText del comando  del comando (1 -query ,4 -procedimiento ,512 -table Direct)                 
        private CommandType _CommandType;


        // get y setters

        public string Connectionstring { get => _Connectionstring; set => _Connectionstring = value; }
        public DbConnection Connection { get => _Connection; set => _Connection = value; }
        public DbCommand Command { get => _Command; set => _Command = value; }
        public DbProviderFactory Factory { get => _factory; set => _factory = value; }
        public DbProviders Provider { get => _provider; set => _provider = value; }
        public CommandType CommandType { get => _CommandType; set => _CommandType = value; }


        //CONSTRUCTOR



        public DbHelper(string ConnectSring ,CommandType commandType,DbProviders ProviderNane = DbHelper.DbProviders.MySQL)
        {
            _provider = ProviderNane;
            _CommandType = commandType;

            _factory = MySqlClientFactory.Instance;

            Connection = _factory.CreateConnection();
            Connection.ConnectionString = ConnectSring;
            Command = _factory.CreateCommand();
            Command.Connection = Connection;

        }



        private void  BeginTransaction()
        {
            if(Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            Command.Transaction = Connection.BeginTransaction();

        }


        private void CommitTransaction()
        {

            if(Connection.State == ConnectionState.Open)
            {
                Command.Transaction.Commit();
                Connection.Close();
            }

            

        }

        private void RollbackTransaction()
        {
            if(Connection.State == ConnectionState.Open)
            {
                Command.Transaction.Rollback();
                Connection.Close();
            }
        }



        public  int  CRUD(string query )
        {


            Command.CommandText = query;
            Command.CommandType = CommandType;
            int i = -1;


            try
            {

                if(Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();

                }
                BeginTransaction();
                i = Command.ExecuteNonQuery();
                CommitTransaction();
            }
            catch (Exception ex)
            {


                RollbackTransaction();
                //logs
                
            }

            finally
            {
                Command.Parameters.Clear();
                if(Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Command.Dispose();
                }
            }

            return i;


        }


        public  DataTable GetDataTable(string query )
        {
            DbDataAdapter adapter = _factory.CreateDataAdapter();
            Command.CommandText = query;
            Command.CommandType = CommandType;
            adapter.SelectCommand = Command;
            DataSet ds = new  DataSet();

            try
            {
                adapter.Fill(ds);
            }
            catch (Exception)
            {
                //logs
                
                //throw;
            }

            finally
            {
                Command.Parameters.Clear();
                if(Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Command.Dispose();
                }
            }

            return ds.Tables[0];

        }
            

        


        //lista de proveedores de base de datos

        public enum DbProviders
        {
            MySQL,sqlserver,oracle,oleDb,SQLite
        }

    }
}
