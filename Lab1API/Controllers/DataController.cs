using Lab1API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DataController : Controller
	{
		private readonly LabContext _context;

		public DataController(LabContext context)
		{
			_context = context;
		}
		[HttpGet]
		public async Task<ActionResult<IEnumerable<DataBase>>> GetBases()
		{
			return await _context.DataBases.Include(d => d.Tables).ToListAsync();
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<DataBase>> GetBase(int id)
		{
			var dataBase = await _context.DataBases.FirstOrDefaultAsync(t => t.Id == id);
			if (dataBase == null)
			{
				return NotFound();
			}
			return dataBase;
		}
		[HttpPost]
		public async Task<ActionResult<DataBase>> CreateBase(DataBase dataBase)
		{
			_context.DataBases.Add(dataBase);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetBase), new { id = dataBase.Id }, dataBase);
		}
	}
}
