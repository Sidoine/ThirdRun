using System;

namespace ThirdRun.Items
{
    public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Value { get; set; }

    public Item(string name, string description, int value)
    {
        Name = name;
        Description = description;
        Value = value;
    }
}

public class Weapon : Equipment
{
    public int Damage { get; set; }
    public Weapon(string name, string description, int value, int bonusStats, int damage)
        : base(name, description, value, bonusStats)
    {
        Damage = damage;
    }
}

public class Armor : Equipment
{
    public int Defense { get; set; }
    public Armor(string name, string description, int value, int bonusStats, int defense)
        : base(name, description, value, bonusStats)
    {
        Defense = defense;
    }
}

public class Potion : Item
{
    public int HealAmount { get; set; }
    public Potion(string name, string description, int value, int healAmount)
        : base(name, description, value)
    {
        HealAmount = healAmount;
    }
    public void Use(Character character)
    {
        character.CurrentHealth += HealAmount;
    }
}
}