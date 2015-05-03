namespace System
{
  public static partial class Either
  {
    /// <summary>
    /// Holds the value of the left side of an <see cref="Either{TLeft,TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left side.</typeparam>
    /// <typeparam name="TRight">Type of the right side.</typeparam>
#if !SILVERLIGHT && !PORT_45 && !PORT_40
    [Serializable]
#endif
    private sealed class LeftValue<TLeft, TRight> : Either<TLeft, TRight>
    {
      /// <summary>
      /// Gets whether the object holds the left value or the right value.
      /// </summary>
      /// <value>Always returns <see langword="true" />.</value>
      public override bool IsLeft
      {
        get
        {
          return true;
        }
      }

      /// <summary>
      /// Gets the left value when <see cref="IsLeft"/> is <see langword="true"/>.
      /// </summary>
      /// <value>Always returns the left value.</value>
      public override TLeft Left
      {
        get
        {
          return value;
        }
      }

      /// <summary>
      /// Gets the right value when <see cref="IsLeft"/> is <see langword="false"/>.
      /// </summary>
      /// <value>Always throws <strong>System.Diagnostics.Contracts.ContractException</strong>.</value> 
      public override TRight Right
      {
        get
        {
          // precondition is inherited by Code Contracts
          return default(TRight);
        }
      }

      private readonly TLeft value;

      /// <summary>
      /// Constructs a new instance of the <see cref="LeftValue{TLeft,TRight}" /> class.
      /// </summary>
      /// <param name="value">The left value.</param>
      public LeftValue(TLeft value)
      {
        this.value = value;
      }

      /// <summary>
      /// Invokes the specified <paramref name="left"/> action.
      /// </summary>
      /// <param name="left">The action to be invoked.</param>
      /// <param name="right">This parameter is ignored.</param>
      public override void Switch(Action<TLeft> left, Action<TRight> right)
      {
        left(value);
      }

      /// <summary>
      /// Invokes the specified <paramref name="left"/> function.
      /// </summary>
      /// <typeparam name="TResult">The type of the result.</typeparam>
      /// <param name="left">The function to be invoked.</param>
      /// <param name="right">This parameter is ignored.</param>
      /// <returns>The return value of the <paramref name="left"/> function</returns>
      public override TResult Switch<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right)
      {
        return left(value);
      }
    }
  }
}