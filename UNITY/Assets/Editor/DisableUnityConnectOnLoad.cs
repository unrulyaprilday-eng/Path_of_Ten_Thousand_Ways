using System;
using System.Reflection;
using UnityEditor;

namespace PathOfTenThousandWays.Editor
{
    [InitializeOnLoad]
    public static class DisableUnityConnectOnLoad
    {
        static DisableUnityConnectOnLoad()
        {
            EditorApplication.delayCall += TryDisableUnityConnect;
        }

        private static void TryDisableUnityConnect()
        {
            TrySetUnityConnectFlag("UnityEditor.Connect.UnityConnect", "enabled", false);
            TryInvokeStatic("UnityEditor.Connect.UnityConnect", "RequestDisableServiceWindow");
            TryInvokeInstanceMethod("UnityEditor.Connect.UnityConnect", "DisableServices");
        }

        private static void TrySetUnityConnectFlag(string typeName, string propertyName, bool value)
        {
            object instance = GetUnityConnectInstance(typeName);
            if (instance == null)
            {
                return;
            }

            PropertyInfo property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null || !property.CanWrite)
            {
                return;
            }

            try
            {
                property.SetValue(instance, value, null);
            }
            catch
            {
            }
        }

        private static void TryInvokeInstanceMethod(string typeName, string methodName)
        {
            object instance = GetUnityConnectInstance(typeName);
            if (instance == null)
            {
                return;
            }

            MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                return;
            }

            try
            {
                method.Invoke(instance, null);
            }
            catch
            {
            }
        }

        private static void TryInvokeStatic(string typeName, string methodName)
        {
            Type type = Type.GetType(typeName + ", UnityEditor");
            if (type == null)
            {
                return;
            }

            MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                return;
            }

            try
            {
                method.Invoke(null, null);
            }
            catch
            {
            }
        }

        private static object GetUnityConnectInstance(string typeName)
        {
            Type type = Type.GetType(typeName + ", UnityEditor");
            if (type == null)
            {
                return null;
            }

            PropertyInfo instanceProperty = type.GetProperty("instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (instanceProperty == null)
            {
                return null;
            }

            try
            {
                return instanceProperty.GetValue(null, null);
            }
            catch
            {
                return null;
            }
        }
    }
}
