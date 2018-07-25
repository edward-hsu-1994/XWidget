using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EFLogic.Test.Models {
    public class Category {
        public Category() {
            Id = Guid.NewGuid();
            Children = new HashSet<Category>();
            Notes = new HashSet<Note>();
        }
        public virtual Guid Id { get; set; }
        public virtual Guid? ParentId { get; set; }
        public virtual string Name { get; set; }
        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
    }
}
