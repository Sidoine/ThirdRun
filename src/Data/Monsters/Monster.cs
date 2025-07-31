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
        public int Level => Type.Level;

        private Texture2D? texture;
        private static readonly int DefaultSize = 40;
        private readonly ContentManager contentManager;


        public Monster(MonsterType type, ContentManager contentManager)
        {
            Type = type;
            this.contentManager = contentManager;

            CurrentHealth = type.BaseHealth;
            MaxHealth = type.BaseHealth;
            AttackPower = type.BaseAttack;
            Position = new Vector2(0, 0); // Initial position
        }

        public void Attack(Character target)
        {
            target.CurrentHealth -= AttackPower;
            if (target.CurrentHealth < 0) target.CurrentHealth = 0;
        }

        public Item DropLoot()
        {
            // Generate a random item based on monster level
            return RandomItemGenerator.GenerateRandomItem(Level);
        }

        public int GetExperienceValue()
        {
            // Valeur d'expérience par défaut, à adapter selon le type de monstre
            return 10;
        }

        public void RenderAtPosition(SpriteBatch spriteBatch, DynamicSpriteFont dynamicFont, Vector2 renderPosition)
        {
            texture ??= contentManager.Load<Texture2D>(Type.TexturePath);
            // N'affiche rien si le monstre est mort
            if (IsDead) return;

            spriteBatch.Draw(texture, new Rectangle((int)(renderPosition.X - DefaultSize / 2), (int)(renderPosition.Y - DefaultSize / 2), DefaultSize, DefaultSize), Color.White);
            
            // Temporarily set position for health bar drawing
            Vector2 originalPos = Position;
            Position = renderPosition;
            DrawHealthBar(spriteBatch, DefaultSize, 6);
            Position = originalPos;
            
            // Display the monster's name above its position
            spriteBatch.DrawString(dynamicFont, Type.Name, new Vector2(renderPosition.X - DefaultSize / 2, renderPosition.Y - DefaultSize), Color.White); 
        }
    }
}