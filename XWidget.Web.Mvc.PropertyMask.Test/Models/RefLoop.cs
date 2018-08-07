using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Mvc.PropertyMask.Test.Models {
    public class RefLoop {
        public virtual RefLoop Loop { get; set; }
    }
}
