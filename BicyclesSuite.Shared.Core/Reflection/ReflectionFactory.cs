using BicyclesSuite.Shared.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BicyclesSuite.Shared.Reflection
{
    /// <summary>
    /// Factory for easy working with Reflection
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static class ReflectionFactory
    {
        #region Members

        internal const string TYPE_UNIQUE_NAME_FORMAT = "{0},{1}";

        private static readonly Stack m_stack = Stack.Synchronized(new Stack());

        #endregion

        #region Thread-safe isolation instance methods

        private static ReflectionFactoryProvider Pop()
        {
            ReflectionFactoryProvider instance;
            if (m_stack.Count == 0)
            {
                instance = new ReflectionFactoryProvider();
            }
            else
            {
                instance = (ReflectionFactoryProvider)m_stack.Pop();
            }
            return instance;
        }

        private static void Push(ReflectionFactoryProvider reflectionFactory)
        {
            m_stack.Push(reflectionFactory);
        }

        #endregion

        #region Public methods

        #region Assembly and Types factory methods

        /// <summary>
        /// Dynamic load assembly
        /// </summary>
        /// <param name="assemblyName">assembly unique name</param>
        /// <returns></returns>
        public static Assembly LoadAssembly(string assemblyName)
        {
            var instance = Pop();
            try
            {
                return instance.LoadAssembly(assemblyName);
            }
            catch (Exception)
            {
                throw new NotImplementedException(); 
#warning Constant.Exception.Runtime.AssemblyNotLoad.CreateException(ex, new KeyValuePair<string, object>("Assembly Name", assemblyName));
            }
            finally
            {
                Push(instance);
            }
        }

        /// <summary>
        /// Create type info by System.Type
        /// </summary>
        /// <param name="type">Runtime type</param>
        /// <returns>TypeInfo</returns>
        public static TypeInfo GetTypeInfo(Type type)
        {
            return type != null ? new TypeInfo(type.FullName, type.Assembly.GetUniqueName()) : null;
        }

        /// <summary>
        /// Get Type unique name
        /// </summary>
        /// <param name="type">Runtime type</param>
        /// <returns>Type unique name</returns>
        public static string GetTypeUniqueName(Type type)
        {
            return type != null ? FormatUniqueName(type.FullName, type.Assembly.GetName()) : null;
        }

        /// <summary>
        /// Get Type by type and assembly name
        /// </summary>
        /// <param name="assemblyName">assembly name</param>
        /// <param name="typeName">type name</param>
        /// <returns></returns>
        public static Type GetTypeByName(string assemblyName, string typeName)
        {
            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            return GetTypeByName(FormatUniqueName(typeName, assemblyName));
        }

        /// <summary>
        /// Get Type by type and assembly name
        /// </summary>
        /// <param name="uniqueTypeName">unique type name</param>
        /// <returns></returns>
        public static Type GetTypeByName(string uniqueTypeName)
        {
            if (string.IsNullOrEmpty(uniqueTypeName))
            {
                return null;
            }
            var instance = Pop();
            try
            {
                return instance.GetRuntimeType(uniqueTypeName);
            }
            finally
            {
                Push(instance);
            }
        }

        #endregion

        #region GetTypes methods

        /// <summary>
        /// Get assembly types with common base class
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <typeparam name="T">Base type</typeparam>
        /// <param name="excludedTypesInfo">Excluded types</param>
        /// <param name="isIncludeBaseType">Use Base type in return types</param>
        /// <returns>Types</returns>
        public static Type[] GetTypes<T>(Assembly assembly, TypeInfo[] excludedTypesInfo = null, bool isIncludeBaseType = false)
        {
            return GetTypes(assembly, typeof(T), excludedTypesInfo, isIncludeBaseType);
        }

        /// <summary>
        /// Get assembly types with common base class
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="baseType">Base type</param>
        /// <param name="excludedTypesInfo">Excluded types</param>
        /// <param name="isIncludeBaseType">Use Base type in return types</param>
        /// <returns>Types</returns>
        public static Type[] GetTypes(Assembly assembly, Type baseType, TypeInfo[] excludedTypesInfo = null, bool isIncludeBaseType = false)
        {
            var instance = Pop();
            try
            {
                return instance.GetTypes(assembly, baseType, excludedTypesInfo, isIncludeBaseType);
            }
            finally
            {
                Push(instance);
            }
        }

        /// <summary>
        /// Get assembly types
        /// </summary>
        /// <param name="assemblies">Assembly names</param>
        /// <param name="baseType">Base type</param>
        /// <param name="excludedTypesInfo">Excluded types</param>
        /// <returns>Types</returns>
        public static Type[] GetTypes(Assembly[] assemblies, Type baseType, TypeInfo[] excludedTypesInfo)
        {
            var list = new List<Type>();
            var cnt = assemblies.Length;
            for (var i = 0; i < cnt; i++)
            {
                list.AddRange(GetTypes(assemblies[i], baseType, excludedTypesInfo));
            }
            return list.ToArray();
        }

        /// <summary>
        /// Get assembly types
        /// </summary>
        /// <param name="assemblies">Assembly list</param>
        /// <param name="excludedTypesInfo">Excluded types</param>
        /// <returns>Types</returns>
        public static Type[] GetTypes<T>(Assembly[] assemblies, TypeInfo[] excludedTypesInfo)
        {
            return GetTypes(assemblies, typeof(T), excludedTypesInfo);
        }


        #endregion

        #region Prototype Instance methods

        /// <summary>
        /// Create referenced prototype
        /// </summary>
        /// <param name="prototype">Prototype instance</param>
        /// <param name="type">Default prototype type</param>
        /// <returns>Prototype created/referenced instance</returns>
        public static IPrototypeInstance PrototypeInstance(ref IPrototypeInstance prototype, Type type)
        {
            return PrototypeInstance(ref prototype, null, type);
        }

        /// <summary>
        /// Create referenced prototype
        /// </summary>
        /// <param name="prototype">Prototype instance</param>
        /// <param name="uniqueType">Unique type name</param>
        /// <param name="type">Default prototype type</param>
        /// <returns>Prototype created/referenced instance</returns>
        public static IPrototypeInstance PrototypeInstance(ref IPrototypeInstance prototype, string uniqueType, Type type = null)
        {
            if (prototype != null)
            {
                return prototype;
            }
            if (string.IsNullOrEmpty(uniqueType) && type == null)
            {
                throw new NotImplementedException();
#warning Constant.Exception.Runtime.PrototypeCantCreate.CreateException();
            }
            try
            {
                if (string.IsNullOrEmpty(uniqueType))
                {
                    prototype = type.CreateInstance<IPrototypeInstance>();
                    return prototype;
                }
                var runtimeType = GetTypeByName(uniqueType);
                if (runtimeType == null)
                {
                    throw new NotImplementedException();
#warning Constant.Exception.Runtime.PrototypeCantCreate.CreateException();
                }
                prototype = runtimeType.CreateInstance<IPrototypeInstance>();
            }
            catch (Exception)
            {
                throw new NotImplementedException();
#warning Constant.Exception.Runtime.PrototypeCantCreate.CreateException(ex);
            }
            return prototype;
        }

        #endregion

        #region CreateInstance methods

        /// <summary>
        /// Create instance by runtime type
        /// </summary>
        /// <param name="type">Runtime type</param>
        /// <returns>Instance object</returns>
        public static object CreateInstance(Type type)
        {
            if (type == null)
            {
                throw new NotImplementedException();
#warning Constant.Exception.Runtime.InvalidArgument.CreateException();
            }
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary>
        /// <typeparam name="T">Runtime type</typeparam>
        /// <returns>Instance object</returns>
        public static T CreateInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary> 
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="parameters">Constructor parameters</param>
        /// <param name="type">Creation type</param>
        /// <returns>Instance object</returns>
        public static T CreateInstance<T>(Type type, object[] parameters)
        {
            return (T)CreateInstance(type, parameters);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary> 
        /// <param name="values">Constructor parameters</param>
        /// <param name="type">Creation type</param>
        /// <returns>Instance object</returns>

        public static object CreateInstance(Type type, object[] values)
        {
            var parameterCount = values != null ? values.Length : 0;
            var args = parameterCount > 0 ? new Tuple<Type, object>[parameterCount] : null;
            for (var i = 0; i < parameterCount; i++)
            {
                var item = values[i];
                if (item == null)
                {
                    throw new NotImplementedException();
#warning Constant.Exception.Runtime.InvalidArgument.CreateException(new KeyValuePair<string, object>("Parameters has null", true));
                }
                args[i] = new Tuple<Type, object>(item.GetType(), item);
            }
            return CreateInstance(type, args);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary> 
        /// <typeparam name="T">Creation type</typeparam>
        /// <param name="parameters">Constructor parameters (type)</param>
        /// <returns>Instance object</returns>
        public static T CreateInstance<T>(Tuple<Type, object>[] parameters)
        {
            return (T)CreateInstance(typeof(T), parameters);
        }

        /// <summary>
        /// Create instance by runtime type
        /// </summary> 
        /// <param name="parameters">Constructor parameters</param>
        /// <param name="type">Creation type</param>
        /// <returns>Instance object</returns>
        public static object CreateInstance(Type type, Tuple<Type, object>[] parameters)
        {
            var instance = Pop();
            try
            {
                return instance.CreateInstance(type, parameters);
            }
            finally
            {
                Push(instance);
            }
        }

        #endregion

        #region Invoke methods

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="source">Type</param>
        /// <param name="methodName">Static method name</param>
        /// <param name="genericParameters">Generic method parameters</param>
        /// <returns>Invoke method result</returns>
        public static object InvokeMethod(Type source, string methodName, Type[] genericParameters)
        {
            return InvokeMethod(null, source, methodName, null, genericParameters);
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="source">Type</param>
        /// <param name="methodName">Static method name</param>
        /// <param name="parameters">parameters</param>
        /// <param name="genericParameters">Generic method parameters</param>
        /// <returns>Invoke method result</returns>
        public static object InvokeMethod(Type source, string methodName, Tuple<Type, object>[] parameters = null, Type[] genericParameters = null)
        {
            return InvokeMethod(null, source, methodName, parameters, genericParameters);
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="source">Source instance</param>
        /// <param name="methodName">Method name</param>
        /// <param name="genericParameters">Generic method parameters</param>
        /// <returns>Invoke method result</returns>
        public static object InvokeMethod(object source, string methodName, Type[] genericParameters)
        {
            return InvokeMethod(source, null, methodName, null, genericParameters);
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="source">Source instance</param>
        /// <param name="methodName">Method name</param>
        /// <param name="parameters">parameters</param>
        /// <param name="genericParameters">Generic method parameters</param>
        /// <returns>Invoke method result</returns>
        public static object InvokeMethod(object source, string methodName, Tuple<Type, object>[] parameters = null, Type[] genericParameters = null)
        {
            return InvokeMethod(source, null, methodName, parameters, genericParameters);
        }

        #endregion

        #region Get property type methods

        /// <summary>
        /// Get property runtime type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="parameterTypes">indexer property parameters</param>
        /// <returns>Return property runtime type</returns>
        public static Type GetPropertyType(object source, Type[] parameterTypes)
        {
            return GetPropertyType(source, null, null, parameterTypes);
        }

        /// <summary>
        /// Get property runtime type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="propertyName">property name</param>
        /// <param name="parameterTypes">indexer property parameters</param>
        /// <returns>Return property runtime type</returns>
        public static Type GetPropertyType(object source, string propertyName, Type[] parameterTypes = null)
        {
            return GetPropertyType(source, null, propertyName, parameterTypes);
        }

        /// <summary>
        /// Get property runtime type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="propertyName">Static property name</param>
        /// <returns>Return property runtime type</returns>
        public static Type GetPropertyType(Type source, string propertyName)
        {
            return GetPropertyType(null, source, propertyName, null);
        }

        #endregion

        #region Set property value methods

        /// <summary>
        /// Set property value
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="propertyName">property name</param>
        /// <param name="value">New value</param>
        public static void SetPropertyValue(object source, string propertyName, object value)
        {
            SetPropertyValue(source, null, propertyName, null, value);
        }

        /// <summary>
        /// Set property value
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="parameters">indexer property parameters</param>
        /// <param name="value">New value</param>
        public static void SetPropertyValue(object source, Tuple<Type, object>[] parameters, object value)
        {
            SetPropertyValue(source, null, null, parameters, value);
        }

        /// <summary>
        /// Set property value
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="propertyName">property name</param>
        /// <param name="parameters">indexer property parameters</param>
        /// <param name="value">New value</param>
        public static void SetPropertyValue(object source, string propertyName, Tuple<Type, object>[] parameters, object value)
        {
            SetPropertyValue(source, null, propertyName, parameters, value);
        }

        /// <summary>
        /// Set property value
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="propertyName">Static property name</param>
        /// <param name="value">New value</param>
        public static void SetPropertyValue(Type source, string propertyName, object value)
        {
            SetPropertyValue(null, source, propertyName, null, value);
        }

        #endregion

        #region Get property value methods

        /// <summary>
        /// Get property value
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="parameters">indexer property parameters</param>
        /// <returns>Return property runtime type</returns>
        public static object GetPropertyValue(object source, Tuple<Type, object>[] parameters)
        {
            return GetPropertyValue(source, null, null, parameters);
        }

        /// <summary>
        /// Get property value
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="propertyName">property name</param>
        /// <param name="parameters">indexer property parameters</param>
        /// <returns>Return property runtime type</returns>
        public static object GetPropertyValue(object source, string propertyName, Tuple<Type, object>[] parameters = null)
        {
            return GetPropertyValue(source, null, propertyName, parameters);
        }

        /// <summary>
        /// Get property value
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="propertyName">Static property name</param>
        /// <returns>Return property runtime type</returns>
        public static object GetPropertyValue(Type source, string propertyName)
        {
            return GetPropertyValue(null, source, propertyName, null);
        }

        #endregion

        #endregion

        #region Internal methods

        internal static string FormatUniqueName(string typeName, AssemblyName assemblyName)
        {
            return FormatUniqueName(typeName, assemblyName != null ? assemblyName.Name : null);
        }

        internal static string FormatUniqueName(string typeName, string assemblyName)
        {
            if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(assemblyName))
            {
                return null;
            }
            return string.Format(TYPE_UNIQUE_NAME_FORMAT, typeName, assemblyName);
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="sourceType">Source type</param>
        /// <param name="methodName">Method name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="genericParameters">Generic parameters</param>
        /// <returns>Method invoke result</returns>
        internal static object InvokeMethod(object source, Type sourceType, string methodName, Tuple<Type, object>[] parameters, Type[] genericParameters)
        {
            var instance = Pop();
            try
            {
                return instance.InvokeMethod(source, sourceType, methodName, parameters, genericParameters);
            }
            finally
            {
                Push(instance);
            }
        }

        /// <summary>
        /// Get property runtime type
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="sourceType">Source type</param>
        /// <param name="propertyName">property name</param>
        /// <param name="parameterTypes">indexer property parameters</param>
        /// <returns>Return property runtime type</returns>
        internal static Type GetPropertyType(object source, Type sourceType, string propertyName, Type[] parameterTypes)
        {
            var instance = Pop();
            Type type;
            try
            {
                type = instance.GetPropertyType(source, sourceType, propertyName, parameterTypes);
            }
            finally
            {
                Push(instance);
            }
            return type;
        }

        internal static object GetPropertyValue(object source, Type sourceType, string propertyName, Tuple<Type, object>[] parameters)
        {
            var instance = Pop();
            object val;
            try
            {
                val = instance.GetPropertyValue(source, sourceType, propertyName, parameters);
            }
            finally
            {
                Push(instance);
            }
            return val;
        }

        internal static void SetPropertyValue(object source, Type sourceType, string propertyName, Tuple<Type, object>[] parameters, object value)
        {
            var instance = Pop();
            try
            {
                instance.SetPropertyValue(source, sourceType, propertyName, parameters, value);
            }
            finally
            {
                Push(instance);
            }
        }

        #endregion
    }

}
