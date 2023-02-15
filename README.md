# Unity Style Guide


<a name="toc"></a>
## Table of Contents

> 1. [Introduction](#introduction)
> 1. [Project Structure](#structure)
> 1. [Scripts](#scripts)
> 1. [Asset workflow](#toc)

<a name="introduction"></a>
## 1. Introduction

### Sections

> 1.1 [Important Terminology](#importantterminology)

<a name="importantterminology"></a>
### 1.1 Important Terminology

<a name="terms-prefab"></a>
#### Prefabs
Unity uses the term Prefab for a system that allows you to create, configure, and store a GameObject complete with all its components, property values, and child GameObjects as a reusable Asset.

<a name="terms-serializable"></a>
#### Serializable
Variables that are Serializable are shown in the Inspector window in Unity. For more information see Unity's documentation on [Serializable](https://docs.unity3d.com/Manual/script-Serialization.html).

<a name="terms-cases"></a>
#### Cases
There are a few different ways you can name things. Here are some common casing types:

> ##### PascalCase
> Capitalize every word and remove all spaces, e.g. `DesertEagle`, `StyleGuide`, `ASeriesOfWords`.
> 
> ##### camelCase
> The first letter is always lowercase but every following word starts with uppercase, e.g. `desertEagle`, `styleGuide`, `aSeriesOfWords`.
>  ##### lowercase
> All letters are lowercase, e.g. `deserteagle`, 
>
> ##### Snake_case
> Words can arbitrarily start upper or lowercase but words are separated by an underscore, e.g. `desert_Eagle`, `Style_Guide`, `a_Series_of_Words`.

**[⬆ Back to Top](#table-of-contents)**

<a name="structure"></a>
## 2. Project Structure
The directory structure style of a project should be considered law. Asset naming conventions and content directory structure go hand in hand, and a violation of either causes unneeded chaos.

<pre>
Assets
    Art
        2D
	    Sprites
	    UI
	3D
	    Props
	    Enviroment
	    Creatures
    <a name="#structure-levels">Scenes</a>
        Frontend
        Act1
            Level1
    Lighting
        HDRI
        Lut
        Textures
    MaterialLibrary
      	Debug
    	Shaders
    Prefabs
        Architecture (Single use big objects)
            DeathStar
        Props (Repeating objects to fill a level)
            ObjectSets
                DeathStar
    Scripts
        AI
        Gameplay
            Input
        Tools
    Sound
        Characters
        Vehicles
            TieFighter
                Abilities
                    Afterburners
        Weapons
    Fonts
    ExpansionPack (DLC)
    Plugins
    ThirdPartySDK  
</pre>


The reasons for this structure are listed in the following sub-sections.

### Sections

> 2.1 [Folder Names](#structure-folder-names)

> 2.2 [Scenes](#levels)

> 2.3 [`Assets`](#structure-assettypes)

> 2.4 [Scene Structure](#scene-structure)


<a name="2.1"></a>
<a name="structure-folder-names"><a>
### 2.1 Folder Names
These are common rules for naming any folder in the content structure.

<a name="2.1.1"></a>
#### Always Use [PascalCase](#terms-cases)
PascalCase refers to starting a name with a capital letter and then instead of using spaces, every following word also starts with a capital letter. For example, `DesertEagle`, `RocketPistol`, and `ASeriesOfWords`.

<a name="2.1.2"></a>
#### Never Use Spaces
Re-enforcing [2.1.1](#2.1.1), never use spaces. Spaces can cause various engineering tools and batch processes to fail. Ideally your project's root also contains no spaces and is located somewhere such as `D:\Project` instead of `C:\Users\My Name\My Documents\Unity Projects`.

<a name="2.1.3"></a>
#### Never Use Unicode Characters And Other Symbols
If one of your game characters is named 'Zoë', its folder name should be `Zoe`. Unicode characters can be worse than [Spaces](#2.1.2) for engineering tools and some parts applications don't support Unicode characters in paths either.

Related to this, if your project has and your computer's user name has a Unicode character (i.e. your name is `Zoë`), any project located in your `My Documents` folder will suffer from this issue. Often simply moving your project to something like `D:\Project` will fix these mysterious issues.

Using other characters outside `a-z`, `A-Z`, and `0-9` such as `@`, `-`, `_`, `,`, `*`, and `#` can also lead to unexpected and hard to track issues on other platforms, source control, and weaker engineering tools. 

<a name="structure-no-empty-folders"></a>
#### No Empty Folders
There simply shouldn't be any empty folders. They clutter the content browser.

If you find that the content browser has an empty folder you can't delete, you should perform the following:
1. Be sure you're using source control.
1. Navigate to the folder on-disk and delete the assets inside.
1. Close the editor.
1. Make sure your source control state is in sync (i.e. if using Perforce, run a Reconcile Offline Work on your content directory)
1. Open the editor. Confirm everything still works as expected. If it doesn't, revert, figure out what went wrong, and try again.
1. Ensure the folder is now gone.
1. Submit changes to source control.

<a name="levels"></a>
### 2.2 All [Scene] Files Belong In A Folder Called Scenes
Scenes files are incredibly special and it is common for every project to have its own map naming system, especially if they work with sub-scenes or streaming scenes. No matter what system of map organization is in place for the specific project, all scenes should belong in `Assets/Scenes`.

Being able to tell someone to open a specific map without having to explain where it is is a great time saver and general 'quality of life' improvement. It is common for scenes to be within sub-folders of `scenes`, such as `Scenes/Campaign1/` or `Scenes/Arenas`, but the most important thing here is that they all exist within `Assets/Scenes`.

This also simplifies the job of cooking for engineers. Wrangling Scenes for a build process can be extremely frustrating if they have to dig through arbitrary folders for them. If a team's Scenes are all in one place, it is much harder to accidentally not cook a map in a build. It also simplifies lighting build scripts as well QA processes.


<a name="2.3"></a>
<a name="structure-assettypes"></a>
### 2.3 Do Not Create Folders Called `Assets`

<a name="2.3.1"></a>
#### Creating a folder named `Assets` is redundant.
All assets are assets.

<a name="2.2"></a>
<a name="scene-structure"></a>
## 2.4 Scene Structure
Next to the project’s hierarchy, there’s also scene hierarchy. As before, we’ll present you a template. You can adjust it to your needs. Use named empty game objects as scene folders.

<pre>
Debug
Managers
PersistentSystems
UI
Cameras
Lights
World
    Terrain
    Props
Gameplay
	Actors
	Items
_Dynamic
    Bullets
    Pickups
    Loot
</pre>

 - All empty objects should be located at 0,0,0 with default rotation and scale.
 - For empty objects that are only containers for scripts, use “@” as prefix – e.g. @Cheats
 - When you’re instantiating an object in runtime, make sure to put it in a subtype of _Dynamic, for example if you spawn a bullet make it a child of _Dynamic/Bullets  – do not pollute the root of your hierarchy or you will find it difficult to navigate through it.

**[⬆ Back to Top](#table-of-contents)**

<a name="scripts"></a>

## 3. Scripts

This section will focus on C# classes and their internals. When possible, style rules conform to Microsoft's C# standard.

### Sections
> 3.1 [Class Organization](#classorganization)

> 3.2 [Compiling](#compiling)

> 3.3 [Variables](#variables)

> 3.4 [Functions](#functions)

<a name="classorganization"></a>
### 3.1 Class Organization
Source files should contain only one public type, although multiple internal classes are allowed.

Source files should be given the name of the public class in the file.

Organize namespaces with a clearly defined structure,

Class members should be alphabetized, and grouped into sections:
* Constant Fields
* Static Fields
* Fields
* Constructors
* Properties
* Events / Delegates
* LifeCycle Methods (Awake, OnEnable, OnDisable, OnDestroy)
* Public Methods
* Private Methods
* Nested types

Within each of these groups order by access:
* public
* serializefield
* internal
* protected
* private
```
namespace ProjectName
{
	/// <summary>  
	/// Brief summary of what the class does
	/// </summary>
    public class Account
    {
      #region Fields
      
      [Tooltip("Public variables set in the Inspector, should have a Tooltip")]
      public static string BankName;
      
	  /// <summary>  
	  /// They should also have a summary
	  /// </summary>
      public static decimal Reserves;
 
	  public string BankName;
	  public const string ShippingType = "DropShip";
	  
	  private float _timeToDie;
	  
	  #endregion
	  
	  #region Properties
	  
      public string Number {get; set;}
      public DateTime DateOpened {get; set;}
      public DateTime DateClosed {get; set;}
      public decimal Balance {get; set;}
            
	  #endregion
	 
	  #region LifeCycle
	  
      public Awake()
      {
        // ...
      }
      
      #endregion
	  #region Public Methods
	  
      public AddObjectToBank()
      {
        // ...
      }
      
      #endregion
    }
}
```
<a name="namespace"></a>
#### Namespace
Use a namespace to ensure your scoping of classes/enum/interface/etc won't conflict with existing ones from other namespaces or the global namespace. The project should at minimum use the projects name for the Namespace to prevent conflicts with any imported Third Party assets.

#### All Public Functions Should Have A Summary

Simply, any function that has an access modifier of Public should have its summary filled out. 

```
/// <summary>
/// Fire a gun
/// </summary>
public void Fire()
{
// Fire the gun.
}
```

#### Commenting
Comments should be used to describe intention, algorithmic overview, and/or logical flow.
It would be ideal if from reading the comments alone someone other than the author could understand a function’s intended behavior and general operation.

While there are no minimum comment requirements and certainly some very small routines need no commenting at all, it is hoped that most routines will have comments reflecting the programmer’s intent and approach.

##### Comment Style
Place the comment on a separate line, not at the end of a line of code.

Begin comment text with an uppercase letter.

End comment text with a period.

Insert one space between the comment delimiter (//) and the comment text, as shown in the following example.

The // (two slashes) style of comment tags should be used in most situations. Where ever possible, place comments above the code instead of beside it. Here are some examples:
```
        // Sample comment above a variable.
        private int _myInt = 5;
```

#### Spacing
Do use a single space after a comma between function arguments.

Example: `Console.In.Read(myChar, 0, 1);`
* Do not use a space after the parenthesis and function arguments.
* Do not use spaces between a function name and parenthesis.
* Do not use spaces inside brackets.
<a name="3.1"></a>
<a name="compiling"></a>
### 3.2 Compiling
All scripts should compile with the least amounth of warnings possible and zero errors. You should fix script warnings and errors immediately as they can quickly cascade into very scary unexpected behavior.

Do *not* submit broken scripts to source control. If you must store them on source control, shelve them instead.

### 3.3 Variables
The words `variable` and `property` may be used interchangeably.

#### Variable Naming

##### Nouns
All non-boolean variable names must be clear, unambiguous, and descriptive nouns. 

##### Considered Context
All variable names must not be redundant with their context as all variable references in the class will always have context.

###### Considered Context Examples:
Consider a Class called `PlayerCharacter`.

**Bad**

* `PlayerScore`
* `PlayerKills`
* `MyTargetPlayer`
* `MyCharacterName`
* `CharacterSkills`
* `ChosenCharacterSkin`

All of these variables are named redundantly. It is implied that the variable is representative of the `PlayerCharacter` it belongs to because it is `PlayerCharacter` that is defining these variables.

**Good**

* `Score`
* `Kills`
* `TargetPlayer`
* `Name`
* `Skills`
* `Skin`

#### Variable Access Level
In C#, variables have a concept of access level. Public means any code outside the class can access the variable. Protected means only the class and any child classes can access this variable internally. Private means only this class and no child classes can access this variable.
Variables should only be made public if necessary.

Prefer to use the attribute `[SerializeField]` instead of making a variable public.

##### Local Variables
Local variables should use camelCase.

###### Implicitly Typed Local Variables
Use implicit typing for local variables when the type of the variable is obvious from the right side of the assignment, or when the precise type is not important.
```
var var1 = "This is clearly a string.";
var var2 = 27;
var var3 = Convert.ToInt32(Console.ReadLine());
// Also used in for loops
for (var i = 0; i < bountyHunterFleets.Length; ++i) {};
```

Do not use var when the type is not apparent from the right side of the assignment.
Example
```
int var4 = ExampleClass.ResultSoFar();
```

<a name="privatevariables"></a>
##### Private Variables
Private variables should have a prefix with a underscore `_myVariable` and use camelCase.

Unless it is known that a variable should only be accessed within the class it is defined and never a child class, do not mark variables as private. Until variables are able to be marked `protected`, reserve private for when you absolutely know you want to restrict child class usage.

##### Do _Not_ use Hungarian notation
Do _not_ use Hungarian notation or any other type identification in identifiers
```
// Correct
int counter;
string name;
 
// Avoid
int iCounter;
string strName;
```

#### Variables accessible in the Editor

##### Tooltips 
All [Serializable](#serializable) variables should have a description in their `[Tooltip]` fields that explains how changing this value affects the behavior of the script.

##### Variable Slider And Value Ranges
All [Serializable](#serializable) variables should make use of slider and value ranges if there is ever a value that a variable should _not_ be set to.

Example: A script that generates fence posts might have an editable variable named `PostsCount` and a value of -1 would not make any sense. Use the range fields `[Range(min, max)]` to mark 0 as a minimum.

If an editable variable is used in a Construction Script, it should have a reasonable Slider Range defined so that someone can not accidentally assign it a large value that could crash the editor.

A Value Range only needs to be defined if the bounds of a value are known. While a Slider Range prevents accidental large number inputs, an undefined Value Range allows a user to specify a value outside the Slider Range that may be considered 'dangerous' but still valid.

#### Variable Types

##### Booleans

###### Boolean Prefix
All booleans should be named in PascalCase but prefixed with a verb.

Example: Use `isDead` and `hasItem`, **not** `Dead` and `Item`.

###### Boolean Complex States
Do not use booleans to represent complex and/or dependent states. This makes state adding and removing complex and no longer easily readable. Use an enumeration instead.

Example: When defining a weapon, do **not** use `isReloading` and `isEquipping` if a weapon can't be both reloading and equipping. Define an enumeration named `WeaponState` and use a variable with this type named `WeaponState` instead. This makes it far easier to add new states to weapons.

##### Enums
Enums use PascalCase and use singular names for enums and their values. Exception: bit field enums should be plural. Enums can be placed outside the class space to provide global access.

Example: 
```
public enum WeaponType
{
    Knife,
    Gun
}

// Enum can have multiple values
[Flags]
public enum Dockings
{
	None = 0,
	Top = 1,
}

public WeaponType Weapon
```

##### Arrays
Arrays follow the same naming rules as above, but should be named as a plural noun.

Example: Use `Targets`, `Hats`, and `EnemyPlayers`, not `TargetList`, `HatArray`, `EnemyPlayerArray`.

##### Interfaces
Interfaces are led with a capital `I` then followed with PascalCase.

Example: ```public interface ICanEat { }```

<a name="functions"></a>
### 3.4 Functions, Events, and Event Dispatchers
This section describes how you should author functions, events, and event dispatchers. Everything that applies to functions also applies to events, unless otherwise noted.

#### Function Naming
The naming of functions, events, and event dispatchers is critically important. Based on the name alone, certain assumptions can be made about functions. For example:

* Is it a pure function?
* Is it fetching state information?
* Is it a handler?
* What is its purpose?

These questions and more can all be answered when functions are named appropriately.

<a name="function-verbrule"></a>
#### All Functions Should Be Verbs
All functions and events perform some form of action, whether its getting info, calculating data, or causing something to explode. Therefore, all functions should start with verbs. They should be worded in the present tense whenever possible. They should also have some context as to what they are doing.

Good examples:

* `Fire` - Good example if in a Character / Weapon class, as it has context. Bad if in a Barrel / Grass / any ambiguous class.
* `Jump` - Good example if in a Character class, otherwise, needs context.
* `Explode`
* `ReceiveMessage`
* `SortPlayerArray`
* `GetArmOffset`
* `GetCoordinates`
* `UpdateTransforms`
* `EnableBigHeadMode`
* `IsEnemy` - ["Is" is a verb.](http://writingexplained.org/is-is-a-verb)

Bad examples:

* `Dead` - Is Dead? Will deaden?
* `Rock`
* `ProcessData` - Ambiguous, these words mean nothing.
* `PlayerState` - Nouns are ambiguous.
* `Color` - Verb with no context, or ambiguous noun.

#### Functions Returning Bool Should Ask Questions
When writing a function that does not change the state of or modify any object and is purely for getting information, state, or computing a yes/no value, it should ask a question. This should also follow [the verb rule](#function-verbrule).

This is extremely important as if a question is not asked, it may be assumed that the function performs an action and is returning whether that action succeeded.

Good examples:

* `IsDead`
* `IsOnFire`
* `IsAlive`
* `IsSpeaking`
* `IsHavingAnExistentialCrisis`
* `IsVisible`
* `HasWeapon` - ["Has" is a verb.](http://grammar.yourdictionary.com/parts-of-speech/verbs/Helping-Verbs.html)
* `WasCharging` - ["Was" is past-tense of "be".](http://grammar.yourdictionary.com/parts-of-speech/verbs/Helping-Verbs.html) Use "was" when referring to 'previous frame' or 'previous state'.
* `CanReload` - ["Can" is a verb.](http://grammar.yourdictionary.com/parts-of-speech/verbs/Helping-Verbs.html)

Bad examples:

* `Fire` - Is on fire? Will fire? Do fire?
* `OnFire` - Can be confused with event dispatcher for firing.
* `Dead` - Is dead? Will deaden?
* `Visibility` - Is visible? Set visibility? A description of flying conditions?

#### Event Handlers and Dispatchers Should Start With `On`
Any function that handles an event or dispatches an event should start with `On` and continue to follow [the verb rule](#function-verbrule).

Good examples:

* `OnDeath` - Common collocation in games
* `OnPickup`
* `OnReceiveMessage`
* `OnMessageRecieved`
* `OnTargetChanged`
* `OnClick`
* `OnLeave`

Bad examples:

* `OnData`
* `OnTarget`

## 5. Asset Workflows

This section describes best practices for creating and importing assets usable in Unity.

<a name="toc"></a>
### Sections

> 5.1 [Textures](#textures)
>
> 5.2 [Audio](#audio)

<a name="textures"></a>
### 5.1 Textures

* Textures follow the [naming convention](#anc-textures) found above. 
* They are a power of two (For example, 512 x 512 or 256 x 1024).
* Use Texture Atlases wherever possible.
* 3D software should point to the Unity project textures for consistency when you save or export.
* It is better to resize the texture in Photoshop then to use Unity’s compression options when the in game texture resolution is already known. This reduces the file size and import time of the texture into Unity.
* When working with a high-resolution source PSD outside your Unity project use the same name for both the high-resolution and the imported Unity file. This allows quick iteration when swapping between the 2 textures.

More information for importing textures can be found here: [https://docs.unity3d.com/Manual/ImportingTextures.html](https://docs.unity3d.com/Manual/ImportingTextures.html)

Textures requiring the use of a Alpha channel should follow this guide: [https://docs.unity3d.com/Manual/HOWTO-alphamaps.html](https://docs.unity3d.com/Manual/HOWTO-alphamaps.html)

**[⬆ Back to Top](#table-of-contents)**

<a name="audio"></a>
### 5.2 Audio

Only import uncompressed audio files in to Unity using WAV or AIFF formats.

Great guide on [Unity Audio Import Optimization](https://www.gamasutra.com/blogs/ZanderHulme/20190107/333794/Unity_Audio_Import_Optimisation__getting_more_BAM_for_your_RAM.php)

**[⬆ Back to Top](#table-of-contents)**
