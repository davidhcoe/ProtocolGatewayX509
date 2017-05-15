using Microsoft.Azure.Devices.ProtocolGateway.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Microsoft.Azure.Devices.ProtocolGateway.IotHubClient
{
    public sealed class X509DeviceIdentityProvider : IDeviceIdentityProvider
    {
        public Task<IDeviceIdentity> GetAsync(string clientId, string username, string password, EndPoint clientAddress)
        {
            throw new NotImplementedException();
        }
        
        public Task<IDeviceIdentity> GetAsync(string clientId, string username, X509Certificate clientCert, string keyStore, EndPoint clientAddress)
        {

            X509Certificate2 signedCert = new X509Certificate2(clientCert);

            CspParameters parameters = new CspParameters();
            parameters.KeyNumber = (int)KeyNumber.Exchange;
            parameters.ProviderName = "Microsoft Enhanced Cryptographic Provider v1.0";
            parameters.ProviderType = 1;
            parameters.KeyContainerName = keyStore;
            RSACryptoServiceProvider cryptoProvider = new RSACryptoServiceProvider(4096, parameters);

            signedCert.PrivateKey = cryptoProvider;




            IotHubDeviceIdentity deviceIdentity;
            if (!IotHubDeviceIdentity.TryParse(username, out deviceIdentity) || !clientId.Equals(deviceIdentity.Id, StringComparison.Ordinal))
            {
                return Task.FromResult(UnauthenticatedDeviceIdentity.Instance);
            }
            deviceIdentity.WithCertificate(signedCert);
            return Task.FromResult<IDeviceIdentity>(deviceIdentity);
        }
    }
}
