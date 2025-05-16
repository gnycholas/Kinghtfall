using System.Threading.Tasks;
using UnityEngine;

public interface IInteract
{
    public Transform GetTarget();
    public Task Execute();

    public AnimatorOverrideController GetInteraction();
}
