using XWidget.Reflection.Test.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace XWidget.Reflection.Test {
    public class MemberInfoExtensionTest {
        [Fact(DisplayName = "MemberInfoExtension.GetMember<T>")]
        public void GetMember() {
            Assert.NotNull(new TestClass().GetMember(x => x.VoidMethod()));
        }

        [Fact(DisplayName = "MemberInfoExtension.GetMember")]
        public void GetMember_T() {
            Assert.NotNull(typeof(TestClass).GetMember(() => new TestClass()));
        }

        [Fact(DisplayName = "MemberInfoExtension.FullTest")]
        public void FullGetMemberTest() {
            //get `Count` property's MemberInfo
            typeof(List<int>)
                .GetMember<List<int>>(x => x.Count);

            //get List<int> constructor
            typeof(List<int>)
                .GetMember(x => new List<int>());

            //get Clear method's MemberInfo(not return value method)
            typeof(List<int>)
                .GetMember<List<int>>(x => x.Clear());

            //get Sum method's MemberInfo(has return value method)
            typeof(List<int>)
                .GetMember<List<int>>(x => x.Sum());

            //other way
            List<int> test = new List<int>();
            test.GetMember(x => x.Count);
            test.GetMember(x => new List<int>());
            test.GetMember<List<int>>(x => x.Clear());
            test.GetMember<List<int>>(x => x.Sum());
        }
    }
}
