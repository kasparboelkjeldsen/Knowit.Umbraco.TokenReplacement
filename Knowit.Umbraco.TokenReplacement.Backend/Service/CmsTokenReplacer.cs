using Knowit.Umbraco.TokenReplacement.DTO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Umbraco.Cms.Core.Cache;

namespace Knowit.Umbraco.TokenReplacement.Service
{
	public class CmsTokenReplacer : ICmsTokenReplacer
	{
		public const string CmsTokenReplacerCacheKey = "knowit.CmsTokenReplacerCacheKey";

		public const string delimiterStart = "{{";
		public const string regexString = @"\{\{([\w\.]+)\}\}";

		private readonly IAppPolicyCache _appPolicyCache;
		private readonly ICmsDictionaryReader _cmsDictionaryReader;
		public CmsTokenReplacer(IAppPolicyCache appPolicyCache, ICmsDictionaryReader cmsDictionaryReader) {
			_appPolicyCache = appPolicyCache;
			_cmsDictionaryReader = cmsDictionaryReader; 
		}
		public string Parse(string input, string culture)
		{
			int firstIndex = input.IndexOf("{{"); // Assuming delimiterStart is "{{"

			// End immediately if no "{{" found
			if (firstIndex == -1) return input;

			StringBuilder result = new StringBuilder();
			result.Append(input.Substring(0, firstIndex));

			// Efficiently handle replacements
			string doReplacements = input.Substring(firstIndex);

			var cacheObject = _appPolicyCache.Get(CmsTokenReplacerCacheKey, _cmsDictionaryReader.GetAll, TimeSpan.FromHours(1));

			if (cacheObject is CmsDictionary cmsDictionary && cmsDictionary.Dictionary != null)
			{
				// Pre-filter dictionary entries by culture
				var filteredDictionary = cmsDictionary.Dictionary
					.ToDictionary(pair => pair.Key,
								  pair => pair.Value.FirstOrDefault(x => x.Culture == culture)?.Value);

				// Regex to find all instances of "{{key}}"
				var regex = new Regex(regexString);

				doReplacements = regex.Replace(doReplacements, match =>
				{
					string key = match.Groups[1].Value;
					filteredDictionary.TryGetValue(key, out string replacement);
					return  replacement ?? match.Value;
				});
			}

			result.Append(doReplacements);
			return result.ToString();
		}

	}
}
