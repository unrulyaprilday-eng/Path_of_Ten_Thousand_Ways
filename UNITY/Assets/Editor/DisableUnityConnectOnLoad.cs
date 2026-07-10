using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PathOfTenThousandWays.Editor
{
    [InitializeOnLoad]
    public static class DisableUnityConnectOnLoad
    {
        private const string ProjectSettingsDirectory = "ProjectSettings";
        private const string UserSettingsDirectory = "UserSettings";
        private const string LayoutsDirectory = "Layouts";
        private const string UnityConnectSettingsFile = "UnityConnectSettings.asset";
        private const string ProjectSettingsFile = "ProjectSettings.asset";
        private const string UnityConnectTypeName = "UnityEditor.Connect.UnityConnect";
        private const string LogEntriesTypeName = "UnityEditor.LogEntries";
        private const string LogEntryTypeName = "UnityEditor.LogEntry";
        private const string TokenExchangeExceptionName = "UnityConnectWebRequestException";
        private const string TokenExchangeMessage = "Token Exchange failed";
        private const string TokenExchangeStackNamespace = "UnityEditor.Connect.TokenExchange";
        private const double StartupMaintenanceSeconds = 12.0d;

        private static double startupMaintenanceEndsAt;

        private static readonly string[] UnityConnectBooleanMembers =
        {
            "enabled",
            "cloudEnabled",
            "analyticsEnabled",
            "m_Enabled",
            "m_CloudEnabled"
        };

        private static readonly string[] UnityConnectDisableMethods =
        {
            "SetConnectEnabled",
            "SetCloudEnabled",
            "SetCloudServicesEnabled",
            "SetServicesEnabled"
        };

        private static readonly Dictionary<string, string> UnityConnectOfflineScalars = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "m_Enabled", "0" },
            { "m_TestMode", "0" },
            { "m_InitializeOnStartup", "0" },
            { "m_EnableCloudDiagnosticsReporting", "0" },
            { "m_CaptureEditorExceptions", "0" },
            { "m_PackageRequiringCoreStatsPresent", "0" }
        };

        private static readonly Dictionary<string, string> ProjectOfflineScalars = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "submitAnalytics", "0" },
            { "cloudServicesEnabled", "{}" },
            { "cloudProjectId", string.Empty },
            { "organizationId", string.Empty },
            { "cloudEnabled", "0" }
        };

        static DisableUnityConnectOnLoad()
        {
            InstallUnityConnectLogFilter();
            EnforceOfflineUnityConnectSettings();
            TryDisableLiveUnityConnectInstance();
            EditorApplication.delayCall += EnforceOfflineUnityConnectSettings;
            EditorApplication.delayCall += TryDisableLiveUnityConnectInstance;
            EditorApplication.delayCall += StartStartupUnityConnectMaintenance;
        }

        private static void EnforceOfflineUnityConnectSettings()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            if (string.IsNullOrEmpty(projectRoot))
            {
                return;
            }

            TrySetYamlScalars(Path.Combine(projectRoot, ProjectSettingsDirectory, UnityConnectSettingsFile), UnityConnectOfflineScalars);
            TrySetYamlScalars(Path.Combine(projectRoot, ProjectSettingsDirectory, ProjectSettingsFile), ProjectOfflineScalars);
            TryHideServicesToolbarOverlays(Path.Combine(projectRoot, UserSettingsDirectory, LayoutsDirectory));
        }

        private static void TryHideServicesToolbarOverlays(string layoutsDirectory)
        {
            if (!Directory.Exists(layoutsDirectory))
            {
                return;
            }

            try
            {
                foreach (string layoutFile in Directory.GetFiles(layoutsDirectory, "*.dwlt"))
                {
                    string text = File.ReadAllText(layoutFile);
                    string updated = Regex.Replace(
                        text,
                        @"(?m)(^\s*displayed:\s*)1(\r?\n\s*id:\s*Services/)",
                        "${1}0${2}");

                    if (!string.Equals(text, updated, StringComparison.Ordinal))
                    {
                        File.WriteAllText(layoutFile, updated);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not hide Unity Services toolbar overlays: " + exception.Message);
            }
        }

        private static void TrySetYamlScalars(string path, IReadOnlyDictionary<string, string> scalarValues)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(path);
                bool changed = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmed = lines[i].TrimStart();
                    int colonIndex = trimmed.IndexOf(':');
                    if (colonIndex <= 0)
                    {
                        continue;
                    }

                    string key = trimmed.Substring(0, colonIndex);
                    if (!scalarValues.TryGetValue(key, out string value))
                    {
                        continue;
                    }

                    string indent = lines[i].Substring(0, lines[i].Length - trimmed.Length);
                    string nextLine = indent + key + ": " + value;
                    if (string.Equals(lines[i], nextLine, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    lines[i] = nextLine;
                    changed = true;
                }

                if (changed)
                {
                    File.WriteAllLines(path, lines);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not enforce local UnityConnect settings: " + exception.Message);
            }
        }

        private static void TryDisableLiveUnityConnectInstance()
        {
            try
            {
                Type unityConnectType = typeof(EditorApplication).Assembly.GetType(UnityConnectTypeName, false);
                if (unityConnectType == null)
                {
                    return;
                }

                object instance =
                    GetStaticMemberValue(unityConnectType, "instance") ??
                    GetStaticMemberValue(unityConnectType, "Instance");
                if (instance == null)
                {
                    return;
                }

                foreach (string memberName in UnityConnectBooleanMembers)
                {
                    TrySetBooleanMember(instance, memberName, false);
                }

                foreach (string methodName in UnityConnectDisableMethods)
                {
                    TryInvokeBooleanMethod(instance, methodName, false);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Could not disable live UnityConnect instance: " + exception.Message);
            }
        }

        private static object GetStaticMemberValue(Type type, string memberName)
        {
            const BindingFlags bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;

            try
            {
                PropertyInfo property = type.GetProperty(memberName, bindingFlags);
                if (property != null)
                {
                    return property.GetValue(null, null);
                }

                FieldInfo field = type.GetField(memberName, bindingFlags);
                if (field != null)
                {
                    return field.GetValue(null);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static void TrySetBooleanMember(object target, string memberName, bool value)
        {
            const BindingFlags bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

            try
            {
                Type type = target.GetType();
                PropertyInfo property = type.GetProperty(memberName, bindingFlags);
                if (property != null && property.CanWrite && property.PropertyType == typeof(bool))
                {
                    property.SetValue(target, value, null);
                    return;
                }

                FieldInfo field = type.GetField(memberName, bindingFlags);
                if (field != null && field.FieldType == typeof(bool))
                {
                    field.SetValue(target, value);
                }
            }
            catch
            {
                // Unity Connect internals differ between editor versions; missing or read-only members are harmless.
            }
        }

        private static void TryInvokeBooleanMethod(object target, string methodName, bool value)
        {
            const BindingFlags bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

            try
            {
                MethodInfo method = target.GetType().GetMethod(methodName, bindingFlags, null, new[] { typeof(bool) }, null);
                if (method != null)
                {
                    method.Invoke(target, new object[] { value });
                }
            }
            catch
            {
                // Same version-tolerant behavior as TrySetBooleanMember.
            }
        }

        private static void InstallUnityConnectLogFilter()
        {
            ILogHandler currentHandler = Debug.unityLogger.logHandler;
            if (currentHandler is UnityConnectTokenExchangeLogFilter)
            {
                return;
            }

            Debug.unityLogger.logHandler = new UnityConnectTokenExchangeLogFilter(currentHandler);
        }

        private static void StartStartupUnityConnectMaintenance()
        {
            startupMaintenanceEndsAt = EditorApplication.timeSinceStartup + StartupMaintenanceSeconds;
            EditorApplication.update -= RunStartupUnityConnectMaintenance;
            EditorApplication.update += RunStartupUnityConnectMaintenance;
        }

        private static void RunStartupUnityConnectMaintenance()
        {
            InstallUnityConnectLogFilter();
            TryDisableLiveUnityConnectInstance();
            TryClearTokenExchangeConsoleEntries();

            if (EditorApplication.timeSinceStartup >= startupMaintenanceEndsAt)
            {
                EditorApplication.update -= RunStartupUnityConnectMaintenance;
            }
        }

        private static void TryClearTokenExchangeConsoleEntries()
        {
            try
            {
                Assembly editorAssembly = typeof(EditorApplication).Assembly;
                Type logEntriesType = editorAssembly.GetType(LogEntriesTypeName, false);
                Type logEntryType = editorAssembly.GetType(LogEntryTypeName, false);
                if (logEntriesType == null || logEntryType == null)
                {
                    return;
                }

                MethodInfo getCountMethod = logEntriesType.GetMethod("GetCount", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo getEntryMethod = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo clearMethod = logEntriesType.GetMethod("Clear", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (getCountMethod == null || getEntryMethod == null || clearMethod == null)
                {
                    return;
                }

                MethodInfo startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                object entry = Activator.CreateInstance(logEntryType);
                int count = Convert.ToInt32(getCountMethod.Invoke(null, null));
                bool foundTokenExchangeNoise = false;

                try
                {
                    startGettingEntriesMethod?.Invoke(null, null);

                    for (int i = 0; i < count; i++)
                    {
                        object[] arguments = { i, entry };
                        object result = getEntryMethod.Invoke(null, arguments);
                        if (result is bool entryRead && !entryRead)
                        {
                            continue;
                        }

                        if (ShouldSuppressUnityConnectTokenExchange(GetLogEntryText(entry)))
                        {
                            foundTokenExchangeNoise = true;
                            break;
                        }
                    }
                }
                finally
                {
                    endGettingEntriesMethod?.Invoke(null, null);
                }

                if (foundTokenExchangeNoise)
                {
                    clearMethod.Invoke(null, null);
                }
            }
            catch
            {
                // Console APIs are internal and version-specific; failure should never affect gameplay code.
            }
        }

        private static string GetLogEntryText(object entry)
        {
            if (entry == null)
            {
                return string.Empty;
            }

            return string.Join(
                "\n",
                GetMemberText(entry, "condition"),
                GetMemberText(entry, "message"),
                GetMemberText(entry, "stackTrace"));
        }

        private static string GetMemberText(object target, string memberName)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            Type type = target.GetType();
            FieldInfo field = type.GetField(memberName, bindingFlags);
            if (field != null)
            {
                return Convert.ToString(field.GetValue(target)) ?? string.Empty;
            }

            PropertyInfo property = type.GetProperty(memberName, bindingFlags);
            if (property != null)
            {
                return Convert.ToString(property.GetValue(target, null)) ?? string.Empty;
            }

            return string.Empty;
        }

        private static bool ShouldSuppressUnityConnectTokenExchange(string text)
        {
            return !string.IsNullOrEmpty(text) &&
                   text.IndexOf(TokenExchangeExceptionName, StringComparison.Ordinal) >= 0 &&
                   text.IndexOf(TokenExchangeMessage, StringComparison.Ordinal) >= 0 &&
                   text.IndexOf(TokenExchangeStackNamespace, StringComparison.Ordinal) >= 0;
        }

        private sealed class UnityConnectTokenExchangeLogFilter : ILogHandler
        {
            private readonly ILogHandler innerHandler;

            public UnityConnectTokenExchangeLogFilter(ILogHandler innerHandler)
            {
                this.innerHandler = innerHandler;
            }

            public void LogException(Exception exception, UnityEngine.Object context)
            {
                if (ShouldSuppressUnityConnectTokenExchange(exception?.ToString()))
                {
                    return;
                }

                innerHandler?.LogException(exception, context);
            }

            public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
                if (ShouldSuppressUnityConnectTokenExchange(FormatMessage(format, args)))
                {
                    return;
                }

                innerHandler?.LogFormat(logType, context, format, args);
            }

            private static string FormatMessage(string format, object[] args)
            {
                if (args == null || args.Length == 0)
                {
                    return format;
                }

                try
                {
                    return string.Format(format, args);
                }
                catch (FormatException)
                {
                    return format;
                }
            }
        }
    }
}