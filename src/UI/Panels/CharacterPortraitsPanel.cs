using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ThirdRun.UI.Components;

namespace ThirdRun.UI.Panels
{
    public class CharacterPortraitsPanel : Container
    {
        private const int PortraitSize = 60;
        private const int PortraitSpacing = 8;
        private const int PanelPadding = 16;

        public CharacterPortraitsPanel(UiManager uiManager, Rectangle bounds) : base(uiManager, bounds)
        {
            CreatePortraitButtons();
        }

        private void CreatePortraitButtons()
        {
            var characters = UiManager.GameState.Player.Characters;
            
            for (int i = 0; i < characters.Count; i++)
            {
                var character = characters[i];
                var portraitBounds = new Rectangle(
                    Bounds.X + PanelPadding,
                    Bounds.Y + PanelPadding + i * (PortraitSize + PortraitSpacing),
                    PortraitSize,
                    PortraitSize
                );

                // Load character texture based on class
                Texture2D? characterTexture = null;
                try
                {
                    string texturePath = GetTexturePathForClass(character.Class);
                    characterTexture = UiManager.ContentManager.Load<Texture2D>(texturePath);
                }
                catch
                {
                    // If texture fails to load, we'll show button without texture
                }

                var portraitButton = new SquareImageButton(
                    UiManager,
                    portraitBounds,
                    characterTexture,
                    () => ShowCharacterDetails(character)
                );

                // Set colors for better visibility
                portraitButton.SetColors(
                    new Color(40, 40, 40, 200), // Default: semi-transparent dark
                    new Color(80, 120, 80, 220)  // Hover: semi-transparent green
                );

                AddChild(portraitButton);
            }
        }

        private string GetTexturePathForClass(CharacterClass characterClass)
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

        private void ShowCharacterDetails(Character character)
        {
            UiManager.CurrentState.SelectedCharacter = character;
            UiManager.CurrentState.IsCharacterDetailsVisible = true;
        }

        public override void Draw()
        {
            if (!Visible) return;

            // Draw semi-transparent background for the portraits panel
            UiManager.SpriteBatch.Draw(
                UiManager.Pixel,
                Bounds,
                new Color(20, 20, 20, 150)
            );

            // Draw children (portrait buttons)
            foreach (var child in Children)
            {
                child.Draw();
            }
        }
    }
}