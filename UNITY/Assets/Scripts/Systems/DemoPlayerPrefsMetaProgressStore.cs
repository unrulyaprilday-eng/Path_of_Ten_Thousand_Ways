using UnityEngine;

namespace PathOfTenThousandWays.Demo.Systems
{
    public sealed class DemoPlayerPrefsMetaProgressStore : IDemoMetaProgressStore
    {
        private const string StorageKey = "PathOfTenThousandWays.DemoMetaProgress.V1";

        public DemoMetaProgress Load()
        {
            string json = PlayerPrefs.GetString(StorageKey, string.Empty);
            DemoMetaProgress progress = string.IsNullOrEmpty(json)
                ? new DemoMetaProgress()
                : JsonUtility.FromJson<DemoMetaProgress>(json) ?? new DemoMetaProgress();
            progress.Normalize();
            return progress;
        }

        public void Save(DemoMetaProgress progress)
        {
            DemoMetaProgress value = progress ?? new DemoMetaProgress();
            value.Normalize();
            PlayerPrefs.SetString(StorageKey, JsonUtility.ToJson(value));
            PlayerPrefs.Save();
        }
    }
}
