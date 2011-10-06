using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Xml;
using System.Net;

namespace MUMS.Web.Config
{
    /// <summary>
    /// A custom config section handler to access the cookieTriggers defined in web.config.
    /// </summary>
    public class CookieConfigSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return ReadTriggers(section).Where(t => t != null).ToList();
        }

        private IEnumerable<CookieTrigger> ReadTriggers(XmlNode root)
        {
            foreach (XmlNode triggerNode in root.ChildNodes)
                yield return ReadTrigger(triggerNode);
        }

        private CookieTrigger ReadTrigger(XmlNode triggerNode)
        {
            var domainsAttr = triggerNode.Attributes["domains"];
            var domains = new List<string>();

            if (domainsAttr != null && !string.IsNullOrWhiteSpace(domainsAttr.Value))
            {
                domains = domainsAttr.Value
                    .Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim().ToLowerInvariant())
                    .ToList();
            }

            if (domains.Count == 0)
                throw new ConfigurationException("The domains attribute must be properly set", triggerNode);

            return new CookieTrigger
            {
                TriggerDomains = domains,
                Cookies = ReadCookies(triggerNode).ToList()
            };
        }

        private IEnumerable<Cookie> ReadCookies(XmlNode triggerNode)
        {
            if (triggerNode.ChildNodes == null || triggerNode.ChildNodes.Count == 0)
                throw new ConfigurationException("The cookieTrigger node must contain at least one cookie node", triggerNode);

            foreach (XmlNode cookieNode in triggerNode.ChildNodes)
            {
                yield return new Cookie
                {
                    Name = TryReadAttribute("name", cookieNode),
                    Value = TryReadAttribute("value", cookieNode),
                    Path = TryReadAttribute("path", cookieNode),
                    Domain = TryReadAttribute("domain", cookieNode)
                };
            }
        }

        private string TryReadAttribute(string attributeName, XmlNode node)
        {
            var attribute = node.Attributes[attributeName];
            if (attribute != null)
                return attribute.Value;

            return null;
        }
    }
}