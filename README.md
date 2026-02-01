<div align="center">

# 🧠 VR Phobia Treatment Simulation

### *Conquering Fears Through Virtual Reality*

<br>

[![Unity](https://img.shields.io/badge/Unity-2022.3+-000000?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
[![VR](https://img.shields.io/badge/Meta%20Quest-00B0FF?style=for-the-badge&logo=oculus&logoColor=white)](https://www.meta.com/quest/)
[![XR Toolkit](https://img.shields.io/badge/XR%20Toolkit-2.6.5-purple?style=for-the-badge)](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.6/manual/index.html)
[![License](https://img.shields.io/badge/License-MIT-00C853?style=for-the-badge)](LICENSE)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)

<br>

<img src="https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png" width="100%">

<br>

> *"Face your fears in a safe virtual environment, overcome them in reality."*

<br>

[🎯 About](#-about) •
[🌟 Features](#-features) •
[🚀 Getting Started](#-getting-started) •
[🎮 Controls](#-controls) •
[📁 Architecture](#-architecture) •
[👥 Team](#-team)

</div>

<br>

---

<br>

## 🎯 About

<table>
<tr>
<td width="60%">

**VR Phobia Treatment Simulation** is an innovative **Exposure Therapy** application designed to help individuals overcome their fears using cutting-edge Virtual Reality technology.

Millions of people worldwide suffer from phobias that significantly impact their daily lives. Traditional therapy methods can be expensive, inaccessible, or impractical. Our solution brings professional-grade exposure therapy directly to the user through immersive VR experiences.

Users can confront their fears in a **completely safe, controlled virtual environment**, gradually building tolerance and developing coping mechanisms at their own pace.

</td>
<td width="40%">

### 🎪 Supported Phobias

| Phobia | Status |
|--------|--------|
| 🏔️ **Acrophobia** | ✅ Complete |
| 🕷️ **Arachnophobia** | ✅ Complete |
| 🚪 **Claustrophobia** | ✅ Complete |

</td>
</tr>
</table>

<br>

---

<br>

## 🌟 Features

<div align="center">

### 🏔️ *Acrophobia Module - Heights*

</div>

<table>
<tr>
<td width="50%">

#### 🌲 Natural Environment - Valley Bridge

Experience crossing a suspension bridge over a deep canyon surrounded by nature.

- **Level 1:** Safe bridge crossing - Beginner friendly
- **Level 2:** High platform challenge - Advanced exposure

</td>
<td width="50%">

#### 🏙️ Urban Environment - Skyscraper

Face your fear of heights on a modern skyscraper rooftop.

- Glass floor platform experience
- Panoramic city views
- Progressive difficulty settings

</td>
</tr>
</table>

<br>

<div align="center">

### 🕷️ *Arachnophobia Module - Spiders*

</div>

<table>
<tr>
<td align="center">

🕸️ Controlled spider exposure with gradual intensity levels

Start with distant, static spiders and progress to closer, more realistic encounters

</td>
</tr>
</table>

<br>

<div align="center">

### 🚪 *Claustrophobia Module - Enclosed Spaces*

</div>

<table>
<tr>
<td align="center">

📦 Shrinking room experience with customizable compression rates

Experience enclosed spaces that gradually become smaller, building tolerance over time

</td>
</tr>
</table>

<br>

---

<br>

<div align="center">

### ⚙️ *Therapeutic Safety Features*

</div>

<br>

<table>
<tr>
<td align="center" width="25%">

### 🛡️
**Safety Rails**
<br><br>
Toggle-able guardrails for comfort during height exposure

</td>
<td align="center" width="25%">

### 🔄
**Fall Respawn**
<br><br>
Automatic safe recovery when falling occurs

</td>
<td align="center" width="25%">

### 🌑
**Vignette Effect**
<br><br>
Screen darkening as fear indicator and motion sickness prevention

</td>
<td align="center" width="25%">

### 💓
**Heartbeat Audio**
<br><br>
Immersive heartbeat sound that intensifies with proximity to fear triggers

</td>
</tr>
</table>

<br>

---

<br>

## 🚀 Getting Started

### Prerequisites

| Requirement | Version |
|------------|---------|
| Unity | 2022.3 LTS or higher |
| XR Interaction Toolkit | 2.6.5+ |
| VR Headset | Meta Quest / HTC Vive / Any OpenXR Compatible |

<br>

### Installation

```bash
# Clone the repository
git clone https://github.com/aliturhan0/phobia_treatment_simulation.git

# Open with Unity Hub
# 1. Launch Unity Hub
# 2. Click "Add" button
# 3. Navigate to the cloned folder
# 4. Select and open the project
```

<br>

### First Run Setup

1. Go to **Window → Package Manager**
2. Install **XR Interaction Toolkit** if not present
3. Go to **Edit → Project Settings → XR Plug-in Management**
4. Enable your VR platform (Oculus, OpenXR, etc.)
5. Connect your VR headset
6. Press **Play** ▶️

<br>

---

<br>

## 🎮 Controls

<div align="center">

| Action | Control |
|:------:|:-------:|
| **Movement** | Left Joystick |
| **Teleport** | Right Joystick |
| **Interact** | Trigger Button |
| **Toggle Safety Rails** | A Button |
| **Open Menu** | Menu Button |

</div>

<br>

---

<br>

## 📁 Architecture

```
📦 phobia_treatment_simulation
 ┣ 📂 Assets
 ┃ ┣ 📂 ali_assetler          # Core scripts and assets
 ┃ ┃ ┣ 📂 Art                 # Materials, textures, UI graphics
 ┃ ┃ ┣ 📂 Audio               # Sound effects (heartbeat, footsteps)
 ┃ ┃ ┣ 📂 Editor              # Unity Editor extensions
 ┃ ┃ ┣ 📂 UI                  # UI components and hover effects
 ┃ ┃ ┣ 📜 LevelManager.cs           # Level progression system
 ┃ ┃ ┣ 📜 FallRespawnManager.cs     # Fall detection & respawn
 ┃ ┃ ┣ 📜 SafetyRailManager.cs      # Safety rail toggle system
 ┃ ┃ ┣ 📜 MainMenuManager.cs        # Main menu controller
 ┃ ┃ ┣ 📜 RoomTeleportButton.cs     # Room teleportation
 ┃ ┃ ┣ 📜 BridgeFootsteps.cs        # Footstep audio system
 ┃ ┃ ┗ 📜 ...
 ┃ ┣ 📂 Scenes
 ┃ ┃ ┣ 🎬 MainMenu.unity      # Main menu scene
 ┃ ┃ ┣ 🎬 ozcan2.unity        # Phobia rooms scene
 ┃ ┃ ┗ 🎬 ...
 ┃ ┗ 📂 XR                    # XR configuration
 ┣ 📂 Packages
 ┣ 📂 ProjectSettings
 ┗ 📜 README.md
```

<br>

---

<br>

## 🛠️ Tech Stack

<div align="center">

<table>
<tr>
<td align="center" width="150">
<img src="https://cdn.worldvectorlogo.com/logos/unity-69.svg" width="50" height="50" alt="Unity"/>
<br><b>Unity</b>
<br><sub>Game Engine</sub>
</td>
<td align="center" width="150">
<img src="https://cdn.worldvectorlogo.com/logos/c--4.svg" width="50" height="50" alt="C#"/>
<br><b>C#</b>
<br><sub>Programming</sub>
</td>
<td align="center" width="150">
<img src="https://img.icons8.com/color/96/000000/virtual-reality.png" width="50" height="50" alt="XR"/>
<br><b>XR Toolkit</b>
<br><sub>VR Framework</sub>
</td>
<td align="center" width="150">
<img src="https://upload.wikimedia.org/wikipedia/commons/a/a5/Archlinux-icon-crystal-64.svg" width="50" height="50" alt="Meta"/>
<br><b>OpenXR</b>
<br><sub>VR Standard</sub>
</td>
</tr>
</table>

</div>

<br>

---

<br>

## 👥 Team

<div align="center">

<table>
<tr>
<td align="center" width="250">

### 👨‍💻 Ali Turhan
**Developer & Designer**

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=flat-square&logo=github&logoColor=white)](https://github.com/aliturhan0)

</td>
<td align="center" width="250">

### 👨‍💻 Şinasi Onuralp Akkurt
**Developer & Designer**

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=flat-square&logo=github&logoColor=white)](https://github.com/squichip)

</td>
<td align="center" width="250">

### 👨‍💻 Özcan Yıldıral
**Developer & Designer**

[![GitHub](https://img.shields.io/badge/GitHub-100000?style=flat-square&logo=github&logoColor=white)](https://github.com/)

</td>
</tr>
</table>

</div>

<br>

---

<br>

## 📄 License

<div align="center">

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

<br>

---

<br>

### 💡 *"The only thing we have to fear is fear itself."*
**— Franklin D. Roosevelt**

<br>

<img src="https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png" width="100%">

<br>

**⭐ If you found this project helpful, please consider giving it a star! ⭐**

<br>

Made with ❤️ and Unity

*2025 - VR Phobia Treatment Simulation*

</div>
