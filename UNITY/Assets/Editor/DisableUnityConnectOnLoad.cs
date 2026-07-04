using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PathOfTenThousandWays.Editor
{
    [InitializeOnLoad]
    public static class DisableUnityConnectOnLoad
    {
        private const string ProjectSettingsDirectory = "ProjectSettings";
        private const string UnityConnectSettingsFile = "UnityConnectSettings.asset";
        private const string ProjectSettingsFile = "ProjectSettings.asset";

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
            EditorApplication.delayCall += EnforceOfflineUnityConnectSettings;
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
    }
}
