namespace ThirdRun.Items
{
    /// <summary>
    /// Repository of unique items with fixed stats
    /// </summary>
    public static class UniqueItemRepository
    {
        // Unique Weapons
        public static readonly UniqueWeapon ExcaliburSword = new(
            "Excalibur", 
            "Une épée légendaire aux pouvoirs magiques", 
            1000, 5, 25, 15, "Items/Weapons/epee");

        public static readonly UniqueWeapon DragonSlayerAxe = new(
            "Hache du Tueur de Dragons", 
            "Une hache forgée pour terrasser les dragons", 
            800, 4, 30, 10, "Items/Weapons/hache");

        public static readonly UniqueWeapon ShadowBlade = new(
            "Lame de l'Ombre", 
            "Une dague maudite qui frappe depuis l'obscurité", 
            600, 3, 18, 20, "Items/Weapons/dague");

        // Unique Armor
        public static readonly UniqueArmor PlateOfLegends = new(
            "Plastron des Légendes", 
            "Un plastron porté par les héros d'antan", 
            750, 4, 20, 12, "Items/Armors/plastron");

        public static readonly UniqueArmor HelmetOfWisdom = new(
            "Casque de la Sagesse", 
            "Augmente la sagesse et la protection", 
            500, 3, 15, 18, "Items/Armors/casque");

        public static readonly UniqueArmor BootsOfSwiftness = new(
            "Bottes de Célérité", 
            "Permettent de se déplacer plus rapidement", 
            400, 2, 8, 25, "Items/Armors/bottes");

        // Unique Potions
        public static readonly UniquePotion ElixirOfLife = new(
            "Élixir de Vie", 
            "Restaure complètement la santé et plus encore", 
            300, 3, 150, "Items/Potions/potion_soin");

        public static readonly UniquePotion PhoenixTears = new(
            "Larmes de Phénix", 
            "Une potion rare aux propriétés de régénération", 
            500, 4, 200, "Items/Potions/potion_magie");

        // Collections for easy access
        public static readonly UniqueWeapon[] AllUniqueWeapons = 
        {
            ExcaliburSword,
            DragonSlayerAxe,
            ShadowBlade
        };

        public static readonly UniqueArmor[] AllUniqueArmor = 
        {
            PlateOfLegends,
            HelmetOfWisdom,
            BootsOfSwiftness
        };

        public static readonly UniquePotion[] AllUniquePotions = 
        {
            ElixirOfLife,
            PhoenixTears
        };

        public static readonly UniqueItem[] AllUniqueItems = 
        {
            ExcaliburSword, DragonSlayerAxe, ShadowBlade,
            PlateOfLegends, HelmetOfWisdom, BootsOfSwiftness,
            ElixirOfLife, PhoenixTears
        };
    }
}