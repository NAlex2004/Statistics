using Statistics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Statistics.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString Pager(this HtmlHelper html, PagerData data, Func<int, string> PageURLFunc)
        {
            if (data.TotalPages < 2)
                return MvcHtmlString.Create("");

            TagBuilder p = new TagBuilder("ul");
            p.AddCssClass("pagination");
            for (int i = 1; i <= data.TotalPages; i++)
            {
                TagBuilder li = new TagBuilder("li");
                if (i == data.CurrentPage)
                    li.AddCssClass("active");
                TagBuilder a = new TagBuilder("a");
                //if (i != data.CurrentPage)
                //    a.AddCssClass("pager");
                //else
                //    a.AddCssClass("selected-page");
                a.MergeAttribute("href", PageURLFunc(i));
                a.InnerHtml = i.ToString();
                li.InnerHtml += a.ToString();
                p.InnerHtml += li.ToString();
            }

            return MvcHtmlString.Create(p.ToString());
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, PagerData data, string UpdateTargetID, Func<int, string> PageURLFunc, Func<int, string> AjaxURLFunc)
        {
            if (data.TotalPages < 2)
                return MvcHtmlString.Create("");

            TagBuilder p = new TagBuilder("ul");
            p.AddCssClass("pagination");
            for (int i = 1; i <= data.TotalPages; i++)
            {
                TagBuilder li = new TagBuilder("li");
                if (i == data.CurrentPage)
                    li.AddCssClass("active");
                TagBuilder a = new TagBuilder("a");
                //if (i != data.CurrentPage)
                //    a.AddCssClass("pager");
                //else
                //    a.AddCssClass("selected-page");
                a.MergeAttribute("data-ajax", "true");
                a.MergeAttribute("data-ajax- mode", "replace");
                a.MergeAttribute("data-ajax-update", "#" + UpdateTargetID);
                a.MergeAttribute("href", PageURLFunc(i));
                a.MergeAttribute("data-ajax-url", AjaxURLFunc(i));
                a.InnerHtml = i.ToString();

                li.InnerHtml += a.ToString();
                p.InnerHtml += li.ToString();
            }

            return MvcHtmlString.Create(p.ToString());
        }

    }
}