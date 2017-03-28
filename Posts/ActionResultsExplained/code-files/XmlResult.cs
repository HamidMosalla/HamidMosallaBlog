using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Internal;

namespace VS2017Test.Controllers
{
    public class XmlResult : ActionResult
    {
        /// <summary>Gets or sets the HTTP status code.</summary>
        public int? StatusCode { get; set; }

        /// <summary>Gets or sets the value to be formatted.</summary>
        public object Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.AspNetCore.Mvc.JsonResult" /> with the given <paramref name="value" />.
        /// </summary>
        /// <param name="value">The value to format as JSON.</param>
        public XmlResult(object value)
        {
            this.Value = value;
        }

        public XmlResult(object value, int? statusCode)
        {
            this.Value = value;
            this.StatusCode = statusCode;
        }

        private string Serialize<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var type = value.GetType();
            XmlSerializer serializer = new XmlSerializer(type);

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, value);
                return writer.ToString();
            }
        }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/xml";
            response.StatusCode = StatusCode ?? 200;
            //context.HttpContext.Response.Headers.Add("Content-Type", new[] { "application/xml" });
            var xmlBytes = Encoding.ASCII.GetBytes(Serialize(Value));
            context.HttpContext.Response.Body.WriteAsync(xmlBytes, 0, xmlBytes.Length);
            return TaskCache.CompletedTask;
        }
    }
}