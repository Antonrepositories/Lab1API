#nullable disable
using System.Text.Json.Serialization;

namespace Lab1API.Models
{
	public class TableModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		[JsonIgnore]
		public DataBase Database { get; set; }
		public List<FieldModel> Fields { get; set; } = new List<FieldModel>();
		public List<RowModel> Rows { get; set; } = new List<RowModel>();

	}
}
