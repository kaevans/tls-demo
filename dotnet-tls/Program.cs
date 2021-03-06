﻿using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Sample.Microsoft.HelloKeyVault;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace dotnet_tls
{
    class Program
    {
        static KeyVaultClient keyVaultClient;
        static InputValidator inputValidator;
        static void Main(string[] args)
        {
            inputValidator = new InputValidator(args);

            ConfigureServicePointManager();

            ServiceClientTracing.AddTracingInterceptor(new ConsoleTracingInterceptor());
            ServiceClientTracing.IsEnabled = true;


            keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                   (authority, resource, scope) => GetAccessToken(authority, resource, scope)));

            KeyBundle keyBundle = null;
            string keyName = default(string);

            try
            {
                keyBundle = CreateKey(keyBundle, out keyName);
                WrapUnwrap(keyBundle);
            }
            catch (KeyVaultClientException keyVaultoops)
            {
                //Something's up with KeyVault, check permissions,
                //TODO: use transient fault handling to retry
                WriteException(keyVaultoops, 0);
            }
            catch (HttpRequestException networkoops)
            {
                //Check network availability, TLS settings
                WriteException(networkoops, 0);
            }
        }

        static void WriteException(Exception oops, int indent)
        {
            string pad = new string(' ', indent);
            
            Console.WriteLine(string.Format("{0}-{1}: {2}", pad, oops.GetType().FullName, oops.Message));            

            if (null != oops.InnerException)
            {
                WriteException(oops.InnerException, ++indent);
            }

            Console.WriteLine(oops.StackTrace);
        }
        /// <summary>
        /// Created the specified key
        /// </summary>
        /// <param name="keyBundle"> key bundle to create </param>
        /// <returns> created key bundle </returns>
        private static KeyBundle CreateKey(KeyBundle keyBundle, out string keyName)
        {
            // Get key bundle which is needed for creating a key
            keyBundle = keyBundle ?? inputValidator.GetKeyBundle();
            var vaultAddress = inputValidator.GetVaultAddress();
            keyName = inputValidator.GetKeyName();

            var tags = inputValidator.GetTags();

            var name = keyName;
            // Create key in the KeyVault key vault
            var createdKey = keyVaultClient.CreateKeyAsync(vaultAddress, name, keyBundle.Key.Kty, keyAttributes: keyBundle.Attributes, tags: tags).GetAwaiter().GetResult();

            Console.Out.WriteLine("Created key:---------------");
            PrintoutKey(createdKey);

            // Store the created key for the next operation if we have a sequence of operations
            return createdKey;
        }


        private static void WrapUnwrap(KeyBundle key)
        {
            KeyOperationResult wrappedKey;

            var algorithm = inputValidator.GetEncryptionAlgorithm();
            byte[] symmetricKey = inputValidator.GetSymmetricKey();

            string keyVersion = inputValidator.GetKeyVersion();

            if (keyVersion != string.Empty)
            {
                var vaultAddress = inputValidator.GetVaultAddress();
                string keyName = inputValidator.GetKeyName(true);
                wrappedKey = Task.Run(() => keyVaultClient.WrapKeyAsync(vaultAddress, keyName, keyVersion, algorithm, symmetricKey)).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else
            {
                // If the key ID is not initialized get the key id from args
                var keyId = (key != null) ? key.Key.Kid : inputValidator.GetKeyId();

                // Wrap the symmetric key
                wrappedKey = Task.Run(() => keyVaultClient.WrapKeyAsync(keyId, algorithm, symmetricKey)).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            Console.Out.WriteLine(string.Format("The symmetric key is wrapped using key id {0} and algorithm {1}", wrappedKey.Kid, algorithm));

            // Unwrap the symmetric key
            var unwrappedKey = Task.Run(() => keyVaultClient.UnwrapKeyAsync(wrappedKey.Kid, algorithm, wrappedKey.Result)).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.Out.WriteLine(string.Format("The unwrapped key is{0}the same as the original key!",
                symmetricKey.SequenceEqual(unwrappedKey.Result) ? " " : " not "));
        }


        /// <summary>
        /// Prints out key bundle values
        /// </summary>
        /// <param name="keyBundle"> key bundle </param>
        private static void PrintoutKey(KeyBundle keyBundle)
        {
            Console.Out.WriteLine("Key: \n\tKey ID: {0}\n\tKey type: {1}",
                keyBundle.Key.Kid, keyBundle.Key.Kty);

            var expiryDateStr = keyBundle.Attributes.Expires.HasValue
                ? keyBundle.Attributes.Expires.ToString()
                : "Never";

            var notBeforeStr = keyBundle.Attributes.NotBefore.HasValue
                ? keyBundle.Attributes.NotBefore.ToString()
                : UnixTimeJsonConverter.EpochDate.ToString();

            Console.Out.WriteLine("Key attributes: \n\tIs the key enabled: {0}\n\tExpiry date: {1}\n\tEnable date: {2}",
                keyBundle.Attributes.Enabled, expiryDateStr, notBeforeStr);
        }



        /// <summary>
        /// Gets the access token
        /// </summary>
        /// <param name="authority"> Authority </param>
        /// <param name="resource"> Resource </param>
        /// <param name="scope"> scope </param>
        /// <returns> token </returns>
        public static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            // Build request to acquire managed identities for Azure resources token
            Console.WriteLine("resource: " + resource);
            Console.WriteLine("scope: " + scope);
            var request = (HttpWebRequest)WebRequest.Create("http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=" + resource);
            request.Headers["Metadata"] = "true";
            request.Method = "GET";

            
            // Call /token endpoint
            var response = (HttpWebResponse)await request.GetResponseAsync();

            // Pipe response Stream to a StreamReader, and extract access token
            var streamResponse = new StreamReader(response.GetResponseStream());
            var stringResponse = streamResponse.ReadToEnd();
            var j = new JavaScriptSerializer();
            Dictionary<string, string> list = (Dictionary<string, string>)j.Deserialize(stringResponse, typeof(Dictionary<string, string>));
            string accessToken = list["access_token"];

            return accessToken;

        }


        static void ConfigureServicePointManager()
        {
            var value = ConfigurationManager.AppSettings["ServicePointManagerConfigEnabled"];
            bool enable = false;
            bool.TryParse(value, out enable);

            if (enable)
            {
                // add TLS 1.2
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                // remove insecure TLS options
                System.Net.ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
                System.Net.ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
            }
        }
    }
}
