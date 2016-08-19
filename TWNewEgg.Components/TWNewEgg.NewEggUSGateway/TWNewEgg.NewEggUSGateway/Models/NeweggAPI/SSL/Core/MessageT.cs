using System;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Message T class.
    /// </summary>
    /// <typeparam name="T">Container type.</typeparam>
    [DataContract]
    public class Message<T>
    {
        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        public Message()
            : this(string.Empty, string.Empty, default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="body">Body object.</param>
        public Message(T body)
            : this(string.Empty, string.Empty, body)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="code">Code number.</param>
        /// <param name="description">Description message.</param>
        public Message(string code, string description)
            : this(code, description, default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        /// <param name="code">Code number.</param>
        /// <param name="description">Description message.</param>
        /// <param name="body">Body object.</param>
        public Message(string code, string description, T body)
        {
            this.Code = code;
            this.Description = description;
            this.Body = body;
        }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        [DataMember(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Body.
        /// </summary>
        [DataMember(Name = "Body")]
        public T Body { get; set; }

        /// <summary>
        /// Gets a value indicating whether HasError.
        /// </summary>
        public bool HasError
        {
            get
            {
                return ErrorCode.Error.Equals(this.Code, StringComparison.InvariantCultureIgnoreCase)
                    || ErrorCode.LogicError.Equals(this.Code, StringComparison.InvariantCultureIgnoreCase)
                    || this.Body == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether HasPrompt.
        /// </summary>
        public bool HasPrompt
        {
            get
            {
                return ErrorCode.HasPrompt.Equals(this.Code, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Gets a value indicating whether HasSuccess.
        /// </summary>
        public bool HasSuccess
        {
            get
            {
                return ErrorCode.NoError.Equals(this.Code, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
