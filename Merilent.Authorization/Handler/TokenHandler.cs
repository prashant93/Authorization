using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Merilent.Authorization
{
    public class TokenHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try

            {
                //  var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJodHRwczovL21kbmphcHAwMS5tZXJpbGVudC5jb20vUmVmZXJNYXN0ZXIvIiwiaXNzIjoiaHR0cDovL01ETkpBUFAwMS5NRVJJTEVOVC5DT00vYWRmcy9zZXJ2aWNlcy90cnVzdCIsImlhdCI6IjE0NzI1NjMxOTYiLCJleHAiOjE0NzI2NDk2MDUsInJvbGUiOlsiTE9BTURldiIsIk11bWJhaUVtcGxveWVlcyIsIkluY3ViYXRvcnMiLCJEZXZlbG9wbWVudCJdLCJlbWFpbCI6InBwYW5pZ3JhaGlAbWVyaWxlbnQuY29tIiwidW5pcXVlX25hbWUiOiJQcmFzaGFudCBQYW5pZ3JhaGkiLCJuYW1laWQiOiJwcGFuaWdyYWhpQE1FUklMRU5ULkNPTSIsImdpdmVuX25hbWUiOiJQcmFzaGFudCIsImZhbWlseV9uYW1lIjoiUGFuaWdyYWhpIiwiYXV0aF90aW1lIjoiMjAxNi0wOC0zMFQxMjoyMDoyMS40OTBaIiwiYXV0aG1ldGhvZCI6InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDphYzpjbGFzc2VzOlBhc3N3b3JkUHJvdGVjdGVkVHJhbnNwb3J0IiwidmVyIjoiMS4wIiwibmJmIjoxNDcyNTYzMjA1fQ.cb19cq4p5XkC8tEXPDxcN_n82Gbrq82fozxBZlrak4w";
                var token = TokenGenerationHandler.GetToken(ConfigurationManager.ConnectionStrings["MerilentAuthorizationConnection"].ConnectionString,
                                                                             ConfigurationManager.AppSettings["ida:APIName"],
                                                                             ConfigurationManager.AppSettings["ida:ADFSMetadata"]);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    ////request.Headers.Add("Authorization", token);
                    //HttpClient client = new HttpClient();
                    //HttpClient client = new HttpClient();
                    //client.DefaultRequestHeaders.Add("Authorization", token);

                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Headers.Add("Authorization", token);

                    var response = base.SendAsync(request, cancellationToken);

                    return response;
                }
                else
                {
                    HttpResponseMessage response = request.CreateErrorResponse(HttpStatusCode.Forbidden, "eror");

                    throw new HttpResponseException(response);
                }
            }
            catch

            {
                HttpResponseMessage response = request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An unexpected error occured...");

                throw new HttpResponseException(response);
            }
        }
    }
}