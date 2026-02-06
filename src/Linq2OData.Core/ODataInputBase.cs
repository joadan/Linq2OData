using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Linq2OData.Core
{
   
    public abstract class ODataInputBase
    {
        private ConcurrentDictionary<string, object> Values { get; } = new();

        internal Dictionary<string, object> GetValues()
        {
            return Values.ToDictionary(e => e.Key, e => e.Value);
        }


        protected void ClearValues()
        {
            Values.Clear();
        }

        protected T? GetValue<T>(string name)
        {
            if (Values.TryGetValue(name, out var value))
            {
                return (T)value;
            }

            return default;
        }


        protected void SetValue(string name, object? value)
        {
            Values.TryRemove(name, out _);
            // Always add the value, even if null
            // This allows explicitly setting a property to null (important for UPDATE operations)
            // Properties that are never set won't be in the dictionary at all
            Values.TryAdd(name, value!);
        }
    }

}

