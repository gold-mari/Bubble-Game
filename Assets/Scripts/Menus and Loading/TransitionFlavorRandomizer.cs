using UnityEngine;

public class TransitionFlavorRandomizer : MonoBehaviour
{
    [SerializeField, Tooltip("The bubble flavor displayed in our animation.")]
    private Bubble_FlavorVar transitionAnimFlavorVar;

    public void Randomize()
    {
        transitionAnimFlavorVar.value = Bubble_Flavor_Methods.random();
    }
}