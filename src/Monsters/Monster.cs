using ThirdRun.Items;
using ThirdRun.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonogameRPG;
using FontStashSharp;

namespace MonogameRPG.Monsters
{
    public class Monster : Unit
    {
        public MonsterType Type { get; set; }

        private readonly Texture2D texture;
        private static readonly int DefaultSize = 40;

        public Monster(MonsterType type, ContentManager contentManager)
        {
            Type = type;
            Health = type.BaseHealth;
            AttackPower = type.BaseAttack;
            texture = contentManager.Load<Texture2D>(type.TexturePath);
            Position = new Vector2(0, 0); // Initial position
        }

        public void Attack(Character target)
        {
            target.Health -= AttackPower;
            if (target.Health < 0) target.Health = 0;
        }

        public Item DropLoot()
        {
            // Logic to drop loot, returning a new Item for simplicity
            return new Item($"Loot from {Type.Name}", "A valuable item dropped by the monster.", 10);
        }

        public int GetExperienceValue()
        {
            // Valeur d'expérience par défaut, à adapter selon le type de monstre
            return 10;
        }

        public void Render(SpriteBatch spriteBatch, DynamicSpriteFont dynamicFont)
        {
            if (texture == null) return;
            spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, DefaultSize, DefaultSize), Color.White);
            DrawHealthBar(spriteBatch, DefaultSize, 6);
            spriteBatch.DrawString(dynamicFont, Type.Name, new Vector2(Position.X, Position.Y - 24), Color.White);
        }
    }
}