using System.Collections.Generic;
using System.Linq;
using ThirdRun.Characters;
using ThirdRun.Items;
using MonogameRPG.Monsters;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;
using MonogameRPG;
using ThirdRun.Data.Abilities;
using System;

public enum CharacterClass
{
    Guerrier,
    Mage,
    Prêtre,
    Chasseur
}

public class Character : Unit
{
    public string Name { get; set; }
    public CharacterClass Class { get; set; }
    public int Experience { get; private set; }
    public Inventory Inventory { get; private set; }
    public Equipment? Weapon { get; private set; }
    public Equipment? Armor { get; private set; }

    public Character(string name, CharacterClass characterClass, int health, int attackPower, MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) : base(map, worldMap)
    {
        Name = name;
        Class = characterClass;
        Level = 1; // All characters start at level 1

        CurrentHealth = health;
        MaxHealth = health;
        AttackPower = attackPower;
        Experience = 0;
        Inventory = new Inventory() { Owner = this };
        Position = new Vector2(0, 0); // Position initiale
        
        // Add class-specific abilities
        InitializeClassAbilities();
    }

    private void InitializeClassAbilities()
    {
        // All characters keep the default melee attack
        // Add class-specific abilities based on character class
        switch (Class)
        {
            case CharacterClass.Chasseur:
                // Hunters get ranged attack and regeneration abilities
                Abilities.Add(new RangedAttackAbility());
                Abilities.Add(new RegenerationBuffAbility()); // Single target buff
                Characteristics.SetValue(ThirdRun.Data.Characteristic.RangedAttackPower, AttackPower);
                break;
                
            case CharacterClass.Prêtre:
                // Priests get healing abilities and group buffs
                Abilities.Add(new HealAbility());
                Abilities.Add(new AttackPowerBuffAbility()); // Group buff
                Abilities.Add(new SelfHealAbility());
                Characteristics.SetValue(ThirdRun.Data.Characteristic.HealingPower, AttackPower / 2);
                break;
                
            case CharacterClass.Mage:
                // Mages get spell abilities and debuffs
                Abilities.Add(new RangedAttackAbility()); // Use as magic attack for now
                Abilities.Add(new WeaknessDebuffAbility()); // Single target debuff
                Characteristics.SetValue(ThirdRun.Data.Characteristic.RangedAttackPower, AttackPower);
                break;
                
            case CharacterClass.Guerrier:
                // Warriors are melee focused, so just the default attack is fine
                // Maybe they get a self-heal for survivability
                Abilities.Add(new SelfHealAbility());
                Characteristics.SetValue(ThirdRun.Data.Characteristic.HealingPower, AttackPower / 3);
                break;
        }
    }

    public void Move(List<Monster> monsters)
    {
        // Check if in town - if so, use town behavior instead
        if (WorldMap.IsInTown)
        {
            MoveInTown(WorldMap.GetNPCsOnCurrentMap());
            return;
        }
        
        // Trouver le monstre vivant le plus proche
        Monster? closest = null;
        float minDist = float.MaxValue;
        foreach (var monster in monsters)
        {
            if (monster.IsDead) continue;
            float dist = Vector2.Distance(Position, monster.Position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = monster;
            }
        }
        if (closest == null)
        {
            var nextMap = WorldMap.GetAdjacentCardWithMonsters();
            MoveTo(nextMap.Position + new Vector2(Map.GridWidth / 2 * Map.TileWidth, Map.GridHeight / 2 * Map.TileHeight));
            return;
        }

        MoveTo(closest.Position);

        // Si le personnage est assez proche, utilise les abilities
        if (Vector2.Distance(Position, closest.Position) < Map.TileHeight)
        {
            UseAbilities();
        }
    }
    
    private void MoveInTown(List<ThirdRun.Data.NPCs.NPC> npcs)
    {
        if (npcs.Count == 0) return;
        
        // Find nearest NPC or move randomly between NPCs
        var rand = new Random();
        var targetNPC = npcs[rand.Next(npcs.Count)];
        
        // Move towards the selected NPC
        MoveTo(targetNPC.Position);
        
        // If close enough to NPC, maybe pause briefly or find new target
        if (Vector2.Distance(Position, targetNPC.Position) < Map.TileHeight)
        {
            // Could add interaction logic here or just let them wander
        }
    }

    /// <summary>
    /// Handles post-combat effects when a monster is defeated by this character
    /// </summary>
    public void OnMonsterDefeated(Monster monster)
    {
        // Distribute experience equally among all living party members
        DistributeExperienceToParty(monster);
        
        // Ramasser automatiquement le loot du monstre vaincu
        var loot = monster.DropLoot();
        Inventory.AddItem(loot);
    }
    
    /// <summary>
    /// Distributes experience equally among all living party members
    /// </summary>
    private void DistributeExperienceToParty(Monster monster)
    {
        if (WorldMap == null) return;
        
        var allCharacters = WorldMap.GetAllCharacters();
        var livingCharacters = allCharacters.Where(c => !c.IsDead).ToList();
        
        if (livingCharacters.Count == 0) return;
        
        int totalXp = monster.GetExperienceValue();
        int xpPerCharacter = totalXp / livingCharacters.Count;
        
        foreach (var character in livingCharacters)
        {
            character.GainExperienceDirectly(xpPerCharacter);
        }
    }
    
    /// <summary>
    /// Gains experience directly (used for party XP distribution)
    /// </summary>
    private void GainExperienceDirectly(int xp)
    {
        Experience += xp;
        
        // Check for level up
        while (Experience >= GetTotalExperienceRequiredForLevel(Level + 1))
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Called when this character defeats another unit
    /// </summary>
    protected override void OnTargetDefeated(Unit target)
    {
        if (target is Monster monster)
        {
            OnMonsterDefeated(monster);
        }
    }

    public void GainExperience(Monster monster)
    {
        int xpGained = monster.GetExperienceValue();
        Experience += xpGained;
        
        // Check for level up
        while (Experience >= GetTotalExperienceRequiredForLevel(Level + 1))
        {
            LevelUp();
        }
    }
    
    /// <summary>
    /// Calculates the total experience required to reach a specific level
    /// </summary>
    private int GetTotalExperienceRequiredForLevel(int level)
    {
        int totalXp = 0;
        for (int i = 1; i < level; i++)
        {
            totalXp += 10 * 10 * i; // 100 XP * level
        }
        return totalXp;
    }
    
    /// <summary>
    /// Calculates the experience required to reach the next level from current level
    /// Formula: 10 * monster XP worth * current level
    /// </summary>
    public int GetExperienceRequiredForNextLevel()
    {
        // Base monster XP is 10, so 10 monsters worth of XP per level
        return 10 * 10 * Level; // 100 XP * current level
    }
    
    /// <summary>
    /// Levels up the character and increases base characteristics
    /// </summary>
    private void LevelUp()
    {
        Level++;
        
        // Increase base characteristics based on class
        int healthIncrease = GetHealthIncreasePerLevel();
        int attackIncrease = GetAttackIncreasePerLevel();
        
        // Increase max health and current health
        MaxHealth += healthIncrease;
        CurrentHealth += healthIncrease; // Heal when leveling up
        
        // Increase attack power
        AttackPower += attackIncrease;
    }
    
    /// <summary>
    /// Gets the health increase per level based on character class
    /// </summary>
    private int GetHealthIncreasePerLevel()
    {
        return Class switch
        {
            CharacterClass.Guerrier => 8,  // Warriors are tanky
            CharacterClass.Chasseur => 6,  // Hunters are moderately sturdy
            CharacterClass.Prêtre => 5,    // Priests are support
            CharacterClass.Mage => 4,      // Mages are fragile
            _ => 5
        };
    }
    
    /// <summary>
    /// Gets the attack power increase per level based on character class
    /// </summary>
    private int GetAttackIncreasePerLevel()
    {
        return Class switch
        {
            CharacterClass.Guerrier => 3,  // Warriors have good attack growth
            CharacterClass.Chasseur => 3,  // Hunters have good attack growth
            CharacterClass.Mage => 2,      // Mages focus on spells
            CharacterClass.Prêtre => 2,    // Priests focus on healing
            _ => 2
        };
    }

    public bool Equip(Equipment equipment)
    {
        // Restriction selon la classe et le type d'équipement
        if (equipment is Weapon && !CanEquipWeapon(equipment)) return false;
        if (equipment is Armor && !CanEquipArmor(equipment)) return false;
        
        // Handle unequipping current item and putting it back in inventory
        if (equipment is Weapon)
        {
            if (Weapon != null)
            {
                Weapon.Unequip(this);
                Inventory.AddItem(Weapon);
            }
            Weapon = equipment;
        }
        else if (equipment is Armor)
        {
            if (Armor != null)
            {
                Armor.Unequip(this);
                Inventory.AddItem(Armor);
            }
            Armor = equipment;
        }
        
        equipment.Equip(this);
        return true;
    }

    private bool CanEquipWeapon(Equipment equipment)
    {
        // Exemple de restriction : le mage ne peut pas équiper d'épée
        if (Class == CharacterClass.Mage && equipment.Name.Contains("Épée")) return false;
        // Ajouter d'autres règles selon les classes
        return true;
    }

    // Correction : suppression de la vérification pour Voleur (classe non définie)
    private bool CanEquipArmor(Equipment equipment)
    {
        // Ajouter d'autres règles selon les classes
        return true;
    }
}