using Microsoft.WindowsAzure.MobileServices;
using System;
using Windows.Foundation;

namespace MobileServices
{
    public class AdminServiceFilter : IServiceFilter
    {
        private bool disableScripts;
        private string masterKey;

        public AdminServiceFilter(bool disableScripts, string masterKey)
        {
            this.disableScripts = disableScripts;
            this.masterKey = masterKey;
        }

        public IAsyncOperation<IServiceFilterResponse> Handle(IServiceFilterRequest request, IServiceFilterContinuation continuation)
        {
            // Add master key to the request's header to have admin level control
            request.Headers["X-ZUMO-MASTER"] = this.masterKey;

            if (this.disableScripts)
            {
                // Get the request's query and append noScript=true as the first query parameter
                var uriBuilder = new UriBuilder(request.Uri);
                var oldQuery = (uriBuilder.Query ?? string.Empty).Trim('?');

                uriBuilder.Query = ("noScript=true&" + oldQuery).Trim('&');

                request.Uri = uriBuilder.Uri;
            }

            return continuation.Handle(request);
        }
    }
}
