using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EF.Extensions.Test.Models {
    public class Note {
        public Note() {
            Id = Guid.NewGuid();
        }
        public virtual Guid Id { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual string Title { get; set; }
        public virtual string Context { get; set; }
        public virtual Category Category { get; set; }
    }
}
