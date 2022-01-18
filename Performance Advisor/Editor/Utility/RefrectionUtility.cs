using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace UPA
{
    public struct RefrectionUtility
    {
        ///Assemblies
        private static List<Assembly> _loadedAssemblies;
        private static List<Assembly> loadedAssemblies
        {
            get
            {
                if (_loadedAssemblies == null)
                {
#if NETFX_CORE
				    _loadedAssemblies = new List<Assembly>();
		 		    var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
				    var folderFilesAsync = folder.GetFilesAsync();
				    folderFilesAsync.AsTask().Wait();

				    foreach (var file in folderFilesAsync.GetResults()){
				        if (file.FileType == ".dll" || file.FileType == ".exe"){
				            try
				            {
				                var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
				                AssemblyName name = new AssemblyName { Name = filename };
				                Assembly asm = Assembly.Load(name);
				                _loadedAssemblies.Add(asm);
				            }
				            catch { continue; }
				        }
				    }
#else

                    _loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
#endif
                }

                return _loadedAssemblies;
            }
        }

        private static Dictionary<Type, Type[]> subTypesMap = new Dictionary<Type, Type[]>();
        ///Get a collection of types assignable to provided type, excluding Abstract types
        public static Type[] GetImplementationsOf(Type type)
        {
            Type[] result = null;
            if (subTypesMap.TryGetValue(type, out result))
            {
                return result;
            }

            var temp = new List<Type>();
            foreach (var asm in loadedAssemblies)
            {
                try { temp.AddRange(asm.GetExportedTypes().Where(t => type.IsAssignableFrom(t) && !t.IsAbstract)); }
                catch { continue; }
            }
            return subTypesMap[type] = temp.ToArray();
        }
    }
}