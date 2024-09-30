#nullable disable
using System.Text.Json.Serialization;

namespace Lab1API.Models
{
	public class RowModel
	{
		public int Id { get; set; }
		[JsonIgnore]
		public TableModel Table { get; set; }
		public string RowData { get; set; }
	}
}
