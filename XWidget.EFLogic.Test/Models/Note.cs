using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XWidget.EFLogic.Test.Models {
    public class Note {
        public Note() {
        }
        [Key]
        public virtual Guid Id { get; set; }

        public virtual Guid CategoryId { get; set; }
        public virtual string Title { get; set; }
        public virtual string Context { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Notes")]
        public virtual Category Category { get; set; }
    }
}
