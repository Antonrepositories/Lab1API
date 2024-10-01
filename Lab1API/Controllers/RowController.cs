using Lab1API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Lab1API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RowController : Controller
	{
		private readonly LabContext _context;

		public RowController(LabContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> CreateRow(int tableId, [FromBody] RowModel rowModel)
		{
			var table = await _context.Tables.Include(t => t.Fields).FirstOrDefaultAsync(t => t.Id == tableId);
			if (table == null)
				return NotFound("Table not found.");

			var fieldValues = rowModel.RowData.Split(';');
			if (fieldValues.Length != table.Fields.Count)
				return BadRequest("Invalid number of values.");

			// Валідація кожного значення відповідно до типу поля
			for (var i = 0; i < table.Fields.Count; i++)
			{
				var field = table.Fields[i];
				var fieldValue = fieldValues[i];

				switch (field.DataType)
				{
					case "integer":
						if (!int.TryParse(fieldValue, out _))
							return BadRequest($"Field '{field.Name}' must be an integer.");
						break;

					case "real":
						if (!double.TryParse(fieldValue, out _))
							return BadRequest($"Field '{field.Name}' must be a real number.");
						break;

					case "char":
						if (fieldValue.Length != 1)
							return BadRequest($"Field '{field.Name}' must be a single character.");
						break;

					case "string":
						// String може бути будь-якої довжини, тому тут немає додаткових перевірок
						break;

					case "date":
						if (!DateTime.TryParseExact(fieldValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
							return BadRequest($"Field '{field.Name}' must be a valid date in the format YYYY-MM-DD.");
						break;

					case "datelnvl":
						var dates = fieldValue.Split(" - ");
						if (dates.Length != 2)
							return BadRequest($"Field '{field.Name}' must contain two dates separated by '-'.");

						if (!DateTime.TryParseExact(dates[0].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate) ||
							!DateTime.TryParseExact(dates[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
						{
							return BadRequest($"Field '{field.Name}' must contain valid dates in the format YYYY-MM-DD.");
						}

						if (startDate > endDate)
							return BadRequest($"Field '{field.Name}' must have the first date earlier than the second date.");
						break;

					default:
						return BadRequest($"Unknown data type for field '{field.Name}'.");
				}
			}

			var newRow = new RowModel
			{
				Table = table,
				RowData = rowModel.RowData
			};

			_context.Rows.Add(newRow);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetRowById), new { id = newRow.Id }, newRow);
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<RowModel>> GetRowById(int id)
		{
			var row = await _context.Rows.FirstOrDefaultAsync(r => r.Id == id);
			if (row == null) return NotFound();
			return row;
		}
		[HttpGet]
		public IActionResult GetRowsByPattern(int tableId, string pattern)
		{
			var table = _context.Tables
								.Include(t => t.Rows)  
								.FirstOrDefault(t => t.Id == tableId);

			if (table == null)
			{
				return NotFound(new { message = "Table not found" });
			}

			var rows = table.Rows.Where(r => r.RowData.Contains(pattern));

			return Ok(rows);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteRow(int id)
		{
			var row = await _context.Rows.FindAsync(id);
			if (row == null) return NotFound();

			_context.Rows.Remove(row);
			await _context.SaveChangesAsync();
			return NoContent();
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateRow(int id, [FromBody] RowModel rowModel)
		{
			var existingRow = await _context.Rows.Include(r => r.Table).ThenInclude(t => t.Fields).FirstOrDefaultAsync(r => r.Id == id);
			if (existingRow == null)
				return NotFound("Row not found.");

			var table = existingRow.Table;

			// Валідація даних
			var fieldValues = rowModel.RowData.Split(';');
			if (fieldValues.Length != table.Fields.Count)
				return BadRequest("Invalid number of values.");

			for (var i = 0; i < table.Fields.Count; i++)
			{
				var field = table.Fields[i];
				var fieldValue = fieldValues[i];

				switch (field.DataType)
				{
					case "integer":
						if (!int.TryParse(fieldValue, out _))
							return BadRequest($"Field '{field.Name}' must be an integer.");
						break;

					case "real":
						if (!double.TryParse(fieldValue, out _))
							return BadRequest($"Field '{field.Name}' must be a real number.");
						break;

					case "char":
						if (fieldValue.Length != 1)
							return BadRequest($"Field '{field.Name}' must be a single character.");
						break;

					case "string":
						// String може бути будь-якої довжини, тому тут немає додаткових перевірок
						break;

					case "date":
						if (!DateTime.TryParseExact(fieldValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
							return BadRequest($"Field '{field.Name}' must be a valid date in the format YYYY-MM-DD.");
						break;

					case "datelnvl":
						var dates = fieldValue.Split(" - ");
						if (dates.Length != 2)
							return BadRequest($"Field '{field.Name}' must contain two dates separated by '-'.");

						if (!DateTime.TryParseExact(dates[0].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate) ||
							!DateTime.TryParseExact(dates[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
						{
							return BadRequest($"Field '{field.Name}' must contain valid dates in the format YYYY-MM-DD.");
						}

						if (startDate > endDate)
							return BadRequest($"Field '{field.Name}' must have the first date earlier than the second date.");
						break;

					default:
						return BadRequest($"Unknown data type for field '{field.Name}'.");
				}
			}

			// Оновлюємо дані рядка
			existingRow.RowData = rowModel.RowData;
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
