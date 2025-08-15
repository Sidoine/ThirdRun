using ThirdRun.Data;

namespace ThirdRun.Items
{
    /// <summary>
    /// Repository of unique items with fixed stats
    /// </summary>
    public static class UniqueItemRepository
    {
        // Helper method to create characteristics
        private static CharacteristicValues CreateCharacteristics()
        {
            return new CharacteristicValues();
        }

        // Unique Weapons
        public static readonly UniqueItem ExcaliburSword = new(
            "Excalibur", 
            "Une épée légendaire aux pouvoirs magiques", 
            1000,
            5, 
            ItemSlot.Weapon,
            CreateExcaliburCharacteristics(),
            "Items/Weapons/epee");

        public static readonly UniqueItem DragonSlayerAxe = new(
            "Hache du Tueur de Dragons", 
            "Une hache forgée pour terrasser les dragons", 
            800,
            4, 
            ItemSlot.Weapon,
            CreateDragonSlayerCharacteristics(),
            "Items/Weapons/hache");

        public static readonly UniqueItem ShadowBlade = new(
            "Lame de l'Ombre", 
            "Une dague maudite qui frappe depuis l'obscurité", 
            600,
            3, 
            ItemSlot.Weapon,
            CreateShadowBladeCharacteristics(),
            "Items/Weapons/dague");

        // Unique Armor
        public static readonly UniqueItem PlateOfLegends = new(
            "Plastron des Légendes", 
            "Un plastron porté par les héros d'antan", 
            750,
            4, 
            ItemSlot.Armor,
            CreatePlateOfLegendsCharacteristics(),
            "Items/Armors/plastron");

        public static readonly UniqueItem HelmetOfWisdom = new(
            "Casque de la Sagesse", 
            "Augmente la sagesse et la protection", 
            500,
            3, 
            ItemSlot.Armor,
            CreateHelmetOfWisdomCharacteristics(),
            "Items/Armors/casque");

        public static readonly UniqueItem BootsOfSwiftness = new(
            "Bottes de Célérité", 
            "Permettent de se déplacer plus rapidement", 
            400,
            2, 
            ItemSlot.Armor,
            CreateBootsOfSwiftnessCharacteristics(),
            "Items/Armors/bottes");

        // Unique Potions
        public static readonly UniqueItem ElixirOfLife = new(
            "Élixir de Vie", 
            "Restaure complètement la santé et plus encore", 
            300,
            3, 
            ItemSlot.Potion,
            CreateElixirOfLifeCharacteristics(),
            "Items/Potions/potion_soin");

        public static readonly UniqueItem PhoenixTears = new(
            "Larmes de Phénix", 
            "Une potion rare aux propriétés de régénération", 
            500,
            4, 
            ItemSlot.Potion,
            CreatePhoenixTearsCharacteristics(),
            "Items/Potions/potion_magie");

        // Collections for easy access
        public static readonly UniqueItem[] AllUniqueWeapons = 
        {
            ExcaliburSword,
            DragonSlayerAxe,
            ShadowBlade
        };

        public static readonly UniqueItem[] AllUniqueArmor = 
        {
            PlateOfLegends,
            HelmetOfWisdom,
            BootsOfSwiftness
        };

        public static readonly UniqueItem[] AllUniquePotions = 
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

        // Characteristic creation methods
        private static CharacteristicValues CreateExcaliburCharacteristics()
        {
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.MeleeAttackPower, 25);
            characteristics.SetValue(Characteristic.CriticalChance, 15);
            return characteristics;
        }

        private static CharacteristicValues CreateDragonSlayerCharacteristics()
        {
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.MeleeAttackPower, 30);
            characteristics.SetValue(Characteristic.Haste, 10);
            return characteristics;
        }

        private static CharacteristicValues CreateShadowBladeCharacteristics()
        {
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.MeleeAttackPower, 18);
            characteristics.SetValue(Characteristic.CriticalChance, 20);
            return characteristics;
        }

        private static CharacteristicValues CreatePlateOfLegendsCharacteristics()
        {
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.Armor, 20);
            characteristics.SetValue(Characteristic.Health, 12);
            return characteristics;
        }

        private static CharacteristicValues CreateHelmetOfWisdomCharacteristics()
        {
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.Armor, 15);
            characteristics.SetValue(Characteristic.SpellPower, 18);
            return characteristics;
        }

        private static CharacteristicValues CreateBootsOfSwiftnessCharacteristics()
        {
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.Armor, 8);
            characteristics.SetValue(Characteristic.Haste, 25);
            return characteristics;
        }

        private static CharacteristicValues CreateElixirOfLifeCharacteristics()
        {
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.HealingPower, 150);
            return characteristics;
        }

        private static CharacteristicValues CreatePhoenixTearsCharacteristics()
        {
            var characteristics = new CharacteristicValues();
            characteristics.SetValue(Characteristic.HealingPower, 200);
            return characteristics;
        }
    }
}