using System;
using System.Globalization;
using System.IO;
using AsmResolver.DotNet;
using BepInEx.AssemblyPublicizer;

namespace AssemblyStripper
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Provide version and path to MadIsland folder.");
				Console.WriteLine("Usage: AssemblyStripper v0.4.4.1 C:/SteamLibrary/MadIsland");
				return;
			}

			var version = args[0];
			var managedFolder = args[1] + "/Mad Island_Data/Managed/";
			var assemblies = new string[] { "Assembly-CSharp.dll", "spine-unity.dll", "Unity.TextMeshPro.dll", "UnityEngine.UI.dll" };

			Directory.CreateDirectory($"../Assemblies/{version}");

			foreach (var assembly in assemblies)
			{
				var outPath = $"../Assemblies/{version}/{assembly}";
				var finalPath = $"../Assemblies/{assembly}";

				AssemblyPublicizer.Publicize(
					managedFolder + assembly,
					outPath,
					new AssemblyPublicizerOptions { Target = PublicizeTarget.None, Strip = true }
				);

				if (assembly == "Assembly-CSharp.dll")
				{
					RemoveAssemblyCSharpProblematicClasses(outPath);
				}
				File.Copy(outPath, finalPath, true);
			}

			File.WriteAllText($"../Assemblies/{version}/version.txt", version);
			File.WriteAllText($"../Assemblies/version.txt", version);


			Console.WriteLine("Done.");
		}

		private static void RemoveAssemblyCSharpProblematicClasses(string path)
		{
			var assembly = FatalAsmResolver.FromFile(path);
			var module = assembly.ManifestModule ?? throw new NullReferenceException();

			foreach (var type in module.GetAllTypes()) {
				if (ShouldRemoveType(type)) {
					module.TopLevelTypes.Remove(type);
					Console.WriteLine($">> Removed {type.FullName}");
				}
			}

			module.FatalWrite(path);
		}

		private static bool ShouldRemoveType(TypeDefinition type)
		{
			// GlitchEffect / Limitless causes post-processing issues on Unity.
			// CustomTexture depends on them.
			// It is unlikely that modders will need those, so we remove them to avoid issues.

			if (type.FullName == "GlitchEffectsManipulationExample") {
				return true;
			}

			if (type.FullName.StartsWith("LimitlessGlitch")) {
				return true;
			}

			if (type.FullName.StartsWith("Limitless_")) {
				return true;
			}

			if (type.FullName == "CustomTexture") {
				return true;
			}

			return false;
		}
	}
}
