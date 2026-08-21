<div align="center">
  <img src="Assets/Downloaded%20Model/Pictures/banner.png" alt="Doofus Goofus Banner" width="100%" />
  
# 🐼 Doofus Goofus

**A fast-paced, data-driven procedural platformer built in Unity.**

[![Play Video](https://img.shields.io/badge/Watch-Gameplay_Trailer-red?style=for-the-badge&logo=youtube)](#)
[![Play Game in Itch.io](https://img.shields.io/badge/Play_Game_in_Itch.io-blue?style=for-the-badge&logo=itch.io&logoColor=white)](#)

*Replace the links above with your actual YouTube video and Itch.io/Webgl links!*

</div>

---

## 🎮 About The Game

**Doofus Goofus** is an infinite procedural platformer where every second counts. The ground beneath your feet is constantly disappearing, and the only way to survive is to keep moving forward. 

With an emphasis on **"Game Feel"**, crisp mechanics, and data-driven game design, Doofus Goofus offers a highly polished core gameplay loop that tests both your reflexes and your quick decision-making.

<div align="center">
  
  *[PLACEHOLDER: Insert a cool GIF of the gameplay here! `<img src="link_to_gif" width="600"/>`]*
  
</div>

---

## ✨ Standout Technical Features

We didn't just build a game; we built scalable, professional game systems. Here is what makes the architecture of Doofus Goofus unique:

### ⚙️ Data-Driven Design (JSON Configs)
Game balance shouldn't require recompiling code. The entire difficulty curve—player speed, pulpit spawn rates, and decay times—is driven by a centralized `doofus_diary.json` configuration file. This allows designers to tweak the game's feel instantly without ever touching a C# script.

### 🦊 Dynamic Character Selection (Data Persistence)
Players can unlock and choose between **10 unique 3D characters** in the Main Menu. 
- Uses a robust **Child-Swapping** architecture to hot-swap 3D meshes without breaking physics or camera follow scripts.
- Integrates Unity's `PlayerPrefs` to ensure skin selections persist perfectly between sessions and scene loads.
- Utilizes **Render Textures** for the UI, creating a professional "live 3D preview" window in the 2D Main Menu.

### 🧠 Intelligent Spawning System
Platforms (Pulpits) don't just spawn randomly—they use an intelligent, mathematically-driven algorithm! 
- **Predictive Placement:** The `PulpitManager` calculates adjacent grid coordinates (North, South, East, West) to ensure the player always has a valid and reachable path forward, avoiding overlapping platforms.
- **Dynamic Variety:** Each pulpit is spawned with randomized Y-axis rotations to ensure the terrain looks organic and visually distinct.
- **Automated Memory Management:** A robust background system tracks active pulpits and automatically destroys them exactly as their countdown timer hits zero, guaranteeing buttery-smooth performance without memory leaks.

### 🎵 Centralized Singleton Audio Management
Features a dedicated `AudioManager` Singleton that listens to gameplay events (like Score updates and Game Overs) to dynamically crossfade music and trigger SFX, keeping audio logic entirely decoupled from player movement scripts.

### 🌟 Additional Polish & "Game Feel"
- **Ground Shake FX:** Adds tactile physical feedback during intense moments, making the world feel heavy and impactful.
- **Heartbeat Timer:** A pulsing, tension-building UI element that warns the player exactly when the platform beneath them is about to disappear.
- **Custom Screen Flow:** Beautiful, dedicated Main Menu and Game Over screens that seamlessly blend pre-rendered 2D backgrounds with live 3D elements.

---

## 📸 Screenshots

<p align="center">
  <img src="https://via.placeholder.com/400x225?text=Main+Menu+Screenshot" width="45%" />
  <img src="https://via.placeholder.com/400x225?text=Gameplay+Screenshot" width="45%" /> 
</p>
<p align="center">
  <img src="https://via.placeholder.com/400x225?text=Character+Selection+Screenshot" width="45%" />
  <img src="https://via.placeholder.com/400x225?text=Game+Over+Screenshot" width="45%" />
</p>

*(Replace these placeholder links with actual screenshots of your beautiful UI and Gameplay!)*

---

## 🛠️ Built With

* **Engine:** Unity 2022+ (URP)
* **Language:** C#
* **Architecture:** Singleton Pattern, Observer Pattern (Events/Delegates), Data-Driven Design (JSON)

---

<div align="center">
  
**Built with ❤️ by [Your Name] for the 2026 Game Developer Application.**

</div>
