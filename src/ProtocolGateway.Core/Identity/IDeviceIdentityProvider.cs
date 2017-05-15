// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.ProtocolGateway.Identity
{
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public interface IDeviceIdentityProvider
    {
        Task<IDeviceIdentity> GetAsync(string clientId, string username, string password, EndPoint clientAddress);

        Task<IDeviceIdentity> GetAsync(string clientId, string username, X509Certificate certificate, string keyStore, EndPoint clientAddress);

    }
}