<img src="docs/images/Title.png" alt="Vital Vial, our game logo" style="display:block; margin-right:auto; margin-left:auto" width=75%>

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

In doing this, I got a lot of experience in multiple areas in Unity and C#. Some important technical features and concepts I worked with were:

- C# events- Used a lot for inter-class communication and to reduce coupling between classes; heavily used our custom [EventManager.cs](Assets/Scripts/Utilities/EventManager.cs) to control events.
- Unity's Animator Controller- While the player asset we used came with an animation controller, I modified it pretty heavily to simplify it and make it work for us.
- Unity's new Input System- This is how we support both keyboard and controller input; we used various input mappings that update based on what the player is doing (the logic for that can be seen in [PlayerInputController.cs](Assets/Scripts/Player/MonoBehaviourScript/PlayerInputController.cs)).
- Unity Coroutines- Used in nearly every part of our project to control things like animation, weapon firing, or ability cooldowns. I made the [CoroutineRunner.cs](Assets/Scripts/Utilities/CoroutineRunner.cs) class to make it easier to control and work with coroutines.

Additionally, working on a team with like this also gave me the opportunity to work on a number of soft skills:
- Git & GitHub- We used git and GitHub as our version control method. Through this, I was able to get better at making changes while the rest of my team was also committing to the same repo. To keep our repo organized, we locked the main branch and made it so each member had to make a PR to merge code into the main branch. Each PR also had to be approved by multiple team members.
- Agile Methodology & Jira- We used a Jira board to track all issues and bugs in the project based on two week sprints. We had biweekly sprint planning meetings and had to decide what work to do in the sprints based on the story points needed for each issue.
- Public Speaking & Presentation- As mentioned above, we had two week sprints. At the end of each sprint, we had to present our updates to the class. As a result, I was greatly able to improve both my presentation-making and public speaking skills.

---
## Contents:

- [Releases](https://github.com/ryanstuckey0/VitalVialGame/releases) - for the most recent working build of the game
- [Docs](docs) - for documentation on how to play, some technical aspects, and more
- [Source Code](Assets/Scripts) - to see all the code we wrote for this project

---
## Sprints
As mentioned above, we had to present at the end of each two week sprint. The presentations for those sprints are linked below:

| Sprint| Accomplishments |
| :---: | :--- |
| [Sprint 1](https://1drv.ms/p/s!AqyxDV2qMP3lhNxBuKw5Yz_L5nK2vQ?e=QwzdRB) | Planned out what we wanted for our game; made basic game state machine |
| [Sprint 2](https://1drv.ms/p/s!AqyxDV2qMP3lhNxCWTr3PsRUjF1bPw?e=fTXcy2) | Further fleshed out gameplay loop; implemented first player ability (time freeze); added simple NPC behavior; created Main Menu draft UI & Logic |
| [Sprint 3](https://1drv.ms/p/s!AqyxDV2qMP3lhNxDrHZ_VoOigie08Q?e=rhetE8) | Found and add player model with walking; dodge animations; added mind control player ability; mapped out full tech tree; added in game player HUD, planned out procedural terrain generation |
| [Sprint 4](https://1drv.ms/p/s!AqyxDV2qMP3lhNxFpFjyUz2Jf3Pkyw?e=lJGgKF) | Implemented player aiming, melee, and range weapons; made abilities load from JSON files; planned out initial PCG world tile system; created initial tech tree UI mockups |
| [Sprint 5](https://1drv.ms/p/s!AqyxDV2qMP3lhNxHEFTkrePGZ-kwOg?e=TTkrQU) | Improved gun aiming; tested out new camera style; moved gun configuration to JSON files, improved ability animations; added shock wave, blink, and elemental attack abilities; moved to use animation instancing for enemy rendering; initial implementation of procedurally generated cities; implemented sound volume options and key rebinding |
| [Sprint 6](https://1drv.ms/p/s!AqyxDV2qMP3lhNxI7oWd_juHH6GVbQ?e=XaUML9) | Drastically reimagined our gameplay loop to be more like a top-down Call of Duty zombie shooter; added throwable/placeable weapons, including proximity mines and turrets; added health boost ability; improved existing abilities; added passive human abilities; hand-created map using Unity Store assets; implemented buyable doors; improvements to save & load system |
| [Sprint 7](https://1drv.ms/p/s!AqyxDV2qMP3lhNxGNu9hNXWfhleUhg?e=pmsGUR) | utilized object pooling in player subsystems to improve performance; added sounds to player, including footstep, death, and hurt sounds; improved user experience when leveling up with new animations & sounds; added and improved gun and turret visual effects; added ability cooldown indicator on UI; added lights to map; added pickups dropped by enemy for player; stylized menus and UI; implemented high scores leaderboard, wave count, inventory UI, loading, and save screens |