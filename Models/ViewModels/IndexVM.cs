using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class IndexVM
    {
        public List<Employee> Empolyees { get; set; } = new List<Employee>();
        public IEnumerable<Qualification> Qualifications { get; set; } = new List<Qualification>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public Employee NewEmployee { get; set; } = new Employee();
    }
}
