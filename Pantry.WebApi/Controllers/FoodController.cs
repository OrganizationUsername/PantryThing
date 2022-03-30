using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //If this is here, something is wrong.
using Pantry.Core.Models;
using Pantry.Data;
using Pantry.ServiceGateways.WebApi.ServiceGateways;
using Serilog.Core;

namespace Pantry.WebApi.Controllers;

[ApiController]
[Route("Food")]
public class FoodController : ControllerBase
{
    private readonly Logger _logger;
    private readonly DataBase _context;

    public FoodController(Logger logger, DataBase context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("{id}")] public async Task<ActionResult<Food>> GetFood(int id) => await _context.Foods.FirstOrDefaultAsync(c => c.FoodId == id);

    [Route("GetAll")]
    [HttpGet] public async Task<ActionResult<IEnumerable<Food>>> GetAllFoods() => await _context.Foods.ToListAsync();
}

[ApiController]
[Route("Admin")]
public class AdminController : ControllerBase
{
    private readonly Logger _logger;
    private readonly DataBase _context;

    public AdminController(Logger logger, DataBase context)
    {
        _logger = logger;
        _context = context;
    }
    [Route("SeedFood")]
    [HttpGet] public void SeedFoods() => Seeder.SeedDatabase(_context);

    [Route("DontUseThis")]
    [HttpGet]
    public void DoSomething()
    {
        var currentTime = DateTime.UtcNow.Ticks;
        Encoding.ASCII.GetBytes("");
        var result = HMACSHA256.HashData(Encoding.ASCII.GetBytes(""), Encoding.ASCII.GetBytes(""));
    }

}