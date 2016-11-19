using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CollectionNameAttribute : Attribute
    {
        public CollectionNameAttribute(string datebaseName, string collectionName)
        {
            this.DatebaseName = datebaseName;
            this.CollectionName = collectionName;
        }

        public string DatebaseName
        {
            get;
            private set;
        }

        public string CollectionName
        {
            get;
            private set;
        }
    }
}
