using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Web.Mvc.PropertyMask.Test.Models {
    public class User {
        public virtual string Id { get; set; }

        [PropertyMask]
        public virtual string Password { get; set; }

        public static IEnumerable<User> GetList() {
            return Enumerable.Range(0, 100).Select(x => new User() {
                Id = x.ToString(),
                Password = "1234"
            });
        }
    }
}
