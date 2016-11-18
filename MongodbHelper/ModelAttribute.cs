﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongodbHelper
{
    [AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public class CollectionNameAttribute : Attribute
    {
        public CollectionNameAttribute(string collectionName)
        {
            Value = collectionName;
        }

        public string Value
        {
            get;
            private set;
        }
    }
}
