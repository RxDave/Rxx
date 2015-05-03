using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace System.Reactive
{
  /// <summary>
  /// Represents a shared observable sequence that replays values from a current index that can be moved,
  /// along with support for optimized branching.
  /// </summary>
  /// <remarks>
  /// <alert type="implement">
  /// The contracts of <see cref="IObservableCursor{T}"/> rely on some of the properties being immutable.
  /// The <see cref="IsSynchronized"/> and <see cref="IsForwardOnly"/> properties must be immutable.  Furthermore, 
  /// the <see cref="IsSequenceTerminated"/> property must be <see langword="false"/> until the sequence terminates, 
  /// at which time it must return <see langword="true"/> and then hold that value indefinitely.
  /// </alert>
  /// </remarks>
  /// <typeparam name="T">The object that provides notification information.</typeparam>
  /// <threadsafety instance="false" />
  [ContractClass(typeof(IObservableCursorContract<>))]
#if !SILVERLIGHT
  public interface IObservableCursor<out T> : IConnectableObservable<T>, IDisposable
#else
  public interface IObservableCursor<T> : IConnectableObservable<T>, IDisposable
#endif
  {
    /// <summary>
    /// Gets a value indicating whether the cursor's methods and properties can be used concurrently by multiple threads.
    /// </summary>
    /// <remarks>
    /// <alert type="implement">
    /// <see cref="IsSynchronized"/> must be immutable.  Contracts of <see cref="IObservableCursor{T}"/> members depend upon it.
    /// </alert>
    /// </remarks>
    /// <value><see langword="True"/> if the cursor is safe for multi-threaded operations; otherwise, <see langword="false"/>.</value>
    bool IsSynchronized
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether the cursor only moves forward.
    /// </summary>
    /// <remarks>
    /// <alert type="implement">
    /// <see cref="IsForwardOnly"/> must be immutable.  Contracts of <see cref="IObservableCursor{T}"/> members depend upon it.
    /// </alert>
    /// </remarks>
    /// <value><see langword="True"/> if the cursor only moves forward; otherwise, <see langword="false"/>.</value>
    /// <seealso cref="Move"/>
    bool IsForwardOnly
    {
      get;
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
    /// indicates that the cursor is at the end of the sequence.  Calling <see cref="IObservable{T}.Subscribe"/> on a cursor that is 
    /// positioned at the end of the sequence causes only the termination notification to be pushed, without replaying any values.
    /// </para>
    /// <para>
    /// If <see cref="CurrentIndex"/> is moved past the latest element in the sequence, as indicated by the value of 
    /// <see cref="LatestIndex"/>, then the existing values in the sequence will not be replayed to new subscriptions; 
    /// furthermore, any new values with indices that are less than <see cref="CurrentIndex"/> will also be excluded from
    /// new subscriptions.
    /// </para>
    /// <alert type="implement">
    /// If the current index is positioned ahead of the latest element in the sequence and the sequence 
    /// subsequently ends, then the current index must automatically be changed to <see cref="LatestIndex"/> + 1 to indicate 
    /// that the current index is at the end of the sequence.  Consumers can check whether <see cref="CurrentIndex"/> has changed 
    /// during the <strong>OnCompleted</strong> notification.
    /// </alert>
    /// </remarks>
    /// <value>The zero-based index of the element in the sequence on which the cursor is positioned after 
    /// <see cref="Move"/> is called; otherwise, -1.</value>
    int CurrentIndex
    {
      get;
    }

    /// <summary>
    /// Gets the zero-based index of the last known element in the sequence.
    /// </summary>
    /// <value>The zero-based index of the last known element in the sequence; -1 until the sequence produces an element, if any.
    /// If the sequence has terminated, then the value is the index of the last element in the sequence.</value>
    int LatestIndex
    {
      get;
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
    bool AtEndOfSequence
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether the sequence has terminated.
    /// </summary>
    /// <remarks>
    /// <alert type="note">
    /// When <see cref="IsSequenceTerminated"/> returns <see langword="true"/> it does not indicate that the cursor is at the
    /// end of the sequence.  It only indicates that the sequence has terminated, regardless of whether the cursor is actually 
    /// positioned at the end of the sequence or not.
    /// </alert>
    /// <alert type="implement">
    /// <see cref="IsSequenceTerminated"/> must be immutable when it returns <see langword="true"/>.  Contracts of 
    /// <see cref="IObservableCursor{T}"/> members depend upon it.
    /// </alert>
    /// </remarks>
    /// <value><see langword="True"/> if the sequence has terminated; otherwise, <see langword="false"/>.</value>
    /// <seealso cref="AtEndOfSequence"/>
    bool IsSequenceTerminated
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether the cursor is disposed.
    /// </summary>
    /// <value><see langword="True"/> if the cursor is disposed; otherwise, <see langword="false"/>.</value>
    bool IsDisposed
    {
      get;
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
    /// <alert type="implement">
    /// If the specified <paramref name="count"/> is greater than the length from the current element to the latest
    /// element in the sequence, then the cursor must virtualize itself by skipping subsequent elements until the position 
    /// of the cursor is reached.
    /// </alert>
    /// </remarks>
    /// <seealso cref="IsForwardOnly"/>
    void Move(int count);

    /// <summary>
    /// Returns a new cursor that starts at the current position of this cursor and is tied to the lifetime of this cursor.
    /// </summary>
    /// <remarks>
    /// <alert type="implement">
    /// <see cref="Branch"/> provides a means for cursor authors to optimize branches so that they can share the same state.
    /// </alert>
    /// </remarks>
    /// <returns>A new cursor that starts at the current position of this cursor.</returns>
    IObservableCursor<T> Branch();

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
    IDisposable Subscribe(IObserver<T> observer, int count);
  }

  [ContractClassFor(typeof(IObservableCursor<>))]
  internal abstract class IObservableCursorContract<T> : IObservableCursor<T>
  {
    public bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    public bool IsForwardOnly
    {
      get
      {
        return false;
      }
    }

    public int CurrentIndex
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() >= 0);
        Contract.Ensures(IsSynchronized || !IsSequenceTerminated || Contract.Result<int>() <= LatestIndex + 1);
        return 0;
      }
    }

    public int LatestIndex
    {
      get
      {
        Contract.Ensures(Contract.Result<int>() >= -1);
        return 0;
      }
    }

    public bool AtEndOfSequence
    {
      get
      {
        Contract.Ensures(!Contract.Result<bool>() || IsSequenceTerminated);
        Contract.Ensures(IsSynchronized || Contract.Result<bool>() == (IsSequenceTerminated && CurrentIndex == LatestIndex + 1));
        return false;
      }
    }

    public bool IsSequenceTerminated
    {
      get
      {
        return false;
      }
    }

    public bool IsDisposed
    {
      get
      {
        return false;
      }
    }

    public void Move(int count)
    {
      Contract.Requires(!IsForwardOnly || count >= 0);
      Contract.Requires(IsSynchronized || !AtEndOfSequence || count <= 0);
      Contract.Requires(IsSynchronized || CurrentIndex + count >= 0);
      Contract.Requires(IsSynchronized || !IsSequenceTerminated || CurrentIndex + count <= LatestIndex + 1);
      Contract.Ensures(IsSynchronized == Contract.OldValue(IsSynchronized));
      Contract.Ensures(IsForwardOnly == Contract.OldValue(IsForwardOnly));
      Contract.Ensures(IsSynchronized || IsSequenceTerminated == Contract.OldValue(IsSequenceTerminated));
      Contract.Ensures(IsSynchronized || LatestIndex == Contract.OldValue(LatestIndex));
      Contract.Ensures(IsSynchronized || CurrentIndex == Contract.OldValue(CurrentIndex) + count);
    }

    public IObservableCursor<T> Branch()
    {
      Contract.Ensures(Contract.Result<IObservableCursor<T>>() != null);
      Contract.Ensures(IsSynchronized == Contract.OldValue(IsSynchronized));
      Contract.Ensures(IsForwardOnly == Contract.OldValue(IsForwardOnly));
      Contract.Ensures(IsSynchronized || IsSequenceTerminated == Contract.OldValue(IsSequenceTerminated));
      Contract.Ensures(IsSynchronized || LatestIndex == Contract.OldValue(LatestIndex));
      Contract.Ensures(IsSynchronized || CurrentIndex == Contract.OldValue(CurrentIndex));
      Contract.Ensures(IsSynchronized || AtEndOfSequence == Contract.OldValue(AtEndOfSequence));
      Contract.Ensures(Contract.Result<IObservableCursor<T>>().IsSynchronized == IsSynchronized);
      Contract.Ensures(Contract.Result<IObservableCursor<T>>().IsForwardOnly == IsForwardOnly);
      Contract.Ensures(IsSynchronized || Contract.Result<IObservableCursor<T>>().IsSequenceTerminated == IsSequenceTerminated);
      Contract.Ensures(IsSynchronized || Contract.Result<IObservableCursor<T>>().LatestIndex == LatestIndex);
      Contract.Ensures(IsSynchronized || Contract.Result<IObservableCursor<T>>().CurrentIndex == CurrentIndex);
      Contract.Ensures(IsSynchronized || Contract.Result<IObservableCursor<T>>().AtEndOfSequence == AtEndOfSequence);
      return null;
    }

    public IDisposable Connect()
    {
      return null;
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
      return null;
    }

    public IDisposable Subscribe(IObserver<T> observer, int count)
    {
      Contract.Requires(observer != null);
      Contract.Requires(count >= 0);
      Contract.Ensures(IsSynchronized == Contract.OldValue(IsSynchronized));
      Contract.Ensures(IsForwardOnly == Contract.OldValue(IsForwardOnly));
      Contract.Ensures(Contract.Result<IDisposable>() != null);
      return null;
    }

    public void Dispose()
    {
    }
  }
}