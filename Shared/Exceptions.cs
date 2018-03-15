using System;
using System.Runtime.Serialization;
using System.Runtime.Remoting;

namespace SuperSoftware.Shared
{
    [Serializable]
    public abstract class SuperSoftwareException : RemotingException, ISerializable
    {
        private string _internalMessage = "";

        public SuperSoftwareException() : base() { }
        public SuperSoftwareException(string msg) : base(msg) {
            _internalMessage = msg;
        }
        public SuperSoftwareException(string msg, Exception ex) : base(msg,ex) { }
        public SuperSoftwareException(SerializationInfo info, StreamingContext context) {
            _internalMessage = (string)info.GetValue("_internalMessage", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_internalMessage", _internalMessage);
        }

        public override string Message
        {
            get
            {
                return "This is your custom remotable exception returning: \""
             + _internalMessage
             + "\"";
            }
        }
    }

    [Serializable]
    public class ConfigurationException : SuperSoftwareException
    {
        public ConfigurationException() : base() { }
        public ConfigurationException(string msg) : base(msg) { }
        public ConfigurationException(string msg, Exception ex) : base(msg,ex) { }
        public ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class ProductNotFoundException : SuperSoftwareException
    {
        public ProductNotFoundException() : base() { }
        public ProductNotFoundException(string msg) : base(msg) { }
        public ProductNotFoundException(string msg, Exception ex) : base(msg,ex) { }
        public ProductNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class FamilyNotFoundException : SuperSoftwareException
    {
        public FamilyNotFoundException() : base() { }
        public FamilyNotFoundException(string msg) : base(msg) { }
        public FamilyNotFoundException(string msg, Exception ex) : base(msg, ex) { }
        public FamilyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class InsuficientStockException : SuperSoftwareException
    {
        public InsuficientStockException() : base() { }
        public InsuficientStockException(string msg) : base(msg) { }
        public InsuficientStockException(string msg, Exception ex) : base(msg,ex) { }
        public InsuficientStockException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
