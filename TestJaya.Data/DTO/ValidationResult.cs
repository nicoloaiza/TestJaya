using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TestJaya.Data.DTO
{
    public class ValidationResult
    {
        private readonly StringBuilder message = new StringBuilder();

        /// <summary>
        /// Returns false when at least one error message has been appended to this <see cref="ValidationResult"/>.
        /// </summary>
        public bool IsValid => string.IsNullOrEmpty(Message);

        /// <summary>
        /// Gets a string including all error messages appended to this <see cref="ValidationResult"/>.
        /// </summary>
        public string Message => message.ToString();

        /// <summary>
        /// Appends an error message to this <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="message">Error message to be appended.</param>
        public void AppendMessage(string message)
        {
            if (this.message.Length > 0) this.message.Append("; ");
            this.message.Append(message);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString() => Message;
    }
}
