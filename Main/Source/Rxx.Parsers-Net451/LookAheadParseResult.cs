using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reactive.Subjects;

namespace Rxx.Parsers
{
  [DebuggerDisplay("Length = {Length}, Value = {Value}, Look-Ahead Succeeded = {Succeeded}")]
  internal sealed class LookAheadParseResult<TValue> : ParseResult<TValue>, ILookAheadParseResult<TValue>
  {
    #region Public Properties
    public bool? Succeeded
    {
      get
      {
        return succeeded;
      }
    }
    #endregion

    #region Private / Protected
    private readonly AsyncSubject<bool> subject = new AsyncSubject<bool>();
    private bool? succeeded;
    #endregion

    #region Constructors
    public LookAheadParseResult(TValue value, int length)
      : base(value, length)
    {
      Contract.Requires(length >= 0);
    }

    public LookAheadParseResult(int length)
      : base(length)
    {
      Contract.Requires(length >= 0);
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(subject != null);
    }

    public IDisposable Subscribe(IObserver<bool> observer)
    {
      return subject.Subscribe(observer);
    }

    public void OnCompleted(bool success)
    {
      if (succeeded.HasValue)
      {
        throw new InvalidOperationException();
      }

      succeeded = success;

      subject.OnNext(success);
      subject.OnCompleted();
    }

    public void Dispose()
    {
      subject.Dispose();
    }
    #endregion
  }
}