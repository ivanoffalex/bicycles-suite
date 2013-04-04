using System;
using System.Diagnostics;
using System.Reflection;

namespace BicyclesSuite.Shared.Core
{
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    internal sealed class MemberInfoHashValue
    {
        public readonly MemberInfo MemberInfo;
        public Func<object, object[], object, object> SetMethodHandler = null;
        public Func<object, object[], object, object> GetMethodHandler = null;

        public MemberInfoHashValue(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }
    }
}
