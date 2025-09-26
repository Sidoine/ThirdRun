namespace MonogameRPG.Monsters
{
    /// <summary>
    /// Template for creating monster types with predefined characteristics
    /// </summary>
    public class MonsterTemplate
    {
        public string Name { get; }
        public string Description { get; }
        public string ImagePath { get; }
        public int BaseHealth { get; }
        public int BaseAttack { get; }
        public int Level { get; }

        public MonsterTemplate(string name, string description, string imagePath, int baseHealth, int baseAttack, int level = 1)
        {
            Name = name;
            Description = description;
            ImagePath = imagePath;
            BaseHealth = baseHealth;
            BaseAttack = baseAttack;
            Level = level;
        }

        /// <summary>
        /// Create a MonsterType from this template
        /// </summary>
        public MonsterType ToMonsterType()
        {
            return new MonsterType(Name, BaseHealth, BaseAttack, ImagePath, Level);
        }
    }
}