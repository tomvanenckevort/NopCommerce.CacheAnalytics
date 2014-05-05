using Nop.Core.Plugins;
using Nop.Plugin.Misc.CacheAnalytics.Code;
using Nop.Services.Common;
using Nop.Services.Localization;
using System.Web.Routing;

namespace Nop.Plugin.Misc.CacheAnalytics
{
    public class CacheAnalyticsPlugin : BasePlugin, IMiscPlugin
    {
        public CacheAnalyticsPlugin()
        {

        }

        /// <summary>
        /// Installs the plugin.
        /// </summary>
        public override void Install()
        {
            // locales
            this.AddOrUpdatePluginLocaleResource(Constants.ResourceIntroPart1, "The Cache Analytics plugin displays which items currently are stored in the ASP.NET MemoryCache.");
            this.AddOrUpdatePluginLocaleResource(Constants.ResourceIntroPart2, "Individual items can be removed from the cache by pressing the Delete button.");
            this.AddOrUpdatePluginLocaleResource(Constants.ResourceIntroPart3, "Note that due to limitations of the .NET Framework, the displayed object size is not entirely accurate.");
            
            base.Install();
        }

        /// <summary>
        /// Uninstalls the plugin.
        /// </summary>
        public override void Uninstall()
        {
            // locales
            this.DeletePluginLocaleResource(Constants.ResourceIntroPart1);
            this.DeletePluginLocaleResource(Constants.ResourceIntroPart2);
            
            base.Uninstall();
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "MiscCacheAnalytics";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Misc.CacheAnalytics.Controllers" }, { "area", null } };
        }
    }
}
