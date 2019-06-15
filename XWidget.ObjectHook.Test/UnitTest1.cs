using System;
using System.Collections.Generic;
using Xunit;

namespace XWidget.ObjectHook.Test {

    public class TestClass {
        public string a;
        public virtual string A {
            get {
                return a;
            }
            set {
                a = value;
            }
        }
        public string b;
        public virtual string this[string A] {
            get { return b; }
            set { b = value; }
        }

    }
    public class UnitTest1 {
        [Fact]
        public void Test1() {

            var obj = new TestClass();

            var injector1 = new ObjectHookInjector<TestClass>();
            var test = 0;
            injector1.HookGetPropertyAfter<string>(x => x.A, (TestClass o, object[] i, ref object v) => {
                v = "654321"; // 複寫結果
                test++;
            });

            injector1.HookSetPropertyBefore<string>(x => x.A, (TestClass o, object[] i, ref object v) => {
                v = "123456"; // 複寫結果
                test--;
            });
            injector1.HookGetPropertyAfter(x => x[default(string)], (TestClass o, object[] i, ref object v) => {
                v = "INDEX SET"; // 複寫結果
                test++;
            });
            injector1.HookSetPropertyBefore(x => x[default(string)], (TestClass o, object[] i, ref object v) => {
                v = "SET INDEX"; // 複寫結果
                test--;
            });


            obj = injector1.Inject(obj);
            obj.A = "value";
            var p = obj.A;
            obj["i1"] = "value";
            var p2 = obj["i1"];

            Assert.Equal(0, test);
        }
    }
}
