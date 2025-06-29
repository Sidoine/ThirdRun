namespace ThirdRun.UI.Panels
{
    using FontStashSharp;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;
    using ThirdRun.Characters;
    using ThirdRun.UI.Components;

    public class ButtonsPanel : Container
    {
        private const int ButtonSize = 40;
        private const int ButtonMargin = 16;

        public ButtonsPanel(UiManager uiManager, Rectangle bounds) :
            base(uiManager, bounds)
        {
            AddChild(new Button(uiManager, new Rectangle(bounds.Right - ButtonSize, bounds.Top, ButtonSize, ButtonSize), () => uiManager.CurrentState.IsInventoryVisible = !uiManager.CurrentState.IsInventoryVisible, "I"));
        }
    }
}