# ThirdRun

## Description

ThirdRun est un jeu en 2D où les joueurs peuvent créer un groupe de personnages qui se déplacent automatiquement sur une carte pour attaquer des monstres. Les monstres offrent de l'expérience et du loot qui sont collectés dans des sacs. Les joueurs peuvent également équiper des objets qui fournissent des bonus aux compétences de leurs personnages.

## Structure du projet

Le projet est organisé comme suit :

- **Content/** : Contient les ressources du jeu.

  - **sprites/** : Images pour les sprites des personnages et des monstres.
  - **maps/** : Fichiers de carte utilisés dans le jeu.

- **src/** : Contient le code source du jeu.

  - **Game1.cs** : Point d'entrée du jeu, gère la boucle principale et le rendu.
  - **Characters/** : Contient les classes liées aux personnages.
    - **Character.cs** : Classe représentant un personnage.
    - **Inventory.cs** : Classe gérant l'inventaire des personnages.
  - **Monsters/** : Contient les classes liées aux monstres.
    - **Monster.cs** : Classe représentant un monstre.
  - **Items/** : Contient les classes liées aux objets.
    - **Item.cs** : Classe représentant un objet.
    - **Equipment.cs** : Classe représentant des objets équipables.
  - **Map/** : Contient la classe gérant la carte du jeu.
    - **Map.cs** : Classe pour charger et rendre la carte.
  - **Utils/** : Contient des méthodes utilitaires.
    - **Helpers.cs** : Méthodes utilitaires pour le jeu.

- **ThirdRun.csproj** : Fichier de configuration du projet pour MonoGame.

## Instructions de configuration

1. Clonez le dépôt sur votre machine locale.
2. Ouvrez le projet dans votre environnement de développement.
3. Assurez-vous que toutes les dépendances de MonoGame sont installées.
4. Ajoutez vos propres sprites et cartes dans les dossiers appropriés.
5. Compilez et exécutez le projet pour commencer à jouer.

## Comment jouer

- Créez un groupe de personnages et équipez-les avec des objets.
- Laissez vos personnages se déplacer automatiquement sur la carte.
- Attaquez les monstres pour gagner de l'expérience et collecter du loot.
- Gérez l'inventaire et équipez des objets pour améliorer les compétences de vos personnages.

Amusez-vous bien dans votre aventure RPG !
