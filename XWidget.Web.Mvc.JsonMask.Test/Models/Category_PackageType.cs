using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Mvc.JsonMask.Test.Models {
    /// <summary>
    /// 分類
    /// </summary>
    public class Category_PackageType {
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
        [JsonPropertyMask(key: typeof(IEnumerable<Category_PackageType>), Method = MaskMethod.PackageType)]
        public ICollection<Category_PackageType> Children { get; set; }

        /// <summary>
        /// 取得分類樹狀結構
        /// </summary>
        /// <returns>分類集合</returns>
        public static IEnumerable<Category_PackageType> GetCategoryTree() {
            return new Category_PackageType[] {
                new Category_PackageType() {
                    Name = "CategoryRoot",
                    Children = new Category_PackageType[] {
                        new Category_PackageType() {
                            Name = "Level1-1",
                            Children = new Category_PackageType[] {
                                new Category_PackageType() {
                                    Name = "Level2-1"
                                },
                                new Category_PackageType() {
                                    Name = "Level2-2"
                                }
                            }
                        },
                        new Category_PackageType() {
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
        public static IEnumerable<Category_PackageType> GetCategories() {
            var tree = GetCategoryTree();

            List<Category_PackageType> result = new List<Category_PackageType>();

            void AddList(IEnumerable<Category_PackageType> categories) {
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
