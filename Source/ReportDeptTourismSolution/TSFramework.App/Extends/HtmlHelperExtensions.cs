using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TSFramework.App.Processors;

namespace TSFramework.App.Extends
{
    public enum ScriptPosition
    {
        HeadEnd,
        BodyStart,
        BodyInside,
        BodyEnd
    }

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ImageFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, string url, string altText, string accept,
            object htmlAttributes = null)
        {
            accept = string.IsNullOrEmpty(accept) ? ".png,.jpeg,.gif,.jpg" : accept;
            var name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(expression));
            var nameInputFile = $"{name}_Img";

            var inputBuilder = new TagBuilder("input");
            inputBuilder.Attributes.Add("type", "file");
            inputBuilder.Attributes.Add("id", name);
            inputBuilder.Attributes.Add("name", name);
            inputBuilder.Attributes.Add("class", "hidden");
            inputBuilder.Attributes.Add("value", "");
            inputBuilder.Attributes.Add("accept", accept);
            var inputFileImg = inputBuilder.ToString(TagRenderMode.Normal);

            var imgBuilder = new StringBuilder();
            imgBuilder.AppendLine(@"function _initImgTag(){$('#" + nameInputFile +
                                  "').before(function(){var element;if($('#" + nameInputFile +
                                  "').prev('input[type=\"file\"]').length >0) return element; if(!$(this).prev().hasClass('input-ghost')){element=$('" +
                                  inputFileImg +
                                  "');;element.change(function(){if(element[0].files && element[0].files[0]){var reader=new FileReader();reader.onload=function(e){$('#" +
                                  nameInputFile +
                                  "').attr('src',e.target.result);};reader.readAsDataURL(element[0].files[0]);}});$(this).css('cursor','pointer');$(this).mousedown(function(){$(this).prev('#" +
                                  name +
                                  "').click();return false;});return element;} return element;});}  $( document ).ajaxComplete(function(event, request, settings ){_initImgTag();});");

            RequireScriptCode(helper, MvcHtmlString.Create(imgBuilder.ToString()));

            var builder = new TagBuilder("img");
            builder.Attributes.Add("id", nameInputFile);
            builder.Attributes.Add("name", nameInputFile);
            builder.Attributes.Add("src", url);
            builder.Attributes.Add("alt", altText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString FileFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, string accept, bool multiple, object htmlAttributes = null)
        {
            accept = string.IsNullOrEmpty(accept) ? ".docx,.pdf" : accept;
            var name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(expression));
            var nameInputFile = $"inputFile_{name}";
            var builderInputFile = new TagBuilder("input");
            builderInputFile.Attributes.Add("type", "file");
            builderInputFile.Attributes.Add("id", name);
            builderInputFile.Attributes.Add("name", name);
            builderInputFile.Attributes.Add("class", "hidden");
            if (multiple) builderInputFile.Attributes.Add("multiple", "True");

            builderInputFile.Attributes.Add("accept", accept);
            var inputFileFile = builderInputFile.ToString(TagRenderMode.Normal);

            var builderInputText = new TagBuilder("input");
            builderInputText.Attributes.Add("type", "text");
            builderInputText.Attributes.Add("class", "form-control");
            builderInputText.Attributes.Add("placeholder", AppProcessor.Messagor.GetMessage("Common_Message_Choose"));
            //builderInputText.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            var builderInputGroup = new TagBuilder("div");
            builderInputGroup.Attributes.Add("id", nameInputFile);
            builderInputGroup.Attributes.Add("class", "input-group");
            new RouteValueDictionary(htmlAttributes).ToList().ForEach(a => {
                if (builderInputGroup.Attributes.ContainsKey(a.Key))
                {
                    var currentHtmlValue = builderInputGroup.Attributes[a.Key];
                    builderInputGroup.Attributes[a.Key] = $"{currentHtmlValue} {a.Value}";
                }
            });

            var templateInputFile = //@"<div class='input-group' id='" + nameInputFile + "'> 
                                    @"<span class='input-group-btn'><button class='btn btn-choose' type='button'><i class='fa fa-folder-open'></i>&nbsp;" +
                                    AppProcessor.Messagor.GetMessage("Common_Message_Choose") + "</button></span>" +
                                    builderInputText.ToString(TagRenderMode.Normal) +
                                    @"<span class='input-group-btn'><button class='btn btn-reset' type='button'><i class='fa fa-refresh'></i>&nbsp;" +
                                    AppProcessor.Messagor.GetMessage("Button_Reset") + "</button></span>" 
                                     + (multiple
                                        ? "<div class='box-body hidden' id='SelectedFiles_" + name + "'><ul></ul></div>"
                                        : string.Empty);
            builderInputGroup.InnerHtml = templateInputFile;

            var scriptInputFileBuilder = new StringBuilder();
            scriptInputFileBuilder.AppendLine(@"function _initInputFile_" + name + "(){$('#" + nameInputFile +
                                              "').before(function(){if(!$(this).prev().hasClass('input-ghost')){var element=$('" +
                                              inputFileFile +
                                              "');element.attr('name',$(this).attr('name'));element.change(function(){$('#SelectedFiles_" +
                                              name +
                                              " ul').empty();if(element[0].files.length>1){element.next(element).find('input').val(element[0].files.length.toString()+' tệp tin');$.each(element[0].files,function(idx,f){$('#SelectedFiles_" +
                                              name +
                                              " ul').append('<li><p class=\"text-light-blue\">'+f.name+'</p></li>');});}else{element.next(element).find('input').val((element.val()).split(\'\\\\').pop());}});$(this).find('button.btn-choose').click(function(){element.click();});$(this).find('button.btn-reset').click(function(){element.val(null);$(this).parents('#" +
                                              nameInputFile +
                                              "').find('input').val('');$(element).trigger('change');});$(this).find('input').css('cursor','pointer');$(this).find('input').mousedown(function(){$(this).parents('#" +
                                              nameInputFile +
                                              "').prev().click();return false;});return element;}});} $(function(){ _initInputFile_" +
                                              name + "(); }); ");

            RequireScriptCode(helper, MvcHtmlString.Create(scriptInputFileBuilder.ToString()),
                ScriptPosition.BodyInside);

            return MvcHtmlString.Create(builderInputGroup.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ChooseFile(this HtmlHelper helper, string name, string accept, bool multiple, object htmlAttributes = null)
        {
            accept = string.IsNullOrEmpty(accept) ? ".docx,.pdf" : accept;
            var nameInputFile = $"inputFile_{name}";
            var builderInputFile = new TagBuilder("input");
            builderInputFile.Attributes.Add("type", "file");
            builderInputFile.Attributes.Add("id", name);
            builderInputFile.Attributes.Add("name", name);
            builderInputFile.Attributes.Add("class", "hidden");
            if (multiple) builderInputFile.Attributes.Add("multiple", "True");

            builderInputFile.Attributes.Add("accept", accept);
            var inputFileFile = builderInputFile.ToString(TagRenderMode.Normal);

            var builderInputText = new TagBuilder("input");
            builderInputText.Attributes.Add("type", "text");
            builderInputText.Attributes.Add("class", "form-control");
            builderInputText.Attributes.Add("placeholder", AppProcessor.Messagor.GetMessage("Common_Message_Choose"));

            var builderInputGroup = new TagBuilder("div");
            builderInputGroup.Attributes.Add("id", nameInputFile);
            builderInputGroup.Attributes.Add("class", "input-group");
            new RouteValueDictionary(htmlAttributes).ToList().ForEach(a => {
                if (builderInputGroup.Attributes.ContainsKey(a.Key))
                {
                    var currentHtmlValue = builderInputGroup.Attributes[a.Key];
                    builderInputGroup.Attributes[a.Key] = $"{currentHtmlValue} {a.Value}";
                }
            });

            var templateInputFile = 
                                    @"<span class='input-group-btn'><button class='btn btn-choose' type='button'><i class='fa fa-folder-open'></i>&nbsp;" +
                                    AppProcessor.Messagor.GetMessage("Common_Message_Choose") + "</button></span>" +
                                    builderInputText.ToString(TagRenderMode.Normal) +
                                    @"<span class='input-group-btn'><button class='btn btn-reset' type='button'><i class='fa fa-refresh'></i>&nbsp;" +
                                    AppProcessor.Messagor.GetMessage("Button_Reset") + "</button></span>"
                                     + (multiple
                                        ? "<div class='box-body hidden' id='SelectedFiles_" + name + "'><ul></ul></div>"
                                        : string.Empty);
            builderInputGroup.InnerHtml = templateInputFile;

            var scriptInputFileBuilder = new StringBuilder();
            scriptInputFileBuilder.AppendLine(@"function _initInputFile_" + name + "(){$('#" + nameInputFile +
                                              "').before(function(){if(!$(this).prev().hasClass('input-ghost')){var element=$('" +
                                              inputFileFile +
                                              "');element.attr('name',$(this).attr('name'));element.change(function(){$('#SelectedFiles_" +
                                              name +
                                              " ul').empty();if(element[0].files.length>1){element.next(element).find('input').val(element[0].files.length.toString()+' tệp tin');$.each(element[0].files,function(idx,f){$('#SelectedFiles_" +
                                              name +
                                              " ul').append('<li><p class=\"text-light-blue\">'+f.name+'</p></li>');});}else{element.next(element).find('input').val((element.val()).split(\'\\\\').pop());}});$(this).find('button.btn-choose').click(function(){element.click();});$(this).find('button.btn-reset').click(function(){element.val(null);$(this).parents('#" +
                                              nameInputFile +
                                              "').find('input').val('');$(element).trigger('change');});$(this).find('input').css('cursor','pointer');$(this).find('input').mousedown(function(){$(this).parents('#" +
                                              nameInputFile +
                                              "').prev().click();return false;});return element;}});} $(function(){ _initInputFile_" +
                                              name + "(); }); ");

            RequireScriptCode(helper, MvcHtmlString.Create(scriptInputFileBuilder.ToString()),
                ScriptPosition.BodyInside);

            return MvcHtmlString.Create(builderInputGroup.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString Button(this HtmlHelper helper, bool isModal, string buttonId, string urlAction,
            string icon, string title, object htmlAttributes = null)
        {
            var tmpUrlAction = urlAction;
            var index = tmpUrlAction?.IndexOf("?");
            if (index != null && index > 0)
                tmpUrlAction = tmpUrlAction.Substring(0, index.GetValueOrDefault(0));

            var pActions = !string.IsNullOrEmpty(tmpUrlAction) ? tmpUrlAction.Split('/') : new string[] { };
            pActions = pActions.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (pActions.Length < 2) return null;
            if (pActions.Length == 2)
                if (!AuthorityExtensions.IsAllow(helper.ViewContext.RequestContext,
                    helper.ViewContext.RequestContext.HttpContext.User.Identity.Name, pActions[0], pActions[1]))
                    return null;

            if (pActions.Length == 3)
                if (!AuthorityExtensions.IsAllow(helper.ViewContext.RequestContext,
                    helper.ViewContext.RequestContext.HttpContext.User.Identity.Name, pActions[1], pActions[2],
                    pActions[0]))
                    return null;

            var builder = new TagBuilder("a");
            if (isModal)
            {
                builder.Attributes.Add("data-modal", "true");
                builder.Attributes.Add("data-modal-id", buttonId);
            }
            else
            {
                builder.Attributes.Add("id", buttonId);
            }

            builder.Attributes.Add("href", urlAction);
            builder.InnerHtml = $"{icon}&nbsp;{title}";

            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ButtonSubmit(this HtmlHelper helper, string buttonId, string icon, string title,
            object htmlAttributes = null)
        {
            var builder = new TagBuilder("button");
            builder.Attributes.Add("type", "submit");
            builder.Attributes.Add("id", buttonId);
            builder.InnerHtml = $"{icon}&nbsp;{title}";
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString TitleFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            return TitleFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString TitleFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            //var labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            var labelText = metadata.DisplayName ?? (metadata.PropertyName != null
                                ? AppProcessor.Messagor.GetMessage($"Label_{metadata.PropertyName}")
                                : AppProcessor.Messagor.GetMessage($"Label_{htmlFieldName.Split('.').Last()}"));

            if (string.IsNullOrEmpty(labelText)) return MvcHtmlString.Empty;

            var isRequired = false;

            if (metadata.ContainerType != null)
                isRequired = metadata.ContainerType?.GetProperty(metadata.PropertyName ?? string.Empty)
                                 ?.GetCustomAttributes(typeof(RequiredAttribute), false)
                                 .Length == 1;

            var tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));


            if (isRequired)
            {
                var span = new TagBuilder("span");
                labelText += " (*)";
                span.Attributes.Add("class", "text-danger");
                span.SetInnerText(labelText);
                tag.InnerHtml = span.ToString(TagRenderMode.Normal);
            }
            else
            {
                tag.InnerHtml = labelText;
            }

            // assign <span> to <label> inner html

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        #region Class

        private class RequiredScript
        {
            public string Source { get; set; }
            public IHtmlString RawCode { get; set; }
            public bool Async { get; set; }
            public bool Defer { get; set; }
            public ScriptPosition Position { get; set; }
        }

        private static IList<RequiredScript> Scripts
        {
            get
            {
                return HttpContext.Current.Items["RequiredScripts"] as IList<RequiredScript> ??
                       new List<RequiredScript>();
            }
            set { HttpContext.Current.Items["RequiredScripts"] = value; }
        }

        #endregion

        #region Script + CSS

        public static void RequireScriptCode(this HtmlHelper html, IHtmlString code,
            ScriptPosition position = ScriptPosition.BodyEnd)
        {
            var scripts = Scripts;
            if (!scripts.All(s => s == null || !s.RawCode.ToString().Equals(code.ToString()))) return;
            scripts.Add(new RequiredScript { RawCode = code, Position = position });
            Scripts = scripts;
        }

        public static IHtmlString RenderScripts(this HtmlHelper html, ScriptPosition position)
        {
            var builder = new StringBuilder();
            foreach (var script in Scripts.Where(s => s.Position == position))
                if (script.RawCode != null && !string.IsNullOrWhiteSpace(script.RawCode.ToString()))
                    builder.AppendLine("<script type='text/javascript'>" + script.RawCode + "</script>");
                else if (!string.IsNullOrWhiteSpace(script.Source))
                    builder.AppendLine("<script src='" + script.Source + "'" + (script.Defer ? " defer" : null) +
                                       (script.Async ? " async" : null) + "></script>");

            return new HtmlString(builder.ToString());
        }

        #endregion
    }
}