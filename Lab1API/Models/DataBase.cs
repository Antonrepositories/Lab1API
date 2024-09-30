#nullable disable
namespace Lab1API.Models
{
	public class DataBase
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<TableModel> Tables { get; set; } = new List<TableModel>();
	}
}
