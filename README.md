# 🌍 WorldBuilding-CrossingCraft

_A project by BONHOMMÉ Mailis, GARNIER Thomas, GÉLÉBART Lauriane, MORICE Romane_

_Supervised by LIORET Alain_

## ℹ️ Project Description

Crossing Craft is a student project by engineering students in their fifth year of the IMAC program at ESIEE Paris. It was carried out as part of an artificial intelligence course taught by Alain Lioret, a professor and researcher specializing in artificial intelligence applied to artistic creation. This project is an artificial life simulation that revolves around the creation of a diverse and colorful universe where creatures live and evolve.

The universe of Crossing Craft is a post-apocalyptic world where humans have given way to much cuter creatures. The world is divided into two biomes: the Forest and the Desert. Each biome has its own unique environments and types of creatures.

From a technical standpoint, the project relies on the Wave Function Collapse algorithm, which generates dynamic environments based on base models and their variations, offering over 240 possibilities for each cell of the world. With each new generation of the environment, a new universe is created—sometimes barren, sometimes lush.

The evolution of the creatures is managed by a genetic algorithm, which uses a 21-bit genome to define physical attributes such as the size and color of their tentacles. The genome also determines other attributes like health points and speed, giving some individuals a better chance to survive and reproduce in the universe.

Creatures interact with their environment by moving around to search for food based on proximity and their adaptation to their original biome.

The project also includes technical elements such as an immersive camera, a water shader based on Perlin Noise, and a carefully designed sound ambiance with spatialized audio.

![image](https://github.com/user-attachments/assets/5785a890-fb4c-4579-aab3-d7b2959f54bb)


## ⚙️ Table of Contents

- [📚 Technologies](#-technologies)
- [🛠️ Installation](#-installation)
  - [💻 Setting Up the Environment](#-setting-up-the-environment)
  - [🎮 Running the Project](#-running-the-project)
- [🛠️ Contributing](#-contributing)
  - [👨‍💼 Workflow](#-workflow)
  - [🔄 Branch Naming](#-branch-naming)
  - [🔖 Commit Naming](#-commit-naming)
  - [✅ Contribution Steps](#-contribution-steps)
- [📝 Naming Convention](#-naming-convention)

## 📚 Technologies

- **Unity**: Game development engine.
- **C#**: Programming language used in Unity.
- **Visual Studio**: IDE for development.

## 🛠️ Installation

### 💻 Setting Up the Environment

You need to have **Unity 2022.3.45f1** to run this project.

#### Install Unity:
- Download Unity Hub from the [official Unity website](https://unity.com/download).
- Open Unity Hub and install **Unity 2022.3.45f1** or a compatible version.
- Once installed, open the Unity project through Unity Hub.

#### Install Dependencies:
- Ensure that you have **Visual Studio** installed with the necessary Unity components.
- You can install Visual Studio through Unity Hub by selecting it as the default IDE during the Unity installation process.

### 🎮 Running the Project:
- Clone the repository to your local machine.
- Open the project via Unity Hub or directly through the Unity Editor.
- Unity will prompt you to import any necessary packages; click **Import** to proceed.
- Once everything is set up, hit the **Play** button in the Unity Editor to run the project.

## 🛠️ Contributing

### 👨‍💼 Workflow

![Workflow](https://camo.githubusercontent.com/aaf2db7f0930e69e7949c815b89844b781690d36f1c2d09173a2660b1bb604ba/68747470733a2f2f74686570726163746963616c6465762e73332e616d617a6f6e6177732e636f6d2f692f676b33796b307532346b3538343966706c7979322e706e67)

💡 For better visualization, it is recommended to use [Git Fork](https://git-fork.com/) or [Git Kraken](https://www.gitkraken.com/).

### 🔄 Branch Naming
- Branch names should be in **snake_case**.
- Use **name abbreviations** (first letter of first name + first letter of last name + last letter of last name).
    - Example: Lauriane GELEBART --> LGT
- In creating branches, distinguish between functionalities: `feature/` for features and `bugFix/` for small corrections on development features.

Examples: `feature/LGT_name_of_the_feature` or `bugFix/LGT_name_of_the_bugFix`

### 🔖 Commit Naming
- Commit names should follow the following format: **action verb + short description**
  - Example: `add create terrain generation` or `update player movement script`

### ✅ Contribution Steps
- Pull the latest version of the project: `git pull`   ⚠️
- Create a branch from the latest version: `git checkout -b feature/ABC_my_new_feature`
- Commit your changes: `git commit -m 'add a new feature'`
  - 💡 You can add co-authors with: `git commit -m "Regular commit message" -m "Co-authored-by: githubName <someemail@example.com>`
- Push your branch: `git push origin feature/ABC_my_new_feature`

If possible, use the **Rebase** feature to reduce the number of commits and only include pertinent ones.

💡 Note that all these actions can be performed on Git Fork or Git Kraken for better visualization.

## 📝 Naming Convention

Source: [C# Coding Standards and Naming Conventions](https://github.com/ktaranov/naming-convention/blob/master/C%23%20Coding%20Standards%20and%20Naming%20Conventions.md)

| Object Name               | Notation   | Length | Plural | Prefix | Suffix | Abbreviation | Char Mask          | Underscores |
|:--------------------------|:-----------|-------:|:-------|:-------|:-------|:-------------|:-------------------|:------------|
| Namespace name            | PascalCase |    128 | Yes    | Yes    | No     | No           | [A-z][0-9]         | No          |
| Class name                | PascalCase |    128 | No     | No     | Yes    | No           | [A-z][0-9]         | No          |
| Method name               | PascalCase |    128 | Yes    | No     | No     | No           | [A-z][0-9]         | No          |
| Method arguments          | camelCase  |    128 | Yes    | No     | No     | Yes          | [A-z][0-9]         | No          |
| Local variables           | camelCase  |     50 | Yes    | No     | No     | Yes          | [A-z][0-9]         | No          |
| Constants name            | PascalCase |     50 | No     | No     | No     | No           | [A-z][0-9]         | No          |

