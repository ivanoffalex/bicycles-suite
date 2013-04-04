using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BicyclesSuite.Shared.Reflection
{
    /// <summary>
    /// Type Info
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public sealed class TypeInfo
    {
        internal TypeInfo(string typeName, string assemblyFullName)
        {
            if (string.IsNullOrEmpty(assemblyFullName)
                || string.IsNullOrEmpty(typeName))
            {
                throw new NullReferenceException();
            }
            TypeName = typeName;
            AssemblyFullName = assemblyFullName;
        }

        /// <summary>
        /// Get unique System.Type name
        /// </summary>
        /// <returns>Type unique name</returns>
        public string UniqueName
        {
            get
            {
                return ReflectionFactory.FormatUniqueName(TypeName, new AssemblyName(AssemblyFullName));
            }
        }

        /// <summary>
        /// Full type name
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Assembly full name
        /// </summary>
        public string AssemblyFullName { get; private set; }

        /// <summary>
        /// Assembly inforamtion
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("TypeName = \"{0}\", AssemblyName = \"{1}\"", TypeName, AssemblyFullName);
        }

        /// <summary>
        /// Equals 
        /// </summary>
        /// <param name="obj">Equals object</param>
        /// <returns>Is equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is TypeInfo)
            {
                var value = (TypeInfo)obj;
                return TypeName == value.TypeName && AssemblyFullName == value.AssemblyFullName;
            }
            return false;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return string.Format("{0}{1}", TypeName, AssemblyFullName).GetHashCode();
        }

        /// <summary>
        /// operator== implementation
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns>Is equals flag</returns>
        public static bool operator ==(TypeInfo x, TypeInfo y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// operator!= implementation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Is not equals flag</returns>
        public static bool operator !=(TypeInfo x, TypeInfo y)
        {
            return !(x == y);
        }
    }
}
