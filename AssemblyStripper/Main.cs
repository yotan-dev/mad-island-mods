using System;
using System.IO;
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
					new AssemblyPublicizerOptions { Strip = true }
				);
				File.Copy(outPath, finalPath, true);
			}

			File.WriteAllText($"../Assemblies/{version}/version.txt", version);
			File.WriteAllText($"../Assemblies/version.txt", version);


			Console.WriteLine("Done.");
		}
	}
}
