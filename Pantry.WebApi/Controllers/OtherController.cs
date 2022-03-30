using Microsoft.AspNetCore.Mvc;
using Pantry.Core.Models;
using Pantry.Data;

namespace Pantry.WebApi.Controllers;

[ApiController]
[Route("Other")]
public class OtherController : ControllerBase
{
    private readonly ILogger<OtherController> _logger;
    private readonly DataBase _dataBase;

    public OtherController(ILogger<OtherController> logger, DataBase dataBase)
    {
        _logger = logger;
        _dataBase = dataBase;
    }

    [HttpGet(Name = "GetOther")]
    public IEnumerable<Food> Get() => _dataBase.Foods.ToList();
}