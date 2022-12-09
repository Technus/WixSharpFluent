using System;
using System.Reflection;

namespace AssemblyAttributesTestHolder
{
    public static class CallMe
    {
        public static T Call<T>(Func<Assembly,T> me)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return me.Invoke(assembly);
        }
    }
}
