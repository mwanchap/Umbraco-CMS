﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Xml;
using Umbraco.Core.Xml.XPath;
using Umbraco.Web.PublishedCache.NuCache.Navigable;

namespace Umbraco.Web.PublishedCache.NuCache
{
    class MediaCache : PublishedCacheBase, IPublishedMediaCache, INavigableData
    {
        private readonly ContentView _view;

        public MediaCache(bool previewDefault, ContentView view)
            : base(previewDefault)
        {
            _view = view;
        }

        public override IPublishedContent GetById(bool preview, int contentId)
        {
            var n = _view.Get(contentId);
            return n == null ? null : n.Published;
        }

        public override bool HasById(bool preview, int contentId)
        {
            var n = _view.Get(contentId);
            return n != null;
        }

        public override IEnumerable<IPublishedContent> GetAtRoot(bool preview)
        {
            return _view.GetAtRoot().Select(n => n.Published);
        }

        public override IPublishedContent GetSingleByXPath(bool preview, string xpath, XPathVariable[] vars)
        {
            var navigator = CreateNavigator(preview);
            var iterator = navigator.Select(xpath, vars);
            return GetSingleByXPath(iterator);
        }

        public override IPublishedContent GetSingleByXPath(bool preview, XPathExpression xpath, XPathVariable[] vars)
        {
            var navigator = CreateNavigator(preview);
            var iterator = navigator.Select(xpath, vars);
            return GetSingleByXPath(iterator);
        }

        private static IPublishedContent GetSingleByXPath(XPathNodeIterator iterator)
        {
            if (iterator.MoveNext() == false) return null;

            var xnav = iterator.Current as NavigableNavigator;
            if (xnav == null) return null;

            var xcontent = xnav.UnderlyingObject as NavigableContent;
            return xcontent == null ? null : xcontent.InnerContent;
        }

        public override IEnumerable<IPublishedContent> GetByXPath(bool preview, string xpath, XPathVariable[] vars)
        {
            var navigator = CreateNavigator(preview);
            var iterator = navigator.Select(xpath, vars);
            return GetByXPath(iterator);
        }

        public override IEnumerable<IPublishedContent> GetByXPath(bool preview, XPathExpression xpath, XPathVariable[] vars)
        {
            var navigator = CreateNavigator(preview);
            var iterator = navigator.Select(xpath, vars);
            return GetByXPath(iterator);
        }

        private static IEnumerable<IPublishedContent> GetByXPath(XPathNodeIterator iterator)
        {
            while (iterator.MoveNext())
            {
                var xnav = iterator.Current as NavigableNavigator;
                if (xnav == null) continue;

                var xcontent = xnav.UnderlyingObject as NavigableContent;
                if (xcontent == null) continue;

                yield return xcontent.InnerContent;
            }
        }

        public override XPathNavigator CreateNavigator(bool preview)
        {
            var source = new Source(this, preview);
            var navigator = new NavigableNavigator(source);
            return navigator;
        }

        public override XPathNavigator CreateNodeNavigator(int id, bool preview)
        {
            var source = new Source(this, preview);
            var navigator = new NavigableNavigator(source);
            return navigator.CloneWithNewRoot(id, 0);
        }

        public override bool HasContent(bool preview)
        {
            return _view.HasContent;
        }

        #region Content types

        public override PublishedContentType GetContentType(int id)
        {
            return _view.GetContentType(id);
        }

        public override PublishedContentType GetContentType(string alias)
        {
            return _view.GetContentType(alias);
        }

        #endregion
    }
}
