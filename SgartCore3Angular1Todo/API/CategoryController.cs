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
  public class CategoryController: ControllerBase
  {
    private readonly ServiceManager _manager;

    // inject del manager
    public CategoryController(ServiceManager manager)
    {
      _manager = manager;
    }

    [HttpGet]
    [Route("api/categories")]
    public async Task<ServiceStatusListItem<CategoryItem>> Get()
    {
      var result = await _manager.CategoryAllAsync();
      return result;
    }
  }
}
