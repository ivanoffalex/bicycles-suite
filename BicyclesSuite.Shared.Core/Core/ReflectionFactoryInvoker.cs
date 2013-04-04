using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace BicyclesSuite.Shared.Core
{
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    internal static class ReflectionFactoryInvoker
    {
        public static Func<object, object[], object, object> GetMethodInvoker(MethodBase methodBase, bool isSetter)
        {
            //Specify return type for generated method
            var returnType = methodBase is ConstructorInfo
                                 ? methodBase.ReflectedType
                                 : ((MethodInfo) methodBase).ReturnType;
            //Create dynamic method
            var method = new DynamicMethod(string.Empty, typeof(object), 
                new [] { typeof(object), typeof(object[]), typeof(object) }, 
                methodBase.DeclaringType.Module, true);

            //Get IL code generator
            var il = method.GetILGenerator();

            //Prepare array of method args
            var ps = methodBase.GetParameters();
            var paramTypesCount = ps.Length;
            var paramTypes = new Type[paramTypesCount];
            for (var i = 0; i < paramTypesCount; i++)
            {
                //Get parameter type by ref or directly
                paramTypes[i] = ps[i].ParameterType.IsByRef 
                    ? ps[i].ParameterType.GetElementType() 
                    : ps[i].ParameterType;
            }

            //Declare locals. 
            var locals = new LocalBuilder[paramTypesCount];
            for (var i = 0; i < paramTypesCount; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }

            //Emit load to evaluation stack from "args" and "value"
            for (var i = 0; i < paramTypesCount; i++)
            {
                //In case setter mode and last args of callin method load from "value"
                if (isSetter && i == paramTypesCount - 1)
                {
                    //Load from "value"
                    il.Emit(OpCodes.Ldarg_2);
                }
                else
                {
                    //Load from "args" and get [i] value
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                }

                //Unbox or Cast parameter value
                var type = paramTypes[i];
                if (type.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, type);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, type);
                }

                //Pop to local variable
                il.Emit(OpCodes.Stloc, locals[i]);
            }

            //Check for non static methods to load "source"
            if (!methodBase.IsStatic && methodBase is MethodInfo)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            //Load "args" from locals
            for (var i = 0; i < paramTypesCount; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                else
                    il.Emit(OpCodes.Ldloc, locals[i]);
            }

            //Choose to call constructor, static or instance method
            if (methodBase is ConstructorInfo)
            {
                il.Emit(OpCodes.Newobj, (ConstructorInfo)methodBase);
            }
            else if (methodBase.IsStatic)
            {
                il.EmitCall(OpCodes.Call, (MethodInfo)methodBase, null);
            }
            else
            {
                il.EmitCall(OpCodes.Callvirt, (MethodInfo)methodBase, null);
            }

            //Proceed return value (in case void - return null)
            if (returnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            //Box value type
            else if (returnType.IsValueType)
            {
                il.Emit(OpCodes.Box, returnType);
            }
            //Process referenced type to output
            for (var i = 0; i < paramTypesCount; i++)
            {
                if (!ps[i].ParameterType.IsByRef) continue;
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldloc, locals[i]);
                if (locals[i].LocalType.IsValueType)
                    il.Emit(OpCodes.Box, locals[i].LocalType);
                il.Emit(OpCodes.Stelem_Ref);
            }
            //Return instruction
            il.Emit(OpCodes.Ret);
            //Create and return delegate if InvokeMethodBaseHandler
            return (Func<object, object[], object, object>)method.CreateDelegate(typeof(Func<object, object[], object, object>));
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }
    }
}
