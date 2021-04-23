using System.Linq;
using UnityEngine;

namespace Utils.MonoBehaviourMethodExtensions
{
    public class LateStartCaller : MonoBehaviour
    {
        private void Start()
        {
            foreach (var obj in FindObjectsOfType<MonoBehaviour>().OfType<ILateStart>())
            {
                obj.LateStart();
            }
        }
    }
}