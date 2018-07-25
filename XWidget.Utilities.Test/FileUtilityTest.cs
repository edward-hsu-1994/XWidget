using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Utilities.Test {
    public class FileUtilityTest {
        [Fact(DisplayName = "FileUtility.SecureDelete")]
        public void SecureDelete() {
            var path = Guid.NewGuid().ToString() + ".txt";
            using (var stream = System.IO.File.CreateText(path)) {
                stream.WriteLine("Test Text");
            }
            FileUtility.SecureDelete(path);
            Assert.False(System.IO.File.Exists(path));
        }
    }
}
