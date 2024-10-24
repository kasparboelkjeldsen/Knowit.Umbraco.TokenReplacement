using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Knowit.Umbraco.TokenReplacement.Service;

namespace Knowit.Umbraco.TokenReplacement.Notifications
{
	public class DictionaryNotifications : INotificationAsyncHandler<DictionaryItemSavedNotification>, INotificationAsyncHandler<DictionaryItemDeletedNotification>, INotificationAsyncHandler<DictionaryCacheRefresherNotification>
	{
		private readonly IAppPolicyCache _appPolicyCache;
		public DictionaryNotifications(IAppPolicyCache appPolicyCache) { 
			_appPolicyCache = appPolicyCache;
		}

		public Task HandleAsync(DictionaryItemDeletedNotification notification, CancellationToken cancellationToken)
		{
			ClearCache();
			return Task.CompletedTask;
		}

		public Task HandleAsync(DictionaryItemSavedNotification notification, CancellationToken cancellationToken)
		{
			ClearCache();
			return Task.CompletedTask;
		}

		public Task HandleAsync(DictionaryCacheRefresherNotification notification, CancellationToken cancellationToken)
		{
			ClearCache();
			return Task.CompletedTask;
		}

		private void ClearCache()
		{
			_appPolicyCache.ClearByKey(CmsTokenReplacer.CmsTokenReplacerCacheKey);
		}
	}

	public class NotificationHandlersComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.AddNotificationAsyncHandler<DictionaryItemSavedNotification, DictionaryNotifications>();
			builder.AddNotificationAsyncHandler<DictionaryItemDeletedNotification, DictionaryNotifications>();
			builder.AddNotificationAsyncHandler<DictionaryCacheRefresherNotification, DictionaryNotifications>();
		}
	}
}
