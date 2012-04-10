namespace DartSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class TypeUtilities
    {
        private static bool referencedAssembliesLoaded = false;

        public static Type GetType(Context context, string name)
        {
            object obj = context.GetValue(name);

            if (obj != null && obj is Type)
                return (Type)obj;

            return GetType(name);
        }

        public static Type AsType(string name)
        {
            Type type = Type.GetType(name);

            if (type != null)
                return type;

            type = GetTypeFromLoadedAssemblies(name);

            if (type != null)
                return type;

            type = GetTypeFromPartialNamedAssembly(name);

            if (type != null)
                return type;

            LoadReferencedAssemblies();

            type = GetTypeFromLoadedAssemblies(name);

            if (type != null)
                return type;

            return null;
        }

        public static Type GetType(string name)
        {
            Type type = AsType(name);

            if (type != null)
                return type;

            throw new InvalidOperationException(string.Format("Unknown type '{0}'", name));
        }

        public static bool IsNamespace(string name)
        {
            return GetNamespaces().Contains(name);
        }

        public static object InvokeTypeMember(Type type, string name, object[] parameters)
        {
            return type.InvokeMember(name, System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static, null, null, parameters);
        }

        private static ICollection<string> GetNamespaces()
        {
            List<string> namespaces = new List<string>();

            LoadReferencedAssemblies();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                    if (!namespaces.Contains(type.Namespace))
                        namespaces.Add(type.Namespace);

            return namespaces;
        }

        private static Type GetTypeFromPartialNamedAssembly(string name)
        {
            int p = name.LastIndexOf(".");

            if (p < 0)
                return null;

            string assemblyName = name.Substring(0, p);

            try
            {
                Assembly assembly = Assembly.LoadWithPartialName(assemblyName);

                return assembly.GetType(name);
            }
            catch
            {
                return null;
            }
        }

        private static Type GetTypeFromLoadedAssemblies(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(name);

                if (type != null)
                    return type;
            }

            return null;
        }

        private static void LoadReferencedAssemblies()
        {
            if (referencedAssembliesLoaded)
                return;

            List<string> loaded = new List<string>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                loaded.Add(assembly.GetName().Name);

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                LoadReferencedAssemblies(assembly, loaded);

            referencedAssembliesLoaded = true;
        }

        private static void LoadReferencedAssemblies(Assembly assembly, List<string> loaded)
        {
            foreach (AssemblyName referenced in assembly.GetReferencedAssemblies())
                if (!loaded.Contains(referenced.Name))
                {
                    loaded.Add(referenced.Name);
                    Assembly newassembly = Assembly.Load(referenced);
                    LoadReferencedAssemblies(newassembly, loaded);
                }
        }
    }
}
