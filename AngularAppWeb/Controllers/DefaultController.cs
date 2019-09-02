using System;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AngularAppWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly IHttpContextAccessor _accessor;

        public DefaultController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress;
            //Todo:
            //Task.Run(async () => { await GetLocation(ip); });
        }
        private async Task GetLocation(string ip)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("http://ip-api.com/json/" + ip);
            var obj = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (GetValue(obj, "status") != "success")
                return;
            var city = GetValue(obj, "city");
            var isp = GetValue(obj, "isp");
            var country = GetValue(obj, "country");
            var lat = int.Parse(GetValue(obj, "lat"));
            var lon = int.Parse(GetValue(obj, "lon"));
        }

        private string GetValue(JObject @object, string property) => @object[property] != null ? @object[property].ToString() : "0";

        [HttpPost("[action]")]
        public async Task<bool> Hey([FromBody]JObject val)
        {
            bool result = false;
            //Type type = typeof(int).MakeGenericType();
            try
            {
                var s = GetPropValue<string>("aref", val, true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        private T GetPropValue<T>(string propName, JObject value, bool isRequired)
        {
            var list = value.Properties().Select(jt => jt.Name).ToList();
            if (!isAvailable())
                throw new NotAvailableException("Property is not available");
            JToken token = value[propName];
            if (typeof(T) == typeof(bool) && token.ToString() != "true" && token.ToString() != "false")
                throw new NotAvailableException("Type is incorrect");
            return (T)token.ToObject(typeof(T));
            bool isAvailable()
            {
                foreach (var item in list)
                {
                    if (item == propName)
                        return true;
                }
                return false;
            }
        }

        #region ip
        //static string GetClientIp(HttpRequestMessage request)
        //{
        //    string ipAddress = String.Empty;
        //    if (request == null)
        //    {
        //        return ipAddress;
        //    }

        //    #region MyRegion

        //    try
        //    {
        //        ipAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null;
        //    }
        //    catch (Exception e)
        //    {
        //        ipAddress = null;
        //    }

        //    #endregion

        //    #region

        //    if (ipAddress == null)
        //    {
        //        try
        //        {
        //            if (request.Properties.ContainsKey("MS_OwinContext"))
        //            {
        //                ipAddress = ((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress;
        //            }

        //            else if (request.Properties.ContainsKey("MS_HttpContext"))
        //            {
        //                var ctx = request.Properties["MS_HttpContext"] as HttpContent;
        //                if (ctx != null)
        //                {
        //                    ipAddress = ctx.Request.UserHostAddress;
        //                }
        //            }
        //            else
        //                ipAddress = null;
        //        }
        //        catch (Exception e)
        //        {
        //            ipAddress = null;
        //        }
        //    }

        //    #endregion

        //    #region MyRegion

        //    if (ipAddress == null)
        //    {
        //        try
        //        {
        //            var userip = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request
        //                .UserHostAddress;
        //            if (userip != null)
        //            {
        //                long macinfo = new long();
        //                string macSrc = macinfo.ToString("X");
        //                if (macSrc == "0")
        //                {
        //                    ipAddress = userip;
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ipAddress = null;
        //        }
        //    }


        //    #endregion

        //    #region MyRegion

        //    if (ipAddress == null)
        //    {
        //        try
        //        {
        //            if (request.Properties.ContainsKey("MS_OwinContext"))
        //            {
        //                ipAddress = ((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress;
        //            }

        //            else if (request.Properties.ContainsKey("MS_HttpContext"))
        //            {
        //                var ctx = request.Properties["MS_HttpContext"] as HttpContent;
        //                if (ctx != null)
        //                {
        //                    ipAddress = ctx.Request.UserHostAddress;
        //                }
        //            }
        //            else
        //                ipAddress = null;
        //        }
        //        catch (Exception e)
        //        {
        //            ipAddress = null;
        //        }
        //    }

        //    #endregion

        //    #region

        //    if (ipAddress == null)
        //    {
        //        try
        //        {
        //            if (request.Properties.ContainsKey("MS_HttpContext"))
        //            {
        //                ipAddress = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
        //            }
        //            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
        //            {
        //                RemoteEndpointMessageProperty prop =
        //                    (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
        //                ipAddress = prop.Address;
        //            }
        //            else if (HttpContext.Current != null)
        //            {
        //                ipAddress = HttpContext.Current.Request.UserHostAddress;
        //            }
        //            else
        //            {
        //                ipAddress = null;
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            ipAddress = null;
        //        }
        //    }

        //    #endregion

        //    #region

        //    if (ipAddress == null)
        //    {
        //        try
        //        {

        //            string ip = string.Empty;
        //            if (request.Properties.ContainsKey("MS_HttpContext"))
        //            {
        //                System.Web.HttpContextBase context =
        //                    (System.Web.HttpContextBase)request.Properties["MS_HttpContext"];
        //                if (context.Request.ServerVariables["HTTP_VIA"] != null)
        //                {
        //                    ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        //                }
        //                else
        //                {
        //                    ip = context.Request.ServerVariables["REMOTE_ADDR"].ToString();
        //                }
        //            }

        //            ipAddress = ip;
        //        }
        //        catch (Exception e)
        //        {
        //            ipAddress = null;
        //        }
        //    }

        //    #endregion

        //    if (ipAddress == "::1")
        //        ipAddress = "127.0.0.1";

        //    return ipAddress;
        //}
        #endregion
    }
}
public class NotAvailableException : Exception
{
    public NotAvailableException() { }
    public NotAvailableException(string message) : base(message) { }
    public NotAvailableException(string message, Exception inner) : base(message, inner) { }
    protected NotAvailableException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
};


