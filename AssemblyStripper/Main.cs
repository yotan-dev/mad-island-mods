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
			foreach (var assembly in assemblies)
			{
				AssemblyPublicizer.Publicize(
					managedFolder + assembly,
					"../Assemblies/" + assembly,
					new AssemblyPublicizerOptions { Strip = true }
				);
			}

			File.WriteAllText("../Assemblies/version.txt", $"{version}\n");

			Console.WriteLine("Done.");
		}
	}
}
