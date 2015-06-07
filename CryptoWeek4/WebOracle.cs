using System;
using System.Net;
using System.Web;

using XMLib;

namespace CryptoWeek4
{
    class WebOracle : IPaddingOracle
    {
        public byte[] GetCipherText()
        {
            return
                StringUtils.FromHex(
                    "f20bdba6ff29eed7b046d1df9fb7000058b1ffb4210a580f748b4ac714c001bd4a61044426fb515dad3f21f18aa577c0bdf302936266926ff37dbf7035d5eeb4");
        }

        public HttpStatusCode QueryOracle(byte[] _cipherTextBytes)
        {
            try
            {
                var targetSiteUri = new UriBuilder("http://crypto-class.appspot.com/po");

                var cipherText = StringUtils.ToHex(_cipherTextBytes, "");
                var queryBuilder = HttpUtility.ParseQueryString("");
                queryBuilder.Add("er", cipherText);
                targetSiteUri.Query = queryBuilder.ToString();

                var request = WebRequest.Create(targetSiteUri.ToString());
                var response = (HttpWebResponse)request.GetResponse();

                var status = response.StatusCode;
                return status;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                if (response.StatusCode != HttpStatusCode.Forbidden && response.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }
                return response.StatusCode;
            }
        }

    }
}