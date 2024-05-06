using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Models
{
    public class JobApplication
    {
        public Applicant Applicant { get; set; }
        public int YearsofExperience { get; set; }
        public List<string> TechStackList { get; set; }

    }
}
