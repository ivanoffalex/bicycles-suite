using System;
using System.Diagnostics;
using System.Text;

namespace BicyclesSuite.Shared.Core
{
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    internal sealed class MemberInfoHashKey : IEquatable<MemberInfoHashKey>
    {
        public MemberInfoHashKey(Type type, string memberName, Type[] types, int genericParameterCount)
        {
            Type = type;
            MemberName = memberName;
            if (Type == null || string.IsNullOrEmpty(MemberName))
            {
                throw new NullReferenceException();
            }
            Parameters = types ?? new Type[0];
            GenericParameterCount = (short)genericParameterCount;
        }

        private readonly Type Type;
        private readonly string MemberName;
        private readonly Type[] Parameters;
        private readonly short GenericParameterCount;
        
        #region IEquatable<MemberInfoHashKey> Members

        public bool Equals(MemberInfoHashKey other)
        {
            return Parameters.IsEqualArrayTo(other.Parameters)
                   && other.MemberName == MemberName
                   && other.Type == Type
                   && other.GenericParameterCount == GenericParameterCount;
        }

        #endregion

        public override string ToString()
        {
            var cnt = Parameters != null ? Parameters.Length : 0;
            var args = new StringBuilder();
            for (var i = 0; i < cnt; i++)
            {
                args.Append(Parameters[i].FullName);
            }
            return string.Format("{0}.{1}{2}{3}", Type.FullName, MemberName,
                                 args,
                                 GenericParameterCount > 0 ? "`" + GenericParameterCount : string.Empty);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((MemberInfoHashKey)obj);
        }

        public override int GetHashCode()
        {
            var hash = Type.GetHashCode() ^ MemberName.GetHashCode() ^ GenericParameterCount.GetHashCode();
            var cnt = Parameters.Length;
            for (var i = 0; i < cnt; i++)
            {
                hash ^= Parameters[i].GetHashCode();
            }
            return hash;
        }
    }
}
