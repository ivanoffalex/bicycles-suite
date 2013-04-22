using BicyclesSuite.Shared.Reflection;
using System;
using System.Reflection;

namespace BicyclesSuite.Shared
{
    /// <summary>
    /// Extension methods for System.Reflection
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get unique assembly name
        /// </summary>
        /// <param name="assembly">System.Reflection.Assembly</param>
        /// <returns>Assembly unique name</returns>
        public static string GetUniqueName(this Assembly assembly)
        {
            return assembly.GetName().FullName;
        }

        /// <summary>
        /// Get qualified type name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetQualifiedTypeName(this Type value)
        {
            return string.Format(ReflectionFactory.TYPE_UNIQUE_NAME_FORMAT, value.FullName, value.Assembly.GetName().Name);
        }

        /// <summary>
        /// Cast object to specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="typeToCastTo"></param>
        /// <returns></returns>
        public static T ToType<T>(this object obj, T typeToCastTo)
        {
            return (T)obj;
        }

        /// <summary>
        /// Create type info by System.Type
        /// </summary>
        /// <param name="type">Runtime type</param>
        /// <returns>TypeInfo</returns>
        public static Reflection.TypeInfo GetTypeInfo(this Type type)
        {
            return ReflectionFactory.GetTypeInfo(type);
        }

        /// <summary>
        /// Get unique type name
        /// </summary>
        /// <param name="type">System.Type</param>
        /// <returns>Type unique name</returns>
        public static string GetUniqueName(this Type type)
        {
            return ReflectionFactory.GetTypeUniqueName(type);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="type">Runtime type</param>
        /// <returns>Instance object</returns>
        public static T CreateInstance<T>(this Type type)
        {
            return (T)ReflectionFactory.CreateInstance(type);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary>
        /// <param name="type">Runtime type</param>
        /// <param name="parameters">Constructor parameters</param>
        /// <returns>Instance object</returns>
        public static object CreateInstance(this Type type, object[] parameters)
        {
            return ReflectionFactory.CreateInstance(type, parameters);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary> 
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="parameters">Constructor parameters</param>
        /// <param name="type">Runtime type</param>
        /// <returns>Instance object</returns>
        public static T CreateInstance<T>(this Type type, object[] parameters)
        {
            return ReflectionFactory.CreateInstance<T>(type, parameters);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary> 
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="parameters">Constructor parameters</param>
        /// <param name="type">Runtime type</param>
        /// <returns>Instance object</returns>
        public static T CreateInstance<T>(this Type type, Tuple<Type, object>[] parameters)
        {
            return (T)ReflectionFactory.CreateInstance(type, parameters);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary> 
        /// <param name="parameters">Constructor parameters</param>
        /// <param name="type">Runtime type</param>
        /// <returns>Instance object</returns>
        public static object CreateInstance(this Type type, Tuple<Type, object>[] parameters)
        {
            return ReflectionFactory.CreateInstance(type, parameters);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary>
        /// <param name="type">Runtime type</param>
        /// <returns>Instance object</returns>
        public static object CreateInstance(this Type type)
        {
            return ReflectionFactory.CreateInstance(type);
        }

        /// <summary>
        /// Get assembly types with common base class
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <typeparam name="T">Base type</typeparam>
        /// <param name="excludedTypesInfo">Excluded types</param>
        /// <param name="isIncludeBaseType">Use Base type in return types</param>
        /// <returns>Types</returns>
        public static Type[] GetTypes<T>(this Assembly assembly, Reflection.TypeInfo[] excludedTypesInfo = null, bool isIncludeBaseType = false)
        {
            return ReflectionFactory.GetTypes<T>(assembly, excludedTypesInfo, isIncludeBaseType);
        }

        /// <summary>
        /// Get assembly types with common base class
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="baseType">Base type</param>
        /// <param name="excludedTypesInfo">Excluded types</param>
        /// <param name="isIncludeBaseType">Use Base type in return types</param>
        /// <returns>Types</returns>
        public static Type[] GetTypes(this Assembly assembly, Type baseType, Reflection.TypeInfo[] excludedTypesInfo = null, bool isIncludeBaseType = false)
        {
            return ReflectionFactory.GetTypes(assembly, baseType, excludedTypesInfo, isIncludeBaseType);
        }


    }
}
