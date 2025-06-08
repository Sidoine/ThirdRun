using System.Collections.Generic;
using ThirdRun.Characters;
using MonogameRPG.Monsters;
using MonogameRPG.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonogameRPG;

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

    private readonly Texture2D texture;
    private static readonly int DefaultSize = 40;
    private static readonly Color DefaultColor = Color.CornflowerBlue;

    private static string GetTexturePathForClass(CharacterClass characterClass)
    {
        return characterClass switch
        {
            CharacterClass.Guerrier => "Characters/warrior",
            CharacterClass.Mage => "Characters/mage",
            CharacterClass.Prêtre => "Characters/priest",
            CharacterClass.Chasseur => "Characters/hunter",
            _ => "Characters/warrior"
        };
    }

    public Character(string name, CharacterClass characterClass, int health, int attackPower, GraphicsDevice graphicsDevice, ContentManager content)
    {
        Name = name;
        Class = characterClass;
        CurrentHealth = health;
        MaxHealth = health;
        AttackPower = attackPower;
        Experience = 0;
        Inventory = new Inventory() { Owner = this };
        Techniques = new List<string>();
        Position = new Vector2(0, 0); // Position initiale
        string path = GetTexturePathForClass(characterClass);
        texture = content.Load<Texture2D>(path);
    }

    public void Move(List<Monster> monsters, Map map)
    {
        if (monsters == null || monsters.Count == 0) return;
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
        if (closest == null) return;
        // Utiliser A* pour trouver le chemin
        var path = map.FindPathAStar(Position, closest.Position);
        if (path.Count > 1)
        {
            Vector2 next = path[1]; // [0] = position actuelle
            Vector2 direction = next - Position;
            if (direction.Length() > 1f)
            {
                direction.Normalize();
                Position += direction * 2f; // Vitesse de déplacement (pixels par frame)
            }
        }
        // Si le personnage est assez proche, attaque
        if (Vector2.Distance(Position, closest.Position) < DefaultSize)
        {
            Attack(closest);
        }
    }

    public void Attack(Monster monster)
    {
        monster.CurrentHealth -= AttackPower;
        if (monster.CurrentHealth <= 0)
        {
            GainExperience(monster);
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
        if (equipment is Weapon) Weapon = equipment;
        else if (equipment is Armor) Armor = equipment;
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

    public void Render(SpriteBatch spriteBatch)
    {
        if (texture == null) return;
        Vector2 pos = Position;
        int size = DefaultSize;
        spriteBatch.Draw(texture, new Rectangle((int)(pos.X - size / 2), (int)(pos.Y - size / 2), size, size), Color.White);
        DrawHealthBar(spriteBatch, size, 6);
    }
}