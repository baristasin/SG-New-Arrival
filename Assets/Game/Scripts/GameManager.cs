using Game.Scripts.Utilities;
using UnityEngine;

namespace Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown("g"))
            {
                ZombieState.IsDancing = !ZombieState.IsDancing;
            }
        }
    }
}