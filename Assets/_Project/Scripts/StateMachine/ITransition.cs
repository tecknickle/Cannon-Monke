using UnityEditorInternal;

namespace CannonMonke
{
    public interface ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }
    }
}
