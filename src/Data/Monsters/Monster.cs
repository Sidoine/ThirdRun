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
            CurrentHealth = type.BaseHealth;
            MaxHealth = type.BaseHealth;
            AttackPower = type.BaseAttack;
            texture = contentManager.Load<Texture2D>(type.TexturePath);
            Position = new Vector2(0, 0); // Initial position
        }

        public void Attack(Character target)
        {
            target.CurrentHealth -= AttackPower;
            if (target.CurrentHealth < 0) target.CurrentHealth = 0;
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
            // N'affiche rien si le monstre est mort
            if (IsDead || texture == null) return;

            //spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, DefaultSize, DefaultSize), Color.White);
            spriteBatch.Draw(texture, new Rectangle((int)(Position.X - DefaultSize / 2), (int)(Position.Y - DefaultSize / 2), DefaultSize, DefaultSize), Color.White);
            DrawHealthBar(spriteBatch, DefaultSize, 6);
            // Uncommented to display the monster's name above its position
            spriteBatch.DrawString(dynamicFont, Type.Name, new Vector2(Position.X - DefaultSize / 2, Position.Y - DefaultSize), Color.White); 
        }
    }
}