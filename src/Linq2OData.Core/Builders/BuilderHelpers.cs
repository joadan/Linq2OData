using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Core.Builders
{
    internal static class BuilderHelper
    {

        internal static string GetEntityPath<T>() where T : IODataEntitySet, new()
        {
            if (typeof(T).GetCustomAttributes(typeof(ODataEntitySetAttribute), true).FirstOrDefault() is not ODataEntitySetAttribute attrib)
            {
                throw new InvalidOperationException($"The type {typeof(T).FullName} is missing the ODataEntitySetAttribute.");
            }
            return attrib.EntityPath;
        }

        internal static string GetEntityKeys<T>(Action<T> keySetter) where T : IODataEntitySet, new()
        {
            var entity = new T();
            keySetter(entity);

            return entity.__Keys;
        }



        

    }
}
