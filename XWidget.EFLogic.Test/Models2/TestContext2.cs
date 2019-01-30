using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.EFLogic.Test.Models2 {
    public class TestContext2 : DbContext {
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseInMemoryDatabase("TestDatabase" + Guid.NewGuid());
                optionsBuilder.ConfigureWarnings(warnnings => warnnings.Ignore(CoreEventId.DetachedLazyLoadingWarning));
            }
        }

        public static TestContext2 CreateInstance() {
            var result = new TestContext2();

            #region CreateUser
            for (int i = 1; i <= 100; i++) {
                result.Add(new User() {
                    Account = $"User_{i}",
                    Name = $"使用者_{i}"
                });
            }
            result.SaveChanges();
            #endregion

            #region CreateProductCategory
            for (int i = 1; i <= 2; i++) {
                var pCategory = new ProductCategory() {
                    Name = $"Category_{i}"
                };
                result.Add(pCategory);
                for (int j = 1; j <= 2; j++) {
                    var pCategory_2 = new ProductCategory() {
                        Name = $"Category_{i}_{j}"
                    };
                    pCategory.Children.Add(pCategory_2);
                    for (int k = 1; k <= 2; k++) {
                        pCategory_2.Children.Add(new ProductCategory() {
                            Name = $"Category_{i}_{j}_{k}"
                        });
                    }
                }

            }
            result.SaveChanges();
            #endregion

            #region CreateProduct
            int productCounter = 1;
            foreach (var category in result.ProductCategory.ToList()) {
                for (int i = 1; i <= 3; i++) {
                    category.Products.Add(new Product() {
                        Name = $"Product_{productCounter++}",
                        Price = i
                    });
                }
            }
            result.SaveChanges();
            #endregion

            #region CreateOrderAndItem
            foreach (var user in result.User.ToList()) {
                for (int i = 1; i <= 3; i++) {
                    var order = new Order() {
                        Time = DateTime.Now
                    };
                    user.Orders.Add(order);
                    foreach (var product in result.Product.ToList()) {
                        order.Items.Add(new OrderItem() {
                            Product = product,
                            Count = i
                        });
                    }

                }
            }
            result.SaveChanges();
            #endregion

            return result;
        }
    }
}
