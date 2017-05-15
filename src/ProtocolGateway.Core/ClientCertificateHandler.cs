using DotNetty.Handlers.Tls;
using DotNetty.Transport.Channels;
using Microsoft.Azure.Devices.ProtocolGateway.Instrumentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.ProtocolGateway
{
    public class ClientCertificateHandler : ChannelHandlerAdapter
    {
        private TlsHandlerWrapper _wrapper;

        public ClientCertificateHandler(TlsHandlerWrapper wrapper)
        {
            _wrapper = wrapper;
        }
        public override void UserEventTriggered(IChannelHandlerContext context, object @event)
        {
            var handshakeCompletionEvent = @event as TlsHandshakeCompletionEvent;
            if (handshakeCompletionEvent != null && !handshakeCompletionEvent.IsSuccessful)
            {
                CommonEventSource.Log.Warning("TLS handshake failed.", handshakeCompletionEvent.Exception.ToString());
            }

            // send it down to the MqttAdapter
            context.FireUserEventTriggered(@event);

            
        }
    }
}
