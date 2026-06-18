using System;

namespace PathOfTenThousandWays.Demo.Combatants
{
    [Serializable]
    public sealed class DemoCombatant
    {
        public string Name;
        public int MaxHealth;
        public int Health;
        public int Block;
        public int SwordIntent;
        public int Shock;
        public int Bleed;

        public bool IsDead => Health <= 0;

        public DemoCombatant(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public int TakeDamage(int amount)
        {
            int blocked = Math.Min(Block, amount);
            Block -= blocked;
            int healthDamage = amount - blocked;
            Health = Math.Max(0, Health - healthDamage);
            return healthDamage;
        }

        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public int TickBleed()
        {
            if (Bleed <= 0)
            {
                return 0;
            }

            int damage = Bleed;
            TakeDamage(damage);
            Bleed = Math.Max(0, Bleed - 1);
            return damage;
        }
    }
}
