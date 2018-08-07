using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Mvc.PropertyMask.Test.Models {
    /// <summary>
    /// 分類
    /// </summary>
    public class Category_PatternName {
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
        /// 子分類
        /// </summary>
        [PropertyMask(key: "MaskName", Method = MaskMethod.PatternName)]
        public virtual ICollection<Category_PatternName> Children { get; set; }

        /// <summary>
        /// 取得分類樹狀結構
        /// </summary>
        /// <returns>分類集合</returns>
        public static IEnumerable<Category_PatternName> GetCategoryTree() {
            return new Category_PatternName[] {
                new Category_PatternName() {
                    Name = "CategoryRoot",
                    Children = new Category_PatternName[] {
                        new Category_PatternName() {
                            Name = "Level1-1",
                            Children = new Category_PatternName[] {
                                new Category_PatternName() {
                                    Name = "Level2-1"
                                },
                                new Category_PatternName() {
                                    Name = "Level2-2"
                                }
                            }
                        },
                        new Category_PatternName() {
                            Name = "Level1-2"
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 取得所有分類集合
        /// </summary>
        /// <returns>分類集合</returns>
        public static IEnumerable<Category_PatternName> GetCategories() {
            var tree = GetCategoryTree();

            List<Category_PatternName> result = new List<Category_PatternName>();

            void AddList(IEnumerable<Category_PatternName> categories) {
                foreach (var category in categories) {
                    result.Add(category);
                    if (category.Children != null) {
                        AddList(category.Children);
                    }
                }
            }

            AddList(tree);

            return result;
        }
    }
}
