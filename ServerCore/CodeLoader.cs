using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace ServerCore {
    public class CodeLoader {
        AssemblyLoadContext assemblyLoadContext;
        public Assembly hotfix;
        public Dictionary<string, Assembly> hotfixDictionary = new Dictionary<string, Assembly>();
        //Dictionary<string, object> hotfixInstance = new Dictionary<string, object>();
        Dictionary<string, Dictionary<string, object>> hotfixInstance = new Dictionary<string, Dictionary<string, object>>();
        public static CodeLoader instance;

        public CodeLoader() {

            instance = this;

        }
        public static CodeLoader GetInstance() {
            return instance;
        }
        //public void Reload( string HotfixPath = @".\ServerLoginHotfix") {
        //    try {
        //        hotfixInstance.Clear();
        //        assemblyLoadContext?.Unload();
        //        GC.Collect();
        //        assemblyLoadContext = new AssemblyLoadContext("ServerLoginHotfix", true);
        //        byte[] dllBytes = File.ReadAllBytes(HotfixPath + ".dll");//加载dll
        //        byte[] pdbBytes = File.ReadAllBytes(HotfixPath + ".pdb");//加载pdb
        //        hotfix = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));
        //        //object obj = hotfix.CreateInstance("ClassLibrary1.Class1");

        //        //MethodInfo mm = obj.GetType().GetMethod("add");
        //        //mm.Invoke(obj, null);

        //        foreach (Type type in hotfix.GetTypes()) {
        //            object instance = Activator.CreateInstance(type);
        //            hotfixInstance.Add(type.FullName, instance);
        //            Console.WriteLine(type.FullName);
        //        }
        //    }
        //    catch(Exception e) {
        //        Console.WriteLine("[ServerCore.Core.CodeLoader]"+ e.ToString());
        //    }
        //}
        public void Reload(string dllName = "LogicHotfix", string HotfixPath = @".\LogicHotfix") {
            assemblyLoadContext?.Unload();

            if (!hotfixDictionary.ContainsKey(dllName)) {
                assemblyLoadContext = new AssemblyLoadContext(dllName, true);
                byte[] dllBytes = File.ReadAllBytes(HotfixPath + ".dll");//加载dll
                byte[] pdbBytes = File.ReadAllBytes(HotfixPath + ".pdb");//加载pdb
                Assembly temphotfix = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));
                hotfixDictionary.Add(dllName, temphotfix);
                Dictionary<string, object> tempInstance = new Dictionary<string, object>();
                foreach (Type type in temphotfix.GetExportedTypes()) {
                    //GetTypes替换为GetExportedTypes
                    object instance = Activator.CreateInstance(type);
                    tempInstance.Add(type.FullName, instance);
                    Console.WriteLine(type.FullName);
                }
                hotfixInstance.Add(dllName, tempInstance);
            }
            else {
                hotfixInstance[dllName].Clear();
                GC.Collect();
                assemblyLoadContext = new AssemblyLoadContext(dllName, true);
                byte[] dllBytes = File.ReadAllBytes(HotfixPath + ".dll");//加载dll
                byte[] pdbBytes = File.ReadAllBytes(HotfixPath + ".pdb");//加载pdb
                hotfixDictionary[dllName] = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));
                foreach (Type type in hotfixDictionary[dllName].GetExportedTypes()) {
                    object instance = Activator.CreateInstance(type);
                    hotfixInstance[dllName].Add(type.FullName, instance);
                    Console.WriteLine(type.FullName);
                }

            }
            //GC.Collect();
        }
        public object Find(string assembly, string className) {
            return hotfixInstance[assembly][className];
        }
        public void FindFunRun(string assembly, string className, string funName, object[] objs) {

            MethodInfo mm = Find(assembly, className).GetType().GetMethod(funName);
            if (mm != null)
                mm.Invoke(Find(assembly, className), objs);
            else
                Console.WriteLine("className: " + className + " funName: " + funName + "没找到");

        }

    }
}
