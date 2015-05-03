using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace System.Reactive.Subjects
{
  /// <summary>
  /// Represents an observer and a replaying observable sequence with a current index that can be moved.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A cursor is a moveable pointer over an immutable sequence.  This differs from the concept of a <see cref="System.IO.Stream"/>, 
  /// which is a moveable pointer over a mutable buffer.  Mutability is a key difference; it means that moving the position of a
  /// stream backward allows you to overwrite the data from that position to the end of the stream, while moving the position of a 
  /// cursor backward only changes the index in the sequence from which data is replayed to new subscribers.  In other words, data 
  /// written to a stream is always inserted at the current position, but data observed by a cursor is always appended to the end 
  /// of the buffered sequence.
  /// </para>
  /// <para>
  /// Furthermore, the position of a stream is shared between reads and writes, but the position of a cursor is not.  A cursor's 
  /// position is only used by reads (subscribers) and is entirely independent of writes.  Subscribing to a cursor replays the data 
  /// in the sequence from the current index to the end; however, the current index is not moved at all.  Changing the index later
  /// has no effect on existing subscriptions, but changing the position of a stream would certainly effect existing readers.
  /// </para>
  /// <para>
  /// <see cref="CursorSubject{T}"/> provides a superset of the functionality that is provided by <see cref="Subject{T}"/>, 
  /// <see cref="BehaviorSubject{T}"/> and <see cref="ReplaySubject{T}"/>.  Each of these subjects could be implemented in terms
  /// of <see cref="CursorSubject{T}"/> by moving the <see cref="CurrentIndex"/> after calling <see cref="OnNext"/>, as necessary.
  /// </para>
  /// </remarks>
  /// <typeparam name="T">The object that provides notification information.</typeparam>
  public sealed class CursorSubject<T> : ISubject<T>, IObservableCursor<T>
  {
    #region Public Properties
    /// <summary>
    /// Gets a value indicating whether the cursor's methods and properties can be used concurrently by multiple threads.
    /// </summary>
    /// <value>Always returns <see langword="true"/>.</value>
    public bool IsSynchronized
    {
      get
      {
        Contract.Ensures(Contract.Result<bool>());

        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the cursor only moves forward.
    /// </summary>
    /// <value><see langword="True"/> if the cursor only moves forward; otherwise, <see langword="false"/>.</value>
    /// <seealso cref="Move"/>
    public bool IsForwardOnly
    {
      get
      {
        lock (gate)
        {
          return cursor.IsForwardOnly;
        }
      }
    }

    /// <summary>
    /// Gets the zero-based index of the element in the sequence at which the cursor is positioned after 
    /// <see cref="Move"/> is called.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="CurrentIndex"/> starts and remains at 0 until it is changed by the <see cref="Move"/> method.  A value of 0 indicates
    /// that the cursor is positioned at the end of an empty sequence.  If the sequence generates one or more values, then 0 indicates
    /// that the cursor is positioned at the beginning of the sequence.  Subscribing to a cursor that has not moved replays all values in 
    /// the sequence, if any.
    /// </para>
    /// <para>
    /// The valid range of values for <see cref="CurrentIndex"/> changes depending upon whether the sequence has terminated.
    /// If the sequence has not terminated, then <see cref="CurrentIndex"/> can be any value that is greater than or equal to zero, 
    /// even if that value is greater than <see cref="LatestIndex"/>; however, once the sequence has terminated, 
    /// <see cref="CurrentIndex"/> cannot be moved past one more than the <see cref="LatestIndex"/>.  This final position 
    /// indicates that the cursor is at the end of the sequence.  Calling <strog>Subscribe</strog> on a cursor that is 
    /// positioned at the end of the sequence causes only the termination notification to be pushed, without replaying any values.
    /// </para>
    /// <para>
    /// If <see cref="CurrentIndex"/> is moved past the latest element in the sequence, as indicated by the value of 
    /// <see cref="LatestIndex"/>, then the existing values in the sequence will not be replayed to new subscriptions; 
    /// furthermore, any new values with indices that are less than <see cref="CurrentIndex"/> will also be excluded from
    /// new subscriptions.
    /// </para>
    /// <para>
    /// If the current index is positioned ahead of the latest element in the sequence and the sequence 
    /// subsequently ends, then the current index is automatically changed to <see cref="LatestIndex"/> + 1 to indicate 
    /// that the current index is at the end of the sequence.  Consumers can check whether <see cref="CurrentIndex"/> has changed 
    /// during the <strong>OnCompleted</strong> notification.
    /// </para>
    /// </remarks>
    /// <value>The zero-based index of the element in the sequence on which the cursor is positioned after 
    /// <see cref="Move"/> is called; otherwise, -1.</value>
    public int CurrentIndex
    {
      get
      {
        lock (gate)
        {
          return cursor.CurrentIndex;
        }
      }
    }

    /// <summary>
    /// Gets the zero-based index of the last known element in the sequence.
    /// </summary>
    /// <value>The zero-based index of the last known element in the sequence; -1 until the sequence produces an element, if any.
    /// If the sequence has terminated, then the value is the index of the last element in the sequence.</value>
    public int LatestIndex
    {
      get
      {
        lock (gate)
        {
          return cursor.LatestIndex;
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the cursor is positioned at the end of the sequence.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// When <see cref="AtEndOfSequence"/> returns <see langword="false"/> it does not indicate that the sequence has 
    /// not terminated.  It only indicates that the cursor is not currently positioned at the end of the sequence,
    /// regardless of whether the sequence has actually terminated or not.
    /// </alert>
    /// </remarks>
    /// <value><see langword="True"/> if the sequence has terminated and the cursor is currently positioned at the end of the sequence;
    /// otherwise, <see langword="false"/>.</value>
    /// <seealso cref="IsSequenceTerminated"/>
    public bool AtEndOfSequence
    {
      get
      {
        bool atEnd;

        lock (gate)
        {
          atEnd = cursor.AtEndOfSequence;
        }

        Contract.Assume(!atEnd || IsSequenceTerminated);

        return atEnd;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the sequence has terminated.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// When <see cref="IsSequenceTerminated"/> returns <see langword="true"/> it does not indicate that the cursor is at the
    /// end of the sequence.  It only indicates that the sequence has terminated, regardless of whether the 
    /// cursor is actually positioned at the end of the sequence or not.
    /// </alert>
    /// </remarks>
    /// <value><see langword="True"/> if the sequence has terminated; otherwise, <see langword="false"/>.</value>
    /// <seealso cref="AtEndOfSequence"/>
    public bool IsSequenceTerminated
    {
      get
      {
        lock (gate)
        {
          return cursor.IsSequenceTerminated;
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the subject is disposed.
    /// </summary>
    /// <value><see langword="True"/> if the subject is disposed; otherwise, <see langword="false"/>.</value>
    public bool IsDisposed
    {
      get
      {
        lock (gate)
        {
          return cursor.IsDisposed;
        }
      }
    }
    #endregion

    #region Private / Protected
    private readonly object gate = new object();
    private readonly ObservableCursor<T> cursor;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="CursorSubject{T}" /> class with bidirectional behavior.
    /// </summary>
    public CursorSubject()
    {
      cursor = new ObservableCursor<T>(isForwardOnly: false, enableBranchOptimizations: false);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="CursorSubject{T}" /> class.
    /// </summary>
    /// <param name="isForwardOnly"><see langword="True"/> if the cursor can only be moved forward; 
    /// otherwise, <see langword="false"/>.</param>
    public CursorSubject(bool isForwardOnly)
    {
      cursor = new ObservableCursor<T>(isForwardOnly, enableBranchOptimizations: isForwardOnly);
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="CursorSubject{T}" /> class.
    /// </summary>
    /// <param name="isForwardOnly"><see langword="True"/> if the cursor can only be moved forward; 
    /// otherwise, <see langword="false"/>.</param>
    /// <param name="enableBranchOptimizations">Specifies whether a forward-only cursor is allowed to truncate the buffered 
    /// sequence, if necessary, whenever a branch is moved.  In the future, it may control other kinds of branch optimizations
    /// as well.  The default value is <see langword="true"/>.</param>
    public CursorSubject(bool isForwardOnly, bool enableBranchOptimizations)
    {
      Contract.Requires(isForwardOnly || !enableBranchOptimizations);

      cursor = new ObservableCursor<T>(isForwardOnly, enableBranchOptimizations);
    }
    #endregion

    #region Methods
    [ContractInvariantMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
    private void ObjectInvariant()
    {
      Contract.Invariant(gate != null);
      Contract.Invariant(cursor != null);
    }

    /// <summary>
    /// Notifies the cursor that an observer is to receive notifications.
    /// </summary>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>The observer's interface that enables resources to be disposed.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public IDisposable Subscribe(IObserver<T> observer)
    {
      lock (gate)
      {
        var subscription = cursor.Subscribe(observer);

        return Disposable.Create(() =>
        {
          lock (gate)
          {
            subscription.Dispose();
          }
        });
      }
    }

    /// <summary>
    /// Notifies the provider that an observer is to receive the specified maximum number of <strong>OnNext</strong> notifications.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This overload to <see cref="IObservable{T}.Subscribe(IObserver{T})"/> behaves similarly except that it has a 
    /// <paramref name="count"/> parameter that specifies the maximum number of elements that may be pushed to the
    /// <paramref name="observer"/>.  Essentially, it provides an optimized alternative to <see cref="Observable.Take{T}(IObservable{T},int)"/>.
    /// </para>
    /// <para>
    /// The <paramref name="count"/> parameter applies to cursors because they typically will buffer data from the source 
    /// sequence and replay it to subscribers, starting from the current index and continuing to the latest value that has 
    /// been buffered.  When a subscriber only wants to view a range of data, then <see cref="Observable.Take{T}(IObservable{T},int)"/> is often 
    /// added to the query to specify the number of notifications that are desired; however, the <see cref="Observable.Take{T}(IObservable{T},int)"/> 
    /// operator cannot cancel the notifications that are being replayed from the cursor, so it simply drops any additional 
    /// notifications that exceed the specified limit.  The specified <paramref name="observer"/> will not observe the 
    /// additional notifications, although the overhead of replaying an entire buffered sequence to the 
    /// <see cref="Observable.Take{T}(IObservable{T},int)"/> operator could have a noticeable impact on the performance of your code.  By implementing
    /// <see cref="Subscribe(IObserver{T},int)"/>, replayed notifications can be stopped when the specified <paramref name="count"/>
    /// is reached.
    /// </para>
    /// </remarks>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <param name="count">The maximum number of elements to be observed.</param>
    /// <returns>The observer's interface that enables resources to be disposed.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "IsForwardOnly is immutable.")]
    public IDisposable Subscribe(IObserver<T> observer, int count)
    {
      lock (gate)
      {
        var subscription = cursor.Subscribe(observer, count);

        return Disposable.Create(() =>
        {
          lock (gate)
          {
            subscription.Dispose();
          }
        });
      }
    }

    /// <summary>
    /// Not supported.
    /// </summary>
    /// <returns>Throws <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">This method is not supported.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "The disposable is returned to the caller.")]
    public IDisposable Connect()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Provides the cursor with new data.
    /// </summary>
    /// <param name="value">The current notification information.</param>
    public void OnNext(T value)
    {
      lock (gate)
      {
        cursor.OnNext(value);
      }
    }

    /// <summary>
    /// Notifies the cursor that the provider has experienced an error condition.
    /// </summary>
    /// <param name="error">An object that provides additional information about the error.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#",
      Justification = "Windows Phone defines IObserver<T>.OnError with the name \"exception\" for the \"error\" parameter, but it's better to" +
                      "leave it as \"error\" here so that callers can use the same named parameter across all platforms.")]
    public void OnError(Exception error)
    {
      lock (gate)
      {
        cursor.OnError(error);
      }
    }

    /// <summary>
    /// Notifies the cursor that the provider has finished sending push-based notifications.
    /// </summary>
    public void OnCompleted()
    {
      lock (gate)
      {
        cursor.OnCompleted();
      }
    }

    /// <summary>
    /// Changes the current index of the cursor to the element at the specified number of elements forward or backward.
    /// </summary>
    /// <param name="count">The number of elements to move after the current index if the specified count is positive
    /// or before the current index if the specified count is negative.</param>
    /// <remarks>
    /// <para>
    /// The specified <paramref name="count"/> may be a negative number only if <see cref="IsForwardOnly"/> is <see langword="false"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="IsForwardOnly"/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
      Justification = "Inherited code contracts.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Requires",
      Justification = "Allow precondition failures at runtime.  They cannot be checked up front due to multi-threading race conditions.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "IsForwardOnly is immutable.")]
    public void Move(int count)
    {
      lock (gate)
      {
        cursor.Move(count);
      }
    }

    /// <summary>
    /// Returns a new cursor that starts at the current position of this cursor and is tied to the lifetime of this cursor.
    /// </summary>
    /// <remarks>
    /// <alert type="implement">
    /// <see cref="Branch"/> provides a means for cursor authors to optimize branches so that they can share the same state.
    /// </alert>
    /// </remarks>
    /// <returns>A new cursor that starts at the current position of this cursor.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Contracts", "Ensures",
      Justification = "IsForwardOnly is immutable.")]
    public IObservableCursor<T> Branch()
    {
      lock (gate)
      {
        return cursor.Branch().Synchronize(gate);
      }
    }

    /// <summary>
    /// Returns a string representation of the cursor.
    /// </summary>
    /// <returns>A string representation of the cursor.</returns>
    public override string ToString()
    {
      lock (gate)
      {
        return cursor.ToString();
      }
    }

    /// <summary>
    /// Permanently releases all observers and buffered elements.
    /// </summary>
    public void Dispose()
    {
      lock (gate)
      {
        cursor.Dispose();
      }
    }
    #endregion
  }
}