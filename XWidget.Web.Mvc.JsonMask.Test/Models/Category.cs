using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Mvc.JsonMask.Test.Models {
    /// <summary>
    /// 分類
    /// </summary>
    public class Category {
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
        [JsonPropertyMask(key: "MaskName", Method = MaskMethod.PatternName)]
        public string Name { get; set; }

        /// <summary>
        /// 子分類
        /// </summary>        
        [JsonPropertyMask(key: typeof(ICollection<Category>), Method = MaskMethod.DeclaringType)]
        public ICollection<Category> Children { get; set; }

        /// <summary>
        /// 取得分類樹狀結構
        /// </summary>
        /// <returns>分類集合</returns>
        public static IEnumerable<Category> GetCategoryTree() {
            return new Category[] {
                new Category() {
                    Name = "CategoryRoot",
                    Children = new Category[] {
                        new Category() {
                            Name = "Level1-1",
                            Children = new Category[] {
                                new Category() {
                                    Name = "Level2-1"
                                },
                                new Category() {
                                    Name = "Level2-2"
                                }
                            }
                        },
                        new Category() {
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
        public static IEnumerable<Category> GetCategories() {
            var tree = GetCategoryTree();

            List<Category> result = new List<Category>();

            void AddList(IEnumerable<Category> categories) {
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
