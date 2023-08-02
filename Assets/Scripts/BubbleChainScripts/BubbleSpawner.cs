using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class BubbleSpawner : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [Tooltip("The bubble prefab to spawn.\n\nIMPORTANT: This bubble should be initialized with the "
           + "NIL chain.")]
    public GameObject bubble;
    [Tooltip("The DangerManager object present in the scene. This is passed off to the "
           + "DangerTracker components in spawned bubbles.")]
    [SerializeField]
    private DangerManager dangerManager;
    [Tooltip("The TimekeeperManager object present in the scene. This is used to time bubble "
           + "spawns, among other things.")]
    [SerializeField]
    private TimekeeperManager timekeeperManager;
    [Expandable]
    [Tooltip("The list of current and upcoming Bubble_Colors to spawn, represented by a list of "
           + "bubble_ColorVars. The 0th index is the current color, and each successive index is "
           + "the color after that.\n\nThis array may be ANY size.")]
    public List<bubble_ColorVar> colors;
    [Tooltip("The center of the playable space.\n\nDefault: (0,0)")]
    public Vector2 center = new Vector2(0,0);

    [Tooltip("The radii (inner, outer) at which to spawn bubbles. When gravity isn't "
           + "inverted, the outer radius is used. When gravity IS inverted, the inner "
           + "radius is used.\n\nDefault: (1,4.4)")]
    public Vector2 radius = new Vector2(1f, 4.4f);
    [Tooltip("The number of bubbles to spawn in mass rounds.\n\nDefault: 20")]
    public uint massRoundSize = 20;
    [Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    public boolVar gravityFlipped;
    [Tooltip("A UnityEvent which communicates with GravityManager, telling it to flip gravity.\n\n"
           + "IMPORTANT: THIS WILL BE OVERHAULED TO BE ON A TIMEKEEPER MANAGER.")]
    public UnityEvent flipGravity;
    // A System.Action version for use with instantiated bubbles.
    public System.Action flipGravityAction;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The internal age variable that is passed along to bubbles. When a bubble is
    // spawned, we increment the age here.
    private uint age = 1;
    // The number of beats that have elapsed in this cycle.
    private uint beatCount = 1;
    // The song being played from TimekeeperManager.
    private songObject song;
    // Indices for the beat lists in song.
    private int spawnIndex = 0, flipIndex = 0, massIndex = 0;

    // ==============================================================
    // Default methods
    // ==============================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to initialize the
        // state of bubbles, and to subscribe to the timekeeper.
        // ================

        RandomizeColors();
        MassSpawnBubble(massRoundSize);

        timekeeperManager.beatUpdated += Spawn;
        song = timekeeperManager.song;
    }

    void OnDestroy()
    {
        // We use OnDestroy to unsubscribe from the timekeeper.
        // ================

        timekeeperManager.beatUpdated -= Spawn;
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        // Prints beatCount to an onscreen GUI box.
        // ================

        GUILayout.Box($"Current beatCount = {beatCount}");
    }
#endif

    // ==============================================================
    // Instantiation/Destruction Methods
    // ==============================================================

    void Spawn()
    {
        // Tracks incoming beatEvents, spawning bubbles, flipping gravity, and spawning
        // mass bubbles as dictated by song. Updates colors after spawning a bubble.
        // ================

        // Everything runs on a shared clock.
        if ( beatCount >= song.loopLength ) {
            beatCount = 1;
            spawnIndex = flipIndex = massIndex = 0;
        }
        else {
            beatCount++;
        }

        print($"spawn: {spawnIndex} | flip: {flipIndex} | mass: {massIndex}");

        if (spawnIndex < song.spawnBeats.Count && beatCount == song.spawnBeats[spawnIndex]) {
            CursorSpawnBubble(colors[0].value);
            UpdateColors();
            spawnIndex++;
        }

        if (flipIndex < song.flipBeats.Count && beatCount == song.flipBeats[flipIndex]) {
            flipGravity.Invoke();
            flipGravityAction.Invoke();
            flipIndex++;
        }

        if (massIndex < song.massBeats.Count && beatCount == song.massBeats[massIndex]) {
            MassSpawnBubble(massRoundSize);
            massIndex++;
        }
    }

    private void MassSpawnBubble(uint number)
    {
        // Spawns number Bubbles at an orbit around the screen at regular intervals, all
        // at once. The radius at which to spawn the bubble is determined in SpawnBubble
        // depending on if gravity is flipped.
        // ================

        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/BubbleSpawn");
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //

        // Used to generate Bubble_Colors non-repetitiously.
        Bubble_Color currentColor;
        Bubble_Color lastColor = Bubble_Color.NONE;
        Bubble_Color colorBeforeThat = Bubble_Color.NONE;

        for (int i = 0; i < number; i++) {
            // Each spawnPoint should be a unit vector, equally spaced out depending on
            // the size of i.
            Quaternion rotation = Quaternion.Euler(0,0,(360*i/number));
            Vector2 spawnPoint = rotation * Vector2.up;

            // If the Color we generated was one of the last two Colors we generated,
            // regenerate it.
            do {
                currentColor = Bubble_Color_Methods.random();
            } while ( currentColor == lastColor || currentColor == colorBeforeThat );

            SpawnBubble(spawnPoint, currentColor);

            // After spawning, pass back the Colors we've seen.
            colorBeforeThat = lastColor;
            lastColor = currentColor;
        }
    }

    private void CursorSpawnBubble(Bubble_Color color)
    {
        // Spawns a bubble at the mouse orbital position. The radius at which to spawn
        // the bubble is determined in SpawnBubble depending on if gravity is flipped.
        // ================

        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/BubbleSpawn");
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //

        // Get the mouse position on the screen.
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // The spawn point is the vector from the center to the mouse position, normalized and then multiplied by the radius.
        Vector2 spawnPoint = (mousePosition - center).normalized;
        
        SpawnBubble(spawnPoint, color);
    }

    private void SpawnBubble(Vector2 spawnPoint, Bubble_Color color)
    {
        // Spawns a Bubble at spawnPoint and initializes its Bubble_Color, age, and
        // sprite color.
        // ================

        // If gravity isn't flipped, multiply by outer radius.
        if (!gravityFlipped.value) {
            spawnPoint *= radius.y;
        }
        // Otherwise, multiply by inner radius.
        else {
            spawnPoint *= radius.x;
        }

        GameObject obj = Instantiate(bubble, spawnPoint, Quaternion.identity, transform);
        Bubble objBubble = obj.GetComponent<Bubble>();

        // Initialize color and age.
        objBubble.bubbleColor = color;
        objBubble.age = age;
        // Increment age after we spawn it.
        age++;

        // Intialize dangerManager reference in dangerTracker.
        obj.GetComponent<DangerTracker>().dangerManager = dangerManager;
    }

    // ==============================================================
    // Data-manipulation methods
    // ==============================================================

    private void RandomizeColors()
    {
        // Updates the Bubble_Colors in the array colors. Each color is randomized.
        // ================
        
        for (int i = 0; i < colors.Count; i++)
        {
            // Generate colors.
            colors[i].value = Bubble_Color_Methods.random();
        }
    }

    private void UpdateColors()
    {
        // Updates the Bubble_Colors in the array colors. For all i > 0, the Bubble_Color
        // at colors[i] is passed to colors[i-1]. The color at colors[Count-1] is then
        // randomly regenerated.
        // ================
        
        for (int i = 1; i < colors.Count; i++)
        {
            // Pass the colors backwards.
            colors[i-1].value = colors[i].value;
        }

        // Regenerate the final color.
        colors[colors.Count-1].value = Bubble_Color_Methods.random();  
    }
}
