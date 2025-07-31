using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using MonogameRPG.Monsters;
using FontStashSharp;
using System;

namespace MonogameRPG.Map
{
    public class WorldMap
    {
        private Dictionary<Vector2, Map> cards = new Dictionary<Vector2, Map>();
        private Vector2 currentCardPosition = Vector2.Zero;
        private List<Character> characters = new List<Character>();
        private ContentManager contentManager;
        private GraphicsDevice graphicsDevice;

        public Map CurrentMap => cards.ContainsKey(currentCardPosition) ? cards[currentCardPosition] : null!;
        public Vector2 CurrentMapPosition => currentCardPosition;

        public WorldMap(ContentManager content, GraphicsDevice graphics)
        {
            contentManager = content;
            graphicsDevice = graphics;
        }

        public void Initialize()
        {
            // Create the initial card at (0,0)
            var initialCard = new Map(Vector2.Zero);
            initialCard.GenerateRandomMap(graphicsDevice);
            initialCard.SpawnMonsters(contentManager);
            cards[Vector2.Zero] = initialCard;
            currentCardPosition = Vector2.Zero;
        }

        public void SetCharacters(List<Character> chars)
        {
            characters = chars;
            // Set characters on the current card
            if (CurrentMap != null)
            {
                CurrentMap.SetCharacters(chars);
            }
        }

        public void Update()
        {
            if (CurrentMap == null) return;

            // Check if current card is cleared of monsters
            if (!CurrentMap.HasLivingMonsters())
            {
                // Check if we need to generate a new adjacent card
                var availableDirections = GetAvailableDirections();
                if (availableDirections.Count > 0)
                {
                    // Generate a new card in a random available direction
                    var rand = new Random();
                    var direction = availableDirections[rand.Next(availableDirections.Count)];
                    GenerateAdjacentCard(direction);
                }
                
                // If there are no available directions, generate one anyway by choosing a random direction
                // This ensures the game always continues
                if (availableDirections.Count == 0)
                {
                    var rand = new Random();
                    var directions = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };
                    var direction = directions[rand.Next(directions.Length)];
                    GenerateAdjacentCard(direction);
                }
            }

            // Check for character transitions between cards
            HandleCharacterTransitions();

            // Clean up empty cards (except current card)
            CleanupEmptyCards();
        }

        private List<Direction> GetAvailableDirections()
        {
            var available = new List<Direction>();
            var directions = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };

            foreach (var dir in directions)
            {
                var adjacentPos = GetAdjacentPosition(currentCardPosition, dir);
                if (!cards.ContainsKey(adjacentPos))
                {
                    available.Add(dir);
                }
            }

            return available;
        }

        private Vector2 GetAdjacentPosition(Vector2 cardPos, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return cardPos + new Vector2(0, -1);
                case Direction.South:
                    return cardPos + new Vector2(0, 1);
                case Direction.East:
                    return cardPos + new Vector2(1, 0);
                case Direction.West:
                    return cardPos + new Vector2(-1, 0);
                default:
                    return cardPos;
            }
        }

        private void GenerateAdjacentCard(Direction direction)
        {
            var newCardPos = GetAdjacentPosition(currentCardPosition, direction);
            
            if (!cards.ContainsKey(newCardPos))
            {
                var newCard = new Map(newCardPos);
                newCard.GenerateRandomMap(graphicsDevice);
                newCard.SpawnMonsters(contentManager);
                cards[newCardPos] = newCard;
            }
        }

        private void HandleCharacterTransitions()
        {
            // Check if characters should move towards new cards when current card is cleared
            if (CurrentMap != null && !CurrentMap.HasLivingMonsters())
            {
                foreach (var character in characters)
                {
                    MoveCharacterTowardsNewCard(character);
                }
            }
        }

        private Map? GetCardAtPosition(Vector2 worldPosition)
        {
            // Convert world position to card coordinates
            foreach (var kvp in cards)
            {
                var card = kvp.Value;
                var cardWorldPos = card.WorldPosition;
                
                // Calculate the bounds of this card in world coordinates
                float cardLeft = cardWorldPos.X * card.GridWidth * card.TileWidth;
                float cardTop = cardWorldPos.Y * card.GridHeight * card.TileHeight;
                float cardRight = cardLeft + card.GridWidth * card.TileWidth;
                float cardBottom = cardTop + card.GridHeight * card.TileHeight;
                
                if (worldPosition.X >= cardLeft && worldPosition.X < cardRight &&
                    worldPosition.Y >= cardTop && worldPosition.Y < cardBottom)
                {
                    return card;
                }
            }
            return null;
        }

        private void TransitionCharacterToCard(Character character, Map newCard)
        {
            // This method is called when a character moves to a different card
            // We might want to update the current card if most characters have moved
            if (newCard.WorldPosition != currentCardPosition && ShouldSwitchToCard(newCard))
            {
                currentCardPosition = newCard.WorldPosition;
                CurrentMap.SetCharacters(characters);
            }
        }

        private bool ShouldSwitchToCard(Map card)
        {
            // Switch to new card if majority of characters are on it
            int charactersOnNewCard = 0;
            foreach (var character in characters)
            {
                var charCard = GetCardAtPosition(character.Position);
                if (charCard == card)
                {
                    charactersOnNewCard++;
                }
            }
            return charactersOnNewCard > characters.Count / 2;
        }

        private void MoveCharacterTowardsNewCard(Character character)
        {
            // Find the nearest adjacent card that exists
            Map? targetCard = null;
            Direction targetDirection = Direction.North;
            float minDistance = float.MaxValue;

            var directions = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };
            foreach (var dir in directions)
            {
                var adjacentPos = GetAdjacentPosition(currentCardPosition, dir);
                if (cards.ContainsKey(adjacentPos))
                {
                    var card = cards[adjacentPos];
                    var edgePos = CurrentMap.GetEdgePosition(dir);
                    var distance = Vector2.Distance(character.Position, edgePos);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetCard = card;
                        targetDirection = dir;
                    }
                }
            }

            if (targetCard != null)
            {
                // Move character towards the edge of current card in the direction of target card
                var edgePosition = CurrentMap.GetEdgePosition(targetDirection);
                var direction = edgePosition - character.Position;
                if (direction.Length() > 5f) // Increased threshold to avoid jittering
                {
                    direction.Normalize();
                    character.Position += direction * 2f; // Same movement speed as normal movement
                }
                else
                {
                    // Character has reached the edge, transition to new card
                    TransitionCharacterToNewCard(character, targetCard, targetDirection);
                }
            }
        }
        
        private void TransitionCharacterToNewCard(Character character, Map newCard, Direction direction)
        {
            // Calculate the entry position on the new card (opposite edge)
            Vector2 entryPos;
            switch (direction)
            {
                case Direction.North:
                    entryPos = new Vector2(newCard.GridWidth * newCard.TileWidth / 2, newCard.GridHeight * newCard.TileHeight - 50);
                    break;
                case Direction.South:
                    entryPos = new Vector2(newCard.GridWidth * newCard.TileWidth / 2, 50);
                    break;
                case Direction.East:
                    entryPos = new Vector2(50, newCard.GridHeight * newCard.TileHeight / 2);
                    break;
                case Direction.West:
                    entryPos = new Vector2(newCard.GridWidth * newCard.TileWidth - 50, newCard.GridHeight * newCard.TileHeight / 2);
                    break;
                default:
                    entryPos = new Vector2(newCard.GridWidth * newCard.TileWidth / 2, newCard.GridHeight * newCard.TileHeight / 2);
                    break;
            }
            
            // Set character position to the entry point of the new card
            character.Position = entryPos;
            
            // Switch to the new card if this is the first character to transition
            if (newCard.WorldPosition != currentCardPosition)
            {
                currentCardPosition = newCard.WorldPosition;
                CurrentMap.SetCharacters(characters);
            }
        }

        private void CleanupEmptyCards()
        {
            var cardsToRemove = new List<Vector2>();
            
            foreach (var kvp in cards)
            {
                var cardPos = kvp.Key;
                var card = kvp.Value;
                
                // Don't remove current card
                if (cardPos == currentCardPosition) continue;
                
                // Don't remove adjacent cards (keep them for potential re-entry)
                if (IsAdjacentToCurrentMap(cardPos)) continue;
                
                // Check if any characters are on this card
                bool hasCharacters = false;
                foreach (var character in characters)
                {
                    var charCard = GetCardAtPosition(character.Position);
                    if (charCard == card)
                    {
                        hasCharacters = true;
                        break;
                    }
                }
                
                // Remove card if it has no characters and no living monsters
                if (!hasCharacters && !card.HasLivingMonsters())
                {
                    cardsToRemove.Add(cardPos);
                }
            }
            
            foreach (var cardPos in cardsToRemove)
            {
                cards.Remove(cardPos);
            }
        }
        
        private bool IsAdjacentToCurrentMap(Vector2 cardPos)
        {
            var diff = cardPos - currentCardPosition;
            var distance = Math.Abs(diff.X) + Math.Abs(diff.Y);
            return distance <= 1; // Adjacent cards have distance 1
        }

        public void Render(SpriteBatch spriteBatch, DynamicSpriteFont dynamicFont)
        {
            // Render all cards relative to their world positions
            foreach (var card in cards.Values)
            {
                card.Render(spriteBatch, dynamicFont);
            }
        }

        public List<Monster> GetMonstersOnCurrentMap()
        {
            return CurrentMap?.GetMonsters() ?? new List<Monster>();
        }

        public Map FindPathAStar(Vector2 start, Vector2 end)
        {
            // For now, just use the current card's pathfinding
            // In the future, this could be extended to pathfind across multiple cards
            return CurrentMap;
        }
    }
}