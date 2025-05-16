using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeriGenX.Infrastructure.DAO
{
    public class WaveformDataDocument
    {
        public string TimeUnit { get; set; }

        public long TimeScale { get;  set; }
        public Dictionary<string, SignalDocument> _signals { get;  set; }
    }
    
}
