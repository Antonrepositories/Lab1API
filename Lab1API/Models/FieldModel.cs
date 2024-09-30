#nullable disable
using System.Text.Json.Serialization;

namespace Lab1API.Models
{
	public class FieldModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string DataType { get; set; }
		[JsonIgnore]
		public TableModel Table { get; set; }
	}
}
