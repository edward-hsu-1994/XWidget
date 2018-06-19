using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Mvc.JsonMask.Test.Models {
    public class TestContext : DbContext {
        public DbSet<Category_EF> Categories { get; set; }
        public TestContext() { }

        public TestContext(DbContextOptions<TestContext> options)
            : base(options) { }
    }
}
