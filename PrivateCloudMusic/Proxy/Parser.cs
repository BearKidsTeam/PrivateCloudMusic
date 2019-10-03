using System;
using System.Text;
using Google.Protobuf;
using Pcm.Proto;

namespace Pcm.Proxy
{
    public enum PayloadType
    {
        Unknown,
        Protobuf,
        Json
    }
    
    public abstract class Parser
    {
        public abstract (Request, PayloadType) From(byte[] data, Encoding encoding = null);
        public abstract byte[] To(Response resp, Encoding encoding = null);
    }

    public class JsonParser : Parser
    {
        public override (Request, PayloadType) From(byte[] data, Encoding encoding = null)
        {
            if (encoding == null)
            {
                throw new NotImplementedException();
            }

            try
            {
                var str = encoding.GetString(data);
                var req = Request.Parser.ParseJson(str);
                return (req, PayloadType.Json);
            }
            catch
            {
                return (null, PayloadType.Unknown);
            }
        }

        public override byte[] To(Response resp, Encoding encoding = null)
        {
            if (encoding == null)
            {
                throw new NotImplementedException();
            }
            
            var str = JsonFormatter.Default.Format(resp);
            var bytes = encoding.GetBytes(str);
            return bytes;
        }
    }

    public class ProtobufParser : Parser
    {
        public override (Request, PayloadType) From(byte[] data, Encoding encoding = null)
        {
            try
            {
                var resp = Request.Parser.ParseFrom(data);
                return (resp, PayloadType.Protobuf);
            } 
            catch
            {
                return (null, PayloadType.Unknown);
            }
        }

        public override byte[] To(Response resp, Encoding encoding = null)
        {
            var data = resp.ToByteArray();
            return data;
        }
    }
}