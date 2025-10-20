using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;
using UnityEngine.Assertions;
using Mono.Cecil.Cil;
using BepInEx.Logging;

public static class Patcher
{
	// List of assemblies to patch
	public static IEnumerable<string> TargetDLLs { get; } = ["Assembly-CSharp.dll"];

	private static readonly ManualLogSource logger = Logger.CreateLogSource("YotanModCorePatcher");

	private static TypeDefinition GetTypeByName(TypeDefinition type, string[] name)
	{
		Assert.IsTrue(name.Length > 0, "Name must not be empty");

		var nestedType = type.NestedTypes.FirstOrDefault(t => t.FullName == name[0]);
		Assert.IsNotNull(nestedType, $"Type {type.FullName} does not have nested type {name[0]}");

		if (name.Length == 1)
			return nestedType;

		return GetTypeByName(nestedType, name.Skip(1).ToArray());
	}

	private static TypeDefinition GetTypeByName(ModuleDefinition module, string[] name)
	{
		Assert.IsTrue(name.Length > 0, "Name must not be empty");

		var type = module.Types.FirstOrDefault(t => t.FullName == name[0]);
		Assert.IsNotNull(type, $"Type {name[0]} not found in module {module.Name}");

		if (name.Length == 1)
			return type;

		return GetTypeByName(type, name.Skip(1).ToArray());
	}

	private static MethodDefinition AddGetter(
		TypeDefinition targetType,
		string name,
		TypeReference propertyType,
		FieldDefinition backingField
	)
	{
		var getter = new MethodDefinition(
					$"get_{name}",
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
					propertyType
				);
		getter.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
		getter.Body.Instructions.Add(Instruction.Create(OpCodes.Ldfld, backingField));
		getter.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

		targetType.Methods.Add(getter);

		return getter;
	}

	private static MethodDefinition AddSetter(
		TypeDefinition targetType,
		string name,
		TypeReference propertyType,
		FieldDefinition backingField
	)
	{
		var setter = new MethodDefinition(
			$"set_{name}",
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
			targetType.Module.ImportReference(typeof(void))
		);
		setter.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, propertyType));
		setter.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
		setter.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
		setter.Body.Instructions.Add(Instruction.Create(OpCodes.Stfld, backingField));
		setter.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

		targetType.Methods.Add(setter);

		return setter;
	}

	private static void AddProperty(TypeDefinition targetType, string name, TypeReference propertyType, byte[] initialValue)
	{
		var backingField = new FieldDefinition(
				$"<{name}>k__BackingField",
				FieldAttributes.Private,
				propertyType
			) { InitialValue = initialValue };

		targetType.Fields.Add(backingField);

		var setter = AddSetter(targetType, name, propertyType, backingField);
		var getter = AddGetter(targetType, name, propertyType, backingField);

		var newProp = new PropertyDefinition(name, PropertyAttributes.None, propertyType)
		{
			SetMethod = setter,
			GetMethod = getter,
		};

		targetType.Properties.Add(newProp);
	}

	private static void AddModDataProperty(ModuleDefinition module, string[] classNamePath)
	{
		AddProperty(GetTypeByName(module, classNamePath), "modData", module.ImportReference(typeof(List<object>)), []);
	}

	// Patches the assemblies
	public static void Patch(AssemblyDefinition assembly)
	{
		logger.LogInfo($"Applying YotanModCore Patch");

		var module = assembly.Modules[0];
		AddModDataProperty(module, ["SaveManager", "SaveManager/SaveEntry"]);
		AddModDataProperty(module, ["SaveManager", "SaveManager/CharaSave"]);

		// This breaks the game for unknown reasons
		// var newField = new FieldDefinition("testField", FieldAttributes.Public, module.ImportReference(typeof(int)));
		// type.Fields.Add(newField);

		logger.LogInfo($"Patch applied");
	}
}
