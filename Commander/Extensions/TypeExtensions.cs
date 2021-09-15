using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public static class TypeExtensions
    {
        public static bool Is(this Type type, Type typeCompare) => type.IsGenericType && (type.Name.Equals(typeCompare.Name) 
            || type.GetGenericTypeDefinition() == typeCompare);
    }
}
