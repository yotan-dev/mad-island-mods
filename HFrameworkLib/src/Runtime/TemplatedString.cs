using System;
using System.Collections.Generic;
using System.Text;

namespace HFramework
{
	public sealed class TemplatedString
	{
		private readonly string template;
		private readonly Part[] parts;
		private readonly int literalLengthSum;

		private readonly struct Part
		{
			public readonly string Value;
			public readonly bool IsPlaceholder;

			public Part(string value, bool isPlaceholder)
			{
				Value = value;
				IsPlaceholder = isPlaceholder;
			}
		}

		public TemplatedString(string template)
		{
			this.template = template ?? throw new ArgumentNullException(nameof(template));

			var parsedParts = new List<Part>();
			var sb = new StringBuilder();
			int literalLen = 0;

			for (int i = 0; i < template.Length; i++)
			{
				char c = template[i];
				if (c != '{')
				{
					sb.Append(c);
					continue;
				}

				int closeIndex = template.IndexOf('}', i + 1);
				if (closeIndex < 0)
				{
					throw new FormatException($"TemplatedString: Unmatched '{{' at index {i} in template '{template}'");
				}

				if (sb.Length > 0)
				{
					string literal = sb.ToString();
					parsedParts.Add(new Part(literal, isPlaceholder: false));
					literalLen += literal.Length;
					sb.Clear();
				}

				string key = template.Substring(i + 1, closeIndex - (i + 1));
				parsedParts.Add(new Part(key, isPlaceholder: true));
				i = closeIndex;
			}

			if (sb.Length > 0)
			{
				string literal = sb.ToString();
				parsedParts.Add(new Part(literal, isPlaceholder: false));
				literalLen += literal.Length;
			}

			parts = parsedParts.Count == 0 ? Array.Empty<Part>() : parsedParts.ToArray();
			literalLengthSum = literalLen;
		}

		public string GetString(Dictionary<string, string> values)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));

			if (parts.Length == 0)
				return string.Empty;

			var sb = new StringBuilder(literalLengthSum);

			for (int i = 0; i < parts.Length; i++)
			{
				Part part = parts[i];
				if (!part.IsPlaceholder)
				{
					sb.Append(part.Value);
					continue;
				}

				if (values.TryGetValue(part.Value, out string value) && value != null)
				{
					sb.Append(value);
					continue;
				}

				PLogger.LogError($"TemplatedString: Missing value for key '{part.Value}' while formatting '{template}'");
			}

			return sb.ToString();
		}

		public override string ToString() => template;
	}
}
