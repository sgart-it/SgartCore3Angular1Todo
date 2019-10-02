using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SgartCoreAngular1Todo.Extensions
{
  public delegate void DBCommandDataReaderFunc(SqlDataReader dr, int rowNumber);

  public class DBCommand : IDisposable
  {
    private static readonly System.Globalization.CultureInfo ciIT = new System.Globalization.CultureInfo(1040);
    private static readonly System.Globalization.CultureInfo ciEN = new System.Globalization.CultureInfo(1033);

    private SqlTransaction transaction = null;
    private SqlConnection cnn = null;
    private SqlCommand cmd = null;
    private SqlParameter pReturnValue = null;
    private SqlDataReader dr = null;

    public DBCommand(string connectionString, string query, bool isStoreProcedure = false, bool addTransaction=false)
    {
      cnn = new SqlConnection(connectionString);
      if (addTransaction == true)
      {
        cnn.Open();
        transaction = cnn.BeginTransaction();
      }
      cmd = cnn.CreateCommand();
      cmd.CommandType = isStoreProcedure ? System.Data.CommandType.StoredProcedure : System.Data.CommandType.Text;
      cmd.CommandText = query;
      //cnn.Open();
    }

    public SqlConnection Connection
    {
      get
      {
        return cnn;
      }
    }

    public SqlCommand Command
    {
      get
      {
        return cmd;
      }
    }


    public async Task<int> ExecuteNonQueryAsync()
    {
      if (cnn.State == System.Data.ConnectionState.Closed)
        await cnn.OpenAsync();

      return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<T> ExecuteScalarAsync<T>()
    {
      if (cnn.State == System.Data.ConnectionState.Closed)
        await cnn.OpenAsync();

      return (T)await cmd.ExecuteScalarAsync();
    }

    public async Task<SqlDataReader> ExecuteReaderAsync()
    {
      if (cnn.State == System.Data.ConnectionState.Closed)
        await cnn.OpenAsync();

      return await cmd.ExecuteReaderAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowFunc"></param>
    /// <returns>ritorna i numero di righe lette</returns>
    public async Task<int> ExecuteReaderWhileAsync(DBCommandDataReaderFunc rowFunc)
    {
      if (dr != null)
        await dr.DisposeAsync();
      int i = 0;
      using (SqlDataReader dr = await ExecuteReaderAsync())
      {
        while (await dr.ReadAsync())
        {
          rowFunc(dr, i);
          i++;
        }
      }
      return i;
    }

    public async Task<int> ExecuteReaderFirstAsync(DBCommandDataReaderFunc rowFunc)
    {
      if (dr != null)
        await dr.DisposeAsync();
      int i = 0;
      using (SqlDataReader dr = await ExecuteReaderAsync())
      {
        if (await dr.ReadAsync())
        {
          rowFunc(dr, i);
          i++;
        }
      }
      return i;
    }
    public int GetReturnValue()
    {
      if (pReturnValue == null)
        throw new ArgumentNullException("Parameter not defined");
      return (int)pReturnValue.Value;
    }

    public async Task CommitAsync()
    {
      await transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
      await transaction.RollbackAsync();
    }


    public void Dispose()
    {
      if (dr != null)
      {
        dr.Close();
        dr.DisposeAsync();
      }
      if (cmd != null)
        cmd.Dispose();
      if (cnn != null)
        cnn.Dispose();
      if (transaction != null)
        transaction.Dispose();
    }

    #region Parameters

    public SqlParameter AddParameterReturnValue()
    {
      pReturnValue = cmd.Parameters.Add("@Return_Value", SqlDbType.Int);
      pReturnValue.Direction = ParameterDirection.Output;
      return pReturnValue;
    }

    public SqlParameter AddParameterInt(string name, int value)
    {
      SqlParameter p = cmd.Parameters.Add(name, SqlDbType.Int);
      p.Value = value;
      return p;
    }

    public SqlParameter AddParameterString(string name, int size, string value)
    {
      SqlParameter p = cmd.Parameters.Add(name, SqlDbType.NVarChar, size);
      if (string.IsNullOrWhiteSpace(value))
        p.Value = string.Empty;
      else
        p.Value = value;
      return p;
    }

    public SqlParameter AddParameterStringNull(string name, int size, string value)
    {
      SqlParameter p = cmd.Parameters.Add(name, SqlDbType.NVarChar, size);
      if (string.IsNullOrWhiteSpace(value))
        p.Value = DBNull.Value;
      else
        p.Value = value;
      return p;
    }

    public SqlParameter AddParameterNote(string name, string value)
    {
      SqlParameter p = cmd.Parameters.Add(name, SqlDbType.NVarChar, -1);
      if (string.IsNullOrWhiteSpace(value))
        p.Value = DBNull.Value;
      else
        p.Value = value;
      return p;
    }

    public SqlParameter AddParameterDate(string name, DateTime? value)
    {
      SqlParameter p = cmd.Parameters.Add(name, SqlDbType.DateTime);
      if (value.HasValue == false)
        p.Value = DBNull.Value;
      else
        p.Value = value.Value.Date;
      return p;
    }

    public SqlParameter AddParameterDateTime(string name, DateTime? value)
    {
      SqlParameter p = cmd.Parameters.Add(name, SqlDbType.DateTime);
      if (value.HasValue == false)
        p.Value = DBNull.Value;
      else
        p.Value = value.Value;
      return p;
    }

    public SqlParameter AddParameterBinaryNull(string name, int size, byte[] value)
    {
      SqlParameter p = cmd.Parameters.Add(name, SqlDbType.VarBinary, size);
      if (value == null)
        p.Value = DBNull.Value;
      else
        p.Value = value;
      return p;
    }
    #endregion

  }
}
