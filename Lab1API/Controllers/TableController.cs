using Lab1API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TableController : Controller
	{
		private readonly LabContext _context;

		public TableController(LabContext context)
		{
			_context = context;
		}
		// Get all tables
		//[HttpPost("{databaseId}")]
		//public async Task<ActionResult<TableModel>> CreateTable(int databaseId, [FromBody] TableModel tableModel)
		//{
		//	var database = await _context.DataBases.FindAsync(databaseId);
		//	if (database == null) return NotFound("Database not found.");

		//	tableModel.Database = database;
		//	foreach (var field in tableModel.Fields)
		//	{
		//		field.Table = tableModel;
		//	}

		//	_context.Tables.Add(tableModel);
		//	await _context.SaveChangesAsync();

		//	return CreatedAtAction(nameof(GetTableById), new { id = tableModel.Id }, tableModel);
		//}
		[HttpPost("{databaseId}")]
		public IActionResult CreateTable(int databaseId, [FromBody] TableModel model)
		{
			var database = _context.DataBases.FirstOrDefault(db => db.Id == databaseId);
			if (database == null)
			{
				return BadRequest("Database not found.");
			}

			model.Database = database;

			foreach (var field in model.Fields)
			{
				field.Table = model; 
				_context.Fields.Add(field); 
			}

			_context.Tables.Add(model);
			_context.SaveChanges(); 

			return Ok(model);
		}




		[HttpGet("{id}")]
		public async Task<ActionResult<TableModel>> GetTableById(int id)
		{
			var table = await _context.Tables.Include(t => t.Fields).FirstOrDefaultAsync(t => t.Id == id);
			if (table == null) return NotFound();
			return table;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTable(int id)
		{
			var table = await _context.Tables.FindAsync(id);
			if (table == null) return NotFound();

			_context.Tables.Remove(table);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}
