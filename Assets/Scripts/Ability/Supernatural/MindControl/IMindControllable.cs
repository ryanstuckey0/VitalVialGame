using UnityEngine;

namespace ViralVial.Ability.Supernatural.MindControl
{
    public interface IMindControllable
    {
        GameObject Target { get; set; }
        void ForceDeath();
        void StartMindControl();
        void StopMindControl();
        float Health { get; set; }
        float Damage { get; set; }
    }
}
