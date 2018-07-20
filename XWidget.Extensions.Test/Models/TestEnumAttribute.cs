using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Extensions.Test.Models {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TestEnumAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class TestEnum2Attribute : Attribute { }
}
