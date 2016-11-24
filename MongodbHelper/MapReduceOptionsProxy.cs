using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    public class MapReduceOptionsProxy<TDocument, TResult>
    {
        public string Map { get; set; }
        public string Reduce { get; set; }
        public string CollectionName { get; set; }
        public string DatabaseName { get; set; }
        public MapReduceOutputOptionsEnum OutputEnum { get; set; }
        public string Filter { get; set; }
        public string Finalize { get; set; }
        public bool? JavaScriptMode { get; set; }
        public long? Limit { get; set; }
        public TimeSpan? MaxTime { get; set; }
        public string Sort { get; set; }
        public bool? Verbose { get; set; }
        public bool? BypassDocumentValidation { get; set; }
    }

    public enum MapReduceOutputOptionsEnum
    {
        Inline = 0,
        Merge = 1,
        Reduce = 2,
        Replace = 3
    }
}
