# Unity: Renaming scriptable objects

To rename/move scriptable objects without breaking everything:

1. Close Unity
2. Open project in VS Code
3. Enable view of .meta files (.vscode > settings.json > "files.exclude": {"*.meta": false})
4. Rename both the class file and the meta file to the new name
5. Open the class file and update the class name to match the file name
6. Add the `MovedFrom` attribute to the class, like that:
	- `[MovedFrom(true, "<old namespace>", null, "<old class name>")]`
	- For Example, if the class was in `HFramework.ScriptNodes` and was named `MySampleNode`, the attribute would be `[MovedFrom(true, "HFramework.ScriptNodes", null, "MySampleNode")]`
7. Reopen Unity

Leave the MovedFrom there for a while, and don't rename again as you will break people who did not convert yet.
