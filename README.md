# K’Ho Chillam 🏹 - Global Game Jam 2026

K’Ho Chillam is a Tower Defense game developed in 48 hours for the **Global Game Jam 2026**. This project represented a personal challenge: adapting my low-level programming background to a modern engine (**Godot 4**) using **C#** under extreme time constraints.

## 🛠️ Tech Stack
* **Engine:** Godot 4.3 (C# / .NET version).
* **Language:** C#.
* **Architecture:** Component-based combat systems and data-driven level loading.

## 🚀 Technical Highlights
* **Engine Adaptation:** Successfully transitioned from custom C++ engines to Godot's node-based workflow and C# scripting in a high-pressure environment.
* **Modular Combat System:** Designed a scalable projectile and tower system using inheritance and decoupled logic.
* **Data-Driven Levels:** Implemented a JSON-based level loader to separate game data from engine logic.
* **Synergy Logic:** Engineered a status effect system that allows different shot types to interact and trigger combo effects.

## 👨‍💻 My Contributions (Programmer Role)
I was responsible for the core gameplay architecture, specifically:
* **The Projectile Framework:** Creating the base class and specific behaviors (bombs, rebounds, direct shots).
* **Level Loading:** Developing the system that parses JSON files into playable waves.
* **Game Management:** Handling the main game loop, wave transitions, and global state.

## 🔗 Code Highlights (Direct Links)
* [Projectile Base Class](src/Scripts/Game/Base/ProjectileClass.cs) - The foundation of the modular combat system.
* [Bomb Implementation](src/Scripts/Game/Projectile/BombProjectile.cs) - Advanced projectile behavior with area-of-effect logic.
* [Level Loader](src/Scripts/Game/Base/CargadorNiveles.cs) - JSON parsing and dynamic scene instantiation.
* [Game Manager](src/Scripts/Game/Base/GameManager.cs) - Central logic managing the tower defense flow.

## ⚠️ Note
This repository contains the source code developed during the jam. It is intended as a showcase of my **fast-learning capabilities** and **system design skills** in new technical environments.

## 🕹️ Play the Game
**Experience the jam build on itch.io:** 👉 [**Play K’Ho Chillam on itch.io**](https://f4loi.itch.io/kohchilam)
---
*Created by a team of 7 for the GGJ 2026.*