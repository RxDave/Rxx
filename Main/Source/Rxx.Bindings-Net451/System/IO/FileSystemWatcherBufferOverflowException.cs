using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace System.IO
{
#if !PORT_45
  [Serializable]
#endif
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic",
    Justification = "For internal use only; FileSystemWatcher exposes an Error event but we need an exception object in the observable.")]
  internal sealed class FileSystemWatcherBufferOverflowException : Exception
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="FileSystemWatcherBufferOverflowException" /> class.
    /// </summary>
    /// <param name="innerException">The exception that was thrown by the buffer overflow.</param>
    public FileSystemWatcherBufferOverflowException(Exception innerException)
      : base(innerException.Message, innerException)
    {
      Contract.Requires(innerException != null);
    }

    private FileSystemWatcherBufferOverflowException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
    #endregion

    #region Methods
    #endregion
  }
}