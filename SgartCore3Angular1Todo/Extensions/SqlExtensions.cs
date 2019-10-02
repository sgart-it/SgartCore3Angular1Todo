using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SgartCoreAngular1Todo.Extensions
{
  public static class SqlExtensions
  {
    //public static SqlConnection GetSqlConnection()
    //{
    //  string connectionString = "Server=(local);Database=NodeJs;Trusted_Connection=True;";
    //  return new SqlConnection(connectionString);
    //}

    //public static SqlCommand GetSqlCommand(this SqlConnection cnn, string query, bool isStoreProcedure = false)
    //{
    //  SqlCommand cmd = cnn.CreateCommand();
    //  cmd.CommandType = isStoreProcedure ? System.Data.CommandType.StoredProcedure : System.Data.CommandType.Text;
    //  cmd.CommandText = query;
    //  return cmd;
    //}

    //#region Parameters

    //public static SqlParameter AddParameterInt(this SqlCommand cmd, string name, int value)
    //{
    //  SqlParameter p = cmd.Parameters.Add(name, System.Data.SqlDbType.Int);
    //  p.Value = value;
    //  return p;
    //}

    //public static SqlParameter AddParameterString(this SqlCommand cmd, string name, int size, string value)
    //{
    //  SqlParameter p = cmd.Parameters.Add(name, System.Data.SqlDbType.NVarChar, size);
    //  p.Value = value ?? "";
    //  return p;
    //}

    //public static SqlParameter AddParameterStringNull(this SqlCommand cmd, string name, int size, string value)
    //{
    //  SqlParameter p = cmd.Parameters.Add(name, System.Data.SqlDbType.NVarChar, size);
    //  if (value == null)
    //    p.Value = DBNull.Value;
    //  else
    //    p.Value = value;
    //  return p;
    //}

    //public static SqlParameter AddParameterDate(this SqlCommand cmd, string name, DateTime? value)
    //{
    //  SqlParameter p = cmd.Parameters.Add(name, System.Data.SqlDbType.DateTime);
    //  if (value.HasValue == false)
    //    p.Value = DBNull.Value;
    //  else
    //    p.Value = value.Value.Date;
    //  return p;
    //}

    //public static SqlParameter AddParameterDateTime(this SqlCommand cmd, string name, DateTime? value)
    //{
    //  SqlParameter p = cmd.Parameters.Add(name, System.Data.SqlDbType.DateTime);
    //  if (value.HasValue == false)
    //    p.Value = DBNull.Value;
    //  else
    //    p.Value = value.Value;
    //  return p;
    //}
    //#endregion

    #region get values

    public static int GetInt(this SqlDataReader dr, string name)
    {
      if (dr[name] == DBNull.Value)
        return 0;
      else
        return (int)dr[name];
    }

    public static string GetString(this SqlDataReader dr, string name)
    {
      if (dr[name] == DBNull.Value)
        return string.Empty;
      else
        return (string)dr[name];
    }

    public static string GetStringNull(this SqlDataReader dr, string name)
    {
      if (dr[name] == DBNull.Value)
        return null;
      else
        return (string)dr[name];
    }


    public static DateTime GetDateTime(this SqlDataReader dr, string name)
    {
      if (dr[name] == DBNull.Value)
        return new DateTime(1990,1,1);
      else
        return ((DateTime)dr[name]);
    }

    public static DateTime? GetDateTimeNull(this SqlDataReader dr, string name)
    {
      if (dr[name] == DBNull.Value)
        return null;
      else
        return ((DateTime)dr[name]);
    }
    #endregion
  }
}
