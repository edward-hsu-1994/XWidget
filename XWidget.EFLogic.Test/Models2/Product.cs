using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XWidget.EFLogic.Test.Models2 {
    [RemoveCascade(Mode = RemoveCascadeMode.OptIn)]
    public class Product {
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        public virtual Guid CategoryId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int Price { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Products")]
        public virtual ProductCategory Category { get; set; }

        [RemoveCascadeProperty]
        [InverseProperty("Product")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}
