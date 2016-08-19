using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Message T class.
    /// </summary>
    /// <typeparam name="T">Container type.</typeparam>
    [DataContract]
    public class Message : Message<object>
    {
        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        public Message()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="body">Body object.</param>
        public Message(object body)
            : base(body)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="code">Code number.</param>
        /// <param name="description">Description message.</param>
        public Message(string code, string description)
            : base(code, description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="code">Code number.</param>
        /// <param name="description">Description message.</param>
        /// <param name="body">Body object.</param>
        public Message(string code, string description, object body)
            : base(code, description, body)
        {
        }
    }
}