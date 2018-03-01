using System;
using System.Runtime.Serialization;

namespace Redmanmale.TelegramToRss.DAL
{
    [Serializable]
    public class DbException : Exception
    {
        public DbException() { }
        public DbException(string message) : base(message) { }
        public DbException(string message, Exception innerException) : base(message, innerException) { }
        protected DbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
