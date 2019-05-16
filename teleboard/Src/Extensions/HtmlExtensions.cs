using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace Teleboard.Extensions
{
    public static class HtmlExtensions
    {
        public static IHtmlString CheckboxListFor<TModel>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, 
            IEnumerable<string>>> ex,
            IEnumerable<string> possibleValues,
            IDictionary<string,string> infos
            )
        {
            var metadata = System.Web.Mvc.ModelMetadata.FromLambdaExpression(ex, html.ViewData);
            var availableValues = (IEnumerable<string>)metadata.Model;
            var name = ExpressionHelper.GetExpressionText(ex);
            return html.CheckboxList(name, availableValues, possibleValues, infos);
        }

        private static IHtmlString CheckboxList(this HtmlHelper html, string name, IEnumerable<string> selectedValues, IEnumerable<string> possibleValues, IDictionary<string,string> infos)
        {
            var result = new StringBuilder();

            foreach (string current in possibleValues)
            {
                var input = new TagBuilder("input");
                var inputId = Guid.NewGuid().ToString();
                input.Attributes["type"] = "checkbox";
                input.Attributes["name"] = name;
                input.Attributes["value"] = current;
                input.Attributes["id"] = inputId;
                input.Attributes["class"] = "filled-in chk-col-yellow";
                var isChecked = (selectedValues?? new string[] { }).Contains(current);
                if (isChecked)
                {
                    input.Attributes["checked"] = "checked";
                }

                var label = new TagBuilder("label");
                label.InnerHtml = infos[current];
                label.Attributes["for"] = inputId;

                result.Append(input);
                result.Append(label);
                result.Append(new TagBuilder("br"));
            }
            return new HtmlString(result.ToString());
        }
    }
}
