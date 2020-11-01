using UnityEngine;

namespace MalbersAnimations
{
    public abstract class IDs : ScriptableObject
    {
        [Tooltip("ID Value for the Animator transitions in order to Execute the wanted animation clip")]
        public int ID;

        public static implicit operator int(IDs reference)
        {
            if (reference == null)
            {
                return 0;
            }
            else
            {
                return reference.ID;
            }
        }
    }
}