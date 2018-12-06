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
        [ForeignKey("Id")]
        [InverseProperty("UserData")]
        public virtual Note Note { get; set; }
    }
}
