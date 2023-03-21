using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Project11E.Entity.Models;

internal class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }


    public virtual ICollection<OrderDetail> OrderDetails { get;set; }

    public Order()
    {
        OrderDetails = new HashSet<OrderDetail>();
    }
}
