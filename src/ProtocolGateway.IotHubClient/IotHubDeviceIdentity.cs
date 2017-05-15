// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.ProtocolGateway.IotHubClient
{
    using Instrumentation;
    using Microsoft.Azure.Devices.ProtocolGateway.Identity;
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    public sealed class IotHubDeviceIdentity : IDeviceIdentity
    {
        string asString;
        string policyName;
        AuthenticationScope scope;

        public IotHubDeviceIdentity(string iotHubHostName, string deviceId)
        {
            this.IotHubHostName = iotHubHostName;
            this.Id = deviceId;
        }

        public string Id { get; }

        public bool IsAuthenticated => true;

        public string IotHubHostName { get; }


        // coe
        public X509Certificate2 Certificate { get; set; }

        public string PolicyName
        {
            get { return this.policyName; }
            private set
            {
                this.policyName = value;
                this.asString = null;
            }
        }

        public string Secret { get; private set; }

        public AuthenticationScope Scope
        {
            get { return this.scope; }
            private set
            {
                this.scope = value;
                this.asString = null;
            }
        }

        public static bool TryParse(string value, out IotHubDeviceIdentity identity)
        {
            string[] usernameSegments = value.Split('/');
            if (usernameSegments.Length < 2)
            {
                identity = null;
                return false;
            }
            identity = new IotHubDeviceIdentity(usernameSegments[0], usernameSegments[1]);
            return true;
        }

        public void WithSasToken(string token)
        {
            this.Scope = AuthenticationScope.SasToken;
            this.Secret = token;
        }

        // coe
        public void WithCertificate(X509Certificate2 clientCert)
        {
            this.Scope = AuthenticationScope.X509Certificate;

            
            //// a 'look up'
            //if(clientCert.GetCertHashString() == "48A50F571EDE0F097B4A019702F645A11B0BD27C")
            //{
            //    var clientCert = new X509Certificate2(@"C:\IoT_Device_Certs\certificate.pfk", "changeit");

            //    CommonEventSource.Log.Verbose("Client connected with cert hashed " + clientCert.GetCertHashString(), Id);

            //    this.Certificate = clientCert;

            //}


            CommonEventSource.Log.Verbose("Client connected with cert hashed " + clientCert.GetCertHashString(), Id);

            // we dont know the private key - and IoT Hub requires a private key

            this.Certificate = clientCert; //new X509Certificate2(clientCert);
            
            Console.WriteLine(this.Certificate.Thumbprint);

        }

        public void WithHubKey(string keyName, string keyValue)
        {
            this.Scope = AuthenticationScope.HubKey;
            this.PolicyName = keyName;
            this.Secret = keyValue;
        }

        public void WithDeviceKey(string keyValue)
        {
            this.Scope = AuthenticationScope.DeviceKey;
            this.Secret = keyValue;
        }

        public override string ToString()
        {
            if (this.asString == null)
            {
                string policy = string.IsNullOrEmpty(this.PolicyName) ? "<none>" : this.PolicyName;
                this.asString = $"{this.Id} [IotHubHostName: {this.IotHubHostName}; PolicyName: {policy}; Scope: {this.Scope}]";
            }
            return this.asString;
        }
    }
}