using System.Diagnostics.Contracts;

namespace System.ComponentModel
{
  [ContractClass(typeof(MemberDescriptorContract))]
  internal abstract class MemberDescriptor
  {
    public abstract string Name
    {
      get;
    }
  }

  [ContractClassFor(typeof(MemberDescriptor))]
  internal abstract class MemberDescriptorContract : MemberDescriptor
  {
    public override string Name
    {
      get
      {
        Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
        return null;
      }
    }
  }
}