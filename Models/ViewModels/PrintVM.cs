using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class PrintVM
    {
        public Employee? Employee { get; set; } 
        public List<Vacation> Vacations { get; set; } = new List<Vacation>();
    }
}
