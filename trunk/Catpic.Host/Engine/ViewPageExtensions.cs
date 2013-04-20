using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;

namespace Catpic.Host.Engine
{
    /// <summary>
    /// Provides extensions for razor view
    /// </summary>
    public static class ViewPageExtensions
    {

        #region ScriptBlock

        private const string ScriptBlockBuilder = "ScriptBlockBuilder";

        /// <summary>
        /// Defines script block. It is used for dynamic addition of scripts to page which are rendered usually as single block at html head
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static MvcHtmlString ScriptBlock(this WebViewPage webPage, Func<dynamic, HelperResult> template)
        {
            if (!webPage.IsAjax)
            {
                var scriptBuilder = webPage.Context.Items[ScriptBlockBuilder] as StringBuilder ?? new StringBuilder();

                scriptBuilder.Append(template(null).ToHtmlString());
                webPage.Context.Items[ScriptBlockBuilder] = scriptBuilder;

                return new MvcHtmlString(string.Empty);
            }

            return new MvcHtmlString(template(null).ToHtmlString());
        }

        /// <summary>
        /// Renders script blocks as single block
        /// </summary>
        /// <param name="webPage"></param>
        /// <returns></returns>
        public static MvcHtmlString WriteScriptBlocks(this WebViewPage webPage)
        {
            var scriptBuilder = webPage.Context.Items[ScriptBlockBuilder] as StringBuilder ?? new StringBuilder();

            return new MvcHtmlString(scriptBuilder.ToString());
        }

        #endregion

        #region StyleBlock

        private const string StyleBlockBuilder = "StyleBlockBuilder";

        /// <summary>
        /// Defines style block. It is used for dynamic addition of styles to page which are rendered usually as single block at html head
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static MvcHtmlString StyleBlock(this WebViewPage webPage, Func<dynamic, HelperResult> template)
        {
            if (!webPage.IsAjax)
            {
                var styleBuilder = webPage.Context.Items[StyleBlockBuilder] as StringBuilder ?? new StringBuilder();

                styleBuilder.Append(template(null).ToHtmlString());
                webPage.Context.Items[StyleBlockBuilder] = styleBuilder;

                return new MvcHtmlString(string.Empty);
            }

            return new MvcHtmlString(template(null).ToHtmlString());
        }

        /// <summary>
        /// Renders style blocks as single block
        /// </summary>
        /// <param name="webPage"></param>
        /// <returns></returns>
        public static MvcHtmlString WriteStyleBlocks(this WebViewPage webPage)
        {
            var styleBuilder = webPage.Context.Items[StyleBlockBuilder] as StringBuilder ?? new StringBuilder();

            return new MvcHtmlString(styleBuilder.ToString());
        }

        #endregion

        #region Labels

        /// <summary>
        /// Attaches htmlAttributes to html lable
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Attaches htmlAttributes to html lable
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.SetInnerText(labelText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }


        #endregion
    }
}