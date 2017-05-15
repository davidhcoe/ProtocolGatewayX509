using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Channels;
using Microsoft.Azure.Devices.ProtocolGateway.Instrumentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.ProtocolGateway
{
    public class TlsHandlerWrapper : ByteToMessageDecoder
    {

        private TlsHandler handler;

        private TlsHandlerWrapper()
        {
            //CheckOnRemoteCert();

            
        }

        //private void CheckOnRemoteCert()
        //{
        //    Task.Run(() =>
        //    {
        //        bool stillNoCert = true;



        //        while(stillNoCert)
        //        {
        //            try
        //            {
        //                if (RemoteCertificate != null)
        //                {
        //                    stillNoCert = false;

        //                    Console.WriteLine("Found the cert!");
        //                }
        //            }
        //            catch { }
        //        }

                
        //    });
        //}

        public TlsHandlerWrapper(TlsSettings settings) 
        {
            handler = new DotNetty.Handlers.Tls.TlsHandler(settings);
            
        }
        public TlsHandlerWrapper(Func<Stream, SslStream> sslStreamFactory, TlsSettings settings)
        {
            handler = new TlsHandler(sslStreamFactory, settings);
        }

        public X509Certificate LocalCertificate { get { return handler.LocalCertificate; } }
        public X509Certificate RemoteCertificate { get { return handler.RemoteCertificate; } }

        public static TlsHandlerWrapper Client(string targetHost)
        {
            TlsHandlerWrapper wrapper = new TlsHandlerWrapper();
            wrapper.handler = TlsHandler.Client(targetHost);
            return wrapper;
        }
        public static TlsHandlerWrapper Client(string targetHost, X509Certificate clientCertificate)
        {
            TlsHandlerWrapper wrapper = new TlsHandlerWrapper();
            wrapper.handler = TlsHandler.Client(targetHost, clientCertificate);
            return wrapper;
            
        }
        public static TlsHandlerWrapper Server(X509Certificate certificate)
        {
            TlsHandlerWrapper wrapper = new TlsHandlerWrapper();

            ServerTlsSettings serverTls = new ServerTlsSettings(certificate, true);

            Func<Stream, SslStream> customSslStream = CreateSslStream;

            TlsHandler tlsHandler = new TlsHandler(CreateSslStream, serverTls);

            wrapper.handler = tlsHandler;
            return wrapper;
            
        }

        public static SslStream CreateSslStream(Stream innerStream)
        {
            SslStream sslStream = new SslStream(
                innerStream, true, remoteCertificateSelectionCallback);

            return sslStream;
        }

        private static bool remoteCertificateSelectionCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("check the client cert");

            // for testing
            if (certificate.GetCertHashString() == "48A50F571EDE0F097B4A019702F645A11B0BD27C")
                return true;
            else
                return false;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            
            handler.ChannelActive(context);
        }
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            handler.ChannelInactive(context);
        }
        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            handler.ChannelReadComplete(ctx);
        }
        public override Task CloseAsync(IChannelHandlerContext context)
        {
            return handler.CloseAsync(context);
        }
        public void Dispose()
        {
            handler.Dispose();
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            handler.ExceptionCaught(context, exception);
        }

   


        public override void Flush(IChannelHandlerContext context)
        {
            handler.Flush(context);
        }
        public override void HandlerAdded(IChannelHandlerContext context)
        {
            handler.HandlerAdded(context);
        }
        public override void Read(IChannelHandlerContext context)
        {
            handler.Read(context);
        }
        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            return handler.WriteAsync(context, message);
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            MethodInfo mi = handler.GetType().GetMethod("Decode", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (mi != null)
                mi.Invoke(handler, new object[] { context, input, output });
        }

        protected override void HandlerRemovedInternal(IChannelHandlerContext context)
        {
            MethodInfo mi = handler.GetType().GetMethod("HandlerRemovedInternal", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (mi != null)
                handler.GetType().GetMethod("HandlerRemovedInternal").Invoke(handler, new object[] { context });
        }

   
    }
}
