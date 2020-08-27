Simple Helvetica - a 3D character plugin for Unity3D
Version 1.0

Contact Info:
http://www.pepwuper.com/
Brandon Wu - brandon@pepwuper.com
copyright © 2012, Studio Pepwuper, inc

================
SIMPLE HELVETICA
================
Simple Helvetica is an easy-to-use 3D character model generator that enable you to instantly create text in 3D space for use in games and in GUI. 

Features
- 3D text models (font: Helvetica)
- Script to easily create, update, and generate 3D models in editor and during gameplay
- Full Inspector integration
- No need to hit Play to see the results
- Full iOS, Android and Flash support
- Full source code - simple C# code
- No DLLs or external resources
- Support for lighting/shadows, normal mapping, refraction (Unity Pro Only), texturing, Substance
- Interaction with other gameobjects
- Option to add box colliders and rigidbody

===============
GETTING STARTED
===============

1. Select from Menu: GameObject > Create Other > Simple Helvetica 
2. Click on the Text Area under "Text"
3. Type away and start using text 3D objects in your scene!

Notice:
- You can hit "enter" or "return" on your keyboard to insert a line break. Multiple lines is supported
- Each time you update your text, new 3D character objects are generated. So if you've gone into the individual child objects and made changes directly to them, those changes will not be retained.
- Undo is not currently supported.

Text:
This defines what characters will be generated
- variable: Text (string)
- default value: "SIMPLE HELVETICA\n \nby Studio Pepwuper"

Character Spacing: 
This determines how far apart the characters are.
- variable: CharacterSpacing (float)
- default value: 4f

Line Spacing: 
This determines the vertical (Y) distance between the lines. 
- variable: LineSpacing (float)
- default value: 22f

Space Width:
This determines how wide the "space" character is. 
- variable: SpaceWidth (float)
- default value: 8f

+ Box Colliders:
Add box colliders to all generated 3D character objects. Option to make them all triggers by checking "Is Trigger" (variable: BoxColliderIsTrigger)

- Box Collders:
Remove box colliders.

+ Rigidbody:
Add rigidbody to all generated 3D character objects. Options to adjust rogidbody settings and apply them to all generated 3D character objects.

Apply Mesh Renderer Settings:
By default, all 3D character objects shared the same "Default" material. You can use different ones for each individual object. You can also force all objects to use the same material and mesh renderer settings as the parent object by pressing down on "Apply Mesh Renderer Settings".


========================================================================
How to Construct a Simple Helvetica Prefab (in case if you overwrite it)
========================================================================
1. Create an empty GameObject (Menu: GameObject > Create Empty)
2. Rename it Simple Helvetica (optional)
3. Add SimpleHelvetica.cs component
4. Move "_Alphabets" from /Simple Helvetica/Models into the same GameObject
5. Add Mesh Renderer if you want to use the "Apply Mesh Renderer Settings" function (Menu: Component > Mesh > Mesh Renderer)
6. Done!

==============
PUBLIC METHODS
==============

GenerateText()
- Generate 3D character objects as defined by the Text string variable.  

AddBoxCollider()
- Add box colliders to all 3D character child objects.

RemoveBoxCollider()
- Remove box colliders on all 3D character child objects.

AddRigidbody()
- Add rigidbody to all 3D character child objects.

RemoveRigidbody()
- Remove rigidbodies on all 3D character child objects.

SetBoxColliderVariables()
- Update box collider variables on all 3D character child objects with the setting on this parent object.

SetRigidbodyVariables()
- Update rigidbody variables on all 3D character child objects with the setting on this parent object.

ResetRigidbodyVariables()
- Reset rigidbody variables on all 3D character child objects to the default value.

ApplyMeshRenderer()
- Apply the mesh renderer settings on this parent object to all 3D character child objects

====================
SUPPORTED CHARACTERS
====================

`1234567890-=
qwertyuiop[]\
asdfghjkl;'
zxcvbnm,./

~!@#$%^&*()_+
QWERTYUIOP{}|
ASDFGHJKL:"
ZXCVBNM<>?



