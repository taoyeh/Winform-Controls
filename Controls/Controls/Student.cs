using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controls
{
    class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Department { get; set; }
        public bool Sex { get; set; }
        public Student(string id,string name,string address,string department, bool sex)
        {
            Id = id;
            Name = name;
            Address = address;
            Department = department;
            Sex = sex;
        }
    }
}
