# VeloGames - Library App made with Unity Game Engine

Made with Unity 2022.3.0f1 LTS 

----------
<!-- TABLE OF SECTIONS -->
  # Sections
  <ol>
	  <li><a href="#Introduction">Introduction</a></li>
	  <li><a href="#Videos">Videos</a></li>
	  <li><a href="#Technical Details">Technical Details</a></li>
	  <li><a href="#Acknowledgements">Acknowledgements</a></li>
  </ol>

----------

<!-- INTRODUCTION -->
## Introduction
A task project assigned by Velo Games.

A simple library app that lets you do simple library stuff.

More info in TaskDescription-tr.pdf in the root folder.

----------

<!-- Videos -->
## Videos


21.01.2024 working build: 

Initially the project had different UI panels for every individual function.

[![Youtube Link](https://img.youtube.com/vi/DhB6S_IOklg/0.jpg)](https://youtu.be/DhB6S_IOklg)

30.01.2024 working build: 

Remade the whole UI setup to handle most operations in PopupPanelUI and ListPanelUI.

[![Youtube Link](https://img.youtube.com/vi/TtaJFg9EAHs/0.jpg)](https://youtu.be/TtaJFg9EAHs)

05.02.2024 Working windows build:

UI sounds, colors added. Admin panel added. Main panel now shows the time, sound can be turned on or off, saved in the player prefs. Supports 21.9 ultra wide displays and more: 

[![Youtube Link](https://img.youtube.com/vi/bxVtJ_iJaIY/0.jpg)](https://youtu.be/bxVtJ_iJaIY)

----------

<!-- Technical Details -->
## Technical Details

-Uses state machine to contextually arrange list / popup windows and searchManager.cs

-Uses scriptable objects for data persistance (in editor only)

-Can use json to import-export data

Feedback Optimizations: 

-Naming conventions altered. Changed Unity's naming conventions to C# standards and conventions.

-AssetDatabase usage revoked. Now altered the json import/export feature to handle saves on program start / closure to make the build possible.

----------

<!-- Acknowledgements -->
## Acknowledgements

A non profit project. 

Assets in use:

MouseClick - mouse-click-153941 - CC0 - https://pixabay.com/sound-effects/mouse-click-153941/

WarningSound - CC0 - https://pixabay.com/sound-effects/error-when-entering-the-game-menu-132111/

SuccessSound - interface-124464 - CC0 - https://pixabay.com/sound-effects/interface-124464/

ErrorSound - CC0 - https://pixabay.com/sound-effects/error-126627/

Sound on / off image - CC0 - https://opengameart.org/content/sound-onoff-images