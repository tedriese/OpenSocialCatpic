using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catpic.Utils.Caching
{
    public interface ICache
    {
        void Add(object key, object value);
        void Add(object key, object value, Metadata metadata);

        object Get(object key);
        void Remove(object key);

        bool Contains(object key);
    }
}
