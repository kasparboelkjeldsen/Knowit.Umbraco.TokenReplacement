using Knowit.Umbraco.TokenReplacement.DTO;
using Microsoft.Extensions.Configuration;
using NPoco.RowMappers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Knowit.Umbraco.TokenReplacement.Service
{
	public class CmsDictionaryReader : ICmsDictionaryReader
	{
		private readonly IUmbracoDatabaseFactory _umbracoDatabaseFactory;
		private readonly string _tokenKey;
		public CmsDictionaryReader(IUmbracoDatabaseFactory umbracoDatabaseFactory, IConfiguration configuration) {
			_umbracoDatabaseFactory = umbracoDatabaseFactory;
			_tokenKey = configuration.GetValue<string?>("Knowit.Umbraco.TokenReplacement:TokenKey") ?? "";
		}
		public CmsDictionary GetAll()
		{
			CmsDictionary dictionary = new CmsDictionary();
			dictionary.Dictionary = new ConcurrentDictionary<string, IEnumerable<CmsDictionaryItem>>();

			using (var scope = _umbracoDatabaseFactory.CreateDatabase())
			{
				var sql = @"
            SELECT i.[key], i.[pk], l.languageIsoCode, l.languageCultureName, d.[value]
            FROM cmsLanguageText d
            JOIN umbracoLanguage l ON d.languageId = l.id
            JOIN cmsDictionary i ON i.id = d.uniqueId
            "
				;

				var dyn = scope.Fetch<dynamic>(sql);

				if(dyn != null && dyn.Count > 0)
				{
					List<CmsDictionaryItem> list = new List<CmsDictionaryItem>();
					
					foreach (var row in dyn) {
						if (row.key == null) continue;
						list.Add(new CmsDictionaryItem
						{
							Key = row.key,
							Culture = row.languageIsoCode,
							Value = row.value,
						});
					}

					if (!string.IsNullOrEmpty(_tokenKey)) list = list.Where(x => x.Key.StartsWith(_tokenKey)).ToList();

					if(list.Count > 0)
					{
						var keys = list.Select(s => s.Key).Distinct().ToList();
                        foreach (var key in keys)
						{
							var keylist = list.Where(x => x.Key == key);
							dictionary.Dictionary.TryAdd(key, keylist);
						}
                    }
				}
			}
			

            return dictionary;
		}
	}
}
