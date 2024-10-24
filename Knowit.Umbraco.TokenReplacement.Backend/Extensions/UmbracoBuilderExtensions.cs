using Knowit.Umbraco.TokenReplacement.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.DependencyInjection;

namespace Knowit.Umbraco.TokenReplacement.Extensions
{
	public static class UmbracoBuilderExtensions
	{
		public static IUmbracoBuilder AddTokenReplacementServices(this IUmbracoBuilder builder)
		{
			/*
			builder.Services.AddSingleton<ICultureExtractor, CultureExtractor>();
			builder.Services.AddSingleton<ICmsDictionaryReader, CmsDictionaryReader>();
			builder.Services.AddSingleton<ICmsTokenReplacer, CmsTokenReplacer>();*/
			return builder;
		}
	}
}
