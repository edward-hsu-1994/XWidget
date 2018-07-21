using XWidget.Reflection.Test.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace XWidget.Reflection {
    public class MethodInfoExtensionTest {
        [Fact(DisplayName = "MethodInfoExtension.InvokeGeneric")]
        public void InvokeGeneric() {
            Assert.Equal("aabb", typeof(TestClass).GetMethod("Add2")
                .InvokeGeneric(
                    new Type[] { typeof(string), typeof(string) },
                    new object[] { "aa", "bb" }));
        }

        [Fact(DisplayName = "MethodInfoExtension.InvokeGeneric(instance)")]
        public void InvokeGenericInstance() {
            Assert.Equal("aabb", typeof(TestClass).GetMethod("Add3")
                .InvokeGeneric(
                    new Type[] { typeof(string), typeof(string) },
                    new TestClass(),
                    new object[] { "aa", "bb" }));
        }

        [Fact(DisplayName = "MethodInfoExtension.Invoke")]
        public void Invoke() {
            Assert.Equal(3, typeof(TestClass).GetMethod("Add")
                .Invoke(new object[] { 1, 2 }));
        }

        [Fact(DisplayName = "MethodInfoExtension.Invoke(generic)")]
        public void InvokeGeneric2() {
            Assert.Equal("aabb", typeof(TestClass).GetMethod("Add2")
                .Invoke(
                    new Type[] { typeof(string), typeof(string) },
                    "aa", "bb"));
        }

        [Fact(DisplayName = "MethodInfoExtension.AsInvoke")]
        public void AsInvoke() {
            Assert.True(typeof(TestClass).GetMethod("Add")
                .AsInvoke(null, out object _, 1, 2));
        }

        [Fact(DisplayName = "MethodInfoExtension.ToDelegate")]
        public void ToDelegate() {
            Assert.Equal(3, typeof(TestClass).GetMethod("Add")
                .ToDelegate()(new object[] { 1, 2 }));
        }
    }
}
