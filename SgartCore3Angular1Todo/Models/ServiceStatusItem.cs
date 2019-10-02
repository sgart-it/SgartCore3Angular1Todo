using Newtonsoft.Json;  // [JsonProperty("t")] 
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text.Json.Serialization; //[JsonPropertyName("m")] 
using System.Threading.Tasks;

namespace SgartCoreAngular1Todo.Models
{
  public class ServiceStatusItem
  {
    public ServiceStatusItem()
    {
      Messages = new List<ServiceStatusMessageItem>();
    }

    public bool Success { get; set; }
    public int ReturnValue { get; set; }
    public List<ServiceStatusMessageItem> Messages { get; set; }

    public void AddError(string message)
    {
      Messages.Add(new ServiceStatusMessageItem("E", message, 30));
    }
    public void AddError(Exception ex)
    {
      Messages.Add(new ServiceStatusMessageItem("E", ex.Message, 30));
    }
    public void AddWarning(string message)
    {
      Messages.Add(new ServiceStatusMessageItem("W", message, 20));
    }
    public void AddSuccess(string message)
    {
      Messages.Add(new ServiceStatusMessageItem("S", message, 2));
    }
    public void AddInfo(string message)
    {
      Messages.Add(new ServiceStatusMessageItem("I", message, 30));
    }
  }

  public class ServiceStatusItem<T> : ServiceStatusItem
  {
    public T Data { get; set; }
  }
  public class ServiceStatusListItem<T> : ServiceStatusItem
  {
    public List<T> Data { get; set; }
  }

  public class ServiceStatusMessageItem
  {
    public ServiceStatusMessageItem(string type, string message, int seconds)
    {
      Type = type;
      Message = message;
      Seconds = seconds;
    }
    /// <summary>
    /// valid: I=info, S=success, W=Warning, E=Error
    /// </summary>
    [JsonProperty("t")]
    public string Type { get; set; }
    [JsonProperty("m")]
    public string Message { get; set; }
    [JsonProperty("s")]
    public int Seconds { get; set; }
  }
}
