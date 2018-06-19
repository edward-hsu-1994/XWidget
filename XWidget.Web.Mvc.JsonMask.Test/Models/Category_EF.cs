using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Mvc.JsonMask.Test.Models {
    public class Category_EF {
        /// <summary>
        /// 唯一識別號
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 父分類唯一識別號
        /// </summary>
        [JsonIgnore]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父節點
        /// </summary>
        [JsonPropertyMask(key: "Mask", Method = MaskMethod.PatternName)]
        public Category_EF Parent { get; set; }

        /// <summary>
        /// 子分類
        /// </summary>
        [JsonPropertyMask(key: "Mask", Method = MaskMethod.PatternName)]
        public ICollection<Category_EF> Children { get; set; } = new List<Category_EF>();
    }
}
