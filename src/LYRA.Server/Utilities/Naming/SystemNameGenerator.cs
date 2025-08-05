using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LYRA.Server.Utilities.Naming
{
	/// <summary>
	/// Generates stable system names based on human-readable inputs.
	/// </summary>
	public static class SystemNameGenerator
	{
		/// <summary>
		/// Ensures a system-friendly name (slug).
		/// </summary>
		public static string Ensure(string displayName)
			=> Slugifier.EnsureSlug(displayName);

		/// <summary>
		/// Try-pattern variant (does not throw).
		/// </summary>
		public static bool TryEnsure(string? displayName, out string systemName)
			=> Slugifier.TryEnsureSlug(displayName, out systemName);
	}

	/// <summary>
	/// Utilities for converting arbitrary strings into URL/system-friendly slugs.
	/// Slugs are lowercase, separator-delimited, trimmed and ASCII-only by default.
	/// </summary>
	internal static class Slugifier
	{
		/// <summary>
		/// Slug generation options.
		/// </summary>
		public sealed record SlugOptions(
			char Separator = '-',
			int? MaxLength = 100,
			bool TrimSeparators = true,
			bool AsciiOnly = true);

		/// <summary>
		/// Converts an input into a slug with the specified options.
		/// </summary>
		public static string ToSlug(string? input, SlugOptions? options = null)
		{
			if (string.IsNullOrWhiteSpace(input))
				return string.Empty;

			options ??= new SlugOptions();

			// Normalize and optionally remove diacritics
			var normalized = input.Trim().ToLowerInvariant();
			if (options.AsciiOnly)
				normalized = RemoveDiacritics(normalized);

			// Replace whitespace/underscores with separator
			var step1 = SpaceOrUnderscore.Replace(normalized, options.Separator.ToString());

			// Remove disallowed characters
			var allowedPattern = $"[^a-z0-9{Regex.Escape(options.Separator.ToString())}]";
			var step2 = Regex.Replace(step1, allowedPattern, string.Empty);

			// Collapse duplicate separators
			var sep = Regex.Escape(options.Separator.ToString());
			var step3 = Regex.Replace(step2, $"{sep}+", options.Separator.ToString());

			var result = options.TrimSeparators ? step3.Trim(options.Separator) : step3;

			// Truncate if needed
			if (options.MaxLength is > 0 && result.Length > options.MaxLength.Value)
				result = result[..options.MaxLength.Value].Trim(options.Separator);

			return result;
		}

		/// <summary>
		/// Converts a display name to a slug.
		/// Throws if the input is null/whitespace or the result is empty.
		/// </summary>
		public static string EnsureSlug(string displayName, SlugOptions? options = null)
		{
			if (string.IsNullOrWhiteSpace(displayName))
				throw new ArgumentException("Display name must not be empty.", nameof(displayName));

			var slug = ToSlug(displayName, options);

			if (string.IsNullOrWhiteSpace(slug))
				throw new InvalidOperationException("Generated name from display name cannot be empty.");

			return slug;
		}

		/// <summary>
		/// Try-pattern variant that does not throw.
		/// </summary>
		public static bool TryEnsureSlug(string? displayName, out string slug, SlugOptions? options = null)
		{
			slug = string.Empty;
			if (string.IsNullOrWhiteSpace(displayName)) return false;

			slug = ToSlug(displayName, options);
			return !string.IsNullOrWhiteSpace(slug);
		}

		/// <summary>
		/// Removes diacritics (é → e, ü → u) using FormD decomposition.
		/// </summary>
		private static string RemoveDiacritics(string text)
		{
			var formD = text.Normalize(NormalizationForm.FormD);
			var sb = new StringBuilder(capacity: formD.Length);

			foreach (var ch in formD)
			{
				var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
				if (uc != UnicodeCategory.NonSpacingMark)
					sb.Append(ch);
			}

			return sb.ToString().Normalize(NormalizationForm.FormC);
		}

		private static readonly Regex SpaceOrUnderscore =
			new(@"[\s_]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
