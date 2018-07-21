using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Reflection.Test.Models {
    public class TestClass {
        private string Value = "123456";
        public string Value2 = "000";
        public string Value3 => "11";

        public static int Add(int a, int b) {
            return a + b;
        }

        public static string Add2<T1, T2>(T1 a, T2 b) {
            return a.ToString() + b.ToString();
        }

        public string Add3<T1, T2>(T1 a, T2 b) {
            return a.ToString() + b.ToString();
        }

        public int Add4(int a, int b) {
            return a + b;
        }

        public void VoidMethod() { }
    }
}
