# Templates

Templates are a way to create reusable sex script structures. For example, in the original game,
pretty much all normal interactions involving 2 NPCs follow the same pattern: (simplified example)

- They move to a place
- A Prefab is created
- They play A_Loop_01
- They play A_Loop_02
- An cum event is emited
- They end the scene

While simple, having to create the same structure tens of times is tedious and error prone, this is where templates comes in.

We can build this structure once, let's say for MaleNative and FemaleNative, save it as a Template, and then we can just
Right click and create a new script with the same node structure and adjust the parameters as needed.

To turn a SexScript into a template, in the Project window:

- Select the script (or scripts) you want to turn into template(s)
- Right click it
- Select `HFramework Tools` -> `Generate SexScript Template Menu`

You are done. A folder called `Editor` will be created near the script(s) you selected, containing the `HFrameworkTemplates_Menus` file,
this is where the templates will be stored.

If you want to recreate (or add more templates from this folder), just delete this script and redo the process.

Note that, that script you used as source will be referenced/copied whenever you use the menu, so changing/deleting it will affect new scripts created from that template.

You will now find on `Create > HFramework` an option with the name of the template you created, click it to create a new script with the same node structure.
