using System;

namespace ThirdRun.Items
{
    public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Value { get; set; }
    public int ItemLevel { get; set; }
    public string? ImagePath { get; set; }

    public Item(string name, string description, int value, int itemLevel = 1)
    {
        Name = name;
        Description = description;
        Value = value;
        ItemLevel = itemLevel;
        ImagePath = null;
    }
}

public class Weapon : Equipment
{
    public int Damage { get; set; }
    public Weapon(string name, string description, int value, int bonusStats, int damage, int itemLevel = 1)
        : base(name, description, value, bonusStats, itemLevel)
    {
        Damage = damage;
    }
}

public class Armor : Equipment
{
    public int Defense { get; set; }
    public Armor(string name, string description, int value, int bonusStats, int defense, int itemLevel = 1)
        : base(name, description, value, bonusStats, itemLevel)
    {
        Defense = defense;
    }
}

public class Potion : Item
{
    public int HealAmount { get; set; }
    public Potion(string name, string description, int value, int healAmount, int itemLevel = 1)
        : base(name, description, value, itemLevel)
    {
        HealAmount = healAmount;
    }
    public void Use(Character character)
    {
        character.CurrentHealth += HealAmount;
    }
}
}