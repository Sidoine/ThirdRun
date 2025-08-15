using System.Collections.Generic;
using System.Linq;
using MonogameRPG.Monsters;

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

    public class MonsterSpawn
    {
        public MonsterType MonsterType { get; set; }
        public int Count { get; set; }
        public bool IsBoss { get; set; }

        public MonsterSpawn(MonsterType monsterType, int count, bool isBoss = false)
        {
            MonsterType = monsterType;
            Count = count;
            IsBoss = isBoss;
        }
    }

    public class DungeonMapDefinition
    {
        public string Description { get; set; }
        public List<MonsterSpawn> MonsterSpawns { get; set; }

        public DungeonMapDefinition(string description, List<MonsterSpawn> monsterSpawns)
        {
            Description = description;
            MonsterSpawns = monsterSpawns;
        }

        // Helper property to check if this map has any boss monsters
        public bool HasBoss => MonsterSpawns.Any(spawn => spawn.IsBoss);
    }
}