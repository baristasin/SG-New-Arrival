using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public abstract class ZombieBaseModule : MonoBehaviour
    {
        public ZombieController ZombieController {get; private set;}
        public virtual void Initialize(ZombieController zombieController)
        {
            ZombieController = zombieController;
        }
    }
}