using Microsoft.Xna.Framework;
using ThirdRun.UI.Components;

namespace ThirdRun.UI.Panels;

public class Root : Container
{
    public Root(UiManager uiManager, Rectangle bounds) : base(uiManager, bounds)
    {
        // Ajout des panels principaux
        AddChild(new ButtonsPanel(uiManager, new Rectangle(bounds.Right - 200, bounds.Bottom - 50, 200, 50)));
        AddChild(new InventoryPanel(uiManager, new Rectangle(bounds.Left, bounds.Top, 200, bounds.Height)));
    }
}