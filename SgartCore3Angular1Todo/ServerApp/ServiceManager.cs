using SgartCoreAngular1Todo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SgartCoreAngular1Todo.Extensions;
using SgartCore3Angular1Todo.Models;

namespace SgartCoreAngular1Todo.ServerApp
{
  public class ServiceManager
  {
    private readonly AppSettings _settings;
    private readonly string _connectionString;

    // inject dei parametri di configurazione
    public ServiceManager(Microsoft.Extensions.Options.IOptions<AppSettings> settings)
    {
      _settings = settings.Value;
      _connectionString = _settings.ConnectionString;
    }

    private static class Queries
    {
      public static string Search = "spu_todos_search";
      public static string Read = "SELECT T.[id], [date], [title], [note], [idCategory], [category], [completed], [created], [modified] FROM [todos] T INNER JOIN [categories] C ON T.[idCategory]=C.[id] WHERE T.[id]=@id;";
      public static string Insert = "INSERT INTO [todos] ([date],[title],[note],[idCategory],[created],[modified]) VALUES(@date,@title,@note,@idCategory,GETDATE(),GETDATE());";
      public static string Update = "UPDATE [todos] SET [date]=@date,[title]=@title,[note]=@note,[idCategory]=@idCategory,[completed]=@completed,[modified]=GETDATE() WHERE [id]=@id;";
      public static string Remove = "DELETE FROM [todos] WHERE [id]=@id;";
      public static string Toggle = "UPDATE [todos] SET [completed]=CASE WHEN [completed] is null THEN GETDATE() ELSE null END, [modified]=GETDATE() WHERE [id]=@id;";
      public static string UpdateCategory = "UPDATE [todos] SET [idCategory]=@idCategory,[modified]=GETDATE() WHERE [id]=@id;SELECT [id],[idCategory] FROM [todos] WHERE [id]=@id;";
      public static string Categories = "SELECT [id],[category],[color] FROM [categories] ORDER BY [id];";
      public static string Statistics = "SELECT c.[id],C.[category], C.[color], count(*) AS [count] FROM [todos] T INNER JOIN [dbo].[categories] C ON T.[IDCategory]=C.[ID] GROUP BY c.[ID],c.category, c.color ORDER BY c.[ID];";
    }

    public async Task<ServiceStatusListItem<CategoryItem>> CategoryAllAsync()
    {
      ServiceStatusListItem<CategoryItem> result = new ServiceStatusListItem<CategoryItem>();
      try
      {
        result.Data = new List<CategoryItem>();
        using (var db = new DBCommand(_connectionString, Queries.Categories))
        {
          result.ReturnValue = await db.ExecuteReaderWhileAsync((dr, rowNumber) =>
           {
             result.Data.Add(new CategoryItem
             {
               ID = dr.GetInt("ID"),
               Category = dr.GetString("Category"),
               Color = dr.GetString("Color")
             });
           });
        }
        result.Success = true;
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

    public async Task<ServiceStatusListItem<TodoItem>> TodoSearchAsync(FilterItem filter)
    {
      ServiceStatusListItem<TodoItem> result = new ServiceStatusListItem<TodoItem>();
      try
      {
        int startIndex = (filter.Page - 1) * filter.Size;

        result.Data = new List<TodoItem>();
        using (var db = new DBCommand(_connectionString, Queries.Search, true))
        {
          db.AddParameterInt("@startIndex", startIndex);
          db.AddParameterInt("@pageSize", filter.Size);
          db.AddParameterStringNull("@text", 100, filter.Text);
          db.AddParameterInt("@idCategory", filter.IDCategory);
          db.AddParameterInt("@status", filter.Status);
          db.AddParameterString("@sort", 100, filter.Sort);

          result.ReturnValue = await db.ExecuteReaderWhileAsync((dr, rowNumber) =>
          {
            result.Data.Add(new TodoItem
            {
              ID = dr.GetInt("id"),
              Date = dr.GetDateTime("date"),
              Title = dr.GetString("title"),
              IDCategory = dr.GetInt("idCategory"),
              Category = dr.GetString("category"),
              Color = dr.GetString("color"),
              Completed = dr.GetDateTimeNull("completed"),
              Modified = dr.GetDateTime("modified"),
              Created = dr.GetDateTime("created"),
              TotalItems = dr.GetInt("totalItems")
            });
          });
        }
        result.Success = true;
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

    public async Task<ServiceStatusItem<TodoItem>> TodoReadAsync(int id)
    {
      ServiceStatusItem<TodoItem> result = new ServiceStatusItem<TodoItem>();
      try
      {
        using (var db = new DBCommand(_connectionString, Queries.Read))
        {
          db.AddParameterInt("@id", id);
          result.ReturnValue = await db.ExecuteReaderFirstAsync((dr, rowNumber) =>
          {
            result.Data = new TodoItem
            {
              ID = dr.GetInt("id"),
              Date = dr.GetDateTime("date"),
              Title = dr.GetString("title"),
              IDCategory = dr.GetInt("idCategory"),
              Category = dr.GetString("category"),
              Completed = dr.GetDateTimeNull("completed"),
              Modified = dr.GetDateTime("modified"),
              Created = dr.GetDateTime("created")
            };
          });
        }
        if (result.Data != null)
          result.Success = true;
        else
          result.AddError($"Not found id:{id}");
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

    public async Task<ServiceStatusItem> TodoInsertAsync(TodoItem item)
    {
      ServiceStatusItem result = new ServiceStatusItem();
      try
      {
        if (item.ID != 0)
          result.AddError("Ivalid `id` in INSERT");
        if (item.Date.HasValue == false || item.Date.Value < new DateTime(1970, 1, 1))
          result.AddError("`date` required");
        if (string.IsNullOrWhiteSpace(item.Title))
          result.AddError("`title` required");
        if (item.IDCategory < 0)
          result.AddError("`idCategory` required");
        if (result.Messages.Count > 0)
        {
          return result;
        }

        using (var db = new DBCommand(_connectionString, Queries.Insert))
        {
          db.AddParameterDateTime("@date", item.Date);
          db.AddParameterString("@title", 100, item.Title);
          db.AddParameterNote("@note", item.Note);
          db.AddParameterInt("@idCategory", item.IDCategory);
          await db.ExecuteNonQueryAsync();

          result.AddSuccess("Inserted");
          result.Success = true;
        }
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

    public async Task<ServiceStatusItem> TodoUpdateAsync(TodoItem item)
    {
      ServiceStatusItem result = new ServiceStatusItem();
      try
      {
        if (item.ID == 0)
          result.AddError("Ivalid `id`");
        if (item.Date.HasValue == false || item.Date.Value < new DateTime(1970, 1, 1))
          result.AddError("`date` required");
        if (string.IsNullOrWhiteSpace(item.Title))
          result.AddError("`title` required");
        if (item.IDCategory < 0)
          result.AddError("`idCategory` required");
        if (result.Messages.Count > 0)
        {
          return result;
        }

        using (var db = new DBCommand(_connectionString, Queries.Update))
        {
          db.AddParameterInt("@id", item.ID);
          db.AddParameterDateTime("@date", item.Date);
          db.AddParameterString("@title", 100, item.Title);
          db.AddParameterNote("@note", item.Note);
          db.AddParameterInt("@idCategory", item.IDCategory);
          db.AddParameterDateTime("@completed", item.Completed);

          await db.ExecuteNonQueryAsync();

          result.AddSuccess($"Updated id: {item.ID}");
          result.Success = true;
        }
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

    public async Task<ServiceStatusItem> TodoDeleteAsync(int id)
    {
      ServiceStatusItem result = new ServiceStatusItem();
      try
      {
        if (id == 0)
          result.AddError("Ivalid `id`");
        if (result.Messages.Count > 0)
        {
          return result;
        }

        using (var db = new DBCommand(_connectionString, Queries.Remove))
        {
          db.AddParameterInt("@id", id);

          await db.ExecuteNonQueryAsync();

          result.AddSuccess($"Deleted id: {id}");
          result.Success = true;
        }
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

    public async Task<ServiceStatusItem<TodoItem>> TodoToggleAsync(int id)
    {
      ServiceStatusItem<TodoItem> result = new ServiceStatusItem<TodoItem>();
      try
      {
        //result.Data = new TodoItem();
        using (var db = new DBCommand(_connectionString, Queries.Toggle + Queries.Read))
        {
          db.AddParameterInt("@id", id);

          result.ReturnValue = await db.ExecuteReaderWhileAsync((dr, rowNumber) =>
          {
            result.Data = new TodoItem
            {
              ID = dr.GetInt("id"),
              Date = dr.GetDateTime("date").Date,
              Title = dr.GetString("title"),
              IDCategory = dr.GetInt("idCategory"),
              Category = dr.GetString("category"),
              Completed = dr.GetDateTimeNull("completed"),
              Modified = dr.GetDateTime("modified"),
              Created = dr.GetDateTime("created")
            };
          });
          result.Success = true;
        }
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

    public async Task<ServiceStatusItem<TodoItem>> TodoCategoryAsync(int id, int idCategory)
    {
      ServiceStatusItem<TodoItem> result = new ServiceStatusItem<TodoItem>();
      try
      {
        using (var db = new DBCommand(_connectionString, Queries.UpdateCategory))
        {
          db.AddParameterInt("@id", id);
          db.AddParameterInt("@idCategory", idCategory);

          result.ReturnValue = await db.ExecuteReaderWhileAsync((dr, rowNumber) =>
          {
            result.Data = new TodoItem
            {
              ID = dr.GetInt("id"),
              IDCategory = dr.GetInt("idCategory")
            };
          });
          result.Success = true;
        }
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

    public async Task<ServiceStatusListItem<CategoryItem>> StatisticAllAsync()
    {
      ServiceStatusListItem<CategoryItem> result = new ServiceStatusListItem<CategoryItem>();
      try
      {
        result.Data = new List<CategoryItem>();
        using (var db = new DBCommand(_connectionString, Queries.Statistics))
        {
          result.ReturnValue = await db.ExecuteReaderWhileAsync((dr, rowNumber) =>
          {
            result.Data.Add(new CategoryItem
            {
              ID = dr.GetInt("ID"),
              Category = dr.GetString("Category"),
              Color = dr.GetString("Color"),
              Count=dr.GetInt("Count")
            });
          });
        }
        result.Success = true;
      }
      catch (Exception ex)
      {
        result.AddError(ex);
      }
      return result;
    }

  }
}
