using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using ThirdRun.UI.Components;
using ThirdRun.Items;
using System;

namespace ThirdRun.UI.Panels
{
    public class CharacterDetailsPanel : Container
    {
        private const int PanelWidth = 400;
        private const int PanelHeight = 500;
        private const int PanelPadding = 20;
        private const int LineSpacing = 25;
        private const int SectionSpacing = 15;

        private Character? _character;
        private DynamicSpriteFont _font;

        public CharacterDetailsPanel(UiManager uiManager, Rectangle screenBounds) 
            : base(uiManager, CalculateCenteredBounds(screenBounds))
        {
            _font = uiManager.FontSystem.GetFont(16);
            
            // Add close button
            var closeButton = new Button(
                uiManager,
                new Rectangle(Bounds.Right - 30, Bounds.Top + 5, 25, 25),
                () => ClosePanel(),
                "X"
            );
            AddChild(closeButton);
        }

        private static Rectangle CalculateCenteredBounds(Rectangle screenBounds)
        {
            int x = (screenBounds.Width - PanelWidth) / 2;
            int y = (screenBounds.Height - PanelHeight) / 2;
            return new Rectangle(x, y, PanelWidth, PanelHeight);
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }

        private void ClosePanel()
        {
            UiManager.CurrentState.IsCharacterDetailsVisible = false;
            UiManager.CurrentState.SelectedCharacter = null;
        }

        public override void Draw()
        {
            if (!Visible || _character == null) return;

            // Draw panel background
            UiManager.SpriteBatch.Draw(
                UiManager.Pixel,
                Bounds,
                new Color(30, 30, 30, 240)
            );

            // Draw border
            DrawBorder();

            // Draw character information
            DrawCharacterInfo();

            // Draw children (close button)
            foreach (var child in Children)
            {
                child.Draw();
            }
        }

        private void DrawBorder()
        {
            int borderWidth = 2;
            Color borderColor = new Color(100, 100, 100, 255);

            // Top border
            UiManager.SpriteBatch.Draw(UiManager.Pixel, 
                new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderWidth), borderColor);
            // Bottom border
            UiManager.SpriteBatch.Draw(UiManager.Pixel, 
                new Rectangle(Bounds.X, Bounds.Bottom - borderWidth, Bounds.Width, borderWidth), borderColor);
            // Left border
            UiManager.SpriteBatch.Draw(UiManager.Pixel, 
                new Rectangle(Bounds.X, Bounds.Y, borderWidth, Bounds.Height), borderColor);
            // Right border
            UiManager.SpriteBatch.Draw(UiManager.Pixel, 
                new Rectangle(Bounds.Right - borderWidth, Bounds.Y, borderWidth, Bounds.Height), borderColor);
        }

        private void DrawCharacterInfo()
        {
            if (_character == null) return;

            int currentY = Bounds.Y + PanelPadding;
            Color textColor = Color.White;

            // Character name and class
            DrawText($"Nom: {_character.Name}", Bounds.X + PanelPadding, currentY, textColor, true);
            currentY += LineSpacing;
            
            DrawText($"Classe: {GetClassDisplayName(_character.Class)}", Bounds.X + PanelPadding, currentY, textColor, true);
            currentY += LineSpacing + SectionSpacing;

            // Stats section
            DrawText("=== STATISTIQUES ===", Bounds.X + PanelPadding, currentY, Color.Yellow, true);
            currentY += LineSpacing;

            DrawText($"Vie: {_character.CurrentHealth} / {_character.MaxHealth}", Bounds.X + PanelPadding, currentY, textColor);
            currentY += LineSpacing;

            DrawText($"Puissance d'attaque: {_character.AttackPower}", Bounds.X + PanelPadding, currentY, textColor);
            currentY += LineSpacing;

            DrawText($"Expérience: {_character.Experience}", Bounds.X + PanelPadding, currentY, textColor);
            currentY += LineSpacing + SectionSpacing;

            // Equipment section
            DrawText("=== ÉQUIPEMENT ===", Bounds.X + PanelPadding, currentY, Color.Yellow, true);
            currentY += LineSpacing;

            if (_character.Weapon != null)
            {
                DrawText($"Arme: {_character.Weapon.Name}", Bounds.X + PanelPadding, currentY, Color.LightGreen);
                currentY += LineSpacing;
                DrawText($"  - Dégâts: {((Weapon)_character.Weapon).Damage}", Bounds.X + PanelPadding + 20, currentY, Color.Gray);
                currentY += LineSpacing;
            }
            else
            {
                DrawText("Arme: Aucune", Bounds.X + PanelPadding, currentY, Color.Gray);
                currentY += LineSpacing;
            }

            if (_character.Armor != null)
            {
                DrawText($"Armure: {_character.Armor.Name}", Bounds.X + PanelPadding, currentY, Color.LightBlue);
                currentY += LineSpacing;
                DrawText($"  - Défense: {((Armor)_character.Armor).Defense}", Bounds.X + PanelPadding + 20, currentY, Color.Gray);
                currentY += LineSpacing;
            }
            else
            {
                DrawText("Armure: Aucune", Bounds.X + PanelPadding, currentY, Color.Gray);
                currentY += LineSpacing;
            }

            currentY += SectionSpacing;

            // Techniques section
            if (_character.Techniques.Count > 0)
            {
                DrawText("=== TECHNIQUES ===", Bounds.X + PanelPadding, currentY, Color.Yellow, true);
                currentY += LineSpacing;

                foreach (var technique in _character.Techniques)
                {
                    DrawText($"• {technique}", Bounds.X + PanelPadding, currentY, Color.LightCyan);
                    currentY += LineSpacing;
                }
            }

            // Inventory count
            currentY += SectionSpacing;
            DrawText($"Objets dans l'inventaire: {_character.Inventory.GetItems().Count}", Bounds.X + PanelPadding, currentY, Color.Orange);
        }

        private void DrawText(string text, int x, int y, Color color, bool bold = false)
        {
            var font = bold ? UiManager.FontSystem.GetFont(18) : _font;
            UiManager.SpriteBatch.DrawString(font, text, new Vector2(x, y), color);
        }

        private string GetClassDisplayName(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Guerrier => "Guerrier",
                CharacterClass.Mage => "Mage",
                CharacterClass.Prêtre => "Prêtre",
                CharacterClass.Chasseur => "Chasseur",
                _ => "Inconnu"
            };
        }
    }
}