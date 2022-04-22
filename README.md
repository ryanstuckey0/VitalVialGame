![Game Logo](docs/images/Title.png)
# Vital Vial - A Game By Viral Studios

This is our project that we did for our capstone class, CSE 5912, at The Ohio State University.

We worked together in a group of four to complete the game in a single semester. Most of the assets used in the project were ones that our professor had from the Unity Store, but we modified them as necessary to make them work for us.

Members of our team were : [Matthew Crabtree](https://github.com/OneMoreOneUp), [Jared Lawson](https://github.com/jaylawson), [Zhihan Li](https://github.com/ZH-project), and [Ryan Stuckey](https://github.com/ryanstuckey0)

---
## Experience

In making the game, my main roles were working on everything to do with the player. This includes:
- Basic player actions, like jumping, moving, etc.
- Advanced actions, like firing weapons, using abilities, and controlling animation states
- Implementing special abilities (from tech tree)
- Implementing all weapons (guns, melee, turrets, grenades, etc.)

In doing this, I got a lot of experience in multiple areas in Unity and C#. Some important features and concepts I worked with were:
- C# events- Used a lot for inter-class communication and to reduce coupling between classes; heavily used our custom [EventManager.cs](Assets/Scripts/Utilities/EventManager.cs) to control events
- Unity's Animator Controller- While the player asset we used came with an animation controller, I modified it pretty heavily to simplify it and make it work for us
- Unity's new Input System- This is how we support both keyboard and controller input; we used various input mappings that update based on what the player is doing (the logic for that can be seen in [PlayerInputController.cs](Assets/Scripts/Player/MonoBehaviourScript/PlayerInputController.cs))
- Unity Coroutines- Used in nearly every part of our project to control things like animation, weapon firing, or ability cooldowns. I made the [CoroutineRunner.cs](Assets/Scripts/Utilities/CoroutineRunner.cs) class to make it easier to control and work with coroutines.

---

## Contents:
- [Releases](https://github.com/ryanstuckey0/VitalVialGame/releases) - for the most recent working build of the game
- [Docs](docs) - for documentation on how to play, some technical aspects, and more
- [Source Code](Assets/Scripts) - to see all the code we wrote for this project