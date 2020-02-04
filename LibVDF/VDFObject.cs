using System;
using System.Collections.Generic;

namespace LibVDF
{
    public class VDFObject
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        internal VDFObject(Dictionary<string, object> values)
        {
            this.values = values;
        }

        public string GetString(string name)
        {
            if (values[name] is string)
                return values[name] as string;
            else
                throw new ArgumentException();
        }

        public VDFObject GetObject(string name)
        {
            if (values[name] is VDFObject)
                return values[name] as VDFObject;
            else
                throw new ArgumentException();
        }
    }
}