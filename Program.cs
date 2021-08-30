using System;

namespace icos_dotnet
{
    class Program
    {
        static void Main(string[] args)        
        {
            // --- link generator --- //

            const string httpMethod = "GET";
            const string host = "{endpoint}";
            const string region = "";
            const string endpoint = "https://" + host;
            const string bucket = "example-bucket";
            const string objectKey = "example-object";
            int expiration = 86400;  // time in seconds

            const string accessKeyId = "{COS_HMAC_ACCESS_KEY_ID}";
            const string secretAccessKey = "{COS_HMAC_SECRET_ACCESS_KEY}";



            Console.WriteLine("IBM Cloud Object Storage - direct link");

            var time = DateTime.UtcNow;
            string timestamp = time.ToString("yyyyMMddTHHmmss") + "Z";
            string datestamp = time.ToString("yyyyMMdd");


            string standardizedQuerystring = "X-Amz-Algorithm=AWS4-HMAC-SHA256" +
                "&X-Amz-Credential=" + System.Uri.EscapeDataString(accessKeyId + "/" + datestamp + "/" + region + "/s3/aws4_request") +
                "&X-Amz-Date=" + timestamp +
                "&X-Amz-Expires=" + expiration.ToString() +
                "&X-Amz-SignedHeaders=host";

            string standardizedResource = "/" + bucket + "/" + objectKey;

            string payloadHash = "UNSIGNED-PAYLOAD";
            string standardizedHeaders = "host:" + host;
            string signedHeaders = "host";

            string standardizedRequest = httpMethod + "\n" +
                standardizedResource + "\n" +
                standardizedQuerystring + "\n" +
                standardizedHeaders + "\n" +
                "\n" +
                signedHeaders + "\n" +
                payloadHash;

            // assemble string-to-sign
            string hashingAlgorithm = "AWS4-HMAC-SHA256";
            string credentialScope = datestamp + "/" + region + "/" + "s3" + "/" + "aws4_request";
            string sts = hashingAlgorithm + "\n" +
                timestamp + "\n" +
                credentialScope + "\n" +
                Crypto.hashHex(standardizedRequest);


            // generate the signature
            byte[] signatureKey = Crypto.createSignatureKey(secretAccessKey, datestamp, region, "s3");
            string signature = Crypto.hmacHex(signatureKey, sts);

            // create and send the request
            // the 'requests' package autmatically adds the required 'host' header
            string requestUrl = endpoint + "/" +
                bucket + "/" +
                objectKey + "?" +
                standardizedQuerystring +
                "&X-Amz-Signature=" +
                signature;


            Console.WriteLine("Get Link");

            Console.WriteLine("Request URL = {0}", requestUrl);

        }
    }
}
