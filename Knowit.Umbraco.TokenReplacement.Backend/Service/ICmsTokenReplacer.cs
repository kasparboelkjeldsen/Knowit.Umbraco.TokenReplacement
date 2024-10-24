using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knowit.Umbraco.TokenReplacement.Service
{
	public interface ICmsTokenReplacer
	{
		string Parse(string input, string culture);
	}
}
