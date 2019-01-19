using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XWidget.EFLogic.Test.Models2 {
    [RemoveCascade(Mode = RemoveCascadeMode.OptIn)]
    public class Order {
        [Key]
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        public virtual string UserAccount { get; set; }

        public virtual DateTime Time { get; set; }

        [ForeignKey("UserAccount")]
        [InverseProperty("Orders")]
        public virtual User User { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
    }
}
