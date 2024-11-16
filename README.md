# WorldBuilding-CrossingCraft

A project by BONHOMMÉ Mailis, GARNIER Thomas, GÉLÉBART Lauriane, MORICE Romane 

Supervised by LIORET Alain

// Add a description


## Table of Contents

- [Technologies](#technologies)
- [Installation](#installation)
  - [Setting Up the Environment](#setting-up-the-environment)
  - [Running the Project](#running-the-project)
- [Contributing](#contributing)
  - [Workflow](#workflow)
  - [Branch Naming](#branch-naming)
  - [Commit Naming](#commit-naming)
  - [Contribution Steps](#contribution-steps)
- [Naming in code](#naming-in-code)
  - [Variables](#variables)
  - [Naming Constants](#naming-constants)
  - [Methods](#methods)
  - [Classes](#classes)
  - [Utility Classes](#utility-classes)

## Technologies

- **Unity**: Game development engine.
- **C#**: Programming language used in Unity.
- **Visual Studio**: IDE for development.

## Installation

### Setting Up the Environment

You need to have **Unity 2022.3.45f1** to run this project.

#### Install Unity:
- Download Unity Hub from the [official Unity website](https://unity.com/download).
- Open Unity Hub and install **Unity 2022.3.45f1** or a compatible version.
- Once installed, open the Unity project through Unity Hub.

#### Install Dependencies:
- Ensure that you have **Visual Studio** installed with the necessary Unity components.
- You can install Visual Studio through Unity Hub by selecting it as the default IDE during the Unity installation process.

### Running the Project:
- Clone the repository to your local machine.
- Open the project via Unity Hub or directly through the Unity Editor.
- Unity will prompt you to import any necessary packages; click **Import** to proceed.
- Once everything is set up, hit the **Play** button in the Unity Editor to run the project.


## Contributing

### Workflow
![Workflow](https://camo.githubusercontent.com/aaf2db7f0930e69e7949c815b89844b781690d36f1c2d09173a2660b1bb604ba/68747470733a2f2f74686570726163746963616c6465762e73332e616d617a6f6e6177732e636f6d2f692f676b33796b307532346b3538343966706c7979322e706e67).

Here is the project's workflow that must be strictly followed.

💡 For better visualization, it is recommended to use [Git Fork](https://git-fork.com/) or [Git Kraken](https://www.gitkraken.com/).

### Branch Naming 
- Branch names should be in **snake_case**
- Use **name abbreviations** (first letter of first name + first letter of last name + last letter of last name). 
    - Example: Lauriane GELEBART --> LGT
- In creating branches, distinguish between functionalities: `feature/` for features and `bugFix/` for small corrections on development features. 

Examples: `feature/LGT_name_of_the_feature` or `bugFix/LGT_name_of_the_bugFix`

### Commit Naming 
- Commit names should follow the following: **action verb + short description**
  - Example: `add create terrain generation` or `update player movement script`

### Contribution Steps
- Pull the latest version of the project: `git pull`   ⚠️
- Create a branch from the latest version: `git checkout -b feature/ABC_my_new_feature`
- Commit your changes: `git commit -m 'add a new feature'`
  - 💡 You can add co-authors with: `git commit -m "Regular commit message" -m "Co-authored-by: githubName <someemail@example.com>`
- Push your branch: `git push origin feature/ABC_my_new_feature`

If possible, use the **Rebase** feature to reduce the number of commits and only include pertinent ones.

💡 Note that all these actions can be performed on Git Fork or Git Kraken for better visualization.

## Naming in Code 

### Variables
- Use **camelCase** style for naming variables.
  - Examples: `playerHealth`, `itemCount`.

### Naming Constants
- Use **ALL_CAPS_SNAKE_CASE** style for naming global or class constants.
  - Example: `const float GRAVITY = 9.8f;`.

### Methods
- Also use **camelCase** style for naming methods, ideally starting with an action verb.
  - Examples: `spawnObject()`, `updatePosition()`.

### Classes
- Use **PascalCase** style for naming classes.
  - Examples: `PlayerController`, `GameManager`.

### Utility Classes
- Utility classes contain static methods or constants shared across the application.
  - Examples: `MathUtils`, `AudioManager`.
