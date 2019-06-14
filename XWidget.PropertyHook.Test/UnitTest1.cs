using System;
using System.Collections.Generic;
using Xunit;

namespace XWidget.PropertyHook.Test {

    public class TestClass {
        public virtual string A { get; set; }
        string b;
        public virtual string this[string A] {
            get { return b; }
            set { b = value; }
        }

    }
    public class UnitTest1 {
        [Fact]
        public void Test1() {

            var obj = new TestClass();

            var injector1 = new PropertyHookInjector<TestClass>(obj);
            injector1.HookGetAfterProperty<string>(x => x.A, (TestClass o, object[] i, ref object v) => {
                v = "654321"; // 複寫結果
            });
            injector1.HookSetBeforeProperty<string>(x => x.A, (TestClass o, object[] i, ref object v) => {
                v = "123456"; // 複寫結果
            });
            injector1.HookGetAfterProperty(x => x[default(string)], (TestClass o, object[] i, ref object v) => {
                v = "INDEX SET"; // 複寫結果
            });
            injector1.HookSetBeforeProperty(x => x[default(string)], (TestClass o, object[] i, ref object v) => {
                v = "SET INDEX"; // 複寫結果
            });


            obj = injector1.Inject();
            obj.A = "value";
            var p = obj.A;
            obj["i1"] = "value";
            var p2 = obj["i1"];

        }
    }
}
