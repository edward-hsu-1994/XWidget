using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XWidget.Web.Mvc.PropertyMask.Test.Controllers;

namespace XWidget.Web.Mvc.PropertyMask.Test.Models {
    /// <summary>
    /// 分類
    /// </summary>
    public class Category_ActionReturnType {
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
        [PropertyMask(key: typeof(IEnumerable<Category_ActionReturnType>), Method = MaskMethod.ReturnType)]
        public virtual ICollection<Category_ActionReturnType> Children { get; set; }

        /// <summary>
        /// 取得分類樹狀結構
        /// </summary>
        /// <returns>分類集合</returns>
        public static IEnumerable<Category_ActionReturnType> GetCategoryTree() {
            return new Category_ActionReturnType[] {
                new Category_ActionReturnType() {
                    Name = "CategoryRoot",
                    Children = new Category_ActionReturnType[] {
                        new Category_ActionReturnType() {
                            Name = "Level1-1",
                            Children = new Category_ActionReturnType[] {
                                new Category_ActionReturnType() {
                                    Name = "Level2-1"
                                },
                                new Category_ActionReturnType() {
                                    Name = "Level2-2"
                                }
                            }
                        },
                        new Category_ActionReturnType() {
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
        public static IEnumerable<Category_ActionReturnType> GetCategories() {
            var tree = GetCategoryTree();

            List<Category_ActionReturnType> result = new List<Category_ActionReturnType>();

            void AddList(IEnumerable<Category_ActionReturnType> categories) {
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
