namespace ThirdRun.Data
{
    using System.Collections.Generic;
    using MonogameRPG.Map;
    using ThirdRun.Characters;

    /// <summary>
    /// Représente un joueur dans le jeu, avec des informations sur son personnage et son inventaire.
    /// </summary>
    public class GameState
    {
        required public Player Player { get; set; }
        required public WorldMap WorldMap { get; set; }
    }
}