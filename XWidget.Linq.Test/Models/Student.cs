using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Linq.Test.Models {
    public class Student {
        private string TestPrivateField = "abc";
        internal string TestInternalField = "def";

        public int Id { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }

        public static string TestMethod() {
            return "Test";
        }

        public override string ToString() {
            return Id + "," + Name + "," + Class;
        }

        public static List<Student> InitList() {
            List<Student> result = new List<Student>();
            result.Add(new Student() {
                Id = 6,
                Class = "B",
                Name = "User06"
            });
            result.Add(new Student() {
                Id = 0,
                Class = "A",
                Name = "User01"
            });
            result.Add(new Student() {
                Id = 4,
                Class = "B",
                Name = "User04"
            });
            result.Add(new Student() {
                Id = 3,
                Class = "A",
                Name = "User03"
            });
            result.Add(new Student() {
                Id = 1,
                Class = "A",
                Name = "User02"
            });
            result.Add(new Student() {
                Id = 5,
                Class = "B",
                Name = "User05"
            });

            return result;
        }
    }
}
