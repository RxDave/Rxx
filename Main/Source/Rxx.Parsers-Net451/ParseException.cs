using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using Rxx.Parsers.Properties;

namespace Rxx.Parsers
{
  /// <summary>
  /// Represents errors that occur while parsing a sequence.
  /// </summary>
#if !SILVERLIGHT && !PORT_45 && !PORT_40
  [Serializable]
#endif
  [DebuggerDisplay("Parse failed at index {sourceIndex}: {BaseMessage}")]
  public class ParseException : Exception
  {
    #region Public Properties
    /// <summary>
    /// Gets the index in the sequence at which the error occurred while parsing.
    /// </summary>
    public int SourceIndex
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() >= 0);

        return sourceIndex;
      }
    }

    /// <summary>
    /// Gets a message that describes the parser error.
    /// </summary>
    public override string Message
    {
      get
      {
        return string.Format(
          CultureInfo.CurrentCulture,
          Errors.ParseExceptionMessageOverrideFormat,
          base.Message,
          Environment.NewLine,
          sourceIndex);
      }
    }
    #endregion

    #region Private / Protected
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
      Justification = "Used in DebuggerDisplayAttribute.")]
    private string BaseMessage
    {
      get
      {
        return base.Message;
      }
    }

    private int sourceIndex;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="ParseException"/> class with the default <see cref="Message"/>.
    /// </summary>
    public ParseException()
      : base(Errors.ParseExceptionMessage)
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ParseException"/> class with the default <see cref="Message"/>
    /// and the specified <see cref="SourceIndex"/>.
    /// </summary>
    /// <param name="sourceIndex">The index in the sequence at which the error occurred while parsing.</param>
    public ParseException(int sourceIndex)
      : base(Errors.ParseExceptionMessage)
    {
      Contract.Requires(sourceIndex >= 0);

      this.sourceIndex = sourceIndex;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ParseException"/> class with the specified <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message that describes the parser error.</param>
    public ParseException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ParseException"/> class with the specified <paramref name="message"/>
    /// and <see cref="SourceIndex"/>.
    /// </summary>
    /// <param name="sourceIndex">The index in the sequence at which the error occurred while parsing.</param>
    /// <param name="message">The message that describes the parser error.</param>
    public ParseException(int sourceIndex, string message)
      : base(message)
    {
      Contract.Requires(sourceIndex >= 0);

      this.sourceIndex = sourceIndex;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ParseException"/> class with the specified <paramref name="message"/>
    /// and <paramref name="innerException"/>.
    /// </summary>
    /// <param name="message">The message that describes the parser error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null" /> if no inner exception is specified.</param>
    public ParseException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ParseException"/> class with the default <see cref="Message"/>
    /// and the specified <see cref="SourceIndex"/> and <paramref name="innerException"/>.
    /// </summary>
    /// <param name="sourceIndex">The index in the sequence at which the error occurred while parsing.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null" /> if no inner exception is specified.</param>
    public ParseException(int sourceIndex, Exception innerException)
      : base(Errors.ParseExceptionMessage, innerException)
    {
      Contract.Requires(sourceIndex >= 0);

      this.sourceIndex = sourceIndex;
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ParseException"/> class with the specified <paramref name="message"/>, 
    /// <see cref="SourceIndex"/> and <paramref name="innerException"/>.
    /// </summary>
    /// <param name="sourceIndex">The index in the sequence at which the error occurred while parsing.</param>
    /// <param name="message">The message that describes the parser error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null" /> if no inner exception is specified.</param>
    public ParseException(int sourceIndex, string message, Exception innerException)
      : base(message, innerException)
    {
      Contract.Requires(sourceIndex >= 0);

      this.sourceIndex = sourceIndex;
    }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
    /// <summary>
    /// Constructs a new instance of the <see cref="ParseException"/> class with serialized data.
    /// </summary>
    /// <param name="info">Holds the serialized data about the exception.</param>
    /// <param name="context">Contains contextual information about the source or destination.</param>
    protected ParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
      Contract.Requires(info != null);

      info.AddValue("sourceIndex", sourceIndex);
    }
#endif
    #endregion

    #region Methods
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(sourceIndex >= 0);
    }

#if !SILVERLIGHT && !PORT_45 && !PORT_40
    /// <summary>
    /// Populates serialization <paramref name="info"/> for the current exception instance.
    /// </summary>
    /// <param name="info">Holds the serialized data about the exception.</param>
    /// <param name="context">Contains contextual information about the source or destination.</param>
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      sourceIndex = info.GetInt32("sourceIndex");

      Contract.Assume(sourceIndex >= 0);

      base.GetObjectData(info, context);
    }
#endif
    #endregion
  }
}