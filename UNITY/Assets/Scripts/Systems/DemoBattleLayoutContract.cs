using System;

namespace PathOfTenThousandWays.Demo.Systems
{
    public struct DemoNormalizedPoint
    {
        public float X;
        public float Y;

        public DemoNormalizedPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public struct DemoNormalizedRect
    {
        public float MinX;
        public float MinY;
        public float MaxX;
        public float MaxY;

        public DemoNormalizedRect(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public bool Contains(DemoNormalizedPoint point)
        {
            return point.X >= MinX && point.X <= MaxX && point.Y >= MinY && point.Y <= MaxY;
        }
    }

    public enum DemoBattleVisualTier
    {
        Minor,
        Elite,
        MiniBoss,
        FinalBoss
    }

    public static class DemoBattleLayoutContract
    {
        public static readonly DemoNormalizedRect CommandSurface = new DemoNormalizedRect(0.34f, 0.02f, 0.78f, 0.24f);
        public static readonly DemoNormalizedRect EnergyGrowthTrack = new DemoNormalizedRect(0.39f, 0.26f, 0.66f, 0.30f);
        public static readonly DemoNormalizedRect PlayerSafeArea = new DemoNormalizedRect(0.12f, 0.32f, 0.34f, 0.72f);
        public static readonly DemoNormalizedRect EnemySafeArea = new DemoNormalizedRect(0.57f, 0.42f, 0.93f, 0.82f);
        public static readonly DemoNormalizedRect AttackCorridor = new DemoNormalizedRect(0.30f, 0.35f, 0.72f, 0.75f);

        public static DemoNormalizedPoint PlayerAnchor(bool openingBattle)
        {
            return openingBattle
                ? new DemoNormalizedPoint(0.24f, 0.50f)
                : new DemoNormalizedPoint(0.24f, 0.47f);
        }

        public static DemoNormalizedPoint BearerAnchor(bool openingBattle)
        {
            return openingBattle
                ? new DemoNormalizedPoint(0.24f, 0.375f)
                : new DemoNormalizedPoint(0.24f, 0.36f);
        }

        public static DemoNormalizedPoint AutoAttackAnchor(bool openingBattle)
        {
            DemoNormalizedPoint bearer = BearerAnchor(openingBattle);
            return new DemoNormalizedPoint(bearer.X + 0.095f, bearer.Y);
        }

        public static DemoNormalizedPoint EnemyAnchor(DemoBattleVisualTier tier, int depthIndex = 1)
        {
            if (tier == DemoBattleVisualTier.FinalBoss)
            {
                return new DemoNormalizedPoint(0.58f, 0.72f);
            }

            int depth = Math.Max(0, Math.Min(2, depthIndex));
            return new DemoNormalizedPoint(
                0.63f + depth * 0.105f,
                0.56f + depth * 0.08f);
        }

        public static bool IsStable()
        {
            DemoNormalizedPoint player = PlayerAnchor(false);
            DemoNormalizedPoint bearer = BearerAnchor(false);
            DemoNormalizedPoint autoAttack = AutoAttackAnchor(false);
            DemoNormalizedPoint enemy = EnemyAnchor(DemoBattleVisualTier.Minor);
            return PlayerSafeArea.Contains(player)
                && PlayerSafeArea.Contains(bearer)
                && AttackCorridor.Contains(autoAttack)
                && EnemySafeArea.Contains(enemy)
                && CommandSurface.MaxY < EnergyGrowthTrack.MinY;
        }
    }
}
