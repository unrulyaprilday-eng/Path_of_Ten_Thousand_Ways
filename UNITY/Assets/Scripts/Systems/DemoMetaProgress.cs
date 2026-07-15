using System;
using System.Collections.Generic;

namespace PathOfTenThousandWays.Demo.Systems
{
    [Serializable]
    public sealed class DemoRunSummary
    {
        public bool Victory;
        public bool DefeatedBoss;
        public int ReachedLayer;
        public int BattlesWon;
        public int MaxSwordCount;
        public int HighestBurstDamage;
        public string MainGongfaName = string.Empty;
        public string CoreArtifactName = string.Empty;
        public readonly List<string> CoreComponents = new List<string>();
        public readonly List<string> NewUnlocks = new List<string>();
    }

    [Serializable]
    public sealed class DemoMetaProgress
    {
        public const int CurrentVersion = 1;
        public const string BrokenSwordTraceId = "trace_broken_sword";

        public int Version = CurrentVersion;
        public int CompletedRuns;
        public int BossVictories;
        public List<string> UnlockedIds = new List<string>();
        public List<string> SeenCardIds = new List<string>();
        public List<string> SeenGongfaIds = new List<string>();
        public List<string> SeenArtifactIds = new List<string>();

        public bool HasUnlock(string unlockId)
        {
            return !string.IsNullOrEmpty(unlockId) && UnlockedIds.Contains(unlockId);
        }

        public bool RecordRun(DemoRunSummary summary)
        {
            if (summary == null)
            {
                return false;
            }

            CompletedRuns++;
            if (!summary.DefeatedBoss)
            {
                return false;
            }

            BossVictories++;
            if (HasUnlock(BrokenSwordTraceId))
            {
                return false;
            }

            UnlockedIds.Add(BrokenSwordTraceId);
            summary.NewUnlocks.Add(BrokenSwordTraceId);
            return true;
        }

        public void RecordCard(string cardId)
        {
            AddUnique(SeenCardIds, cardId);
        }

        public void RecordGongfa(string gongfaId)
        {
            AddUnique(SeenGongfaIds, gongfaId);
        }

        public void RecordArtifact(string artifactId)
        {
            AddUnique(SeenArtifactIds, artifactId);
        }

        public void Normalize()
        {
            Version = CurrentVersion;
            UnlockedIds = UnlockedIds ?? new List<string>();
            SeenCardIds = SeenCardIds ?? new List<string>();
            SeenGongfaIds = SeenGongfaIds ?? new List<string>();
            SeenArtifactIds = SeenArtifactIds ?? new List<string>();
        }

        private static void AddUnique(ICollection<string> destination, string value)
        {
            if (destination == null || string.IsNullOrEmpty(value) || destination.Contains(value))
            {
                return;
            }

            destination.Add(value);
        }
    }

    public interface IDemoMetaProgressStore
    {
        DemoMetaProgress Load();
        void Save(DemoMetaProgress progress);
    }

    public sealed class DemoMemoryMetaProgressStore : IDemoMetaProgressStore
    {
        private DemoMetaProgress progress = new DemoMetaProgress();

        public DemoMetaProgress Load()
        {
            progress.Normalize();
            return progress;
        }

        public void Save(DemoMetaProgress value)
        {
            progress = value ?? new DemoMetaProgress();
            progress.Normalize();
        }
    }
}
