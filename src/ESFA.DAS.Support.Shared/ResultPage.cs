using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ESFA.DAS.Support.Shared
{
    public class ResultPage<T>
    {
        [JsonProperty("_links")]
        public PageLinks Links { get; set; }
        public IEnumerable<T> Results { get; set; }
        public int Size { get; set; }
        public int Start { get; set; }
    }
}
