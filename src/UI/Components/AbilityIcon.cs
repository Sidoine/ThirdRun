using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using ThirdRun.Data.Abilities;
using MonogameRPG;

namespace ThirdRun.UI.Components
{
    public class AbilityIcon : UIElement
    {
        private readonly Ability _ability;
        private readonly Unit _unit;
        private Texture2D? _abilityTexture;
        private Texture2D? _targetDotTexture;
        private readonly Texture2D _pixelTexture;
        private float _gameTime;

        // Colors for different states
        private readonly Color _normalColor = Color.White;
        private readonly Color _cooldownOverlayColor = new Color(0, 0, 0, 160); // Dark overlay for cooldown
        private readonly Color _targetInRangeColor = Color.White;
        private readonly Color _targetOutOfRangeColor = Color.Red;

        public AbilityIcon(UiManager uiManager, Rectangle bounds, Ability ability, Unit unit)
            : base(uiManager, bounds)
        {
            _ability = ability;
            _unit = unit;
            
            // Create pixel texture for drawing rectangles
            _pixelTexture = new Texture2D(uiManager.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            // Create small dot texture for target indicator
            _targetDotTexture = new Texture2D(uiManager.GraphicsDevice, 4, 4);
            Color[] dotData = new Color[16];
            for (int i = 0; i < 16; i++)
            {
                dotData[i] = Color.White;
            }
            _targetDotTexture.SetData(dotData);
            
            LoadTexture();
        }

        private void LoadTexture()
        {
            try
            {
                _abilityTexture = UiManager.ContentManager.Load<Texture2D>(_ability.IconPath);
            }
            catch (Exception)
            {
                // If texture loading fails, we'll draw without it
                _abilityTexture = null;
            }
        }

        public void UpdateGameTime(float gameTime)
        {
            _gameTime = gameTime;
        }

        public override void Draw()
        {
            if (!Visible) return;

            // Draw background frame
            UiManager.SpriteBatch.Draw(_pixelTexture, Bounds, new Color(60, 60, 60, 255));
            
            // Draw border
            DrawBorder();

            // Draw ability icon
            if (_abilityTexture != null)
            {
                var iconRect = new Rectangle(
                    Bounds.X + 2, 
                    Bounds.Y + 2, 
                    Bounds.Width - 4, 
                    Bounds.Height - 4
                );
                UiManager.SpriteBatch.Draw(_abilityTexture, iconRect, _normalColor);
            }
            else
            {
                // Draw placeholder text if no texture
                var font = UiManager.FontSystem.GetFont(10);
                var textSize = font.MeasureString(_ability.Name);
                var textPos = new Vector2(
                    Bounds.X + (Bounds.Width - textSize.X) / 2,
                    Bounds.Y + (Bounds.Height - textSize.Y) / 2
                );
                UiManager.SpriteBatch.DrawString(font, _ability.Name, textPos, Color.White);
            }

            // Draw cooldown overlay
            if (_ability.IsOnCooldown(_gameTime))
            {
                DrawCooldownOverlay();
            }

            // Draw target indicator
            DrawTargetIndicator();
        }

        private void DrawBorder()
        {
            int borderWidth = 1;
            Color borderColor = new Color(120, 120, 120, 255);

            // Top border
            UiManager.SpriteBatch.Draw(_pixelTexture, 
                new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderWidth), borderColor);
            // Bottom border
            UiManager.SpriteBatch.Draw(_pixelTexture, 
                new Rectangle(Bounds.X, Bounds.Bottom - borderWidth, Bounds.Width, borderWidth), borderColor);
            // Left border
            UiManager.SpriteBatch.Draw(_pixelTexture, 
                new Rectangle(Bounds.X, Bounds.Y, borderWidth, Bounds.Height), borderColor);
            // Right border
            UiManager.SpriteBatch.Draw(_pixelTexture, 
                new Rectangle(Bounds.Right - borderWidth, Bounds.Y, borderWidth, Bounds.Height), borderColor);
        }

        private void DrawCooldownOverlay()
        {
            float remainingCooldown = _ability.GetCooldownRemaining(_gameTime);
            float cooldownPercentage = remainingCooldown / _ability.Cooldown;
            
            // Draw dark overlay covering the percentage of cooldown remaining
            int overlayHeight = (int)((Bounds.Height - 4) * cooldownPercentage);
            if (overlayHeight > 0)
            {
                var overlayRect = new Rectangle(
                    Bounds.X + 2,
                    Bounds.Y + 2,
                    Bounds.Width - 4,
                    overlayHeight
                );
                UiManager.SpriteBatch.Draw(_pixelTexture, overlayRect, _cooldownOverlayColor);
            }

            // Draw cooldown text
            if (remainingCooldown > 0.1f) // Only show if more than 0.1 seconds remaining
            {
                var font = UiManager.FontSystem.GetFont(10);
                string cooldownText = remainingCooldown.ToString("F1");
                var textSize = font.MeasureString(cooldownText);
                var textPos = new Vector2(
                    Bounds.X + (Bounds.Width - textSize.X) / 2,
                    Bounds.Y + (Bounds.Height - textSize.Y) / 2
                );
                UiManager.SpriteBatch.DrawString(font, cooldownText, textPos, Color.Yellow);
            }
        }

        private void DrawTargetIndicator()
        {
            // Only show indicator if ability needs a target and unit has a target
            if (_ability.TargetType == TargetType.Self) return;
            
            // For this demo, we'll assume the unit might have a target
            // In a real implementation, you would check if the unit has a current target
            // and determine if it's in range
            
            // We'll simulate having a target for abilities that need one
            bool hasTarget = _ability.TargetType != TargetType.Self;
            
            if (hasTarget && _targetDotTexture != null)
            {
                // For demo purposes, we'll assume target is in range if cooldown is not active
                bool inRange = !_ability.IsOnCooldown(_gameTime);
                Color dotColor = inRange ? _targetInRangeColor : _targetOutOfRangeColor;
                
                // Draw dot in lower right corner
                var dotRect = new Rectangle(
                    Bounds.Right - 8,
                    Bounds.Bottom - 8,
                    4,
                    4
                );
                UiManager.SpriteBatch.Draw(_targetDotTexture, dotRect, dotColor);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pixelTexture?.Dispose();
                _targetDotTexture?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}