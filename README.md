A little something written in C# to help with migrating saves from other computers that might have incompatible DLCs installed.
And solving similar issues due to, uh, other reasons.

This utility rummages through your game save files and patches out an entry that contains required DLC.

Requires .NET Framework / .NET Core 2.0+ to build.

**Be sure to make a backup before attempting to use it. I mean it. Not going to be responsible for whatever trouble the tool might cause**

It can also be used to edit other things, but requires some work - 
feel free to fork and hack on it!
So far, only serialization of the main save entry ("Summary") is supported. 
If anybody bothers to reverse-engineer other data entry serialization routines, 
I'm open to pull-requests.
