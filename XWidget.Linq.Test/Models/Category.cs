using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Linq.Test.Models {
    public class Category {
        public string Name { get; set; }
        public IEnumerable<Category> Children { get; set; }

        public static IEnumerable<Category> GetTestSample() {
            return new Category[] {
                new Category() {
                    Name = "1",
                    Children = new Category[]{
                        new Category() {
                            Name = "2"
                        },
                        new Category() {
                            Name = "3"
                        }
                    }
                },
                new Category() {
                    Name = "4",
                    Children = new Category[]{
                        new Category() {
                            Name = "5"
                        },
                        new Category() {
                            Name = "6",
                            Children = new Category[] {
                                new Category() {
                                    Name = "7"
                                }
                            }
                        }
                    }
                },
                new Category() {
                    Name = "8"
                }
            };
        }
    }
}
