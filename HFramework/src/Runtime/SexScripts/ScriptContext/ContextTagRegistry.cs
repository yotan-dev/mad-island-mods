using System;
using System.Collections.Generic;
using System.Linq;

namespace HFramework.SexScripts.ScriptContext
{
	/// <summary>
	/// Registry for context tags. Used to provide a list of known tags for dropdowns and validation.
	///
	/// Other plugins may register their own tags using <see cref="Register"/>.
	/// See <see cref="ContextTags"/> for example.
	/// </summary>
	[Experimental]
	public static class ContextTagRegistry
	{
		private static readonly HashSet<string> tags = new(StringComparer.OrdinalIgnoreCase) { };

		public static IReadOnlyCollection<string> KnownTags => tags;

		public static void Register(string tag) {
			if (string.IsNullOrWhiteSpace(tag))
				return;

			tags.Add(tag);
		}

		public static string[] GetKnownTagsSorted() {
			return tags
				.OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
				.ToArray();
		}
	}
}
