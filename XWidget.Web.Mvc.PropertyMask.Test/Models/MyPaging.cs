using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using XWidget.Linq;

namespace XWidget.Web.Mvc.PropertyMask.Test.Models {
    public class MyPaging<TSource> : Paging<TSource>
        where TSource : class {
        public MyPaging(IEnumerable<TSource> source, int skip, int take) : base(source, skip, take) {

        }

        public override IEnumerable<TSource> Result => base.Result.ToArray().Select(x => Masker.Mask(x, null));
    }

}
