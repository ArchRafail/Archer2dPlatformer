# Archer2dPlatformer
Scripts for Unity 2d-game platformer - Archer.<br>
</br>
<img src="https://github.com/ArchRafail/Archer2dPlatformer/blob/b058e1b919afe7d19f272b6cd9ac6134405d503d/Archer2DCover.jpg"></br>
</br>
### About game
2D game platformer with AD (left-right) controls, space button to jump and left mouse button for firing from bow.</br>
You manage archer with bow and three points of life.</br>
On your way you will faced with 2 types of enemies: melee zombie and mage zombie.</br>
Your target is to reach the end of the level and to collect all or almost all coins and diamonds.</br>
There are 2 end in the game: "You win" and "Game Over".</br>
If you die from different reason you will see result: "Game Over".</br>
If you reach the end of the level you will see "You win" with stars plate with grey stars and:</br>
- 1 colored star (collected from 10 to 24 coins);</br>
- 2 colored stars (collected from 25 to 34 coins);</br>
- 3 colored stars (collected more than 35 coins).</br>
</br>
In total you can collect 40 coins:</br>
- 1 coin from coin.</br>
- 2 coins from heart. Players health poins should be full before picking.</br>
- 10 coins from diamond.</br>
</br>
### About enemies
There are 2 type of enemies melee zombie and mage zombie.</br>
<strong>Melee zombie</strong> has to come close to you to hit you.</br>
It detect distance not so long. But it can interrupt your shot.</br>
If you run from them, they will also run from you.</br>
Their patrol area is larger than mages.</br>
Melee zombie has 3 points of health, so keep your distance from them.</br>
<strong>Mage zombie</strong> will try to shoot you from larger distance.</br>
It detect distance are longer than melee zombie, so it try to run from you if you came close.</br>
After each detect of the player, they will cast fireball and retreat for a few distance to hide from you.</br>
Mage zombie has 2 points of health that's why better stay closer and to jump from their fireball.</br>
</br>
### Other
There are 2 scenes in the game - Menu and Game.</br>
On MenuScene locate 4 buttons: New Game, Control Elements, Credits and Quit.</br>
GameScene player character, enemies , informations with players health points and totall collected coins.</br>
