
namespace VeriGenX.Infrastructure.DAO
{
    public class SignalDocument
    {
        public string Type { get;  set; }
        public string Name { get;  set; }
        public int Size { get;set; }
        public SortedList<int, double> _signalValues { get;  set; } 
    }
}
