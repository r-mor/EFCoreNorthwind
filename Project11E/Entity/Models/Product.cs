using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Project11E.Entity.Models;

internal class Product
{
    [Column("ProductId")]
    public int Id { get; set; }

    [Column("ProductName")]
    [Required]
    [StringLength(40)]
    public string Name { get; set; } = null!;
    public int SupplierId { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
    public int CategoryId { get; set; }

    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Column(TypeName = "money")]
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
}
