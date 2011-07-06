using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MUMS.Utorrent.Service
{
    [CollectionDataContract]
    public class StringList: List<string[]>
    {
    }
}
