using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SgartCoreAngular1Todo.Models;
using SgartCoreAngular1Todo.ServerApp;
namespace SgartCore3Angular1Todo.API
{
  [ApiController]
  public class StatisticsController : ControllerBase
  {
    private readonly ServiceManager _manager;

    // inject del manager
    public StatisticsController(ServiceManager manager)
    {
      _manager = manager;
    }

    [HttpGet]
    [Route("api/statistics")]
    public async Task<ServiceStatusListItem<CategoryItem>> Get()
    {
      var result = await _manager.StatisticAllAsync();
      return result;
    }
  }
}