# 2D Platformer

<img width="1057" height="793" alt="image" src="https://github.com/user-attachments/assets/26eed458-73ee-48c8-9a6e-3a88fc9146f7" />

A classic 2D platformer game built in Unity featuring multiple levels, smooth character animations, and dynamic camera movement.

## Features

### Core Gameplay
- **Multi-Level System**: Three progressively challenging levels with a level selection menu that unlocks as you complete each stage
- **Smooth Character Movement**: Physics-based player controller using Rigidbody2D with jumping, double-jumping, and collision detection via Box Collider 2D
- **Sprite Animation**: Fully animated player character with idle, run, jump, and fall states controlled by Unity's Animator system

### Technical Highlights
- **Cinemachine Camera**: Dynamic camera system that smoothly follows the player throughout the level
- **Parallax Background**: Background sprites follow the main camera with parallax effect for depth perception
- **Audio System**: Integrated SFX for jumps, damage, and other gameplay events
- **Progress Saving**: PlayerPrefs-based save system that remembers unlocked levels between sessions

## Controls
- **A/D or Arrow Keys**: Move left/right
- **Space**: Jump (double jump available)
