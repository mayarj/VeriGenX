using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeriGenX.Infrastructure.DAO
{
    public class TestResultDocument
    {
        public bool Passed { get; set; }
        public HashSet<int>? CodeErrorLines { get;  set; }
        public HashSet<int>? TestErrorLines { get;  set; }
        public DateTime TestTime { get; set; }
        public List<string>? Failures { get;set; }
    }
}
