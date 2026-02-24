using System;
using System.Globalization;
using System.IO;
using AsmResolver.DotNet;
using BepInEx.AssemblyPublicizer;

namespace AssemblyStripper
{
	public class Program
	{
		private static void StripMadIsland(string[] args)
		{
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
					// For Assembly-CSharp, also create a publicized version, as we can use it for preload patch
					// For unity, the publicized version contains references to BepInEx, which causes issues.
					AssemblyPublicizer.Publicize(
						managedFolder + assembly,
						outPath.Replace(".dll", "_publicized.dll"),
						new AssemblyPublicizerOptions { Strip = true }
					);
					File.Copy(outPath.Replace(".dll", "_publicized.dll"), finalPath.Replace(".dll", "_publicized.dll"), true);

					RemoveAssemblyCSharpProblematicClasses(outPath);
				}
				File.Copy(outPath, finalPath, true);
			}

			File.WriteAllText($"../Assemblies/{version}/version.txt", version);
			File.WriteAllText($"../Assemblies/version.txt", version);
		}

		private static void StripUnityEditor(string[] args)
		{
			var managedFolder = args[1] + "Editor/Data/Managed/";
			var assemblies = new string[] { "UnityEditor.dll" };

			foreach (var assembly in assemblies)
			{
				var outPath = $"../Assemblies/{assembly}";

				AssemblyPublicizer.Publicize(
					managedFolder + assembly,
					outPath,
					new AssemblyPublicizerOptions { Target = PublicizeTarget.None, Strip = true }
				);
			}
		}

		public static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Provide version and path to MadIsland folder.");
				Console.WriteLine("Usage: AssemblyStripper v0.4.4.1 C:/SteamLibrary/MadIsland");
				return;
			}

			var version = args[0];
			if (version.ToLower() == "editor")
			{
				StripUnityEditor(args);
			}
			else
			{
				StripMadIsland(args);
			}

			Console.WriteLine("Done.");
		}

		private static void RemoveAssemblyCSharpProblematicClasses(string path)
		{
			var assembly = FatalAsmResolver.FromFile(path);
			var module = assembly.ManifestModule ?? throw new NullReferenceException();

			foreach (var type in module.GetAllTypes())
			{
				if (ShouldRemoveType(type))
				{
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

			if (type.FullName == "GlitchEffectsManipulationExample")
			{
				return true;
			}

			if (type.FullName.StartsWith("LimitlessGlitch"))
			{
				return true;
			}

			if (type.FullName.StartsWith("Limitless_"))
			{
				return true;
			}

			if (type.FullName == "CustomTexture")
			{
				return true;
			}

			return false;
		}
	}
}
