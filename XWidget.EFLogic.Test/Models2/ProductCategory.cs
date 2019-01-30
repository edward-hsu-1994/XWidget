using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XWidget.EFLogic.Test.Models2 {
    [RemoveCascade(Mode = RemoveCascadeMode.OptIn)]
    public class ProductCategory {
        [Key]
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        public virtual Guid? ParentId { get; set; }

        public virtual string Name { get; set; }

        [ForeignKey("ParentId")]
        [InverseProperty("Children")]
        public virtual ProductCategory Parent { get; set; }

        [InverseProperty("Parent")]
        public virtual ICollection<ProductCategory> Children { get; set; } = new HashSet<ProductCategory>();

        [RemoveCascadeProperty]
        [InverseProperty("Category")]
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        public bool ShouldRemoveCascadeChildren() => true;
    }
}
