using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Knowit.Umbraco.TokenReplacement.Notifications
{
    public class SavingNotifications : INotificationAsyncHandler<ContentSavingNotification>
    {
        public Task HandleAsync(ContentSavingNotification notification, CancellationToken cancellationToken)
        {

            // we need to remove this from saved RTE content
            // '<span class="token-replacement-iframe-match ' + extraClass + '" title="' + value + '">' + match + '<button style="display:none"></button></span>'
            foreach (var entity in notification.SavedEntities)
            {
                
                foreach(var prop in entity.Properties)
                {
                    foreach (var val in prop.Values)
                    {
                        var edited = val.EditedValue;
                        var published = val.PublishedValue;

                        if(edited != null && edited.ToString()!.Contains("token-replacement-iframe-match")) {
                            string input = PerformReplacement(edited);
                            val.EditedValue = input;
                        }
                        if (published != null && published.ToString()!.Contains("token-replacement-iframe-match"))
                        {
                            string input = PerformReplacement(published);
                            val.PublishedValue = input;
                        }
                    }
                }
                
            }
            return Task.CompletedTask;
        }

        private static string PerformReplacement(object? published)
        {
            string input = published.ToString()!;
            input = input.Replace("<span class=\\\"token-replacement-iframe-match \\\" style=\\\"position: relative;\\\">{{", "{{");
            input = input.Replace("<span class=\\\"token-replacement-iframe-match error\\\" style=\\\"position: relative;\\\">{{", "{{");
            input = input.Replace("}}<button></button></span>", "}}");

            // fall back replacements 
            input = input.Replace("<button></button></span>", "");
            input = input.Replace("<span class=\\\"token-replacement-iframe-match \\\" style=\\\"position: relative;\\\">", "");
            input = input.Replace("<span class=\\\"token-replacement-iframe-match error\\\" style=\\\"position: relative;\\\">", "");

            return input;
        }
    }

    public class SavingNotificationsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationAsyncHandler<ContentSavingNotification, SavingNotifications>();
        }
    }
}
