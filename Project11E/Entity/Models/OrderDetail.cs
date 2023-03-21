using System.ComponentModel.DataAnnotations.Schema;

namespace Project11E.Entity.Models;

internal class OrderDetail
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public short Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}

