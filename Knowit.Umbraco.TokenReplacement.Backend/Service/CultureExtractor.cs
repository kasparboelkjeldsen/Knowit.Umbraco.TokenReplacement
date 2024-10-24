using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using System.Globalization;
using Umbraco.Cms.Core;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Persistence.Repositories;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Cache;

namespace Knowit.Umbraco.TokenReplacement.Service
{
	public class CultureExtractor : ICultureExtractor
	{
		private readonly IDomainService _domainService;
		private string _fallbackCulture;
		private IAppPolicyCache _appPolicyCache;
		public CultureExtractor(IDomainService domainService, ILanguageRepository languageRepository, ICoreScopeProvider scopeProvider, IAppPolicyCache appPolicyCache)
		{
			_domainService = domainService;
			_appPolicyCache = appPolicyCache;
			using (var scope = scopeProvider.CreateCoreScope())
			{
				_fallbackCulture = languageRepository.GetDefaultIsoCode();
			}
		}

		/// <summary>
		/// will return the last matching domain culture 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public string? GetCultureFromUrl(string host, string path)
		{
			if (path == null || path.Length == 0 || !path.StartsWith("/")) return _fallbackCulture;

			string pathString = SanitizePath(path);

			string cacheKey = $"{CmsTokenReplacer.CmsTokenReplacerCacheKey}-domain-{pathString}";

			return _appPolicyCache.GetCacheItem(cacheKey, () =>
			{
				var domains = _domainService.GetAll(true);
				foreach (var domain in domains)
				{
					string domainString = domain.DomainName.ToLower();

					// remove host if it's a match
					if (domainString.StartsWith("http"))
					{
						domainString = domainString.Replace("https://", string.Empty);
						domainString = domainString.Replace("http://", string.Empty);
						domainString = domainString.Replace(host, string.Empty);
					}

					if (pathString == domainString)
					{
						// MATCH!
						return domain.LanguageIsoCode ?? _fallbackCulture;
					}
				}

				return _fallbackCulture;
			}, TimeSpan.FromHours(1));

			
		}

		private string SanitizePath(string path)
		{
			path = path.Substring(1);

			var pathString = "/" + path.ToLower().Split('/')[0];

			if (pathString.Contains("#")) pathString = pathString.Split('#')[0];
			if (pathString.Contains("?")) pathString = pathString.Split('?')[0];
			return pathString;
		}
	}
}
