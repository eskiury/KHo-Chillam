# K'Ho Chillam | C# Tower Defense Game (Global Game Jam 2026)

<p align="center">
  <img src="https://img.shields.io/badge/Engine-Godot-blue.svg?style=for-the-badge&logo=godotengine" alt="Godot Engine">
  <img src="https://img.shields.io/badge/Language-C%23-green.svg?style=for-the-badge&logo=c-sharp" alt="C#">
  <img src="https://img.shields.io/badge/Event-Global%20Game%20Jam%202026-orange.svg?style=for-the-badge" alt="GGJ 2026">
</p>

## 🕹️ Overview
K'Ho Chillam is a strategic Tower Defense game developed in **only 3 days** during the Global Game Jam (MalagaJam) 2026, under the core theme "Masks". Built using the Godot Engine and C#, the project showcases rapid technical adaptation, entity management optimization, and modular data-driven architectural guidelines under strict time constraints.

---

## 📽️ Gameplay & Media
<img width="45%" alt="Gameplay 0" src="https://github.com/user-attachments/assets/cba69c14-759a-4e73-b889-5cdb080588d9" />
<img width="45%" alt="Gameplay 1" src="https://github.com/user-attachments/assets/e1f3bfb0-3300-401e-8ae6-dc3ab8d78546" />


---

## 🛠️ Technical Achievements & Architecture

### 📊 Data-Driven Design & Balancing
To ensure rapid iteration during a 72-hour game jam, rigid hardcoding was avoided in favor of structural flexibility:
*   **Modular Architecture:** Designed tower attributes, enemy waves, and economic scaling behaviors using decoupled data containers.
*   **Rapid Balancing:** Enabled the design team to tweak parameters and values on the fly without breaking core gameplay loops or requiring code modifications.

### 👾 Optimized Entity Management System
Managing multiple enemy waves, tracking navigation targets, and monitoring projectiles simultaneously in real-time requires clean structural foundations:
*   Implemented a high-performance **Entity Management pipeline** in C# to pool, update, and clear active entities efficiently.
*   Decoupled combat state parameters from visual representations, minimizing resource allocations per frame loop.

### 🖥️ Dynamic UI & HUD Integration
*   Architected a highly responsive HUD system connecting active base states, monetary parameters, and tower selection cards natively in C#.
*   Utilized event-driven programming (Signals) to refresh UI layers only when data modifications occur, completely avoiding expensive updates inside the main frame process.

---

## ⚡ The Engineering Challenge: Rapid Technical Adaptation
**The Problem:** Developing a complete, balanced tactical game loop in under 72 hours usually leads to unstable spaghetti code, performance drops during crowded waves, and broken build branches.

**The Solution:** 
*   Utilized strict Object-Oriented Programming (OOP) layouts in C# to ensure clear separation of concerns between game managers, enemies, and towers.
*   Applied early-out branching rules inside target acquisition functions to prevent grid lockups when computing turret sight bounds.
*   **Result:** Delivered a polished, fully functional, and playable jam entry featuring stable entity flows and dynamic UI responses within the 3-day deadline.

---

## 📂 Project Structure
*   **Engine:** Godot Engine (C# Edition)
*   **Language:** C#
*   **Project Layout:** Data-driven Game Loop, Custom Entity Manager, Signals-driven HUD

## 🕹️ Play the Game
**Experience the jam build on itch.io:** 👉 [**Play K’Ho Chillam on itch.io**](https://f4loi.itch.io/kohchilam)
