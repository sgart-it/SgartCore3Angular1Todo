using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SgartCoreAngular1Todo.Models;
using SgartCoreAngular1Todo.ServerApp;

namespace SgartCoreAngular1Todo.API
{
  [ApiController]
  [Route("api/todo")]
  public class TodoController : ControllerBase
  {
    private readonly ServiceManager _manager;

    // inject del manager
    public TodoController(ServiceManager manager)
    {
      _manager = manager;
    }

    [HttpGet]
    [Route("version")]
    public string Version()
    {
      return Constants.VERSION;
    }

    [Route("search")]
    public async Task<ServiceStatusListItem<TodoItem>> TodoSearch([FromQuery] FilterItem filter)
    {
      return await _manager.TodoSearchAsync(filter);
    }

    [Route("{id}")]
    public async Task<ServiceStatusItem<TodoItem>> TodoReadAsync(int id)
    {
      return await _manager.TodoReadAsync(id);
    }

    [HttpPost]
    [Route("insert")]
    public async Task<ServiceStatusItem> TodoInsertAsync(TodoItem item)
    {
      return await _manager.TodoInsertAsync(item);
    }

    [HttpPost]
    [Route("update")]
    public async Task<ServiceStatusItem> TodoUpdateAsync(TodoItem item)
    {
      return await _manager.TodoUpdateAsync(item);
    }

    [HttpPost]
    [Route("delete")]
    public async Task<ServiceStatusItem> TodoDeleteAsync(FilterItem filter)
    {
      return await _manager.TodoDeleteAsync(filter.ID);
    }

    [HttpPost]
    [Route("toggle")]
    public async Task<ServiceStatusItem<TodoItem>> TodoToggle(FilterItem filter)
    {
      return await _manager.TodoToggleAsync(filter.ID);
    }

    [HttpPost]
    [Route("category")]
    public async Task<ServiceStatusItem> TodoCategoryAsync(FilterItem filter)
    {
      return await _manager.TodoCategoryAsync(filter.ID, filter.IDCategory);
    }
   
  }
}
