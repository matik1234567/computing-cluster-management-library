using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MethaneMeasurement
{
    internal class MyLog : Logger
    {
        private HttpListener _httpListener;

        private static string htmlBegin = "<html><head><title>Localhost server -- port 5000</title> <meta http-equiv=\"refresh\" content=\"1\" /><link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-rbsA2VBKQhggwzxH7pPCaAqO46MgnOM80zW1RWuH61DGLwZJEdK2Kadq2F9CUG65\" crossorigin=\"anonymous\">\r\n<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-kenU1KFdBIe4zVF0s0G1M5b4hcpxyD9F7jL+jjXkk+Q2h455rYXK/7HAuoJl+0I4\" crossorigin=\"anonymous\"></script></head><body><div class=\"table-div\" style=\"height:900px;overflow:auto;\"><table class=\"table\"><tr><th>Type</th><th>Source</th><th>Message</th></tr>";
        private static string htmlEnd = "</table></div></body></html>";
        private string _logString = "";

        public MyLog(LogLevel logLevel) : base(logLevel) 
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://localhost:5000/");
            _httpListener.Start(); // start server (Run application as Administrator!)
            Thread _responseThread = new Thread(ResponseThread);
            _responseThread.Start(); // start the response thread
        }

        public override void Write(LogType type, string source, string message)
        {
            _logString += $"<tr><td>{type}</td><td>{source}</td><td>{message}</td></tr>";
        }

        private void ResponseThread()
        {
            while (true)
            {
                HttpListenerContext context = _httpListener.GetContext(); // get a context
                                                                          // Now, you'll find the request URL in context.Request.Url
                byte[] _responseArray = Encoding.UTF8.GetBytes(htmlBegin+ _logString+htmlEnd); // get the bytes to response
                context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                context.Response.KeepAlive = false; // set the KeepAlive bool to false
                context.Response.Close(); // close the connection
                Thread.Sleep(1000);
            }
        }
    }
}
