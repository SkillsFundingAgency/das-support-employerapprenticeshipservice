using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Support.Shared
{
    public class SearchResultMetadata
    {
        public string SearchResultCategory { get; set; }
        public List<SearchColumnDefinition> ColumnDefinitions { get; set; }
    }
}
