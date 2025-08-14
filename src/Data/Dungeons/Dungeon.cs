using System.Collections.Generic;

namespace ThirdRun.Data.Dungeons
{
    public class Dungeon
    {
        public string Name { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public List<DungeonMapDefinition> Maps { get; set; }

        public Dungeon(string name, int minLevel, int maxLevel, List<DungeonMapDefinition> maps)
        {
            Name = name;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            Maps = maps;
        }

        public bool IsAppropriateForLevel(int level)
        {
            return level >= MinLevel && level <= MaxLevel;
        }
    }

    public class DungeonMapDefinition
    {
        public string Description { get; set; }
        public bool HasBoss { get; set; }
        public int MonsterCount { get; set; }
        public int MinMonsterLevel { get; set; }
        public int MaxMonsterLevel { get; set; }

        public DungeonMapDefinition(string description, bool hasBoss, int monsterCount, int minMonsterLevel, int maxMonsterLevel)
        {
            Description = description;
            HasBoss = hasBoss;
            MonsterCount = monsterCount;
            MinMonsterLevel = minMonsterLevel;
            MaxMonsterLevel = maxMonsterLevel;
        }
    }
}