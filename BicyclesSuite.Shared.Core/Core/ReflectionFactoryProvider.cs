using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BicyclesSuite.Shared.Core
{
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    internal class ReflectionFactoryProvider
    {
//#warning Move to configuration
//        internal sealed class ReflectionSettings
//        {
//            //const ReflectionInvokeMethod InvokeMethod = ReflectionInvokeMethod.Emit;
//            //const int CacheLimit = 10000;
//        }

        private const string PROPERTY_INDEXER_KEY = "<%INDEXER%>";

        internal static int CacheLimit = 10000;
        internal static ReflectionInvokeMethod InvokeMethodType = ReflectionInvokeMethod.Emit;

        private static readonly Dictionary<string, Type> m_types;
        private static readonly Dictionary<MemberInfoHashKey, MemberInfoHashValue> m_reflectionCache;

        private static readonly object m_syncTypes;
        private static readonly object m_syncMember;

        static ReflectionFactoryProvider()
        {
            m_types = new Dictionary<string, Type>();
            m_reflectionCache = new Dictionary<MemberInfoHashKey, MemberInfoHashValue>();
            m_syncTypes = new object();
            m_syncMember = new object();
        }

        public Assembly LoadAssembly(string assemblyName)
        {
            lock (m_syncTypes)
            {
                return Assembly.Load(assemblyName);
            }
        }

        public Type GetRuntimeType(string uniqueTypeName)
        {
            Type type;
            lock (m_syncTypes)
            {
                if (m_types.TryGetValue(uniqueTypeName, out type))
                {
                    return type;
                }
                type = Type.GetType(uniqueTypeName, false, true);
                m_types.Add(uniqueTypeName, type);
            }
            return type;
        }

        public Type[] GetTypes(Assembly assembly, Type baseType, Reflection.TypeInfo[] excludedTypesInfo, bool isIncludeBaseType)
        {
            if (assembly == null)
            {
                return new Type[0];
            }
            var list = new List<Type>();
            var excluded = excludedTypesInfo != null && excludedTypesInfo.Length > 0 ? new List<Reflection.TypeInfo>(excludedTypesInfo) : null;
            var types = assembly.GetTypes();
            var cnt = types.Length;
            for (var i = 0; i < cnt; i++)
            {
                var type = types[i];
                if (baseType != null && !baseType.IsAssignableFrom(type))
                {
                    continue;
                }
                if (!isIncludeBaseType && type == baseType)
                {
                    continue;
                }
                if (excluded != null && excluded.Contains(type.GetTypeInfo()))
                {
                    continue;
                }
                list.Add(type);
            }
            return list.ToArray();
        }

        public object CreateInstance(Type type, Tuple<Type, object>[] args)
        {
            Type[] paramTypes;
            object[] paramObject;
            GetReflectionParameters(args, out paramTypes, out paramObject);
            var hashValue = GetConstructorInfo(type, paramTypes);
            if (hashValue == null)
            {
                throw new NotImplementedException(); 
#warning Constant.Exception.Runtime.ConstructorNotFound.CreateException();
            }
            if (hashValue.GetMethodHandler != null)
            {
                return hashValue.GetMethodHandler(null, paramObject, null);
            }
            var ci = (ConstructorInfo) hashValue.MemberInfo;
            if (InvokeMethodType == ReflectionInvokeMethod.Emit 
                && hashValue.GetMethodHandler == null)
            {
                hashValue.GetMethodHandler = ReflectionFactoryInvoker.GetMethodInvoker(ci, false);
                return hashValue.GetMethodHandler(null, paramObject, null);
            }
            return ci.Invoke(paramObject);
        }

        public object InvokeMethod(object source, Type sourceType, string methodName, Tuple<Type, object>[] parameters, Type[] genericParameters)
        {
            if (sourceType == null && source == null)
            {
                throw new NotImplementedException(); 
#warning Constant.Exception.Runtime.SourceObjectOrTypeIsNull.CreateException();
            }
            if (string.IsNullOrEmpty(methodName))
            {
                throw new NotImplementedException(); 
#warning Constant.Exception.Runtime.MethodNameIsNull.CreateException();
            }
            var type = sourceType ?? source.GetType();
            var genericCount = genericParameters != null ? genericParameters.Length : 0;


            Type[] paramTypes;
            object[] paramObject;
            GetReflectionParameters(parameters, out paramTypes, out paramObject);

            var hashValue = GetMethodInfo(type, methodName, paramTypes, genericParameters);
            if (hashValue == null)
            {
                throw  new NotImplementedException(); 
#warning Constant.Exception.Runtime.MethodNotFound.CreateException(
                    //new[]
                    //    {
                    //        new KeyValuePair<string, object>("Method Name", methodName),
                    //        new KeyValuePair<string, object>("Types", paramTypes.Join("|"))
                    //    }
                    //);
            }
            if (hashValue.GetMethodHandler != null)
            {
                return hashValue.GetMethodHandler(source, paramObject, null);
            }
            var mi = (MethodInfo)hashValue.MemberInfo;
            if (genericCount > 0)
            {
                mi = mi.MakeGenericMethod(genericParameters);
            }
            if (genericCount == 0 
                && InvokeMethodType == ReflectionInvokeMethod.Emit 
                && hashValue.GetMethodHandler == null)
            {
                hashValue.GetMethodHandler = ReflectionFactoryInvoker.GetMethodInvoker(mi, false);
                return hashValue.GetMethodHandler(source, paramObject, null);
            }
            return mi.Invoke(source, paramObject);
        }

        private static void GetReflectionParameters(Tuple<Type, object>[] parameters, out Type[] paramTypes, out object[] paramObject)
        {
            paramTypes = null;
            paramObject = null;
            var parametersCount = parameters != null ? parameters.Length : 0;
            if (parametersCount <= 0)
            {
                return;
            }
            paramTypes = new Type[parametersCount];
            paramObject = new object[parametersCount];
            for (var i = 0; i < parametersCount; i++)
            {
                var parameter = parameters[i];
                if (parameter.Item1 == null)
                {
                    throw new NotImplementedException(); 
#warning Constant.Exception.Runtime.InvalidArgument.CreateException();
                }
                paramTypes[i] = parameters[i].Item1;
                paramObject[i] = parameters[i].Item2;
            }
        }

        public Type GetPropertyType(object source, Type sourceType, string propertyName, Type[] parameterTypes)
        {
            if (source == null && sourceType == null) return null;
            var parametersCount = parameterTypes != null ? parameterTypes.Length : 0;
            var isEmptyPropertyName = string.IsNullOrEmpty(propertyName);
            if (isEmptyPropertyName && parametersCount == 0) return null;
            var type = sourceType ?? source.GetType();

            if (!isEmptyPropertyName && propertyName.IndexOf('.') > 0)
            {
                var names = propertyName.Split('.');
                var cnt = names.Length - 1;
                for (var i = 0; i < cnt; i++)
                {
                    if (source == null) return null;
                    source = GetSingleProperty(source, null, names[i], null);
                }
                if (source == null) return null;
                type = GetSinglePropertyType(source.GetType(), names[names.Length - 1], Type.EmptyTypes);
            }
            else
            {
                type = GetSinglePropertyType(type, propertyName, Type.EmptyTypes);
            }

            if (parametersCount > 0)
            {
                type = GetSinglePropertyType(type, string.Empty, parameterTypes);
            }
            return type;
        }

        public object GetPropertyValue(object source, Type sourceType, string propertyName, Tuple<Type, object>[] parameters)
        {
            if (source == null && sourceType == null) return null;
            var parametersCount = parameters != null ? parameters.Length : 0;
            var isEmptyPropertyName = string.IsNullOrEmpty(propertyName);
            if (isEmptyPropertyName && parametersCount == 0) return null;
            var type = sourceType ?? source.GetType();

            if (!isEmptyPropertyName && propertyName.IndexOf('.') > 0)
            {
                var names = propertyName.Split('.');
                var cnt = names.Length;
                for (var i = 0; i < cnt; i++)
                {
                    if (source == null) return null;
                    source = GetSingleProperty(source, null, names[i], null);
                }
            }
            else
            {
                source = GetSingleProperty(source, type, propertyName, null);
            }
            if (source == null) return null;
            if (parametersCount > 0)
            {
                type = source.GetType();
                source = GetSingleProperty(source, type, string.Empty, parameters);
            }
            return source;
        }

        public void SetPropertyValue(object source, Type sourceType, string propertyName, Tuple<Type, object>[] parameters, object value)
        {
            if (source == null && sourceType == null) return;
            var isEmptyPropertyName = string.IsNullOrEmpty(propertyName);
            var parametersCount = parameters != null ? parameters.Length : 0;
            if (isEmptyPropertyName && parametersCount == 0) return;
            var type = sourceType ?? source.GetType();

            if (!isEmptyPropertyName && propertyName.IndexOf('.') > 0)
            {
                var names = propertyName.Split('.');
                var cnt = names.Length - 1;
                for (var i = 0; i < cnt; i++)
                {
                    if (source == null) return;
                    source = GetSingleProperty(source, null, names[i], null);
                }
                propertyName = names[names.Length - 1];
                if (source == null) return;
            }

            type = source != null ? source.GetType() : type;

            if (parametersCount > 0)
            {
                if (!isEmptyPropertyName)
                {
                    source = GetSingleProperty(source, type, propertyName, null);
                    if (source == null) return;
                    type = source.GetType();
                }
                propertyName = string.Empty;
            }

            SetSingleProperty(source, type, propertyName, parameters, value);
        }

        #region Private methods

        #region Get reflection info methods

        private MemberInfoHashValue GetPropertyInfo(Type type, string propertyName, Type[] types)
        {
            var typesCount = types != null ? types.Length : 0;
            if (string.IsNullOrEmpty(propertyName) && typesCount == 0)
            {
                throw new NotImplementedException(); 
#warning Constant.Exception.Runtime.MethodNameIsNull.CreateException();
            }
            if (typesCount > 0)
            {
                propertyName = PROPERTY_INDEXER_KEY;
            }
            var key = new MemberInfoHashKey(type, propertyName, types, 0);
            MemberInfoHashValue hashValue;
            bool bResult;
            lock (m_syncMember)
            {
                bResult = m_reflectionCache.TryGetValue(key, out hashValue);
            }
            if (bResult)
            {
                return hashValue;
            }
            PropertyInfo pi = null;
            if (typesCount > 0)
            {
                var propertyInfos =
                    type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var cnt = propertyInfos.Length;
                for (var i = 0; i < cnt; i++)
                {
                    var tmp = propertyInfos[i];
                    if (IsEqualParameters(tmp.GetIndexParameters(), types))
                    {
                        pi = tmp;
                        break;
                    }
                }
            }
            else
            {
                pi = type.GetProperty(propertyName,
                                      BindingFlags.Public | BindingFlags.NonPublic |
                                      BindingFlags.Instance | BindingFlags.Static,
                                      null, null, Type.EmptyTypes, null);
            }
            hashValue = pi != null ? new MemberInfoHashValue(pi) : null;
            AddMemberInfoToCache(key, hashValue);
            return hashValue;
        }

        private MemberInfoHashValue GetConstructorInfo(Type type, Type[] types)
        {
            var key = new MemberInfoHashKey(type, ConstructorInfo.ConstructorName, types, 0);
            MemberInfoHashValue mi;
            bool bResult;
            lock (m_syncMember)
            {
                bResult = m_reflectionCache.TryGetValue(key, out mi);
            }
            if (bResult)
            {
                return mi;
            }
            var ci = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
            mi = ci != null ? new MemberInfoHashValue(ci) : null;
            AddMemberInfoToCache(key, mi);
            return mi;
        }

        private MemberInfoHashValue GetMethodInfo(Type type, string methodName, Type[] types, Type[] genericParameters)
        {
            var genericParameterCount = genericParameters != null ? genericParameters.Length : 0;
            var key = new MemberInfoHashKey(type, methodName, types, genericParameterCount);
            MemberInfoHashValue mi;
            bool bResult;
            lock (m_syncMember)
            {
                bResult = m_reflectionCache.TryGetValue(key, out mi);
            }
            if (bResult)
            {
                return mi;
            }

            var methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                        BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var cnt = methodInfos.Length;
            for (var i = 0; i < cnt; i++)
            {
                var tmp = methodInfos[i];
                var tmpGenericParameterCount = tmp.IsGenericMethod ? tmp.GetGenericArguments().Length : 0;

                if (tmp.Name == methodName
                    && tmpGenericParameterCount == genericParameterCount
                    && IsEqualParameters(tmp.GetParameters(), types))
                {
                    mi = new MemberInfoHashValue(tmp);
                    break;
                }
            }

            AddMemberInfoToCache(key, mi);
            return mi;
        }

        #endregion

        private Type GetSinglePropertyType(Type type, string propertyName, Type[] types)
        {
            var typesCount = types != null ? types.Length : 0;
            if (string.IsNullOrEmpty(propertyName) && typesCount == 0)
            {
                return type;
            }
            var mi = GetPropertyInfo(type, propertyName, types);
            return mi != null ? ((PropertyInfo)mi.MemberInfo).PropertyType : null;
        }

        private object GetSingleProperty(object source, Type sourceType, string propertyName, Tuple<Type, object>[] parameters)
        {
            var parametersCount = parameters != null ? parameters.Length : 0;
            if (string.IsNullOrEmpty(propertyName) && parametersCount == 0)
            {
                return source;
            }
            var type = source != null ? source.GetType() : sourceType;

            Type[] paramTypes;
            object[] paramObject;
            GetReflectionParameters(parameters, out paramTypes, out paramObject);

            var hashValue = GetPropertyInfo(type, propertyName, paramTypes);
            if (hashValue != null && hashValue.GetMethodHandler != null)
            {
                return hashValue.GetMethodHandler(source, paramObject, null);
            }
            var pi = hashValue != null ? (PropertyInfo)hashValue.MemberInfo : null;
            if (pi != null && pi.CanRead)
            {
                if (InvokeMethodType == ReflectionInvokeMethod.Emit 
                    && hashValue.GetMethodHandler == null)
                {
                    var mi = pi.GetGetMethod(true);
                    hashValue.GetMethodHandler = ReflectionFactoryInvoker.GetMethodInvoker(mi, false);
                    return hashValue.GetMethodHandler(source, paramObject, null);
                }
                return pi.GetValue(source, paramObject);
            }
            return null;
        }

        private void SetSingleProperty(object source, Type sourceType, string propertyName, Tuple<Type, object>[] parameters, object value)
        {
            var isEmptyPropertyName = string.IsNullOrEmpty(propertyName);
            var parametersCount = parameters != null ? parameters.Length : 0;
            if (isEmptyPropertyName && parametersCount == 0) return;
            var type = source != null ? source.GetType() : sourceType;
            Type[] paramTypes;
            object[] paramObject;
            GetReflectionParameters(parameters, out paramTypes, out paramObject);
            var hashValue = GetPropertyInfo(type, propertyName, paramTypes);
            if (hashValue != null && hashValue.SetMethodHandler != null)
            {
                hashValue.SetMethodHandler(source, paramObject, value);
                return;
            }
            var pi = hashValue != null ? (PropertyInfo)hashValue.MemberInfo : null;
            if (pi == null || !pi.CanWrite) return;
            if (InvokeMethodType == ReflectionInvokeMethod.Emit
                    && hashValue.SetMethodHandler == null)
            {
                var mi = pi.GetSetMethod(true);
                hashValue.SetMethodHandler = ReflectionFactoryInvoker.GetMethodInvoker(mi, true);
                hashValue.SetMethodHandler(source, paramObject, value);
                return;
            }
            pi.SetValue(source, value, paramObject);
        }

        private static void AddMemberInfoToCache(MemberInfoHashKey key, MemberInfoHashValue value)
        {
            if (CacheLimit <= 0) return;
            lock (m_syncMember)
            {
                if (m_reflectionCache.Count >= CacheLimit)
                {
                    m_reflectionCache.Clear();
                }
                if (!m_reflectionCache.ContainsKey(key))
                {
                    m_reflectionCache.Add(key, value);
                }
            }
        }

        private static bool IsEqualParameters(ParameterInfo[] parameterInfos, Type[] parameterTypes)
        {
            var cnt = parameterInfos != null ? parameterInfos.Length : 0;
            var otherCnt = parameterTypes != null ? parameterTypes.Length : 0;
            if (cnt != otherCnt) return false;
            for (var i = 0; i < cnt; i++)
            {
                if (!parameterInfos[i].ParameterType.Equals(parameterTypes[i])) return false;
            }
            return true;
        }

        #endregion
    }
}