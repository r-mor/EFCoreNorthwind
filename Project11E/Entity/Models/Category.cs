using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project11E.Entity.Models;

internal class Category
{
    [Column("CategoryId")]
    public int Id { get; set; }

    [Required]
    [Column("CategoryName")]
    [StringLength(15)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; }

    public Category()
    {
        Products = new HashSet<Product>();
    }
}
