using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XWidget.EFLogic.Test.Models {
    public class UserData {
        [Key]
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        [RemoveCascadeStopper]
        [InverseProperty("UserData")]
        public virtual ICollection<Note> Note { get; set; }
    }
}
