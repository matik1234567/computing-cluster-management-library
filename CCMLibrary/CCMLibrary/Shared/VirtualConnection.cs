using CCMLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    public class VirtualConnection
    {
        //private List<ConnectionModel> connections = new List<ConnectionModel>();
        private object _locker = new();

        private (ClientRequest, object?) _request;
        private (ServerResponse, object?) _response;

        private volatile bool _isClientRequestAvailable = false;
        private volatile bool _isClientResponseAvailable = false;

        private volatile bool _stopVirtualConnection = false;

        public void Stop()
        {
            _stopVirtualConnection = true;
        }

        /// <summary>
        /// Method send response from server
        /// </summary>
        /// <param name="serverResponse"></param>
        /// <param name="data"></param>
        public void SendResponse(ServerResponse serverResponse, ref object? data)
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
                if (_stopVirtualConnection)
                {
                    return (ClientRequest.None, null);
                }
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
        public void SendRequest(ClientRequest request, ref object? data) 
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
                if (_stopVirtualConnection)
                {
                    return (ServerResponse.Null, null);
                }
            }
            lock (_locker)
            {
                
                _isClientResponseAvailable = false;
                return _response;
                
            }
        }

    }
}
