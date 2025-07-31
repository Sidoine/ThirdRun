using System;

namespace ThirdRun.Items
{
    public class Equipment : Item
    {
        public int BonusStats { get; set; }

        public Equipment(string name, string description, int value, int bonusStats, int itemLevel = 1) : base(name, description, value, itemLevel)
        {
            BonusStats = bonusStats;
        }

        public void Equip(Character character)
        {
            character.AttackPower += BonusStats; // Example of applying bonus stats
        }
    }
}