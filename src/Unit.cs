using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ThirdRun.Data;
using ThirdRun.Data.Abilities;
using System.Linq;

namespace MonogameRPG
{
    public abstract class Unit
    {
        public Vector2 Position { get; set; }
        public int CurrentHealth { get; set; }
        public CharacteristicValues Characteristics { get; private set; }
        public bool IsDead => CurrentHealth <= 0;
        
        // Map properties for movement and pathfinding
        public MonogameRPG.Map.Map Map { get; set; }
        protected ThirdRun.Data.Map.WorldMap WorldMap { get; set; }
        
        // Ability system
        public List<Ability> Abilities { get; private set; }
        public Ability DefaultAbility { get; protected set; }

        // Properties that delegate to characteristics
        public int MaxHealth 
        { 
            get => Characteristics.GetValue(Characteristic.Health);
            set => Characteristics.SetValue(Characteristic.Health, value);
        }
        
        public int AttackPower 
        { 
            get => Characteristics.GetValue(Characteristic.MeleeAttackPower);
            set => Characteristics.SetValue(Characteristic.MeleeAttackPower, value);
        }

        protected Unit()
        {
            Characteristics = new CharacteristicValues();
            Abilities = new List<Ability>();
            DefaultAbility = new MeleeAttackAbility();
            Abilities.Add(DefaultAbility);
        }
        
        // Game time tracking - this should be set by the game loop
        protected float CurrentGameTime { get; set; }
        
        public void UpdateGameTime(float gameTime)
        {
            CurrentGameTime = gameTime;
        }
        
        public bool CanUseAbility(Ability ability, Unit? target)
        {
            return ability.CanUse(this, target, CurrentGameTime);
        }
        
        public void UseAbility(Ability ability, Unit? target)
        {
            ability.Use(this, target, CurrentGameTime);
        }
        
        // Convenience method for default attack behavior
        public void Attack(Unit target)
        {
            UseAbility(DefaultAbility, target);
        }
        
        /// <summary>
        /// Moves the unit towards the specified position using pathfinding
        /// </summary>
        /// <param name="position">Target position to move to</param>
        protected void MoveTo(Vector2 position)
        {
            if (WorldMap == null || Map == null) return;
            
            // Utiliser A* pour trouver le chemin sur la carte actuelle
            var path = WorldMap.FindPathAStar(Position, position);
            if (path.Count > 1)
            {
                Vector2 next = path[1]; // [0] = position actuelle
                Vector2 direction = next - Position;
                if (direction.Length() > 1f)
                {
                    direction.Normalize();
                    
                    // Calculate the new position we want to move to
                    Vector2 newPosition = Position + direction * 2f; // Vitesse de déplacement (pixels par frame)
                    
                    // Collision detection: check if the new position would collide with another unit
                    if (!WouldCollideWithOtherUnit(newPosition))
                    {
                        var oldPosition = Position;
                        Position = newPosition;
                        
                        // Update unit position tracking on the current map
                        Map.UpdateUnitPosition(this, oldPosition);
                    }
                    // If collision would occur, don't move (unit stops)
                }
                
                // Check for map transitions
                var mapAtPosition = WorldMap.GetMapAtPosition(Position);
                if (mapAtPosition != null && mapAtPosition != Map)
                {
                    Map.RemoveUnit(this);
                    mapAtPosition.AddUnit(this);
                    Map = mapAtPosition;
                    WorldMap.UpdateCurrentMap();
                }
            }
        }
        
        /// <summary>
        /// Checks if moving to the specified position would cause a collision with another unit
        /// </summary>
        /// <param name="newPosition">The position we want to move to</param>
        /// <returns>True if collision would occur, false otherwise</returns>
        private bool WouldCollideWithOtherUnit(Vector2 newPosition)
        {
            if (Map == null) return true;
            
            // Convert new position to tile coordinates
            var tileCoords = Map.WorldPositionToTileCoordinates(newPosition);
            if (!tileCoords.HasValue)
            {
                // Position is outside the map, consider it a collision
                return true;
            }
            
            // Check if there's another unit at this tile position
            var unitAtTile = Map.GetUnitAtTile(tileCoords.Value.X, tileCoords.Value.Y);
            if (unitAtTile != null && unitAtTile != this)
            {
                // There's another unit occupying this tile
                return true;
            }
            
            // No collision detected
            return false;
        }
        

    }
}
