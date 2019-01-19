using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XWidget.EFLogic.Test.Models2 {
    [RemoveCascade(Mode = RemoveCascadeMode.OptIn)]
    public class OrderItem {
        [Key]
        public virtual Guid Id { get; set; } = Guid.NewGuid();


        public virtual Guid OrderId { get; set; }

        public virtual Guid ProductId { get; set; }

        public virtual int Count { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("Items")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("OrderItems")]
        public virtual Product Product { get; set; }
    }
}
