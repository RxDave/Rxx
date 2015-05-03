namespace System.Reactive.Linq
{
  /// <summary>
  /// Indicates whether an observed value being projected into an <see cref="IObservable{T}"/> of 
  /// <see cref="Either{TLeft,TRight}"/> is a left value, right value, neither or both.
  /// </summary>
  public enum PairDirection
  {
    /// <summary>
    /// The value is excluded.
    /// </summary>
    Neither,

    /// <summary>
    /// The value is for the left channel.
    /// </summary>
    Left,

    /// <summary>
    /// The value is for the right channel.
    /// </summary>
    Right,

    /// <summary>
    /// This value is for both the left and right channels.
    /// </summary>
    Both
  }
}