using System.Collections.Generic;

namespace PathOfTenThousandWays.Demo.Battle
{
    public sealed class DemoBattleLog
    {
        private const int MaxLines = 12;
        private readonly Queue<string> lines = new Queue<string>();

        public IEnumerable<string> Lines => lines;

        public void Add(string line)
        {
            lines.Enqueue(line);

            while (lines.Count > MaxLines)
            {
                lines.Dequeue();
            }
        }

        public void Clear()
        {
            lines.Clear();
        }
    }
}
