using System.Collections.Generic;
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
    public List<string> Techniques { get; private set; }
    public Map Map { get; set; }

    private readonly WorldMap worldMap;

    public Character(string name, CharacterClass characterClass, int health, int attackPower, WorldMap worldMap)
    {
        Name = name;
        Class = characterClass;
        this.worldMap = worldMap;

        CurrentHealth = health;
        MaxHealth = health;
        AttackPower = attackPower;
        Experience = 0;
        Inventory = new Inventory() { Owner = this };
        Techniques = new List<string>();
        Position = new Vector2(0, 0); // Position initiale
        Map = worldMap.CurrentMap;
        
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
                // Hunters get ranged attack
                Abilities.Add(new RangedAttackAbility());
                Characteristics.SetValue(ThirdRun.Data.Characteristic.RangedAttackPower, AttackPower);
                break;
                
            case CharacterClass.Prêtre:
                // Priests get healing abilities
                Abilities.Add(new HealAbility());
                Abilities.Add(new SelfHealAbility());
                Characteristics.SetValue(ThirdRun.Data.Characteristic.HealingPower, AttackPower / 2);
                break;
                
            case CharacterClass.Mage:
                // Mages could get spell abilities (ranged magic attacks)
                Abilities.Add(new RangedAttackAbility()); // Use as magic attack for now
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
        if (worldMap.IsInTown)
        {
            MoveInTown(worldMap.GetNPCsOnCurrentMap());
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
            var nextMap = worldMap.GetAdjacentCardWithMonsters();
            MoveTo(nextMap.Position + new Vector2(Map.GridWidth / 2 * Map.TileWidth, Map.GridHeight / 2 * Map.TileHeight));
            return;
        }

        MoveTo(closest.Position);

        // Si le personnage est assez proche, attaque
        if (Vector2.Distance(Position, closest.Position) < Map.TileHeight)
        {
            Attack(closest);
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

    private void MoveTo(Vector2 position)
    {
        // Utiliser A* pour trouver le chemin sur la carte actuelle
        var path = worldMap.FindPathAStar(Position, position);
        if (path.Count > 1)
        {
            Vector2 next = path[1]; // [0] = position actuelle
            Vector2 direction = next - Position;
            if (direction.Length() > 1f)
            {
                direction.Normalize();
                Position += direction * 2f; // Vitesse de déplacement (pixels par frame)
            }
            
            var mapAtPosition = worldMap.GetMapAtPosition(Position);
            if (mapAtPosition != null && mapAtPosition != Map)
            {
                Map.RemoveUnit(this);
                mapAtPosition.AddUnit(this);
                Map = mapAtPosition;
                worldMap.UpdateCurrentMap();
            }
        }
    }


    public void Attack(Monster monster)
    {
        monster.CurrentHealth -= AttackPower;
        if (monster.CurrentHealth <= 0)
        {
            GainExperience(monster);
            // Ramasser automatiquement le loot du monstre vaincu
            var loot = monster.DropLoot();
            Inventory.AddItem(loot);
        }
    }

    public void GainExperience(Monster monster)
    {
        Experience += monster.GetExperienceValue();
        // Débloquer des techniques selon la classe et le niveau
        UnlockTechniques();
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

    private void UnlockTechniques()
    {
        // Exemple : débloquer une technique au niveau 10
        if (Experience >= 100 && !Techniques.Contains("Coup spécial"))
        {
            Techniques.Add("Coup spécial");
        }
        // Ajouter d'autres techniques selon la classe
    }
}