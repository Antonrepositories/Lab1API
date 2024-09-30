using Microsoft.EntityFrameworkCore;

namespace Lab1API.Models
{
	public class LabContext : DbContext
	{
		public LabContext(DbContextOptions<LabContext> options) : base(options)
		{

		}
		public DbSet<DataBase> DataBases { get; set; }
		public DbSet<TableModel> Tables { get; set; }
		public DbSet<RowModel> Rows { get; set; }
		public DbSet<FieldModel> Fields { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TableModel>().HasMany(m => m.Fields).WithOne(f => f.Table).OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<TableModel>().HasMany(m => m.Rows).WithOne(f => f.Table).OnDelete(DeleteBehavior.Cascade);
			base.OnModelCreating(modelBuilder);
		}
	}
}

