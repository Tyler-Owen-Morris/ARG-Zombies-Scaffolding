
ORK Framework
http://orkframework.com


-------------------------------------------------------------------------------------------------------------------------------------------------------
Content
-------------------------------------------------------------------------------------------------------------------------------------------------------

- General Information
- Editor and Scene Wizard
- Tutorials
- Support
- Documentation
- Demo
- Package Description
- iOS Hints
- ORK Version Changelog



-------------------------------------------------------------------------------------------------------------------------------------------------------
General Information
-------------------------------------------------------------------------------------------------------------------------------------------------------

- DLL information
ORK Framework is included in your Unity project using DLLs.
This will dramatically increase your project's compile time if you change any of your own scripts.
The source code for gameplay related code is included - see the 'Package Description' section for details.

- ORK Project asset
When you first open the ORK Framework editor, a new ORK Project asset will be created.
This asset will contain all your ORK Framework settings made in the editor.
It's located at 'Assets/ORK Framework/ORKProject.asset'.

- Event assets
When you create events (e.g. game events, battle events, etc.), each event is stored in a separate file.
You can save the event assets anywhere in your Unity project.
To keep things organized, it's recommended to create a folder structure mirroring the purpose of the events.
E.g. separating game events by location.



-------------------------------------------------------------------------------------------------------------------------------------------------------
Editor and Scene Wizard
-------------------------------------------------------------------------------------------------------------------------------------------------------

- ORK Framework editor
In ORK Framework you'll create your game's data in the ORK Framework editor.
You can open the editor using Unity's menu: Window > ORK Framework.
You can also open the editor using the hot-key 'CTRL + ALT + O'.

The editor is separated into different sections, each section contains multiple sub-sections.
When saving, the editor will display what will change and, depending on what's been changed, offers to update events, scenes and prefabs.
When scenes are updated, please make sure to save your current scene first.

- ORK Scene Wizard
The ORK Scene Wizard is used to add your data to your scenes to create gameplay.
You can open the scene wizard using Unity's menu: Window > Scene Wizard.
You can also open the scene wizard using the hot-key 'CTRL + ALT + W'.



-------------------------------------------------------------------------------------------------------------------------------------------------------
Tutorials
-------------------------------------------------------------------------------------------------------------------------------------------------------

You can find different types of tutorials here:
http://orkframework.com/tutorials/

It's recommended to start with the Game Tutorials to learn using ORK Framework.
This series of tutorials covers creating a game with ORK Framework from start to finish.
You can find them here:
http://orkframework.com/tutorials/game/

If you're seeking information/help with a specific feature, take a look at the how-tos:
http://orkframework.com/tutorials/howto/

If you have any tutorial requests, don't hesitate and contact me:
contact@orkframework.com



-------------------------------------------------------------------------------------------------------------------------------------------------------
Support
-------------------------------------------------------------------------------------------------------------------------------------------------------

Need help or found a bug?

Please search for a solution in the ORK Community forum:
http://forum.orkframework.com/

Didn't find anything? Contact me:
contact@orkframework.com



-------------------------------------------------------------------------------------------------------------------------------------------------------
Documentation
-------------------------------------------------------------------------------------------------------------------------------------------------------

The documentation is built into the ORK Framework editor.
All settings are explained in the help window in the ORK Framework editor.

Information and help texts will be displayed if you hover with the mouse over a setting or foldout.
You can change this behaviour in the editor settings (Editor > Editor Settings).

There currently is no separate documentation available.



-------------------------------------------------------------------------------------------------------------------------------------------------------
Demo
-------------------------------------------------------------------------------------------------------------------------------------------------------

The demo.unitypackage file contains a demo project.

- Setting up the demo
To get the demo running, import the contents of demo.unitypackage into an empty project.
Make sure that ORK Framework is NOT imported before importing the demo.

Now, add all Unity scenes found in 'Assets/Tutorial Resources/Scenes/' to the project's build settings.
Use the Unity menu to open the build settings (File > Build Settings...) and drag the scenes on the 'Scenes In Build' area.

- Starting the demo
To start the demo, open the main menu scene (0 Main Menu) and press play.

- Controls
Use the 'Arrow' keys to move your player and change menu selections.
The 'Escape' key opens the menu. Use 'Enter' to accept and 'Right-Ctrl' to cancel.
In real time combat, use 'Space' to attack and 'Left-Ctrl' to open the battle menu.



-------------------------------------------------------------------------------------------------------------------------------------------------------
Package Description
-------------------------------------------------------------------------------------------------------------------------------------------------------

- Gizmos (folder)
Contains the gizmo icons (32x32) used by ORK Framework.

- DLL (folder)
3 DLLs containing ORKFramework.

- demo.unitypackage
Contains a complete demo project.
Please see the 'Demo' section for details.

- gameplay_source_code.zip
Contains the gameplay related source code and a plugin template as MonoDevelop projects.
Please read the included README.txt for details.
There currently is no separate documentation available.



-------------------------------------------------------------------------------------------------------------------------------------------------------
iOS Hints
-------------------------------------------------------------------------------------------------------------------------------------------------------

If you're building your project on iOS, you may run into some problems:

- Crash after splash screen: "You are using Unity iPhone Basic. You are not allowed to remove the Unity splash screen from your game"
To fix this, change in XCode's build settings the 'Compress PNG files' setting to NO and build the project again.

- Crash after splash screen: "Ran out of trampolines of type 2"
This is caused by use of generics, you have to tell the AOT compiler allocate more trampolines.
To fix this, change in Unity's Player Settings > Other Settings the 'AOT Compiler Options' to nimt-trampolines=512.
If you're still receiving a trampoline error, increase the number, e.g. 1024.

- Using the new UI (Unity 4.6+) and content is displayed outside of masks (e.g. text extends outside of GUI box).
Go to 'Edit > Project Settings > Player' and enable 'Use 24-bit Depth Buffer' in the iOS 'Resolution and Presentation' settings.


You can find more detailed instructions on the Unity trouble shooting page:

http://docs.unity3d.com/Documentation/Manual/TroubleShooting.html



-------------------------------------------------------------------------------------------------------------------------------------------------------
ORK Version Changelog
-------------------------------------------------------------------------------------------------------------------------------------------------------

Version 2.3.1:
- new: Combatant Spawners: 'Remember Combatants' setting available. Optionally remembers the spawned combatants (status and position) and respawn times when leaving the scene. The combatants will be respawned where they where when reloading the scene.
- new: Save Game Menu: 'Spawned Combatants' setting available in the 'Save Settings'. Optionally saves the combatants spawned by 'Combatant Spawner' components with 'Remember Combatants' enabled with the save game. Either saves the combatants of the 'Current' scene, 'All' scenes or 'None' scene at all.
- new: Input Keys: 'Invert Axis' setting available for all input origins. Optionally inverts the input axis, i.e. 1 will become -1, -1 will become 1.
- new: Weapons, Armors: 'Chance' settings available for random status value bonuses. Optionally only adds random bonuses to status values based on chance.
- new: Weapons, Armors: 'Add Bonus Range' setting available for random status value bonuses. Either adds 'All' random bonuses, a 'Random' bonus (no bonus is added if chance fails), or adds the 'First' bonus due do chance settings.
- new: Float Values: 'Random' value type available. Uses a random value between two defined values. Available throughout the framework.
- new: GUI Boxes: Choice Settings: 'Position' choice mode available. Optionally define positions for choices. When exceeding the defined number of positions, the remaining choices will be displayed in 'List' mode.
- new: HUDs: Combatant: 'Position' cell mode available for 'Shortcut' HUD elements. Optionally define the positions of each shorcut slot cell.
- new: HUDs: Combatant: 'Use Auto Target' setting available for 'Shortcut' HUD elements. Optionally uses shortcuts when clicked on automatically selected targets.
- new: HUDs: Combatant: 'Set Size' settings available for 'Shortcut' HUD element info labels. You can now optionally set the size of info labels used to display empty or assigned shortcut slots.
- new: HUDs: Combatant: 'Use Icon' settings available for 'Shortcut' HUD element info labels. Optionally gives more control over displaying a shortcut's icon (e.g. scale mode) than using the '%i' text code in text.
- new: HUDs: Tooltip: 'Use Bar' settings available. Tooltip HUDs can now display level points (ability, equipment) and durability (equipment) value bars instead of text.
- new: HUDs: Tooltip: New text codes available to display level points (ability, equipment) and durability (equipment) information.
- new: Event System: Scene Steps: 'Remove Spawned Combatants' setting available in the 'Remove Scene Data' step. Optionally removes stored data of spawned combatants from the scene. Useful if you e.g. want to clear all spawners when leaving a dungeon.
- new: Battle Events: Battle Steps: 'Consume Costs' step available. Consumes the use costs of an action (e.g ability use costs, consumes an item). The step will consume the costs each time it's used.
- new: Abilities, Items: Target Settings: 'Auto Target' settings available. Optionally uses status requirements and variable conditions to automatically select a target. This is used in battle menus to select the a combatant (still requires the player to accept the choice), control maps with auto target and AI target selection.
- new: Combatants: AI Settings: 'Auto Target' setting available. Uses the 'Auto Target' settings of a used ability or item when selecting an AI target. If not used by the ability/item or not found, the nearest (if enabled) or a random target will be used.
- new: Camera Events: 'In Blocked Controls' setting available. The camera event component will be executed while the player controls are blocked (e.g. in events). By default enabled.
- new: Save Game Menu: 'Add Cancel' settings available in the 'Save Game Menu' and 'Load Game Menu' settings. Adding the 'Cancel' choice to the file list is now optional. By default enabled.
- new: Menu Settings: 'Bonus Display' settings available. Define the text representation of status value bonuses and attack/defence attribute bonuses. Can be displayed in descriptions of bonus giving things using the '%bonus' text code.
- new: Status Effects, Passive Abilities, Weapons, Amrors, Combatants, Classes: '%bonus' text code available in descriptions. Displays the status value and attack/defence attribute bonuses as defined in the 'Bonus Display' settings.
- new: Text Codes: '#percent' text code available to display a '%' sign. Use this text code to display '%' in texts that use special text codes (e.g. '%' to display the quantity of an item).
- change: Click Controls: All 'Enable Double Click' settings have been replaced by 'Enable Clicking' and 'Click Count' settings. You can now define the number of clicks if needed. By default set to 2 clicks (i.e. restores the old double click setting).
- change: HUDs: Combatant: 'Enable Drop' no longer needs to be enabled to use drag+drop to change shortcuts in 'Shortcut' HUD elements.
- fix: Group: Getting average group levels/turns no longer throws errors when no members are in a group.
- fix: Real Time Battle Areas: Using colliders to limit the area could result in the battle not starting when spawning inside the area due to first entering with the interaction controller.
- fix: Menu Screens: Equipment, Inventory: 'Durability' HUD info displays will now also be displayed when 'Enable Dragging', 'Enable Double Click' and 'Enable Tooltip' are disabled.
- fix: Move AIs: Close distance target position checks could lead to the check interval not being reset, resulting in the combatant not using an updated target position.
- fix: Mounting Objects: Setting the scale when mounting an object on another object will now use the correct scale in relation to the parent object.


Version 2.3.0:
- new: Group Abilities: Abilities can now optionally be learned by the group of a combatant instead of a single combatant. Group abilities are available to all members of the group and can be used like normal abilities.
- new: Abilitiy Learning: 'Group Ability' setting available. Optionally learns the ability to the combatant's group instead to the combatant. Available throughout the framework.
- new: Abilitiy Forgetting: 'Group Ability' setting available. Optionally forgets the ability of the combatant's group instead of the combatant. Available throughout the framework.
- new: Shops: 'Abilities' settings available. Shops can now also sell abilities and group abilities. A bought ability will be learned by the shop's user combatant, or the user's group (when using group abilities). An ability can only be sold once per combatant/group, there is no quantity selection displayed.
- new: Status Effect Types: 'Status Effect Types' sub-section now available in the 'Status' section. Status effect types can be used to separate status effects. Effect changes can optionally use all status effects of a selected type, the HUD can optionally display only selected status effect types.
- new: Status Effects: 'Status Effect Type' setting available. Defines the status effect type of the status effect. The type is used to separate status effects, e.g. when displayed in a HUD or change all effects of a selected type.
- new: Status Effect Changes: 'Use Type' and settings available when changing status effects (e.g. ability status changes, auto effects, etc.). Optionally checks uses all status effects of a selected status effect type when changing effects (e.g. remove all 'Negative' type effects).
- new: Status Requirements: 'Status Effect Type' status check available. Checks if a status effect of the selected status effect type is or isn't applied to the combatant.
- new: Status Requirements: 'Weapon Item Type' and 'Armor Item Type' status checks available. Checks if a weapon/armor of the selected item type is or isn't equipped on the combatant.
- new: Classes, Combatants, Passive Abilities, Weapons, Armors, Status Effects: 'Attack Status Effects' settings available. Abilities and items can automatically use the user's attack status effect changes on the target.
- new: Classes, Combatants, Passive Abilities, Weapons, Armors, Status Effects: 'Defence Status Effects' settings available. Abilities and items can automatically use the target's defence status effect changes on the user.
- new: Abilities, Items: 'Use Attack Effects' and 'Use Defence Effects' settings available for value changes of active abilities and items. Optionally uses the user's 'Attack Status Effects' and the target's 'Defence Status Effects'. By default disabled.
- new: Weapons, Armors: 'Durability' settings available for the different level settings of equipment. A weapon or armor can optionally have a durability and wear off. If an equipment is outworn (i.e. durability reaches 0), it can either not give any bonuses any more, be unequipped or destroyed. Durability can be changes using the event system.
- new: Weapons, Armors: 'Own Outworn Text' settings available in the equipment's console texts. Optionally overrides the default console text displayed when an equipment's durability reaches 0.
- new: Input Keys: 'Custom' input origin settings available. Input keys can now use custom scripts to get button and axis values. Calls a static function of a class using reflection - button function must return a bool value, axis function must return a float value.
- new: Status Values: 'From Minimum' setting available for 'Experience' type status values. Optionally restarts the status value from it's minimum value when leveling up instead of continuing to grow (using the previous level's maximum value as minimum).
- new: GUI Boxes: Choice Settings: 'Choice Mode' settings available. Choices can be displayed in 'List' and 'Circle' mode. 'List' mode displays choices in a list (as it has been until now). 'Circle' mode displays choices in a circle around a center position in the GUI box.
- new: GUI Boxes: Choice Settings: 'Header Settings' available. A GUI box can now optionally display headers above choice buttons.
- new: Menu System: 'Header Texts' settings available in most menu parts. Optionally display headers above the choice buttons.
- new: Shop Layouts: 'Header Texts' settings available in 'Buy/Sell Box', 'Type Box' and 'List Box' settings. Optionally display headers above the choice buttons.
- new: Battle Menus: 'Header Texts' settings available. Optionally display headers above the choice buttons. Sub-menus can optionally have different header texts.
- new: Battle Menus: 'Whole Group' setting available for 'Auto' battle menu items. The 'Auto' command can now optionally be used for the whole battle group of the combatant - each member of the group will use the 'Auto' command once. Not used in 'Real Time' battles.
- new: HUDs: Combatant, Turn Order: 'Type Display Limit' settings available for 'Status Effect' HUD elements. Optionally only displays status effects of selected status effect types. Can be used to e.g. separate positive and negative effects.
- new: HUDs: Combatant, Turn Order: New text codes available for 'Status Value' HUD elements. '%m2' displays the minimum value, '%r' displays the remaining value to the maximum value (i.e. max - current value), '%r2' displays the remaining value to the minimum value (i.e. current - min value).
- new: HUDs: Combatant, Turn Order: 'Check Attribute Value' settings available for 'Attack Attribute' and 'Defence Attribute' HUD elements when listing attributes. Optionally checks the value of each attribute before displaying it, e.g. to only display attributes greater than 100.
- new: HUDs: Combatant, Turn Order: 'Relative Offset' settings available when using 'Group' combatant HUDs or turn order HUDs. The offset of the GUI boxes used for the individual HUDs can optionally be relative to the GUI box of the previous combatant. The position will be automatically adjusted, e.g. if the HUD size of one combatant is changed.
- new: HUDs: Combatant: 'Cell Mode' settings available for 'Shortcut' HUD elements. Like choices, shortcut slots can now also either be displayed as a 'List' (like until now) or in a 'Circle' around a defined position in the HUD element.
- new: Event System: Check Steps: 'Check Language' step available. Checks the currently used language of the game.
- new: Event System: Animation Steps: 'Auto Animation' step available. Use this step to enable or disable the auto move animation of a combatant.
- new: Event System: Function Steps: 'Is Static' setting available in the 'Call Function', 'Check Function' and 'Function To Variable' steps. Optionally calls a static function of a class - i.e. it doesn't require a component added to a game object.
- new: Event System: Function Steps: 'Is Static' setting available in the 'Change Fields' and 'Check Fields' step. Optionally uses static fields/properties of a class - i.e. it doesn't require a component added to a game object.
- new: Event System: Function Steps: 'Emit After (s)' setting available in the 'Emit Particles' step. Optionally starts/stops emitting particles on a game object after a defined amount of time.
- new: Event System: Movement Steps: 'Ignore Y' setting available in the 'Change Position' and 'Move Into Direction' steps. Ignores the Y-axis difference when moving/facing the target, i.e. the mover will maintain its Y-axis position (unless gravity is applied). By default enabled.
- new: Event System: Equipment Steps: 'Change Equip Durability' step available. Changes the durability of an equipment currently equipped on an equipment part or all parts of a combatant.
- new: Event System: Equipment Steps, Check Steps: 'Check Equip Durability' step available. Checks the durability of an equipment currently equipped on an equipment part or all parts of a combatant.
- new: Event System: Equipment Steps, Inventory Steps: 'Change Inventory Durability' step available. Changes the durability of an equipment currently placed in the inventory of a combatant.
- new: Event System: Equipment Steps, Inventory Steps, Check Steps: 'Check Inventory Durability' step available. Checks the durability of an equipment currently placed in the inventory of a combatant.
- new: Event System: Active Group Steps: 'Destroy Prefab' setting available in the 'Leave Active Group' and 'Leave Battle Group' steps. Optionally destroys a spawned prefab of a combatant that'll leave the group. By default enabled.
- new: Event System: Variable Steps: 'Variable To Transform' step available. Sets a game object's position, rotation or scale to the value of a Vector3 game variable.
- new: Battle Events: Battle Steps: 'Reconsume' setting available in the 'Calculate' step. Optionally consumes use costs or items again, even if they already have been consumed.
- new: Battle Events: Battle Steps: 'Can Use Action' step available. Checks if the user can use the action of the battle event.
- new: Main Menu: Custom Choices: 'Is Static' setting available for custom choices. Optionally calls a static function of a class - i.e. it doesn't require a component added to a game object.
- new: Combatants: AI Settings: 'Real Time AI Range' settings available. Using the battle AI can optionally be limited to a defined range to the leader of the combatant's group. You can use this setting to let AI controlled player group members stop fighting when the player moves away.
- new: Move AI: 'Prioritise Leader' settings available. Following the leader can optionally take priority over other targets when outside a defined range to the leader of the combatant's group.
- new: Status Bonuses: 'Status Change Modifier' bonus settings available. Optionally add bonuses to status value changes by abilities or items (value change settings), the bonus can be given for positive, negative or all changes. The default change modifier is 100 (i.e. 100 %), the bonuses are added to it. E.g. adding -10 bonus to negative MP changes will reduce the MP use costs to 90 %.
- new: Abilities, Items: 'Ignore Change Modifier' setting available for all value change settings (e.g. use costs, target changes). A status value change can optionally ignore the status change modifier bonuses and use the default 100 % modifier.
- new: Abilities: 'Own Group Learning Text' and 'Own Group Forgetting Text' settings available in the 'Console Texts' of the ability. Optionally overrides the default console texts for learning/forgetting a group ability.
- new: Abilities: 'Buy Price' setting available per ability level. Defines the price an ability will be sold for at shops. Can optionally be overridden by the shop's item list.
- new: Console Settings: 'Group Ability Text' settings available in 'Learning Texts' and 'Forgetting Texts' settings. Define the console texts that will be displayed when a group ability is learned or forgotten.
- new: Console Settings: 'Weapon Outworn Text' and 'Armor Outworn Text' settings availble in 'Inventory Texts' settings. Deinfe the console texts that will be displayed when a weapon's or armor's durability reaches 0.
- new: Battle End: New text codes available for 'Money Text', 'Item Text', 'Status Value Text', 'Ability Learn Text' and 'Ability Tree Learn Text' settings. Use '%cn' (name), '%cd' (description) and '%ci' (icon) to display information about the combatant.
- new: Battle End: 'Group Ability Learn Text' setting available. Will be displayed if a new group ability is learned through level up.
- change: Status Requirements: 'Ability Check' and 'Is Valid' settings replace the old ability status check. You can know check if an ability is known to the combatant (i.e. learned, temporary or group ability), learned or a group ability. Old settings will automatically be upated.
- change: Menu Screens: Equipment, Inventory: The 'Level Points Display' settings are now named 'HUD Info Display' and can also display durability information of an equipment.
- fix: Combatant Spawners: Spawning single combatants on a position (i.e. not using a collider for area spawning) could result in creating multiple combatants when respawning.
- fix: GUI System: New UI: The event system's 'Image Steps' are now working correctly when using the new UI.
- fix: GUI System: New UI: 'Selected Choice Offset' setting is now working correctly when using the new UI.
- fix: GUI System: Legacy GUI: The drag box will no longer be displayed twice, with one being displayed even after the drag finished.
- fix: Event System: Movement Steps: The 'Change Position' step's movement behaved differently when enabling/disabling 'Face Direction'.
- fix: Blocking Controls: Changing group members involving the player could result in the control components not being blocked when the control was blocked while changing members.
- fix: Turn Based Battles: A combatant dying while having the battle menu open could result in the battle not progressing.


Version 2.2.4:
- new: Status Requirements: 'Inventory' status check available. The combatant (or group) must or mustn't have a defind item, weapon, armor or currency in the inventory.
- new: Crafting System: The crafting system now also allows creating new items by adding items, equipment and currency to a combatant's crafting list. When creating using the crafting list, the first crafting recipe matching the items in the crafting list will be used. Either uses only known recipes or checks all recipes and learns unknown recipes upon successful creation.
- new: Menu Screens: 'Crafting List' menu part available. Displays the items, equipment and currency added to the user's crafting list. When using the crafting list for crafting, the first recipe that matches the items added to the list will be used.
- new: Menu Screens: Sub Menus: 'Crafting List' sub menu action available. Adds a selected item, equipment or currency to the user's crafting list.
- new: Menu Screens: Bestiary (Area): 'Show OK Button' and 'Show Cancel Button' settings available. Optionally displays the ok/cancel buttons of the GUI box.
- new: Event System: Dialogue Steps: 'Show OK Button' and 'Show Cancel Button' settings available in the 'Bestiary Dialogue' step. Optionally displays the ok/cancel buttons of the GUI box.
- new: Event System: Dialogue Steps: 'Quest Conditions' settings available in 'Choice' type 'Show Dialogue' steps. A choice can optionally use quest or task status conditions to determine if it will be displayed.
- new: Event System: Dialogue Steps: 'Status Requirements' settings available in 'Coice' type 'Show Dialogue' steps. A choice can optionally use status requirements to determine if it will be displayed.
- new: Event System: Active Group Steps: 'Spawn Group Members' step available. Spawns the other members of the active player group or battle group. Requires the player to already be spawned in the scene.
- new: Event System: Active Group Steps: 'Destroy Group Members' step available. Destroys all spawned members of the active player group except the player.
- new: Event System: Crafting Steps: 'Add Recipe Outcome' step available. Adds the outcome of a selected crafting recipe to a combatant's inventory. This doesn't consume ingredients or checks for requirements or crafting chance.
- new: Event System: Crafting Steps: 'Remove Needed Ingredients' step available. Removes the ingredients needed for a selected crafting recipe from a combatant's inventory.
- new: Event System: Base Steps, Function Steps: 'Add Control Component' step available. Adds a defined component of a game object to the player or camera control list. The component will be enabled/disabled with the selected controls.
- new: Event System: Base Steps, Function Steps: 'Remove Control Component' step available. Removes a defined component of a game object from the player or camera control list.
- new: Value Bars: 'Hide Empty Bar' setting available. Value bars (e.g. displaying status values in HUDs) can optionally be hidden if the displayed value is at the minimum value or below.
- new: Value Bars: 'Interpolate Colors' and 'Interpolate Empty Colors' settings available. Optionally interpolates between the colors used for the different percentages of the value bar. Only used when using colors instead of images.
- new: HUDs: Combatant, Turn Order: 'Limit Bar Display' settings available. Status value bars can optionally be limited to a defined value range (percent or absolute values). This allows creating split value bars, e.g. 2 HP bars from 0-50 % and 50-100%.
- new: Combatants: Death Settings: 'Variable Origin' setting available for death variable changes. The variable changes when a combatant dies can now be made on global variables, the combatant's object variables or the combatant spawner's object variables. By default using global variables.
- new: Abilities, Weapons, Armors: 'Own Description' settings available for the different level settings of abilities, weapons and armors. Optionally overrides the description of the ability, weapon or armor with a new description for a defined level.
- new: Abilities, Items: 'Active Time Order Changes' settings available. An ability/item can now optionally change the timebar of the target. A positive value will increase, a negative value will decrease the timebar.
- change: HUDs: Combatant: The 'No Inner Assign' setting has been replaced by the 'Inner Assign' setting in 'Shortcut' HUD elements. When a shortcut slot is assigned from the same HUD element, you can now either 'Keep' the old assignment, 'Remove' the old assignment, 'Spap' old and new slot assignment or prevent inner assignment ('None').
- change: Editor: Menu Screens: The 'Add Menu Part' list is now sorted alphabetically.
- fix: GUI System: New UI: Fixed problems with drag+drop when using the new UI.
- fix: GUI System: New UI: When first displaying a GUI box or changing focus, the selected choice will now be marked as selected object in the new UI event system.
- fix: Turn Based Battles: Using 'Death Immediately' without 'End Immediately' could lead to starting multiple new turns while the player was dying.
- fix: HUDs: Combatants: Only using backgrounds for 'Shortcut' element slots could result in a wrongly calculated content size, hiding parts of the HUD.
- fix: Event System: Spawn Steps: Using the 'Spawn Prefab' step without using the audio options and spawning at a position could lead to an error.


Version 2.2.3:
- new: Abilities, Items: 'Use Requirement' settings available for status value changes of active abilities and items. Optionally checks 'Status Requirements' and 'Variable Conditions' on the user or target, the changes will only be performed if the check is valid.
- new: Status Effect Changes: 'Use Requirement' settings available when changing status effects (e.g. ability status changes, auto effects, etc.). Optionally checks 'Status Requirements' and 'Variable Conditions' on the user or target, the changes will only be performed if the check is valid.
- new: Combatants: Battle Settings: 'Use Requirement' settings available for 'Auto Start Battle' settings. Optionally checks 'Status Requirements' and 'Variable Conditions' on the user (enemy) or target (player), the auto battles will only start if the check is valid.
- new: Combatants: Conditional Prefabs: 'Use Spawned Scale' setting available. Optionally uses the scale of the currently spawned prefab when changing prefabs. By default enabled.
- new: Battle Events: Battle Steps: 'Change Last Target' step available. Changes the last targets of a combatant. This can influence various things, e.g. AI target selection.
- new: Formulas: Variable Steps: 'Store Formula Value' step available. Stores the current value of the formula into a float game variable.
- new: Text Input: 'Is Password' setting available for game options and input dialogues. Optionally uses a text input field as password input, masking the user's input with a '*'.
- new: Music: 'Use PCM' settings available for music clip loops. Optionally checks and sets the playback position in PCM samples instead of time.
- new: GUI System: New UI: 'Keep Prefab Height' settings available for text, toggle and slider input prefabs. Optionally keeps the height of the used prefabs instead of adjusting the height to the used font size.
- new: Menu System: Crafting: 'Info Only' setting available in the 'Crafting Box Settings'. Optionally only displays the crafting recipe information and doesn't allow using them.
- new: Crafting Recipes: 'Use Requirements' settings available. Optionally checks 'Status Requirements' and 'Variable Conditions' on the user of the recipe to determine if it can be used.
- new: Event System: Crafting Steps: 'Can Use Recipe' step available. Checks if a combatant can use a selected crafting recipe.
- new: Event System: Camera Steps: 'Change Camera Options' step available. Changes various settings on a camera that can't be changed with the 'Change Field' step.
- new: Event System: Function Steps: 'Set Object Layer' step available. Sets the layer of a game object.
- new: Event System: Function Steps: 'Check Object Layer' step available. Checks the layer of a game object.
- new: Event System: Function Steps: 'Place Object' setting avaialble in the 'Mount Object' step. Placing a game object when mounting it to another object is now optional. By default enabled.
- new: Event System: Status Steps: 'Change Defence Attribute' step available. Changes a defence attribute of a combatant permanently.
- new: Event System: Battle Steps: 'Add Loot' step available. Adds items, weapons, armors or currency to the battle's loot.
- new: Event System: Battle Steps: 'Add Experience Reward' step available. Adds experience to the battle's experience gains.
- new: Event System: Status Steps: 'Revive' step available. Revives a combatant or the combatant's group. The step doesn't change 'Consumable' type status values, i.e. the combatant will die again if the status value causing the death is still the same.
- new: Status Effects: 'Animate Damage' settings available for status conditions. Negative status changes can now optionally play a damage animation on the combatant.
- new: Status Requirements: 'Group Leader' status check available. The combatant must or mustn't be the leader of his group.
- new: Status Requirements: 'Group Size' status check available. The combatant's group size will be compared with a defined value.
- change: GUI System: New UI: The GUI box instance used for a box object in the new UI can now be accessed through the 'GUIBox' property in the 'GUIBoxComponent' attached to it.
- change: GUI Boxes: The 'Inactive Alpha' settings for choices and tabs are no longer available when using the new UI. This is handled by the prefabs used for buttons/tabs, the 'Inactive Alpha' setting wasn't used in the new UI.
- change: Combatant Component Inspector: The inspector display of the combatant component now also displays the defence attributes of the selected combatant game object.
- fix: Combatants: A battle action without targets (e.g. an AoE ability) could cause errors when setting the last target's of a combatant.
- fix: Event System: Camera Steps: Changing the 'Field of View' in a 'Change Camera Position' step is working again.
- fix: Event System: Rotation Steps: The 'Rotate To' step will now correctly rotate into the shorter direction when using fading.
- fix: GUI System: New UI: Opening/selecting the ORK Framework editor while playing wont throw errors any longer.
- fix: HUDs: Combatant: Using the 'Change Combatant' click type and clicking on the current player's HUD removed the player controls but didn't add them again. Changing player now only works if the combatant isn't the current player.
- fix: Changing Player: Changing the player could result in still using the old interaction controller in some cases.


Version 2.2.2:
- new: Combatants: 'Shortcut List Count' setting available. A combatant can now have more than one shortcut list, each list contains of a various amount of slots. Lists can be switched using the event system.
- new: Combatants: 'Use Class Shortcuts' setting available. A combatant can optionally use class based shortcut lists.
- new: Classes: 'Shortcut List Count' and 'Default Shortcut' settings available. Shortcut lists can now optionally be based on classes, changing a class will also change the shortcut lists.
- new: Event System: Shortcut Steps: 'Change Shortcut List' step available. Changes a combatant's active shortcut list.
- new: HUDs: Combatant: 'No Inner Assign' setting available in 'Shortcut' HUD elements. Prevents assigning a shortcut slot from another slot of the same HUD element.
- change: Event System: The shortcut steps can now be found in the new 'Shortcut Steps' list.
- fix: Event System: Dialogue Steps: Restarting an event while an 'Autoclose' dialogue (without using 'Wait) is displayed wont result in errors.
- fix: HUDs: A HUD without any HUD elements (e.g. only using a background image) caused errors.
- fix: GUI System: New UI: Using text typing displayed the whole text for a frame when first displaying a GUI box.
- fix: Status Effects: Using 'End On Abilities' caused a status effect to be removed by the same ability that applied it.
- fix: Status Effects: Using an 'End Action' in combination with 'End On Abilities' could in some cases lead to an error.


Version 2.2.1:
- new: GUI System: New UI: The new UI now also supports text typing.
- new: GUI System: New UI: 'Layer' setting available. Select the layer the UI canvas will be placed at.
- new: Save Games: Support for saving/loading custom save data with ORK Framework save games. Use the function 'ORK.SaveGame.RegisterCustomData' to register a class (implementing 'ISaveData') that saves/loads your custom data.
- new: Event System: Base Steps: 'Add Combatant' step available. Adds a combatant to a game object (only if the game object doesn't already have a combatant).
- new: Event System: Base Steps: 'Use Mouse Position' setting available in the 'Raycast Object' step. Optionally uses the mouse position as target for raycasts when not using input.
- new: Event System: Variable Steps: 'Use Mouse Position' setting available in the 'Raycast to Variable' step. Optionally uses the mouse position as target for raycasts when not using input.
- new: Abilities: 'Hidden' setting available. Optionally hides the ability in 'Ability' and 'Ability Tree' menu screen parts and in battle menus.
- new: Items: 'Hidden' setting available. Optionally hides the item in 'Inventory' menu screen parts and in battle menus.
- new: Combatants: Battle Settings: 'Base Experience Factor' settings available. Defines the base experience factor of a combatant. The experience factor influences how much experience a combatant gains from battle. E.g. factor 1 gains 100 %, factor 2 gains 200 %.
- new: Combatants, Classes, Equipment, Abilities, Status Effects: Bonus Settings: 'Experience Factor Bonus' setting available. Defines a bonus that will be added to a combatant's experience factor.
- new: Status Effects: 'Stackable' setting available. Optionally allows applying a status effect multiple times, either generally (i.e. a user can apply the same effect multiple times) or once per combatant (i.e. a user can apply the effect only once, but a different user can apply it as well).
- new: Status Requirements: The 'Status Effect' check can now optionally check the quantity a status effect is stacked on a combatant. This is available throughout the framework (e.g. in the 'Check Status' event step).
- change: Event System: Base Steps: The 'Block Input Key' and 'Block Control Map' steps will now be executed in events that are stopped (e.g. when a combatant died during using an action). This makes sure the input keys or control maps wont stay blocked when an event is stopped before unblocking them.
- change: Event System: Dialogue Steps: The 'Block HUD' step will now be executed in events that are stopped. This makes sure the HUDs wont stay blocked when an event is stopped before unblocking them.
- change: Event System: Camera Steps: The 'Block Camera Control' step will now be executed in events that are stopped. This makes sure the camera control wont stay blocked when an event is stopped before unblocking it.
- change: Event System: Movement Steps: The 'Block Player Control' and 'Block Move AI' step will now be executed in events that are stopped. This makes sure the player control or move AI wont stay blocked when an event is stopped before unblocking them.
- fix: GUI System: New UI: Using the 'GUI Editor' to change a GUI box after playing the project no longer creates instances of UI parts.
- fix: Battle System: The correct player group leader will be used when the leader was changed using the player change keys.
- fix: Move AI: 'Auto Stop' issue fixed, leading to enemies circling around the player.


Version 2.2.0:
- new: Unity 4.6: ORK Framework now supports/requires Unity 4.6 and the new UI.
- new: Menu Settings: 'GUI System Type' setting available. Select if you want to use the 'Legacy GUI' (old GUI system) or the 'New UI' of Unity 4.6.
- new: Menu Settings: Settings for the 'New UI' GUI system type available.
- new: Menu Settings, GUI Boxes: GUI Skins: New settings available when using the 'New UI' gui system type. The settings replace the GUI skins used in the 'Legacy GUI' system and are used to select prefabs to create the GUI boxes, buttons, etc.
- new: Combatants: 'Auto Start Battles' setting available. Defines if a combatant will automatically start a battle when coming within a defined range to the player. By default enabled.
- new: Event System: Base/Function Steps: The 'Start Battle' step can now also start battles with objects who have a combatant. Previously, only objects with a 'Battle' component attached could be used.
- new: Battle Events: Battle Steps: 'Set Attacked By' step available. Marks that a combatant has been attacked by another combatant. This can influence various other things, e.g. victory gains or faction sympathy changes.
- fix: Status Effects: End Action: The end action marked by a status effect to be called by a combatant will no longer check if it can be used due to turn/time. The check resulted in the action not being marked if the combatant currently couldn't perform an action due to turn/time.


Version 2.1.11:
- new: Main Menu: 'Override Question' settings available for the auto save slot selection menu. Optionally display a question dialogue if an already used auto save slot should be overridden by a new game.
- change: Components: Getting and adding components now uses type functions instead of string functions. This allows Unity 5 compatibility.
- change: Components: The Unity 4 component getters (e.g. rigidbody, collider, etc.) have been replaced with GetComponent calls. This allows Unity 5 compatibility.
- change: Combatants, Classes: The experience of class level up status values will be stored when changing class. The experience of a class will be restored when changing back to it.
- change: Event System: Spawning prefabs now sets the position and rotation upon instantiating. This solves issues with shurikan particles producing a trail when spawned.
- change: GUI Boxes: Clicking on 'Ok' or 'Cancel' button positioned outside of the GUI box will now be recognized as click on the box.
- fix: Event System: Inventory Steps: 'Add To Item Box' and 'Remove From Item Box' steps wont throw errors in the editor.
- fix: Main Menu, Save Points, Menu Screens: The 'Load' buttons are now also available if only auto save games exist.
- fix: Abilities, Items, Event System: Target raycasts and raycast steps are now work correclty on different resolutions.
- fix: Combatants, Classes: Changing classes will now reset the class level up status value to the correct value.
- fix: Battle Gains: Collecting an item while an auto close battle gain dialogue is displayed wont lock the item collection dialogue any longer.
- fix: Item Collectors: Preventing an error when no item is added to the collectors list.


Version 2.1.10:
- new: Status Values: 'Real Value Count' setting available when using 'Count to Value'. Optionally uses the counted display value as real value of the status value.
- new: Battle System: 'Leave Arena Settings' available in all battle system types. The battle can optionally end when the player moves outside a defined range of the battle arena (i.e. the game object with the 'Battle' component).
- new: Battle System, Battle Components: 'Leave Arena Event' available. The battle end event used when the player leaves the battle arena.
- new: Battle Start Events, Battle End Events: 'Block Move AI' and 'Block Actor Move AI' settings available. Optionally blocks the move AI for all game objects or only for actors of the event. Blocking the move AI will only be active while the event is running.
- new: Event System: Base Steps: 'Auto Load Game' step available. Loads the AUTO save game or a temporary retry save game. The event ends after this step.
- new: Event System: Status Steps: 'Check Shortcut' step available. Checks a combatant's shortcut and executes 'Success' or 'Failed' based on the check.
- new: Event System: Battle Steps: 'Check Turn' step available. Checks a combatant's turn number and executes 'Success' or 'Failed' based on the check.
- new: Event System: Dialogue Steps: 'Button Info' and 'Button Title' settings available for 'Choice' type dialogues. Optionally add an info text and title to a choice.
- new: Event System: Dialogue Steps: 'Wait' setting available for 'Message' type 'Show Dialogue' steps. If disabled, the event will continue with the next step while the dialogue is displayed. By default enabled.
- new: Event System: Dialogue Steps: 'Close All Dialogues' step available. Closes all dialogues opened by this event (only from 'Show Dialogue' steps).
- new: Event System: Scene Steps: 'Check Item Box' step available. Checks the number of items in an item box and executes 'Success' or 'Failed' based on the check.
- new: Formulas: Combatant Steps: 'Turn' step available. Uses a combatant's turn number or the combatant's group average turn number to change the formula value.
- new: Formulas: Combatant Steps: 'Check Turn' step available. Checks a combatant's turn number or the combatant's group average turn number and executes 'Success' or 'Failed' based on the check.
- new: Menu Screens: Sub Menus: 'Assign Shortcut' sub menu item available. Shows a menu to assign items, abilities or equipment to a shortcut slot of the menu user.
- new: Menu Screens: '%lvl' text code available for content layouts (for choice buttons). Displays the level of an ability or equipment.
- new: Save Game Menu: 'Auto Save Slots' setting available. Defines the numbers of available auto save slots.
- new: Main Menu: 'Auto Save Slot Menu' settings available. Optionally select the auto save slot when starting a new game. Auto saving will use the selected slot in the running game.
- new: Abilities, Items: Target Raycast: 'Use Mouse Position' settings available for raycast auto targeting. Use this option if you're directly firing your actions from control maps without target selection.
- new: Abilities: 'Quantity' and 'Chance (%)' settings of an active ability's item consume now support formulas, game variables and other value origins.
- new: Ability Trees, Status Development: 'Quantity' and 'Chance (%)' settings of item learn costs now support formulas, game variables and other value origins.
- new: Control Maps: 'Show Tooltip' settings available for 'Action' and 'Shortcut' control keys. Optionally displays a 'Tooltip' type HUD instead of using the action/shortcut.
- new: Editor: 'Help Text Color' setting available in the editor settings. Changes the color of the help text.
- change: Combatants: Using 'Local Variables' in the combatant's object variable settings will now keep the local variables on the combatants and not the objects. This allows to keep the local object variables when the game object is destroyed, e.g. when changing prefab.
- change: Animations: Mecanim animations can now use play and set parameters at the same time.
- change: Event System: Inventory Steps: The item box steps are now also available in the 'Inventory Steps'.
- change: Battle Start Events: The 'Join Battle' step is now also available in battle start events.
- fix: Colors: Fading or blinking colors (e.g. in the 'Fade Object' event step) with time set to 0 resulted in a black color.
- fix: Event System: Battle Steps: The 'Look At Enemies' step works correctly again.
- fix: Event System: Function Steps: The 'Mount Object' step now correctly unmounts objects.
- fix: Item Collectors: 'Box' type item collectors now set the variables after closing the item box.
- fix: GUI Boxes: Buttons now can't be clicked while the GUI box is fading out.
- fix: Damage Dealers: Stopping particle emitting on prefabs is now working correctly.
- fix: Menu Screens: Changing/removing the player using a 'Group' menu part will now correctly change the player if needed.
- fix: Abilities, Items: Target Raycast: The auto targeting from screen position was inversed.


Version 2.1.9a:
- new: GUI Boxes: 'Title Text Alignment' setting available when using 'Set Title Width' in the choice settings. Manages the horizontal alignment of the title of a choice within its bounds.
- new: GUI Boxes: 'Title Line Alignment' setting available in the choice settings. Manages the vertical alignment of labels in a line of the title of a choice.
- new: Combatant Selections: 'Back Button' settings available. Optionally adds a back button to combatant selections.
- fix: Equipment: Changing equipment will now correctly update displayed values.
- fix: Shops: Tooltips are now working correctly in shops.
- fix: Variable Conditions: Checking 'Conditions' type had a display bug in the editor.


Version 2.1.9:
- new: HUDs: Combatant, Turn Order: 'Click Action' settings available for 'Combatant' and 'Turn Order' HUD elements. Clicking on a HUD element can optionally call a menu screen, change the current menu/shop user, change the player or select a target. Also works in menu screens and shops.
- new: HUDs: Combatant, Turn Order: '%c' text code available for 'Status Value', 'Attack Attribute' and 'Defence Attribute' elements. Displays the change between the current and preview value of a status value or attack/defence attribute.
- new: HUDs: 'Expand Bounds' setting available for HUD element background images. The element's display bounds will be increased when the image bounds exceed them.
- new: HUDs: 'Start Toggle State' setting available when using a 'Toggle Key'. Defines if the HUD is toggled on or off at the start of the game. When using 'Only While Key', the display state is inversed, i.e. the HUD will be hidden when the input key is valid. By default enabled (i.e. toggled on).
- new: Menu Screens: Ability, Inventory, Ability Tree, Status Value Upgrade: 'Don't Return' setting available when using 'Close After Use'. The menu screen wont return to a previously opened menu screen when closing after use.
- new: Menu Screens: Ability, Inventory: 'Animate Use' setting available. Defines if using an item/ability in a menu screen will be animated, i.e. using it's battle events. By default enabled.
- new: Menu Screens: Combatant: 'Show Ok Button' and 'Show Cancel Button' settings available. Optionally show the 'Ok'/'Cancel' buttons of the GUI box. The 'Ok' button can change to the next page when using 'Accept Next Page', the 'Cancel' button can close the menu screen.
- new: Battle System: 'Defend First' setting available in all battle system types. The defend command will optionally perform before other actions. Doesn't take already performing actions into account.
- new: Battle System: 'Auto Close' settings available when showing battle gains immediately. Optionally automatically closes the battle gains dialogue after a defined amount of time. Available in all battle systems.
- new: Battle End Events: Battle Steps: 'Auto Close' settings available in the 'Collect Battle Gains' step. Optionally automatically closes the battle gains dialogue after a defined amount of time.
- new: Input Keys: 'Mouse' and 'Touch' input origin settings available. Use mouse button clicks or touch input as input keys (can't be used as axis).
- new: Game Variables: 'Approximately' check type available for float game variable checks. The check is valid if the current value of the game variable is similar to a defined value. Uses the Mathf.Approximately function for the check.
- new: Game Variables: 'Range Inclusive', 'Range Exclusive' and 'Approximately' check types available for Vector3 distance checks.
- new: Event System: Check Steps: 'Check Value' step available. Checks a value with another defined value, e.g. check if the result of a formula is within a defined range.
- new: Event System: Movement Steps: 'Range Inclusive', 'Range Exclusive' and 'Approximately' check types available in 'Check Distance' and 'Check Transform' steps.
- new: Event System: Rotation Steps: 'Range Inclusive', 'Range Exclusive' and 'Approximately' check types available in 'Check Angle' steps.
- new: Event System: Faction Steps: 'Range Inclusive', 'Range Exclusive' and 'Approximately' check types available in 'Check Faction Sympathy' steps.
- new: Formulas: Check Steps: 'Range Inclusive', 'Range Exclusive' and 'Approximately' check types available in 'Check Formula Value' steps.
- new: Formulas: Position Steps: 'Range Inclusive', 'Range Exclusive' and 'Approximately' check types available in 'Check Distance' and 'Check Angle' steps.
- new: Game Settings: Bestiary: 'Use in Battle AI' setting available. Uses bestiary knowledge when a player combatant does status checks on enemy combatants in a battle AI. A status check fails if the informatin is not known to the player.
- new: Battle AI: Status Steps: 'Force Knowledge' setting available for 'Check Status', 'Get Status Value', 'Get Attack Attribute' and 'Get Defence Attribute' steps. Optionally forces player combatants to have full knowledge about an enemy's status.
- new: Battle AI: Position Steps: 'Range Inclusive', 'Range Exclusive' and 'Approximately' check types available in 'Check Distance' and 'Check Angle' steps.
- new: Status Requirements: 'Combatant' requirement available. Status requirements can now check if a combatant is a selected combatant or not.
- new: Editor: 'Copy to Clipboard' button available for help texts. Copies the current help text to the system clipboard.
- change: Text Formats: The minimum value the 'Font Size' can be set to is now 0 instead of 1.
- fix: Menu Screens: Equipment: Changing between equipment with status value bonuses and status requirements will now correctly refresh the equipment list.
- fix: Menu Screens: Ability, Equipment, Inventory: Sub menus can now also be used on inactive choices (e.g. unequipable equipment, not useable items).
- fix: Menu Screens: Ability, Inventory: 'Close After Use' now correctly closes the menu screen when using 'Use Screen Combatant' in the menu screen or item.
- fix: Menu Screens: Selecting a choice via click will now correctly update the description when displaying a combatant choice.
- fix: Menu Screens: 'Description' menu part updates could cause menu screens to not fully close, blocking the game.
- fix: Status Values: Using a combined status value as another value's maximum value could lead to limiting it before the real maximum value has been calculated.
- fix: Status Effects: 'Auto Attack' and 'Attack Allies' will now correctly perform actions.
- fix: Battle System: Using 'Leave on Death' on player combatants could lead to errors upon losing a battle (when no combatant was left in the player group, i.e. no player).


Version 2.1.8:
- new: Abilities: 'Delay Time' settings available for active abilities. Abilities can set a delay time on the user. A combatant can't perform actions (e.g. attack, use an ability or item) during the delay time.
- new: Inventory Settings: 'Default Delay Time' settings available. Items can set a delay time on the user. A combatant can't perform actions (e.g. attack, use an ability or item) during the delay time.
- new: Items: 'Delay Time' settings available. Items can override the default delay time defined in the inventory settings.
- new: HUDs: Combatant, Turn Order: 'Delay Time' element type available. Displays a combatant's delay time as a text or bar. Only displayed while the time is running.
- new: HUDs: Combatant, Turn Order: 'Time Format' setting available for 'Cast Time' and 'Timebar' text time displays.
- new: HUDs: Combatant, Turn Order: 'Reverse Value' setting available for 'Cast Time'. The cast time will be displayed reversed, i.e. the time will count down instead of up.
- new: Status Requirement Templates: The status requirement templates allow you to set up status requirements that can be reused in other status requirements. Status requirement templates can be created in 'Base/Control > Status Requiremenet Templates'.
- new: Status Requirements: 'Requirement Type' setting available. Select if a requirement either checks a single 'Status', other 'Requirements' or a status requirement 'Template'. When checking other requirements, you can create multiple status requirements in a single condition, allowing you to set up checks like '(requirement1 && requirement2) || requirement3'.
- new: Event System: Dialogue Steps: You can now use actor text codes in choices added to a 'Dialogue Step'.
- new: Event System: Dialogue Steps: Choices can now use global, local and object variables in variable conditions.
- new: Event System: Active Group Steps: 'Check Group Size' step available. Checks the size of the active player group.
- new: Event System: Variable Steps: 'Variable to Variable' step available. Stores the value of a game variable into another game variable. You can use this step to transfer variables between variable origins (local, global and object).
- new: Combatants: Battle Animations: 'Join Look At' setting available. Optionally looks at enemy combatants when joining a running battle. By default disabled.
- new: Game Settings, Interactions: Setting 'Max. Click Distance' to -1 will allow starting 'Interact' type interactions without distance limitations.
- new: Status Development, Ability Trees: 'Experience' type learn costs can now use a defined value, formula, game variable, etc. for the required/consumed experience.
- fix: Status Values: 'Experience' type status values that level ub the base/class level will now correctly receive experience exceeding the current maximum value.
- fix: Status Requirements: Passive abilities can now also be checked without enabling 'Learned'.
- fix: Abilities: Passive abilities wont be added to the list multiple times.
- fix: Abilities: Passive abilities will now correctly add their status effects outside of battles.
- fix: Event System: The 'Found Objects' list was updated when using a child object, resulting in the child object being in the list instead of the original object.
- fix: GUI Boxes: Cut off text issues are solved.
- fix: Global Events: Wait times are now working correctly in 'Scene' type global events.
- fix: Equipment Viewers: Using the 'Add Combatant' component with 'Equipment Viewers' could still lead to not displaying equipment.
- fix: Music: If no music is played when storing music, now also no music will play when playing the stored music (also applies to shop/menu music).


Version 2.1.7:
- new: Main Menu: 'Continue Button' settings available. Loads the last saved game. Requires the save games to be saved with this version to find the latest save game.
- new: Main Menu: 'Hide Inactive' setting available for the load button. The load button wont be displayed if no save games are available.
- new: Status Requirements: 'Weapon' and 'Armor' requirements available. Status requirements can now check if a selected weapon or armor is equipped or not equipped on a combatant. Available throughout the framework.
- new: Text Format: 'Font' setting available. You can now optionally set the font in the text format settings throughout the framework.
- new: Status Values: 'Init to Level' setting available for 'Experience' type status values. The experience status value will be initialized to the value according to the combatant's level and status development. By default enabled.
- change: Status Values: 'Experience Type' setting replaces 'Level Up' and 'Class Level Up' settings for 'Experience' type status values.
- change: Status Values: 'Experience' type status values that don't level up (i.e. 'Experience Type' set to 'None') will now use the status value's minimum value as minimum value. Level up type values will still use the previous level's value as minimum value.
- fix: Battle Settings: Auto Join: Joining a running battle was possible when the battle already ended, leading to the joining combatants being removed.
- fix: Equipment Viewer: Using the 'Add Combatant' component with 'Equipment Viewers' could lead to not displaying equipment if the combatant was initialized after the equipment viewer.
- fix: Status Values: Having a 'Consumable' type status value above it's maximum status value (i.e. with a lower index) could lead to initialization to a wrong value.
- fix: Active Time Battles: Using 'Pause on Menu' with AI controlled player group members wont stop the battle any longer after an AI action.
- fix: Battle System: 'Death' actions and loot collection now works again when not using 'Death Immediately'.
- fix: Editor: The 'Console Settings' sub-section is working again.


Version 2.1.6:
- new: Variable Condition Templates: The variable condition templates allow you to set up variable conditions that can be reused in other variable conditions. Variable condition templates can be created in 'Base/Control > Variable Condition Templates'.
- new: Variable Conditions: 'Condition Type' setting available. Select if a condition either checks a single 'Variable', other 'Conditions' or a variable condition 'Template'. When checking other conditions, you can create multiple variable conditions in a single condition, allowing you to set up checks like '(condition1 && condition 2) || condition3'.
- new: Variable Conditions: New float checks available. 'Range Inclusive' checks if the current value of a float game variable is between two defined values, including the values. 'Range Exclusive' checks if the value is between two defined values, excluding the values.
- new: HUDs: Display Conditions: 'Only While Key' setting available when using a toggle key. The HUD will only be displayed while the selected toggle key input is valid, i.e. use a 'Hold' input key and the HUD will only be displayed while the key is pressed.
- new: HUDs: Combatant, Turn Order: 'Equipment' HUD element type available for 'Combatant' and 'Turn Order' type HUDs. Displays a combatant's equipment parts and equipped weapons/armors - either listing all parts or displaying a selected part.
- new: HUDs: Combatant, Turn Order: 'Display Requirements' settings available for 'Combatant' and 'Turn Order' type HUD elements. Optionally limit displaying a HUD element to status requirements and variable conditions.
- new: Game Settings: Bestiary: 'Equipment' settings available. The equipment of an enemy combatant can now also be learned for a bestiary entry.
- new: Status Values: 'Normal' type status values can now optionally use other 'Normal' type status values to define their minimum and maximum values. The min/max status value's current value will be used - this allows creating flexible min/max values, e.g. for upgrading the MaxHP with special equipment.
- new: Menu Screens: Status Value Upgrade: 'Show No Upgrade Req.' and 'No Upgrade Req. Content Layout' settings available. Used to display status values with upgrades that aren't available due to requirements (e.g. for maxed out ugprades).
- new: Event System: Function Steps: 'Close Menu Screen' step available. Closes a selected or all opened menu screens.
- new: Event System: Function Steps: 'Start Item Collector' step available. Starts an item collector attached to an event object (e.g. actor). The event continues after the item collection has been finished. You can use this to e.g. do additional animations when collecting items or opening item boxes.
- new: Event System: Spawn Steps: 'Spawn Player' and 'Destroy Player' steps can now also spawn/destroy a different member of the player group.
- new: Event System: Variable Steps: 'Function to Variable' step available. Stores the return value of a function into a game variable. Supports string, bool, int/float and Vector3 return values.
- new: Combatants: Movement Settings: 'Use Position Change' setting available. If enabled, the change of the combatant's position every update will be used to determine the real speed (used e.g. in auto move animations). Otherwise components will be check in that order: CharacterController > Rigidbody > Rigidbody2D > NavMeshAgent > Position Change.
- new: Combatants: 'Set Object Name' setting available. Optionally sets the name of the game object spawned for the combatant to the combatant's name.
- new: Battle Notifications: The notifications (e.g. status value changes) can now also set other properties of renderer materials like the 'Fade Object' event step.
- new: Combatant Spawners: 'Set Scale' settings available. You can now optionally set the scale of spawned combatants.
- new: Weapons, Armors: 'Viewer Name' setting available. Optionally sets the name of the game object spawned by the equipment viewer. You can use this to unify the name of displayed equipment and access it easier through events (e.g. child objects).
- new: Game States: The 'Menu Screen' game state can now optionally be limited to defined menu screens. The 'Yes' option will be valid if one of the added menu screens is opened - if no screen is added, any opened screen is valid.
- new: Equipment Viewers: 'Combatant in Battle' setting available when setting the 'In Battle' game state to 'Yes' or 'Ignore'. If enabled, the equipment will only be displayed when the combatant itself is in battle, not the game.
- new: Combatants: Battle Animations: 'Join Battle Animation' available. Uses battle events to animate a combatant joining the battle through 'Auto Join' in a running battle.
- new: Battle Settings, Classes, Combatants: New battle menu settings available. You can now optionally define a battle menu for each battle system type.
- new: Battle Settings: Auto Join: 'Join Running Battle' setting available. Combatants can optionally also join running battles when coming into auto join range.
- new: Battle System: Move AI: 'Block In Battle' setting available in all battle system type settings. Optionally blocks the move AI of combatants that are participating in battle when allowing move AI during battle. This allows not fighting combatants to still move while those who are fighting don't move.
- new: Text Codes: The new 'Float with Format' text code allows defining a text format when displaying float variables. E.g. 00.0 will display at least two digits before the decimal point and one after the decimal point (1.5 would be come 01.5).
- new: Equipment Viewers: 'Preview' inspector settings available. You can now preview a selected weapon or armor on an equipment viewer. You need to disable the preview again when you're done to get rid of the created preview object.
- change: Event System: Steps using combatants can now also used unspawned members of the combatant group as long as no game object is needed in the step.
- change: Event System: Fade Steps: The 'Blink Object' and 'Fade Object' steps now set the color property of 'Sprite Renderers' instead of the materials color property if a 'Sprite Renderer' is used (fading Sprite objects).
- change: Editor: The ORK Framework editor no longer closes automatically when entering 'Play' mode in Unity.
- fix: Save Games: Loading a save game without having 'Spawn Group' enabled doesn't spawn other group members any longer.
- fix: Status Effects: The turns of combatants with 'Stop Move' effects will now increase (and reduce turn bound effects).


Version 2.1.5:
- new: Bestiary: The bestiary is a collection of status information on enemy combatants. You can define when to learn which status information (e.g. status values on encountering an enemy, attack attributes on attacking with the individual attributes). The 'Bestiary Settings' are defined in 'Game > Game Settings'.
- new: Menu Screens: 'Bestiary (Area)' menu part available. Displays information on bestiary entries using HUD elements. The bestiary entries are separated by areas and area types in which they've been encountered.
- new: Event System: Bestiary Steps: 'Add To Bestiary' step available. Adds an entry to the bestiary - uses either an event object's combatant or a defined combatant.
- new: Event System: Bestiary Steps: 'Remove From Bestiary' step available. Removes an entry from the bestiary - uses either an event object's combatant or a defined combatant.
- new: Event System: Bestiary Steps: 'Check Bestiary' step available. Checks if a combatant's entry is in the bestiary (complete or partial entry) - uses either an event object's combatant or a defined combatant.
- new: Event System: Bestiary Steps: 'Bestiary Dialogue' step available. Displays a dialogue with information on a combatant's bestiary entry using HUD elements. Uses either an event object's combatant or a defined combatant, if no combatant or bestiary entry was found, the dialogue is skipped.
- new: Combatants: 'Not Scanable' setting available. The combatant wont add information to the bestiary, only an empty entry without status information will be made.
- new: Combatants: 'No Bestiary Entry' setting available. The combatant wont add an entry to the bestiary at all.
- new: Abilities, Items: 'Scan Target' setting available. The target will be added with a complete entry to the bestiary. Only possible if the user is part of the player group and target is an enemy.
- new: Save Game Menu: 'Save Bestiary' setting available. If enabled, the learned enemy combatant information will be saved. By default enabled.
- new: Requirements: 'Has Bestiary Entry' setting available. At least one bestiary entry must be available for this requirement to be valid.
- new: Status Development, Ability Trees: 'Cost Type' setting available for learn costs. You can now also use items, weapons, armors and currency as costs for learning abilities or increasing status values.
- new: Menu Settings: Use Cost Display: 'Item Cost', 'Weapon Cost', 'Armor Cost' and 'Currency Cost' settings available. Also, all use cost settings can now use %i to display icons.
- new: Images: All image settings can now also use alpha masks when using colors instead of images.
- new: Value Bars: 'Crop Image' setting available for all Value bars (e.g. for status values) with scale mode 'Stretch to Fill' or using alpha masks. If enabled, the image will use the whole bar's bounds for scaling, but crop the parts of the image that exceed the filling (e.g. 50 % filling will only display half of the image).
- new: Battle AIs: 'Check Target Count' step available. Checks the number of found targets and executes 'Success' or 'Failed' next step.
- new: Add Combatant Component: 'Battle Type' setting available for the 'Add Combatant' component. Defines the battle type used when a battle is started when entering the combatant's battle type.
- new: Inspectors: The 'Combatant' component inspector now also displays a combatant's name, level, class and class level.
- new: Menu Screens: 'Accept Next Page' setting available for 'Combatant' and 'Information' menu parts. The 'Accept' input key (and 'Ok' button) can optionally be used to change to the next page.
- new: Editor: 'Create HUD' button available for combatant page, information page and combatant choice layouts. Creates a new HUD from the defined HUD elements of the page/layout and uses the HUD instead.
- change: Item Collectors: 'Destroy Collected' setting is now 'Destroy Collected Object' and also available for item collectors not using scene ID. Warning: This will reset the setting on all item collectors to enabled.
- fix: GUI Editor: The WYSIWYG GUI editor is again working correctly with all GUI settings.
- fix: Event System: Status Steps: Using 'Level Up' to level up the base or class level now also works when no experience status value is set to level up base/class levels.
- fix: Player Group: Unspawned members of the player group didn't get updated properly, leading to equipment changes not adding bonuses, etc.


Version 2.1.4:
- new: Plugins: The new plugin system allows adding your own settings and functions to ORK Framework without changin ORK's code. The data will be saved with the rest of ORK's settings. Plugins can be added in 'Editor > Plugins'. The full version includes a plugin template project.
- new: Event System: Function Steps: 'Call Plugin' step available. Calls a plugin's 'Call' function with an information string.
- new: Main Menu: 'Call Plugin' settings available for 'Custom Choice' menu items. Custom choices can now call a plugin instead of a game object's component.
- new: GUI Layers: 'Use Full Screen' setting available. The layer ignores the GUI settings in the 'Game Settings' and uses 'Stretch to Fill' for all GUI boxes on this layer, using the full screen for placement.
- new: Game Settings: 'GUI Padding' setting available. Adds a padding at the borders of the screen when placing the GUI.
- new: Game Settings: 'GUI Anchor' setting available for all GUI scale modes except 'Stretch to Fill'. Select the anchor for the GUI when placing/scaling the GUI (e.g. Upper Left, Middle Center, ...).
- new: Game Settings: 'Raycast Type' setting available. Select the type of raycasts that are used throughout the framework, you can either use 'Only 3D', 'Only 2D' or mixed 'First 3D' and 'First 2D'. 3D raycasts will interact with 3D colliders, 2D raycasts with 2D objects. By default 'Only 3D'.
- new: Abilities, Items: 'Auto Target' setting available in 'Raycast Settings'. Used for AI/auto target raycasts - if enabled, a nearby combatant will automatically be targeted by the raycast, otherwise the screen center will be used. By default enabled.
- new: Battle Spots: 'Set Rotation' and 'Set Scale' settings available for the individual battle spots. You can now set the rotation and scale of the default battle spots.
- new: GUI Boxes: 'Select Unfocused' setting available for choices. Optionally allows selecting a choice on an unfocused GUI box.
- new: Animations: Mecanim: 'Trigger' parameter type available for Mecanim animations.
- new: Animations: Mecanim: 'Duration' setting available for Mecanim animations. Used in game events to set the wait time for animation steps.
- new: Event System: All advanced int, float and Vector3 selection fields now support local, global and object game variable selections.
- new: Event System: Base Steps: 'Raycast Object' step available. Adds/removes a game object that was hit by a raycast to the 'Found Objects' list.
- new: Event System: Variable Steps: 'Raycast to Variable' step available. Changes a Vector3 game variable using the hit point of a raycast.
- new: Event System: Spawn Steps: The 'Spawn Prefab' step can now also spawn prefabs at defind positions (e.g. from game variables) instead of using an event object.
- new: Shops: 'Block Move AI' and 'Block Owner Move AI' settings available. Blocks either the move AI of all combatants or only the move AI of the shop's game object while the shop is opened.
- new: Shop Interactions: 'Turn Player to Event' and 'Turn Event to Player' settings available. Turns the event object and the player to one another upon opening the shop.
- new: Move AIs: 'Hunting Range' settings available. A combatant can optionally hunt only within a defined range of it's start position (i.e. spawn position). When hunting outside of the defined range, the combatant will stop hunting and return to the start position.
- new: Logs: 'Portrait Settings' available. Logs can display portraits in menu screens.
- new: Log Texts: 'Portrait Settings' available. Log texts can override their log's portrait.
- new: Menu Screens: 'Show Portrait' settings available for 'Log' menu parts. The log choice and info display can show a log's portrait.
- new: Input Keys: New 'Input Handling' type 'Any' available. Receives input at down, hold and up.
- new: Save Game Menu: The 'Save Slots' setting can now also be set to 0. Having 0 save slots only allows using AUTO save games and retry.
- new: Item Collectors: 'Destroy Collected' setting available for 'Single' and 'Random' item collectors. If enabled, the game object will be destroyed when an item has been collected, otherwise only the item collector component will be removed. By default enabled.
- new: Item Collectors: 'Show Dialogue' setting available for 'Single' and 'Random' item collectors. If enabled, the item collection dialogue will be displayed, otherwise the item will be collected immediately. By default enabled.
- new: Item Collectors: 'Show Console' setting available. If enabled, the console output for adding items to the inventory will be dispalyed. By default enabled.
- change: Item Collectors: The 'Show Notification' setting will now handle showing the item collection notification (inventory notifications) instead of showing the item collection dialogue.
- change: Event System: Spawn Steps: The 'Destroy Player' step is now also available in 'Battle Start' and 'Battle End' events.
- change: Animations: 'Auto Move Speed Parameters' for Mecanim animations can now also be used without using the combatant's 'Automatic Move Animation'.
- fix: Damage Dealers, Damage Zones: The damage dealer/zone components no longer require a 'Rigidbody' component, i.e. you can now also use a 'Rigidbody2D' component instead.
- fix: Interactions: 'In Trigger' and 'While Colliding' now work correctly without the other one enabled.
- fix: Game Time: The game time is now set correctly when starting a game without going through the main menu.
- fix: Animations: 'Mecanim State Checks' are now used correctly.
- fix: Screen Fader: Fading the screen will now fit the screen size in each GUI scale mode.
- fix: Menu Screens: Quantity selections called by menu screens with 'Pause Game' enabled will now work properly.


Version 2.1.3:
- new: Interactions: 'Trigger Stay' start types available. Starts an interaction (e.g. event, item collector) while the player (or another object) is within the object's trigger.
- new: Interactions: 'Collision Enter' start types available. Starts an interaction (e.g. event, item collector) when the player (or another object) starts colliding with the object. Requires both objects to have a collider and at least one object to have a rigidbody.
- new: Interactions: 'Collision Exit' start types available. Starts an interaction (e.g. event, item collector) when the player (or another object) stops colliding with the object. Requires both objects to have a collider and at least one object to have a rigidbody.
- new: Interactions: 'Collision Stay' start types available. Starts an interaction (e.g. event, item collector) while the player (or another object) is colliding with the object. Requires both objects to have a collider and at least one object to have a rigidbody.
- new: Battle Menus: 'Change Member' menu item type available. Allows changing a combatant against a member of the non-battle group.
- new: Combatants: Battle Animations: 'Retreat Animation' available. Uses battle events to animate a combatant's retreat from battle when using the 'Change Member' command. Needs a 'Calculate' step to exchange members.
- new: Combatants: Battle Animations: 'Enter Battle Animation' available. Uses battle events to animate a combatant joining the battle when using the 'Change Member' command.
- new: Control Maps: 'Change Member' action available. Exchanges the combatant with a defined non-battle member (defined by index).
- new: Status Effects: 'Change Member' end action type available. Exchanges the combatant with a defined non-battle member (defined by index) after the status effect ends.
- new: Battle AI: Action Steps: 'Change Member' step available. Exchanges the combatant with a defind non-battle member (defind by index).
- new: Battle AI: Status Steps: 'Get Status Value' step available. The combatant with the highest or lowest value of a selected status value will be added to the target list.
- new: Battle AI: Status Steps: 'Get Attack Attribute' step available. The combatant with the highest or lowest value of a selected attack attribute will be added to the target list.
- new: Battle AI: Status Steps: 'Get Defence Attribute' step available. The combatant with the highest or lowest value of a selected defence attribute will be added to the target list.
- new: Loot: 'Variable Condition' settings available in loot tables. If a loot table is available can depend on variable conditions (either global or object variables on the combatatant or spawner).
- new: Loot: 'Set After Drop' settings available in loot tables. Loot tables can change game variables (either global or object variables on the combatatant or spawner) after being used/dropped by a combatant.
- new: Combatant Spawners: 'Move AI' settings available. A combatant spawner can block or override the move AI of spawned combatants.
- new: Event System: Status Steps: 'Check Combatant' step available. Checks if an object (e.g. actor) is a selected combatant.
- new: Event System: Active Group Steps: The 'Check Player' step can now check objects (e.g. actors) if they are the player.
- new: Inspectors: The ORK Framework object (created when playing) now shows all global game variables in the inspector.
- new: Inspectors: The 'Object Variables' component now shows all game variables of the object in the inspector.
- new: Item Collectors: 'Show Notification' setting available for 'Single' and 'Random' item collectors. The item collection notification will be skipped when disabled.
- new: Shop Layouts: 'Sell Without Items' setting available. If disabled, 'Sell' button will be disabled when no items are available to be sold. By default disabled.
- new: Shop Layouts: 'Auto Close Sell' setting available. If enabled, the sell mode will automatically be closed when the last item has been sold. By default enabled.
- new: Inventory Settings: Drop Item Settings: 'Collector Star Type' setting available. Select the start type of the item collector used for dropped items. By default set to 'Interact'.
- new: HUDs: Combatant HUDs: 'Start Index Offset' setting available for 'Group' mode combatant HUDs. Starts displaying the combatant list from the defind offset, i.e. combatants before that index will be skipped.
- new: HUDs: Combatant HUDs: 'Limit List Length' settings available for 'Group' mode combatant HUDs. Limit the number of displayed combatants.
- new: Animations: Mecanim: 'Set Layer Weight' settings available for mecanim animations. Optionally sets the current weight of the animation's layer before playing it.
- new: Menu Screens: 'Close Screens (Opening)' settings available for menu screens not using 'Single Screen' mode. Automatically closes selected menu screens when opening the (non-single) menu screen.
- new: Menu Screens: 'Close Screens (Closing)' settings available for menu screens not using 'Single Screen' mode. Automatically closes selected menu screens when closing the (non-single) menu screen.
- change: Combatant Spawners: The spawner will now set its game variables each time a combatant spawned by the spawner is killed.
- fix: Menu Screens: The drag info will now be properly displayed in menu screens that pause the game.
- fix: Camera Events: Camera events wont throw errors and block control when changing scene while a camera event is active.
- fix: Event System: Movement Steps: The 'Transform to Variable' step is now correctly setting the Vector3 game variable.
- fix: Combatant Groups: The additional battle gains are now also collected when using loot tables.
- fix: Shops: 'Accept' input key doesn't throw errors when shop is open in sell mode without items to sell.
- fix: HUDs: Control HUDs: 'Control' type HUDs now correctly support inputs with hold time.


Version 2.1.2:
- new: Battle Settings: 'Auto Join' settings available. Combatants within range of a starting battle can automatically join the battle. This is only available in arena battles (i.e. using the 'Battle' component), not in real time area battles.
- new: Battles: Battle components can override the default 'Auto Join' settings from the 'Battle Settings'.
- new: Status Effects: 'Keep Overflow' setting available for status conditions. Changes exceeding the possible changes for status values will be remembered and added to the next change. E.g. 5.25 can only do a change of 5, 0.25 will be remembered and added to the next change.
- new: Status Values: 'Ignore 0 Damage' setting available for damage and critical damage notifications. Optionally ignores a damage of 0 (i.e. no damage) and doesn't perform the notification text and HUD flash.
- new: Abilities, Items: Status Changes: 'Ignore Barrier' setting available in user/target changes. Optionally ignores barrier values of 'Consumable' type status values and targets the status value directly.
- new: Status Effects: Status Conditions: 'Ignore Barrier' setting available. Optionally ignores barrier values of 'Consumable' type status values and targets the status value directly.
- new: Combatants: Status Value Time Changes: 'Ignore Barrier' setting available. Optionally ignores barrier values of 'Consumable' type status values and targets the status value directly.
- new: Battle Settings: Player/Enemy Advantage: 'Ignore Barrier' setting available. Optionally ignores barrier values of 'Consumable' type status values and targets the status value directly.
- new: Battle System: Bonus Settings: 'Ignore Barrier' setting available. Optionally ignores barrier values of 'Consumable' type status values and targets the status value directly.
- new: Event System: Status Steps: 'Ignore Barrier' setting available in the 'Change Status Value' step. Optionally ignores barrier values of 'Consumable' type status values and targets the status value directly.
- new: Event System: Movement Steps: 'Change Gravity' step available. Change Physics.gravity or Physics2D.gravity - influences all game objects with a 'Rigidbody' or 'Rigidbody2D' component.
- new: Battle Menus: 'Audio Settings' available. A battle menu can now play an audio clip when being opened and closed.
- new: Abilities, Items: 'Target Requirements' settings available. A target can optionally depend on status requirements and variable conditions. If the requirements aren't met, the combatant can't be targeted.
- change: Battles: Battles can't start while changing scenes.
- change: Save Games: Status effects of player combatants are now saved.
- change: Dialogues, Menus: Accepting a choice will now also reset all ORK input keys instead of only the Unity input axes. This fixes problems when using the accept key also to call the menus.
- fix: Menu Screens: Displaying the back or unequip button first can cause errors when not displaying unequippable equipment.
- fix: Combatants: Level up now allows to gain more than one level at once.
- fix: Battle Settings: The enemy counter type 'Letter' now correctly displays letters exceeding A-Z as AA-AZ, BA-BZ, etc.
- fix: Event System: Image Steps: Removing lower ID images before higher ID images resulted in errors.
- fix: HUDs: Using icons for status value bars without empty icons displayed the already consumed icons at the position of the last icon.
- fix: Battles, Combatants: Enabling 'Keep Prefab' in the combatant's settings could prevent a battle from ending when all enemies have been killed.
- fix: Real Time Battle Areas: Implemented a workaround for a Unity bug where the player quickly leaves and enters the area's trigger, although he is still inside it.


Version 2.1.1:
- new: Unity 4.5: ORK Framework now supports/requires Unity 4.5.
- new: Game Controls: Collision Camera: 'Collision Camera' settings available in the game control settings. The new 'Collision Camera' component uses racasts to find objects between player and camera and places the camera accordingly. Can be used with all built-in and custom camera controls, and is also active during events (i.e. while camera changes).
- new: Float Values: Advanced float settings throughout the framework (e.g. changing game variables) now allow using the current time of day (i.e. time since midnight in seconds) and date and time (i.e. time since 1-1-1970 in seconds). You can use this to use the real time or check for days since something was done.
- new: Status Values: 'Barrier' settings available for 'Consumable' type status values. You can use other 'Consumable' type status values as barriers, i.e. they will consume damage instead (as long as they haven't reached their minimum value). They can either fully consume a damage, or only a defined percent range (0-100 %).
- new: Status Values: 'Death On' setting replaces 'Death on Minimum' for 'Consumable' type status values. You can now select 'None' (i.e. doesn't kill the combatant), 'On Minimum' (combatant dies when value reaches its minimum) and 'On Maximum' (combatant dies when value reaches its maximum).
- new: Items: 'Equipment Part Changes' settings available. Items can add/remove equipment parts to/from a combatant. This can be used to either add additional equipment parts or to block/remove equipment parts from the combatant/class settings.
- new: Weapons, Armors: 'Equipment Part Changes' settings available. Weapons and armors can add additional equipment parts and block other equipment parts while being equipped. This can be done per equipment level.
- new: Abilities: 'Equipment Part Changes' settings available in passive abilities (i.e. useable in 'None'). Passive abilities can add additional equipment parts and block other equipment parts. This can be done per ability level.
- new: Status Effects: 'Equipment Part Changes' settings available. Status effects can add additional equipment parts and block other equipment parts.
- new: Classes, Combatants: The weapon/armor settings are now split into available and unavailable equipments (due to equipment parts). Unavailable equipment was previously not displayed and couldn't be enabled. Also, automatic unselection of weapons/armors due to equipment part changes is now disabled.
- new: Camera Positions: 'Rotation Is Offset' setting available. Uses the defined rotation of the camera as an offset to the target's rotation.
- new: Quests: Quests can now learn log texts to the player when being added to the player's quest list, the quest finishes or fails.
- new: Quest Tasks: Quest tasks can now learn log texts to the player when being activated or the task is finished or failed.
- new: Quests, Quest Tasks: Experience rewards can now optionally be given to the whole group (or only battle group) and split between group members.
- new: Event System: Spawn Steps: 'Destroy Object' step available. Destroys a game object.
- new: Event System: Equipment Steps: 'Check Part Available' step available. Checks if an equipment part is available (i.e. can be equipped, ignoring locked parts).
- new: Event System: Equipment Steps: 'Change Equipment Part' step available. Adds/removes equipment parts to/from a combatant. Removing only allows removing previously added parts. Can be used to add additional equipment parts to combatants.
- new: Event System: Equipment Steps: 'Change Blocked Part' step available. Adds/removes a blocked equipment part to/from a combatant. A blocked equipment part isn't available for equipping. Can be used to remove equipment parts from combatant/class settings.
- new: Event System: Crafting Steps: 'Learn Recipe' and 'Forget Recipe' steps can now optionally learn/forget all crafting recipes of a selected crafting type.
- new: Event System: Check Steps: 'Chance Fork' step available. Define multiple value ranges, the next step of the first range that contains the random chance (minimum <= chance <= maximum) will be executed.
- new: Event System: Camera Steps: 'Collision Camera' step available. Enable/disable the collision camera - requires a 'Collision Camera' component attached to the camera.
- new: Event System: Movement Steps: 'Stop Movement' step available. Stops movement from 'Change Position', 'Move Into Direction' and 'Curve Move' steps.
- new: Game Events: 'Starting Object' actor type available. Uses the game object that started the event as actor. In most cases this will be the player, but you can use this to access other objects that started an event.
- new: Battle AI: Check Steps: 'Chance Fork' step available. Define multiple value ranges, the next step of the first range that contains the random chance (minimum <= chance <= maximum) will be executed.
- new: Battle AI: Check Steps: 'Check Game Variable' step available. Checks for game variable conditions - if the conditions are valid, the 'Success' step will be extecuted, otherwise 'Failed'.
- new: Battle AI: Check Steps: 'Game Variable Fork' step available. Checks a single game variable for multiple conditions and executes the first valid condition's next step.
- new: Formulas: Check Steps: 'Check Chance' step available. Which step will be executed next is decided by chance.
- new: Formulas: Check Steps: 'Chance Fork' step available. Define multiple value ranges, the next step of the first range that contains the random chance (minimum <= chance <= maximum) will be executed.
- new: Menu Screens: 'Alive User' setting available. When opening the menu screen and the current user is dead, the first alive member of the group will be used as user. Since dead combatants can't use items, this can be used to automatically switch to a different user when using items in the inventory menu.
- change: Items, Abilities: Status value and status effect change settings can now be copied and moved.
- change: Editor: The drag bar positions are now saved each time when the editor is closed. Previously, they have only been saved when the data has been saved. The remembered sections will be reset the first time you open the editor after this update.
- change: Game Events: The blocking event state will be removed during battles started by the the 'Start Battle' step. The battle controls are now fully useable during those battles.
- change: Scene Changers: When using a scene changer during battles, the battle will end.
- fix: Input Keys: Releasing an input key using 'Hold Time' will now correctly reset the hold timer.
- fix: Combatants, Status Effects: Auto effects will now be checked after initializing the start values of consumable status values.
- fix: Menu Screens: Hiding not equipable equipment (i.e. disabling 'All Weapons/Armors') caused a equipping wrong equipment.
- fix: Combatants: Conditional prefabs now correctly check for the combatant's death.
- fix: Battle System: Active Time Battles: The 'Start Calculation' formula is now correctly used at the start of the battle.
- fix: Battle System: The death event of the last player combatant didn't play when not using 'End Immediately'.
- fix: Game Over: Dead combatants played the idle animation while fading to the game over scene.
- fix: Item Collectors: Not using quantity limits could lead to items not being collected.


Version 2.1.0:
- new: Quest System: The new quest system consists of 'Quest Types', 'Quests' and 'Quest Tasks'. They are created in the 'Game' section of the ORK Framework editor.
- new: Quest Types: Quest types are used to separate quests into different types. The quest types can be used in menus and HUDs to filter/limit displaying quests.
- new: Quests: Quests can give rewards (experience, items, equipment and money) upon successful completion. A quest consists of multiple quest tasks.
- new: Quest Tasks: Quest tasks are the actual things a player needs to do in a quest. You can define requirements for activating, finishing or failing a task. The task's progress can be handled automatically.
- new: Menu Screens: 'Quest' menu part available. Displays the quests of the player and can be separated by quest types. Quests can optionally be set active/inactive.
- new: Interactions: Quest/task status conditions available in all interactions (e.g. Event Interaction, Item Collector).
- new: Requirements: 'Knows Logs', 'Has Quests', 'Has Not Ended Quests' and 'Has Ended Quests' settings available. Requires the player to knows at least one log, quest, active/inactive quest or finished/failed quest.
- new: Game Variables: Advanced Vector3 operations available. Setting a Vector3 game variable can now use the following operators: Add, Sub, Set, Cross, Min, Max, Scale, Project and Reflect. This is available throughout the entire framework.
- new: Inventory, Currencies, Items, Weapons, Armors: 'Inventory Notifications' settings available. Adding/removing items, weapons, armors and currency to/from the player's inventory can optionally display notifications. Also, if something can't be added due to inventory limitations can display a notification. Currencies, items, weapons and armors can override the default notifications.
- new: Inventory, Crafting Recipes: 'Crafting Notifications' settings available. Learning/forgetting a crafting recipe and using it (successfully/failed) can optionally display notifications. Crafting recipes can override the default notifications.
- new: Combatants: Auto Attacks: 'Target Type' setting available. Select the possible targets of auto attacks, e.g. only group/individual targets or all combatants.
- new: Combatants: 'Keep Prefab' settings available in the 'Death Settings'. The combatant's prefab wont be destroyed when the combatant dies (only for non-members of the player group).
- new: Text Codes: New text codes available for quest, quest task and quest type information.
- new: Event System: 'Quest Steps' available. There are various new steps available to manage/check the status of a quest and quest tasks.
- new: Event System: Dialogue Steps: 'Quest Choice' step available. Displays a dialogue or choice dialogue with quest information of a selected quest.
- new: Event System: Dialogue Steps: The message of 'Show Dialogue', 'Teleport Choice' and 'Value Input Dialogue' steps can optionally also be printed to the console.
- new: Event System: Dialogue Steps: The cancel choice in 'Teleport Choice' steps can optionally be the first choice in the list.
- new: Event System: Status Steps: 'Initialize to Level' step available. Initialize a combatant/group to a defind level and class level. All previous progress (e.g. learned abilities, status values) will be lost, all equipment will be unequipped.
- new: Event System: Base Steps: 'Wait For Input Fork' step available. Waits for an input key (out of multiple keys) to be pressed, either for a set amount of time, or until the key has been pressed. The next step of the input key that's pressed first will be executed.
- new: Event System: Statistic Steps: 'Clear Statistic' can now optionally reset a selected statistic value instead of all values.
- new: Event System: Statistic Steps: 'Check Statistic' step available. Checks a selected statistic value against a defined value.
- new: Event System: Statistic Steps: 'Statistic To Variable' step available. Stores a selected statistic value into a float game variable.
- new: Event System: Movement Steps: 'Transform To Variable' step available. Stores a transform's position, rotation or scale into a Vector3 game variable.
- new: Event System: Movement Steps: 'Block Move AI' step can now optionally block the move AI of a selected object instead of blocking it completely (i.e. for all objects).
- new: Battle Events: Battle Steps: 'End Phase' step available. The phase will end after the action finishes - combatants who didn't chose an action yet will not be able to do so. Only used in 'Phase' battles.
- new: Status Values: 'Start Value' settings available for 'Consumable' type status values. Define the start value of 'Consumable' status values either in percent of their maximum status value or an absolute value. By default 100 % of their maximum status value.
- new: Status Values: 'No Regeneration' setting available for 'Consumable' type status values. Optionally excludes a status value from being fully recovered upon regeneration (e.g. on level up or by the 'Regenerate' event step).
- new: Menu Settings, GUI Boxes: 'Show Unfocused' setting available for choice icons. The choice selection icon can optionally be displayed on a not focused GUI box.
- new: Battle AI: 'End Phase' setting available in all action steps. The phase will end after the action finishes - combatants who didn't chose an action yet will not be able to do so. Only used in 'Phase' battles.
- new: Game Settings: Area Notifications: 'Queue Area Notifications' setting available. Area notifications can optionally be displayed in sequence instead of replacing the current notification with a new one.
- new: Areas, Game Settings: Area Notification: 'Play Sound' settings available. An area notification can play an audio clip (even when not displaying the notification box).
- new: Areas, Game Settings: Area Notification: 'Show Portrait' setting available. Showing an area's portrait in the area notification is now optional. By default enabled.
- new: Battle Texts: 'Queue Infos' setting available. Battle infos can optionally be displayed in sequence instead of replacing the current info with a new one.
- new: HUDs: 'Quest' type HUD available. Displays a list of quests and their tasks. The displayed quests can be filtered by quest type and status.
- new: HUDs: 'Hide Empty HUD' setting available. A HUD will be hidden if it doesn't display any elements. Not available for 'Control', 'Navigation' and 'Console' type HUDs.
- new: HUDs: 'Quest' and 'Quest Task' settings available in 'Tooltip' type HUDs. A tooltip can be dispalyed when hovering the mouse above a quest or quest task in a 'Quest' type HUD.
- new: Main Menu: 'Stop Music' setting available. The current music is stopped when starting a new game. By default enabled.
- new: Status Effects, Battle Texts: 'Remove Status Effect' notification text settings available. You can display a notification text when a status effect is removed from a combatant.
- change: Object Variables: Accessing an object variable's component in the start function of a script now also initializes the variables.
- change: Event System: Moving, rotating and scaling objects over time will now start immediately. Previously, the changes started in the next frame, leading to sometimes not being finished completely after the wait time.
- change: Event System: PlayerPref Steps: The PlayerPrefs are saved each time after setting a PlayerPrefs.
- change: Inventory: Inventory limits prevent getting items from item collectors/boxes and buying items.
- fix: GUI Boxes: 'Select First' is now working correctly.
- fix: Menu Screens: The description part in equipment screens now displays the correct part information.
- fix: Menu Screens: Crafting menus could throw an error when using a choice for creating recipe outcomes.
- fix: Menu Screens: Group menus could throw an error (and block the menu from exiting) when using complex combatant layouts.
- fix: Event System: Base Steps: The 'Wait For Input' step now receives the input from input keys with 'Hold Time' correctly.
- fix: Event System: Game objects that are destroyed before being used in steps wont throw errors.
- fix: Console: Learning and forgetting console texts used the action range settings instead of learn/forget range.
- fix: Weapons, Armors: Enabling/disabling equipment parts wont throw errors when not using combatant equipment override.
- fix: Shops: Selling the last item of the inventory (without displaying 'Back' buttons) wont get you stuck in the shop any longer.
- fix: Save Games: Saving scene data (e.g. finished battles, collected items) is now working correctly.
- fix: Status Effects, Abilities, Items: Status effects overriding an ability/item attack attribute could result in an error.


Version 2.0.9a:
- new: Battle Events: Battle Steps: 'Use Ability Calculation' step available. Uses an ability (without animation) - the user/target changes will be calculated. The user doesn't need to know the ability.
- new: Battle Events: Battle Steps: 'Use Item Calculation' step available: Uses an item (without animation) - the user/target changes will be calculated.
- new: Shop Layouts: 'Show Portrait' setting available in the 'List Box' settings. The portrait of a selected item, weapon or armor in can be displayed.
- change: Controls: Blocking events will now prevent the use of battle related controls (e.g. control maps, group/individual target keys, ...).
- change: Editor: Combatants: The combatant's settings have been rearranged into 'Base Settings', 'Status Settings', 'Attacks & Abilities', 'Inventory & Equipment', 'Battle Settings' and 'Animations & Movement'.
- fix: Custom Controls: Custom camera controls aren't blocked by player control anymore.
- fix: Menu Screens: The description part now displays the correct content information of an equipped weapon/armor when selecting an equipment part.
- fix: Phase Battles: The next phase now starts automatically if no combatant of the current phase can perform an action.


Version 2.0.9:
- new: Event System: 'Found Objects' are available in all actor/waypoint/prefab-selections. Found objects are game objects in the scene that have been searched (and found) by the new 'Search Object' step.
- new: Event System: Base Steps: 'Search Objects' step available. Searches for game objects in the scene and adds or removes them to the 'Found Objects' list, or removes all found objects from the list.
- new: Battle Events: New actors available. Use 'User Group', 'Target Groups', 'All Allies', 'All Enemies' and 'All Combatants' actors for advanced battle events.
- new: Battle Events: Battle Steps: 'Change Action Targets' step available. A combatant is added or removed as a target of the action, or all targets can be removed.
- new: Game Events: Actors and waypoints have advanced 'Find Object' settings. You can now search for objects within a defined range.
- new: Combatants: 'Equipment Settings' available. Combatants can optionally override the equipment settings of their class, making the available equipment parts, weapons and armors independent from the class.
- new: Combatants: 'Experience Rewards' now have advanced reward value settings. You can use a value, game variables, formulas and other sources for the experience reward value.
- new: Abilities, Items: 'Animation Settings' available in the 'Damage Dealer Settings'. Damage dealers can optionally use battle events to create more complex actions when hitting a damage zone.
- new: Damage Zones: 'Audio Settings' available. Damage zones can play audio clips or a sound type when receiving damage from a damage dealer.
- new: Equipment Viewers: Advanced material settings available. You can optionally use the renderer of a child object and use an indexed material (if the renderer uses multiple materials).
- new: Menu Settings: 'Mouse Over Selection' setting available. Optionally selects (not accepts) a choice when the mouse cursor is above it (only on the currently focused GUI box).
- new: Game Controls: Custom Controls: The custom control behaviours are now separated from player/camera controls. You can select on which object (player or camera) a control is located and with which control they'll be blocked (player or camera).
- new: Placement: 'Scale' settings available for placing/mounting objects (e.g. spawn prefab step, cursor placement, etc.) throughout the entire framework.
- new: Quantity Selections: 'Hide Buttons' setting available for quantity selection buttons. Optionally hide the buttons, but still allows changing the quantity with the input keys.
- new: Quantity Selections: Advanced text codes available to display inventory money/item changes, details about the currency and the icon of the item.
- new: Combatant Component Inspector: The combatant component's inspector now displays advanced information on a combatant's battle data and actions.
- new: Menu Screens, Item Boxes: Description Part: Advanced text codes available and optional custom description text.
- change: Value Input: The value input field name is now multi-lingual.
- fix: Combatants: The start equipment's requirements will now also use bonuses from classes.
- fix: Shop Layouts: Buy/sell quantity selections got mixed up when using something other than 'Default'.
- fix: Battle Menus: The target cursor wasn't removed when clicking on a target to select it.
- fix: Phase Battles: Selecting 'End Phase' after canceling a combatant's battle menu didn't progress to the next phase.
- fix: Phase Battles: The combatant selection with 'Only Available' enabled and combatants not able to perform an action didn't progress to the next phase.
- fix: Active Time Battles: Doing nothing (i.e. 'End' battle menu item) resultet in the battle menu not reappearing when the timbar filled again.
- fix: GUI Boxes: Disabling the 'Use Cancel Button' setting was ignored.


Version 2.0.8:
- new: Formulas: 'Initial Value' settings available for formula selections throughout the entire framework. The formula will use the initial value as it's base and start the calculation with that value. The start value of the formula will be calculated to the initial value based on the selected operator in the formula's settings.
- new: Abilities, Items, Currencies, Weapons, Armors, Crafting Recipes: 'Portrait Settings' available. The portrait can be displayed by menus when the ability, item, currency, weapon, armor or crafting recipe has been selected.
- new: Menu Screens: 'Show Portrait' setting available in 'Ability', 'Ability Tree', 'Crafting', 'Equipment' and 'Inventory' menu parts. The portrait of a selected item, currency, weapon or armor in the inventory screen can be displayed.
- new: GUI Boxes: 'Show Button' setting available. Displaying a button around the content of a choice, tab or ok/cancel button is now optional (by default enabled).
- new: Abilities: Advanced 'Cast Time' settings available. The cast time of an ability can now also use formulas, game variables and other sources.
- new: Abilities: Advanced 'Reuse After' settings available. The reuse turns/time of an ability can now also use formulas, game variables and other sources.
- new: Shop Layouts: The 'List Box' settings can now optionally use a different layout for the back button. Use it when using custom info in the content layout that would otherwise also be displayed on the back button.
- new: Game Controls: Advanced 'Member Change Keys' settings available. Switching the player is optional for the field, turn based, active time, real time and phase battles. Switching the current menu user is available for active time and real time battles only.
- new: Interactions: 'In Blocked Control' setting available for 'Trigger Enter', 'Trigger Exit', 'Key Press' and 'Drop' interactions. The interaction can be started even while the player control is blocked (e.g. during a control blocking battle or event).
- new: Main Menu: The main menu can display a portrait. Optionally, every main menu option can display a different portrait.
- new: Battle Menus: Battle menus can display portraits of selected items, abilities, equipment, targets and command targets.
- new: Battle Menus: 'Use Sub-Menu Boxes' setting available. Battle menus can optionally use different GUI Boxes for different sub menus (e.g. ability type menu, item menu, target menu).
- new: Battle Events: Battle Steps: 'Activate Damage Dealer' step can now use the ability's or item's audio clip and prefab defined in their damage dealer settings.
- new: Damage Dealers: Environmental damage dealers now use the audio/prefab settings defined in the ability's damage dealer settings.
- fix: Shop Layouts: The content layout of the 'List Box' settings now displays custom info.
- fix: Event System: Movement Steps: 'Change Position' step using movement by speed now only faces the move direction if 'Face Direction' is enabled.
- fix: Real Time Battle Areas: Battle now ends correctly when leaving real time battle areas.
- fix: Input Keys: Getting the axis value from input keys set via HUD or code is now correct.
- fix: Menu Screens: Spamming the 'Cancel' key when having multiple non-single menu screens opened caused problems.
- fix: Turn Based Battles: Killing an enemy that had the last action could lead to the battle not continuing.
- fix: Real Time Battles: Death of combatants could lead to wait times between actions.


Version 2.0.7:
- new: GUI Boxes: You can use different text format settings for the text, info and title of choices.
- new: Control Maps: 'Need Targets' setting available. 
- new: Camera Controls: 'Mouse' camera control zoom by keys received an option to use a single key as axis instead of two separate buttons. Use this option for mouse wheels.
- new: Finding Objects: Finding objects in game event actors, navigation markers and custom main menu choices can now search for attached components. The found object is the one the component is attached to.
- new: Weapons, Armors: 'Viewer Material' setting available. Weapons and armors can change the material used by a renderer attached to the same game object as an 'Equipment Viewer'.
- new: Group Targets: You can now select different group targets for different abilities, ability types, items and item types. Settings are made in 'Battle System > Battle Settings'.
- new: Individual Targets: Set individual targets for a combatant to use abilities and items on. Work like group targets. Settings are made in 'Battle System > Battle Settings'.
- new: Combatants: 'Attack Individual Target' setting available. Uses an individual target for AI attacks.
- new: Combatants: 'Run Speed Threshold' and 'Sprint Speed Threshold' settings available in the combatant's 'Animation Settings'. Use thresholds for a smoother transition between walk/run and run/sprint animations.
- new: Battle Menus: 'Auto Target Settings' available. Optionally use abilities (also attacks) and items automatically on a group target or individual target.
- new: HUDs: 'Combatant Origin' setting available in 'Combatant' type HUD elements (except for 'Shortcut' elements). Select if the displayed information comes from the HUD's user, a group/individual target or the last target (of an attack, ability or item) of the user.
- new: HUDs: Shortcuts ('Combatant' type HUDs') can optionally target a group/individual target automatically when double clicked.
- new: Damage Types: The new damage types define the animation types used for damage, critical damage and evasion. Damage types are assigned to ability/item types and can optionally be assigned to abilities and items as well. This allows having different damage/evasion animations for each ability or item.
- new: Ability Types, Item Types: Set the default damage type for all abilities/items of the ability/item type.
- new: Abilities, Items: Abilities and items can override the default damage type of their ability/item type.
- new: Move AI: Advanced status requirements available for hunting and flee conditions.
- new: Interactions: The 'Trigger Enter' and 'Trigger Exit' start types of interactions can now be started by other game objects than the player. If an object can start the event can be checked by name/tag or by checking of added components, or both.
- new: Crafting Recipes: 'Audio Settings' available. Crafting recipes can play an audio clip when the creation has been successful or failed.
- new: Custom Controls: 'From Root' setting available. Searching/adding behaviour components will use the root of the game object - use this setting if the object isn't the real root.
- new: Custom Controls: You can optionally change fields and properties of custom control behaviours.
- new: Value Checks: 'Not Equal' check available. You can now use the not equal check in all value checks (previously only is equal, less or greater) - e.g. used in status requirements, distance checks, etc.
- new: Animations: 'Auto Move Speed Parameter' settings available for Mecanim. Optionally set the horizontal and vertical move speed of a combatant to float parameters of the game object's Mecanim animator.
- new: Scene Wizard: You can now create interaction controllers for 2D and 3D interactions.
- change: Status Values: 'Consumable' type status values can now only select 'Normal' type status values as maximum status value.
- change: Event System: 'Show Notification' step is now available in battle events, battle start events, battle end events and phase battle events.
- change: Battle Events: The 'Death' event now uses the combatants who attacked the dying combatant as targets.
- change: Animation Types: The animation types for damage, critical damage and evasion are now defined in 'Base/Control > Damage Types'.
- change: Difficulties: A combatant's 'Experience Rewards' are now influenced by the difficulty's faction status multipliers.
- change: Status Values: The percent bonus to combined 'Normal' type status values is now added after the final value calculation.
- change: Status Effects: When using formulas for status conditions, the calculation now happens every time, not only once.
- change: Status Effects: The caster of a status effect will now be used as the user for calculations of all changes, not only on cast.
- fix: Main Menu: The difficulty menu couldn't be displayed due to a bug.
- fix: Input Keys: Input keys of type 'Unity Input Manager' used as axis didn't receive input when the time scale was set to 0 (e.g. when using 'Freeze Game' in menu screens).
- fix: Menu Screens: The combatant's status values didn't update when the menu screen used 'Freeze Game'.
- fix: Menu Screens: Sub menus of menu screens that paused the game wheren't displayed and the game locked.
- fix: Abilities, Items: Using raycasts for 'None' target abilities/items is working now.
- fix: Event Interactions: When playing in the editor with an event interaction selected in the inspector, the event could be reloaded, stopping a running event from continuing.
- fix: Battle Start Events: The 'Spawn Combatants' step will now add player controls when spawning the player (e.g. when the battle takes place in a different scene).
- fix: Move AI: Stopping due to external influences (e.g. 'Stop Movement' status effect) wont cause loss of the hunted target any longer.
- fix: Combatant Spawners: Using combatant spawners with teleport battles (i.e. fighting in a different scene) wont result in the already spawned combatants missing in the battle.
- fix: Save Games: Error when loading inventory with money solved. Status values will now display the correct value. Equipment is now loaded correctly.
- fix: Compatibility: The 'EaseType' enumeration is now in the ORKFramework namespace and can be used with HOTween.
- fix: Status Effects: Having wrong auto apply/remove settings could lead to a stack overflow because a status effect was added and removed at the same time.
- fix: GUI Editor: Displaying 'Combatant' or 'Turn Order' type HUDs in the GUI Editor could crash Unity when having deadlocked auto apply/remove status effects.
- fix: Shops: Clicking on the 'Exit' button without selecting it first didn't return the control back to the player.


Version 2.0.6:
- new: Input Keys: 'Key Combo' input type available. Use a sequence of other input keys as input (e.g. 'A+A+B') as input. Define the used input keys and time the player has to press the next button. Each combo key can ignore selected other input keys (i.e. they wont cancel a combo).
- new: Value Inputs: Value inputs can now be selected and changed using input keys (i.e. vertical keys for selection, horizontal keys for slider changes, toggle bools and start text editing with accept).
- new: Event System: Equipment Steps: The 'Check Equipment' step can now check if an equipment part is generally equipped with a weapon or armor without specifying a specific equipment.
- new: Images: Use alpha mask textures to hide parts of displayed images. Used in image event steps, background images, portraits and HUDs.
- new: Event System: Rigidbody Steps: 'Add Force Rigidbody' step available. Add a force to 2D and 3D rigidbodies - optionally relative force (3D only).
- new: Event System: Rigidbody Steps: 'Add Torque Rigidbody' step available. Add torque to 2D or 3D rigidbodies.
- new: Event System: Rigidbody Steps: 'Position Force Rigidbody' step available. Add a force to a position, applying force/torque to 2D and 3D rigidbodies.
- new: Event System: Rigidbody Steps: 'Explosion Rigidbody' step available. Adds an explosion effect force to 3D rigidbodies.
- new: Event System: Rigidbody Steps: 'Stop Rigidbody' step available. Stops adding force/torque from other event steps to rigidbodies.
- new: Menu Screens: 'Remember Selection' setting available. Remembers the selected menu item when returning to the menu screen.
- new: GUI Boxes: 'Scale Settings' available in the 'Move In' and 'Move Out' settings. You can use this to create zoom effects when moving a GUI box in or out.
- new: Combatants: 'Experience Reward' settings available. You can now define experience rewards the player receives for defeating a combatant, without using the status development settings of the combatant. The real status values of the combatant and the exp reward can now be separated.
- new: Game Options: 'Random Battle Chance' option available in option menus (main menu and menu screens). Changes the chance for random battles happening. Used as percent of the chance defind in 'Random Battle Areas' (plus bonuses). E.g. 100 % is the default chance, 0 % is no random battles, 200 % is double chance.
- new: Abilities, Items: 'Random Battle Factor' settings available. Abilities and items can change the random battle factor for a defined amount of time. While the factor is changed by an ability/item, the factor bonuses of player group members are ignored. The factor is defined in percent.
- new: Control Maps: You can optionally select the equipment part used for an attack. The attack will only be used if a weapon with 'Override Attack' is equipped on the selected equipment part. Uses the attack of the equipped weapon.
- new: Battle Menus: 'Equipment' menu item type available. Lists the combatant's equipment parts and allows changing the equipment (similar to the equipment menu).
- new: Battle menus: 'Command' menu item type available. Give commands to other members of the group. Optionally only for 'AI Controlled' combatants. The given command will be performed the next time they're able to choose an action, giving a different command will override the previous one.
- new: Status Effects: 'Block Equipment Change' and 'Block Command' settings available. Status effects can now prevent a combatant from changing equipment and giving commands to other combatants.
- change: Battle Menus: Abilities and items can only be selected if targets are available.
- change: Control Maps: Abilities and items can only be used (and display a target menu) if targets are available.
- fix: Scene Wizard: Creating camera positions with the scene wizard will again add them to the data when the ORK Framework editor isn't already opened.
- fix: Combatants, Classes: Class development now uses the correct maximum values for class experience. The class experience is based on the status development of the class.
- fix: Battle System: Using items/abilities on combatants without doing damage will now also mark them as attacked by the user and allows the player to get battle gains from them.
- fix: Battle System: An enemy can't start a battle (when the player enters the combatant's battle start range) while a blocking menu is opened.
- fix: Battle Settings: Not using the target menu will now prevent the target menu from being displayed.


Version 2.0.5:
- new: Game Controls: Camera Control: 'Top Down Border' camera control available. Top down camera following the player until he crosses a border. The border is defind in the scene using a 'Camera Border' component with a 'Box Collider' (the collider defines the border).
- new: Scene Wizard: 'Camera Border' can be added to the scene through 'Create Object'. Adds a new game object with a 'Box Collider' and a 'Camera Border' attached.
- new: Event System: Camera Steps' 'Camera Control Target' step available. Changes the target of the camera control (i.e. the object the camera will follow). Requires the control to be a descendant of 'BaseCameraControl' (e.g. all built-in camera controls).
- new: Status Effects: 'End On Death' setting available. The effect will end when the combatant dies. By default enabled.
- new: Status Effects: 'Absorb Damage' settings available in 'Status Conditions'. Optionally absorbs damage dealt to a selected status value by abilities/items (with 'Use Absorb Effect' enabled).
- new: Status Effect Changes: 'Force' setting available. The status effect change will be forced, ignoring the target's status effect immunities.
- new: Abilities, Items: 'Use Effect Absorb' setting available. The user of the ability/item will absorb damage dealt according to applied absorb status effects.
- new: Abilities, Items: 'Volume' settings available for playing audio clips on status changes.
- new: Game Settings: 'Initial Game Variables' settings available. Define game variables that will be automatically set up when starting a game (i.e. before the main menu).
- new: Main Menu: 'Options' settings available. Add an options button to the main menu to display an options menu. You can change the music and sound volume, text speed and custom options (using global game variables).
- new: Menu Screens: 'Volume' settings available for the open and close audio clips.
- new: Menu Screens: 'Pause Settings' available. The game can optionally be paused while a menu screen is open. Pausing the game can also optionally pause the game time and freeze the game completely.
- new: Menu Screens: 'Options' menu part available. You can change the music and sound volume, text speed and custom options (using global game variables).
- new: Menu Audio: 'Value Input' audio settings available. A sound can be played when changing an input value (e.g. options menu, value input dialogues). Setting can be found in 'Menu Settings' and in GUI boxes which override the default audio settings.
- new: Inventory Settings: Item Collection: A sound can be played when collecting an item. The used sound can be either a selected sound type assigned to the player's combatant, or a selected audio clip.
- new: Inventory Settings: Item Box: Like the item collection, item boxes can now also play an animation and sound when the player interacts with them.
- change: Event System: Dialogue Steps: Choices in 'Show Dialogue' steps can be copied and moved.
- change: Event System: Inventory Steps: 'Add To Inventory', 'Remove From Inventory', 'Has In Inventory' and 'Drop Items' steps can now use formulas, game variables and other values to set the quantity and chance of items.
- change: Game Variables: The 'Is Valid' setting when checking game variables is now by default enabled.
- change: Stealing: Stealing items/money will now remove the stolen item/money the target's inventory, even if it's not the player.
- fix: Shops: Clicking on a 'Cancel' button when not using the buy/sell box and type box caused an error.
- fix: Node Editor: Event System: 'Show Dialogue' could throw an error when loading an event with a 'Choice' type dialogue without any choices.
- fix: Status Effects: Spawning prefabs for auto applied effects could sometime be displayed when spawning a combatant and immediately removing the effect.
- fix: Battle System: Enemies killed by other NPCs (not part of the player faction) wont give the player experience and loot - only if the player also attached the combatant.


Version 2.0.4:
- new: Shops: Selling individual items, weapons and armors can now depend on variable conditions.
- new: Shops: 'Sell to Shop' settings available. You can optionally limit which item types the player can sell to a shop.
- new: Text Code: 'Log Text' text code available. Displays the text of a log text, e.g. #logtext5# displays the text of log text with ID (index) 5.
- new: Game Controls: Custom player/camera control scripts can now be added to child objects.
- new: Console Settings: 'Unity Console Output' setting available. Optionally print new console lines in the Unity console when playing in the Editor.
- new: Event System: Equipment Steps: 'Lock Equipment Part' step available. Lock/unlock an equipment part of a combatant. Locked equipment parts can't change equipment.
- new: Event System: Equipment Steps: 'Check Part Locked' step available. Checks if a selected equipment part of a combatant is locked.
- new: Event System: Variable/Inventory Steps: 'Inventory To Variable' step available. Stores the quantity of a selected item, currency, weapon or armor found in an inventory into a float game variable.
- new: Status Effects: End after turn/time now also allows using formulas, game variables, PlayerPrefs and game time to determine the turns/time.
- new: Mouse/Touch Controls: New mode 'Hold' available for mouse/touch controls. Recognizes the input while holding it down (i.e. everything between 'Start' and 'End'). E.g. used in the 'Mouse' player controls.
- new: GUI Boxes: 'Selected Choice Offset' setting available. Optionally add an offset to the x-position of a selected choice. Setting also editable in the WYSIWYG editor.
- new: Node Editor: You can remove a focused node by pressing 'Delete'. You can remove the focused nodes and all following nodes (remove chain) by pressing 'ALT + Delete'. A focused node is displayed with an additional highlight around the node.
- new: Main Menu: 'Custom Choice Settings' available. You can add custom choices to the main menu (before the 'Exit' button). A custom choice will search for a game object in the scene and try to call a defined function on it. The call happens after the main menu is closed.
- new: Input Keys: 'Hold Time' setting available. When using input handling 'Hold' or 'Down', you can define the time the key has to be held to recognize the input.
- new: Inventory Settings: Item Collection: 'Auto Close' settings available. The item collection dialogue can automatically close after time.
- new: Combatants: The combatant's object variables can be initialized with default variables.
- new: Scene Objects: 'Object Variables' settings available. You can automatically add an 'Object Variable' component to a scene object and initialize them with default values.
- change: Editor: Events: The 'Close' button will now display a question dialogue if you really want to close the event.
- change: Status Effects: Reapplying a status effect (i.e. applying it again when it's already added) now also checks the hit chance.
- change: Inventory Settings: Item Collection: Playing an animation now uses Animation Types.
- fix: HUDs: 'Console' type HUDs don't throw an error when using 'All Console Types'.
- fix: Random Battle Area: Using a 'Battle Object' for the random battles blocked starting battles after the first battle.
- fix: Music: Playing the same music that is currently stored could sometimes result in playing from the stored time.


Version 2.0.3
- new: Editor: GUI Editor: The WYSIWYG editor allows editing HUDs.
- new: Editor: Updating events when saving or scanning for game variables in events can optionally only scan within a defind folder in the 'Assets' path.
- new: Editor: Status Development Curves: Creating a status development curve can now optionally use a curve instead of interpolation points.
- new: Abilities: 'Remove Turn' setting available. In 'Turn Based' battles, the target of the ability will be forced to skip a turn (if not yet performed).
- new: Items: 'Turn Based Order Changes' settings available. Like abilities, items can now change the turn order or remove the turn of a target in 'Turn Based' battles.
- new: Menu Screens: Equipment Part: Highlighting the 'Unequip' button will now also display preview values (for unequipping the equipment part).
- new: Status Effects: 'End Action' settings available. A status effect can optionally let the combatant perform an action when it ends. The action will be performed the next time the combatant can choose an action.
- new: Status Effects: 'No Turn Order Change' and 'No Turn Remove' settings available. Optionally grants immunity to turn order changes and removing a turn using abilities or items.
- new: HUDs: The new 'Turn Order' type HUD displays the current turn order of 'Turn Based' battles.
- new: HUDs: Navigation: 'Navigation' type HUD elements can display a background image.
- new: HUDs: Combatant: 'Combatant' type HUD status effect elements now allow adding multiple lables (similar to shortcut elements) to create more complex status effect information.
- new: HUDs: Combatant: 'Combatant' type HUD status effect elements can display the remaining time/turns of a status effect using % (without decimals, %1 (with 1 decimal) and %2 (with 2 decimals). Remaining turns are always displayed without decimals.
- new: HUDs: Tooltip: 'Status Effect' setting available in 'Tooltip' type HUDs. A tooltip can now be displayed when hovering the mouse above a status effect in a HUD.
- new: Combatants: 'Not Controllable' setting available. An 'AI Controlled' combatant can optionally be set to not controllable by the player. The combatant can't be selected as player using the player change keys.
- new: Languages: 'Initial Language' setting available. Set a language to be the initially selected language when starting the game.
- new: Damage Dealers: 'Environmental Damage' settings available. Set a damage dealer to be 'Always On' - damage will be dealt without firing an action first. You can use this for things like traps, hazardous areas or damage on contact with enemies.
- new: Damage Dealers: 'Reset Targets' settings available. When using 'One Time Damage' or 'One Target' you can use this to remove blocked targets after a defined amount of time, enabling additional damage by the damage dealer.
- new: GUI Boxes: 'Input Field Settings' available. Set spacing and alignment of input fields in dialogues.
- new: Event System: Dialogue Steps: 'Value Input Dialogue' step available. Displays a dialogue to input string, bool and float values and store them in game variables.
- new: Event System: Inventory Steps: 'Clear Inventory' step available. Removes everything (money, items, weapons and armors) from an inventory.
- new: Event System: Variable Steps: 'Clear Game Variables' step available. Removes all game variables (either local, global or object game variables).
- change: Abilities: 'Turn Based Order Changes' now also remove already fired actions that haven't yet been started. When using multi-turns, the turn value of the target will be changed instead of the turn order.
- change: Reflection: Call/check functions and check/change field/property exceptions now display the stack trace.
- fix: Move AI: 'Target Position Check' intervals of 0 now correctly update the target's position.
- fix: GUI Boxes: Selecting the last choice in a menu screen (e.g. equipment) which removes the choice wont cause an error any longer.
- fix: Event System: Animation Steps: 'Legacy Animation' now set's the layer and speed of animations correctly.
- fix: Node Editor: Adding unconnected nodes with the context menu will now place them correctly when scrolled down.
- fix: Player Group: Joining a combatant to the active player group after the player has already been spawned didn't spawn the new battle group member (requires 'Spawn Group' enabled in 'Game > Game Settings').


Version 2.0.2
- new: Editor: GUI Editor: The new GUI Editor allows editing GUI boxes in a WYSIWYG editor.
- new: Editor: You can open the ORK Framework editor using the hotkey 'CTRL + ALT + O'.
- new: Scene Wizard: You can open the ORK Scene Wizard using the hotkey 'CTRL + ALT + W'.
- new: Game Controls: Custom player/camera control scripts can now also be attached to child objects.
- new: Game Events: New event actor type 'Camera' available. Uses the event's camera as an actor.
- new: Event System: Movement Steps: 'Curve Move' step available. Move a game object using curves.
- new: Event System: Rotation Steps: 'Curve Rotation' step available. Rotate a game object using curves.
- new: Event System: Active Group Steps: 'Join Active Group' step allows adding a combatant group to the active player group.
- new: Abilities, Items: 'Use Screen Combatant' setting available. When using it's own combatant selection in menus, items and abilities can automatically be used on the current menu user without showing a combatant selection.
- new: Main Menu Settings: 'Set Start Group' setting available. You can use a Combatant Group as the player group at the start of a new game.
- change: GUI Boxes: The default settings for new GUI boxes have changed. The content/name box bounds have changed and the name box is now relative to the content box.
- fix: Event System: 'Use Center' setting in various steps wont cause objects to disappear when only a single target was used.
- fix: Battle System: Combatants dying when initialized (e.g. due to wrong setup, like 0 HP) wont cause an error.
- fix: Menu Screens: Hidden group members wont be displayed in 'Group Menu Parts' any longer.
- fix: Damage Dealers: Using an ability with the last available needed status value (e.g. MP) will no longer fail to activate the damage dealer.
- fix: Editor: Formulas: Testing formulas in the editor caused errors in some cases.
- fix: Status Effects: 'Auto Apply' caused an error in some cases.
- fix: GUI Boxes: The name box will is now displayed at the correct position when not relative to the content box with all anchor types.


Version 2.0.1
- new: Node Editor: Major update to the node editor. Button toolbar available at the top of the node display. Optional background grid display available. Searching the node list available.
- new: Editor: The variable list is automatically updated when saving the settings or an event. When saving the settings, only the settings will be scanned. When saving an event, only the event will be scanned.
- new: Items: 'Requirements' settings available. Using an item can depend on status requirements and variable conditions.
- new: Abilities: 'Requirements' settings now contain variable conditions. Using an ability can depend on variable conditions.
- new: Abilities, Items: 'Not On Self' setting available. When targetting allies or all combatants, the user can be excluded from being a target.
- new: Status Values: 'Critical Refresh Notification' and 'Critical Damage Notification' settings available. Optionally display different change notifications for critical hits.
- new: Event System: The new 'Image Steps' allow showing, fading and moving images on the screen. 'Show Image', 'Change Image Position', 'Change Image Color' and 'Remove Image' steps available.
- new: Event System: Variable Steps: 'Game Variable Fork' step available. Similar to 'Check Game Variable' step, but checks a single game variable for multiple values. The next step depends on which condition is valid.
- new: Event System: Status Steps: 'Status Fork' step available. The next step depends on which status requirement is valid.
- new: Event System: Status Steps: 'Change Status Value' step can optionally display change notifications as critical changes.
- new: Event System: Equipment Steps: 'Equipment Fork' step available. The next step depends on which weapon or armor is equipped on an equipment part.
- new: Event System: Fade Steps: 'Blink Object' step available. Starts/stops blinking a game object.
- new: Event System: Base Steps: 'Block Control Map' step available. Blocks/unblocks a selected control map or all control maps. A blocked control map can't be used.
- new: Event System: Dialogue Steps: 'Block HUD' step available. Blocks/unblocks a selected HUD or all HUDs. A blocked HUD wont be displayed.
- new: Event System: Function Steps: 'Change Time Factor' step available. Changes the time factor of Unity (everything in the game) or ORK Framework (things like battles and events only). You can use this to create slow motion effects.
- new: Event System: Function Steps: 'Check Component Enabled' step available. Checks if a component is enabled.
- new: Event System: Function Steps: 'Change Fields' step can now also change properties. Use the new 'Is Property' setting.
- new: Event System: Function Steps: 'Check Fields' step available. Checks the value of fields or properties of a component.
- new: Event System: Function Steps: 'Check Function' step available. Checks the return value of a function of a component.
- new: Event System: Movement Steps: 'Change Nav Mesh Target' step available. Sets the destination and speed of a Nav Mesh agent, or stops the agent.
- new: Formulas: Variable Steps: 'Game Variable Fork' step available. Similar to 'Check Game Variable' step, but checks a single game variable for multiple values. The next step depends on which condition is valid.
- new: Formulas: Combatant Steps: 'Status Fork' step available. The next step depends on which status requirement is valid.
- new: Logs: 'Invert Text Order' setting available. Optionally display log texts in inverted order (i.e. from high index to low index).
- new: Move AIs: 'Detect Only Leader' setting available. Detecting enemies can optionaly be limited to the group leader of an enemy group.
- new: Save Game Menu: Save Point: The 'Save' and 'Cancel' button are now optional (by default enabled). The 'Cancel' button can optionally override the default 'Cancel' button's content.
- new: Control Maps: 'Requirements' settings available for the whole control map and single control map keys. Using the control map or individual keys can optionally depend on status requirements and variable conditions.
- new: Battle AI: 'Check Distance' and 'Check Angle' steps available. Check possible targets upon distance and angle conditions. New 'Position Steps' group in step selection.
- new: Battle System: 'Stack Loot' setting available in all battle system types. Loot of the same kind will optionally be stacked. By default enabled.
- new: Turn Based Battles: 'Invert Turn Order' setting available. The turn order can optionally be inverted, i.e. sorted ascending, not descending (the combatant with the lowest value will have the first action).
- new: Turn Based Battles: 'Use Multi-Turns' setting available. A combatant can optionally perform multiple turns before another combatant had his turn. After a combatant performed his turn, his turn value will be reset to 0 and the turn value of all combatants will be increased. Available when 'Active Command' is enabled.
- new: Save Game Menu: 'Save Block States' setting available. The stated of blocked Input Keys, Control Maps and HUDs can be saved.
- new: Menu Screens: 'Menu Part Change Keys' settings available. Optionally use input keys to change the active menu part. You can only change to controlable parts (e.g. Inventory).
- change: Editor: Hotkeys: The navigation history hot keys now require the 'ALT' key to be pressed. Use 'ALT + Home' and 'ALT + End' to browse through the navigation history.
- change: Menu Screens: Items and abilities can't be used in a menu screen while the combatant is performing an action.
- fix: Menu Screens: Abilities and equipment displaying level up information will now display the correct inactive alpha value when inactive.
- fix: Event System: Function Steps: The 'Enable Component' step will now also enable/disable colliders, renderers, particle emitters and LOD groups.


Version 2.0.0
Initial release.

For changes to the beta versions, please visit http://orkframework.com.

