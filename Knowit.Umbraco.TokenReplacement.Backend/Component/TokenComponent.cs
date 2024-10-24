using Knowit.Umbraco.TokenReplacement.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;

namespace Knowit.Umbraco.TokenReplacement.Backend.Component
{
    public class TokenComponent : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ICultureExtractor, CultureExtractor>();
            builder.Services.AddSingleton<ICmsDictionaryReader, CmsDictionaryReader>();
            builder.Services.AddSingleton<ICmsTokenReplacer, CmsTokenReplacer>();
        }
    }
}
