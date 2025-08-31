using UnityEngine;

public class SIXLevelSelect : MonoBehaviour
{
    public MenuTree tree;

    // Update is called once per frame
    void Update()
    {
        if (InputHandler.GetDEBUGDown()) {
            tree.WarpToName("Level Select");
        }
    }
}
