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
    /// Assembly Info
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Assembly = {AssemblyName}")]
    public sealed class AssemblyInfo
    {
        internal AssemblyInfo(Assembly assembly, TypeInfo[] excludedTypes)
        {
            if (assembly == null)
            {
                throw new NullReferenceException();
            }
            Assembly = assembly;
            ExcludedTypes = excludedTypes ?? new TypeInfo[0];
        }

        /// <summary>
        /// Assembly instance
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Assembly name
        /// </summary>
        public string AssemblyName
        {
            get
            {
                return Assembly.GetUniqueName();
            }
        }

        /// <summary>
        /// Excluded types
        /// </summary>
        public TypeInfo[] ExcludedTypes { get; private set; }

        /// <summary>
        /// Equals 
        /// </summary>
        /// <param name="obj">Equals object</param>
        /// <returns>Is equal</returns>
        public override bool Equals(object obj)
        {
            return obj is AssemblyInfo ? AssemblyName == ((AssemblyInfo)obj).AssemblyName : false;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return AssemblyName.GetHashCode();
        }

        /// <summary>
        /// operator== implementation
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns>Is equals flag</returns>
        public static bool operator ==(AssemblyInfo x, AssemblyInfo y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// operator!= implementation
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns>Is equals flag</returns>
        public static bool operator !=(AssemblyInfo x, AssemblyInfo y)
        {
            return !(x == y);
        }
    }
}
