using System;
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy;

namespace DivertR.DynamicProxy
{
    public class CustomProxyGenerator : ProxyGenerator
    {
        public TTarget CreateClassProxyWithRootFields<TTarget>(TTarget root, params IInterceptor[] interceptors) where TTarget : class?
        {
            var hasEmptyConstructor = typeof(TTarget).GetConstructor(Type.EmptyTypes) != null;

            if (hasEmptyConstructor || !TryCreateUninitializedClassProxy(interceptors, out TTarget proxy))
            {
                proxy = CreateClassProxy<TTarget>(interceptors);
            }
            
            CopyFields(root, proxy);

            return proxy;
        }
        
        private bool TryCreateUninitializedClassProxy<TTarget>(IInterceptor[] interceptors, out TTarget proxy) where TTarget : class?
        {
            var proxyType = CreateClassProxyType(typeof(TTarget), null, ProxyGenerationOptions.Default);
            proxy = (TTarget) FormatterServices.GetUninitializedObject(proxyType);
            
            return TrySetInterceptor(proxy, interceptors);
        }

        private static bool TrySetInterceptor(object proxy, IInterceptor[] interceptors)
        {
            var field = proxy.GetType().GetField("__interceptors", BindingFlags.Instance | BindingFlags.NonPublic);
            
            if (field == null)
            {
                return false;
            }

            field.SetValue(proxy, interceptors);

            return true;
        }
        
        private static void CopyFields<TTarget>(TTarget src, TTarget dest)
        {
            const BindingFlags FieldFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            
            var currentType = typeof(TTarget);

            while (currentType != null && currentType != typeof(object))
            {
                foreach (var field in currentType.GetFields(FieldFlags))
                {
                    var fieldValue = field.GetValue(src);
                    field.SetValue(dest, fieldValue);
                }
                
                currentType = currentType.BaseType;
            }
        }
    }
}