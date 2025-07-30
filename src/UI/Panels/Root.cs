using Microsoft.Xna.Framework;
using ThirdRun.UI.Components;

namespace ThirdRun.UI.Panels;

public class Root : Container
{
    public Root(UiManager uiManager, Rectangle bounds) : base(uiManager, bounds)
    {
        // Ajout des panels principaux
        AddChild(new ButtonsPanel(uiManager, new Rectangle(bounds.Right - 100, bounds.Bottom - 60, 100, 60)));
        AddChild(new InventoryPanel(uiManager, new Rectangle(bounds.Left, bounds.Top, 320, bounds.Height)));
    }
}