using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XWidget.EFLogic.Test.Models2 {
    [RemoveCascade(Mode = RemoveCascadeMode.OptIn)]
    public class User {
        [Key]
        public virtual string Account { get; set; }
        public virtual string Name { get; set; }

        [RemoveCascadeProperty]
        [InverseProperty("User")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
