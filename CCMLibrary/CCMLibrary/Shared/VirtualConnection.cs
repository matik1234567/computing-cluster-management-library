using CCMLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    internal class VirtualConnection
    {
        class ConnectionModel
        {
#pragma warning disable CS0649 // Field 'VirtualConnection.ConnectionModel.Request' is never assigned to, and will always have its default value
            public (ClientRequest, object?) Request;
#pragma warning restore CS0649 // Field 'VirtualConnection.ConnectionModel.Request' is never assigned to, and will always have its default value
#pragma warning disable CS0649 // Field 'VirtualConnection.ConnectionModel.Response' is never assigned to, and will always have its default value
            public (ClientRequest, object?) Response;
#pragma warning restore CS0649 // Field 'VirtualConnection.ConnectionModel.Response' is never assigned to, and will always have its default value
        }

        public static VirtualConnection? Connection;

        //private List<ConnectionModel> connections = new List<ConnectionModel>();
        private object _locker = new();

        private (ClientRequest, object?) _request;
        private (ServerResponse, object?) _response;

        private bool _isClientRequestAvailable = false;
        private bool _isClientResponseAvailable = false;

        public VirtualConnection()
        {
            Connection = this;
        }

        public static bool IsClientConnected()
        {
            return Connection != null;
        }

        /// <summary>
        /// Method send response from server
        /// </summary>
        /// <param name="serverResponse"></param>
        /// <param name="data"></param>
        public void SendResponse(ServerResponse serverResponse, object? data)
        {
            lock(_locker)
            {
                _response = (serverResponse, data);
                _isClientResponseAvailable = true;
            }
            
        }

        /// <summary>
        /// Get client request from server side
        /// </summary>
        /// <returns></returns>
        public (ClientRequest, object?) ReceiveRequest()
        {
            while (!_isClientRequestAvailable)
            {
                ;
            }
            lock (_locker)
            {
                _isClientRequestAvailable = false;
                return _request;
                
            }

        }

        /// <summary>
        /// Send request to server from client
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data"></param>
        public void SendRequest(ClientRequest request, object? data) 
        {
            lock(_locker)
            {
                _request = (request, data);
                _isClientRequestAvailable = true;
            }
        }

        /// <summary>
        /// Receive response from server on client side
        /// </summary>
        /// <param name="serverResponse"></param>
        /// <param name="data"></param>
        public (ServerResponse, object?) ReceiveResponse()
        {
            while (!_isClientResponseAvailable)
            {
                ;
            }
            lock (_locker)
            {
                
                _isClientResponseAvailable = false;
                return _response;
                
            }
        }

    }
}
