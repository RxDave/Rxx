namespace System
{
  public static partial class Either
  {
    /// <summary>
    /// Holds the value of the right side of an <see cref="Either{TLeft,TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left side.</typeparam>
    /// <typeparam name="TRight">Type of the right side.</typeparam>
#if !SILVERLIGHT && !PORT_45 && !PORT_40
    [Serializable]
#endif
    private sealed class RightValue<TLeft, TRight> : Either<TLeft, TRight>
    {
      /// <summary>
      /// Gets whether the object holds the left value or the right value.
      /// </summary>
      /// <value>Always returns <see langword="false" />.</value>
      public override bool IsLeft
      {
        get
        {
          return false;
        }
      }

      /// <summary>
      /// Gets the left value when <see cref="IsLeft"/> is <see langword="true"/>.
      /// </summary>
      /// <value>Always throws <strong>System.Diagnostics.Contracts.ContractException</strong>.</value>
      public override TLeft Left
      {
        get
        {
          // precondition is inherited by Code Contracts
          return default(TLeft);
        }
      }

      /// <summary>
      /// Gets the right value when <see cref="IsLeft"/> is <see langword="false"/>.
      /// </summary>
      /// <value>Always returns the left value.</value>
      public override TRight Right
      {
        get
        {
          return value;
        }
      }

      private readonly TRight value;

      /// <summary>
      /// Constructs a new instance of the <see cref="RightValue{TLeft,TRight}" /> class.
      /// </summary>
      /// <param name="value">The right value.</param>
      public RightValue(TRight value)
      {
        this.value = value;
      }

      /// <summary>
      /// Invokes the specified <paramref name="right"/> action.
      /// </summary>
      /// <param name="left">This parameter is ignored.</param>
      /// <param name="right">The action to be invoked.</param>
      public override void Switch(Action<TLeft> left, Action<TRight> right)
      {
        right(value);
      }

      /// <summary>
      /// Invokes the specified <paramref name="right"/> function.
      /// </summary>
      /// <typeparam name="TResult">The type of the result.</typeparam>
      /// <param name="left">This parameter is ignored.</param>
      /// <param name="right">The function to be invoked.</param>
      /// <returns>The return value of the <paramref name="right"/> function.</returns>
      public override TResult Switch<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right)
      {
        return right(value);
      }
    }
  }
}