using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knowit.Umbraco.TokenReplacement.DTO
{
	public class CmsDictionary
	{
		public ConcurrentDictionary<string, IEnumerable<CmsDictionaryItem>>? Dictionary { get; set; }
	}

	public class CmsDictionaryItem
	{
		public string? Culture { get; set; }
		public string? Key { get; set; }
		public string? Value { get; set; }
	}
}
