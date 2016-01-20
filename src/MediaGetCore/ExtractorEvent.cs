using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGetCore{
    public delegate void ProcessEvent(IExtractor sender,double percent);
    public delegate void CompletedEvent(IExtractor sender, MediaInfo[] result);
}
