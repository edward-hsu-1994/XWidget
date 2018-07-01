using Microsoft.AspNetCore.Mvc;
using System;

namespace XWidget.Web.Mvc.Multipart {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class FromJsonAttribute : FromFormAttribute {

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class FromJson2Attribute : Attribute {

    }
}
