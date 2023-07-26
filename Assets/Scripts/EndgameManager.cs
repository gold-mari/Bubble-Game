using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class EndgameManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The scene we transition to on a loss- reaching max danger.")]
    [SerializeField] [Scene]
    private int sceneOnLose;
    [Tooltip("The scene we transition to on a win- reaching the end of the song.")]
    [SerializeField] [Scene]
    private int sceneOnWin;

    // ...

    // ================================================================
    // Default methods
    // ================================================================

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ================================================================
    // State-changing methods
    // ================================================================

    public void TriggerWin()
    {
        print("You win!");
        SceneManager.LoadScene(sceneOnWin);
    }

    public void TriggerLoss()
    {
        print("You lose!");
        SceneManager.LoadScene(sceneOnLose);
    }
}
