using System;
using System.Collections.Generic;

namespace PathOfTenThousandWays.Demo.Systems
{
    [Serializable]
    public sealed class DemoRunNodeSummary
    {
        public string NodeId = string.Empty;
        public string NodeName = string.Empty;
        public string NodeType = string.Empty;
        public int Layer;
        public bool Completed;
        public bool Succeeded;
    }

    [Serializable]
    public sealed class DemoRunRouteSummary
    {
        public string RouteId = string.Empty;
        public string RouteName = string.Empty;
        public int Layer;
        public string Risk = string.Empty;
        public List<DemoRunNodeSummary> NodeSequence = new List<DemoRunNodeSummary>();
    }

    [Serializable]
    public sealed class DemoRunComponentSummary
    {
        public string Id = string.Empty;
        public string DisplayName = string.Empty;
    }

    [Serializable]
    public sealed class DemoRunSummary
    {
        public bool Victory;
        public bool DefeatedBoss;
        public int ReachedLayer;
        public int BattlesWon;
        public int MaxSwordCount;
        public int HighestBurstDamage;
        public float DurationSeconds;
        public string FailureNodeId = string.Empty;
        public string FailureNodeName = string.Empty;
        public string FailureNodeType = string.Empty;
        public string MainGongfaId = string.Empty;
        public string MainGongfaName = string.Empty;
        public string CoreArtifactId = string.Empty;
        public string CoreArtifactName = string.Empty;
        public List<DemoRunRouteSummary> RouteHistory = new List<DemoRunRouteSummary>();
        public List<DemoRunNodeSummary> CompletedNodeHistory = new List<DemoRunNodeSummary>();
        public List<string> CoreComponents = new List<string>();
        public List<DemoRunComponentSummary> CoreComponentDetails = new List<DemoRunComponentSummary>();
        public List<string> NewUnlocks = new List<string>();

        public float TotalDurationSeconds => DurationSeconds;

        public void Normalize()
        {
            FailureNodeId = FailureNodeId ?? string.Empty;
            FailureNodeName = FailureNodeName ?? string.Empty;
            FailureNodeType = FailureNodeType ?? string.Empty;
            MainGongfaId = MainGongfaId ?? string.Empty;
            MainGongfaName = MainGongfaName ?? string.Empty;
            CoreArtifactId = CoreArtifactId ?? string.Empty;
            CoreArtifactName = CoreArtifactName ?? string.Empty;
            RouteHistory = RouteHistory ?? new List<DemoRunRouteSummary>();
            CompletedNodeHistory = CompletedNodeHistory ?? new List<DemoRunNodeSummary>();
            CoreComponents = CoreComponents ?? new List<string>();
            CoreComponentDetails = CoreComponentDetails ?? new List<DemoRunComponentSummary>();
            NewUnlocks = NewUnlocks ?? new List<string>();

            for (int i = 0; i < RouteHistory.Count; i++)
            {
                DemoRunRouteSummary route = RouteHistory[i];
                if (route == null)
                {
                    RouteHistory[i] = new DemoRunRouteSummary();
                    continue;
                }

                route.RouteId = route.RouteId ?? string.Empty;
                route.RouteName = route.RouteName ?? string.Empty;
                route.Risk = route.Risk ?? string.Empty;
                route.NodeSequence = route.NodeSequence ?? new List<DemoRunNodeSummary>();
            }
        }
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

            summary.Normalize();
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
