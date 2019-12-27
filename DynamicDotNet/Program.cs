using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace DynamicDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            // Dynamic Type
            dynamic name = "Joey";
            Console.WriteLine($"Hello {name}");

            // Convert Type
            name = 1;
            Console.WriteLine($"Hello {name}");

            // Expand Object Type
            dynamic student = new ExpandoObject();
            student.Name = "Joey";
            Console.WriteLine($"Hello {student.Name}");

            // Passing delagate to Expand Object
            student.DisplayName = (Func<string,string>)((str) => str);
            Console.WriteLine($"Hello {student.DisplayName(student.Name)}");

            // Dynamic Object
            dynamic students = new StudentList();
            students.Add(new Student() { ID = "1", Name = "Bo" });
            students.Add(new Student() { ID = "2", Name = "Bi" });

            Console.WriteLine(students["1"]); // ID
            Console.WriteLine(students[1]); // index

            Console.ReadKey();
        }
    }

    class Student
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return String.Format("Student: ID = {0}, Name = {1}", ID, Name);
        }
    }

    class StudentList : DynamicObject
    {
        IList<Student> students;

        public StudentList()
        {
            students = new List<Student>();
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            MethodInfo method = students.GetType().GetMethod(binder.Name);

            if (method != null)
            {
                result = method.Invoke(students, args);
                return true;
            }
            result = null;
            return false;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes[0].GetType() == typeof(int)) // Lấy student tại index
            {
                result = students[(int)indexes[0]];
            }
            else    // lấy student đầu tiên có ID phù hợp
            {
                string id = indexes[0].ToString();
                result = (from st in students
                          where st.ID == id
                          select st).First();
            }
            return true;
        }
    }


}
