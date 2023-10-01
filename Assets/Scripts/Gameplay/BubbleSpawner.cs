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

    [Tooltip("The beat reader object in the scene.")]
    public BeatReader beatReader;
    [SerializeField, Tooltip("The DangerManager object present in the scene. This is passed off to the "
                           + "DangerTracker components in spawned bubbles.")]
    private DangerManager dangerManager;
    [SerializeField, Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    private boolVar gravityFlipped;
    [SerializeField, Tooltip("The bubble prefab to spawn.\n\nIMPORTANT: This bubble should be initialized with the "
           + "NIL chain.")]
    private GameObject bubble;
    [SerializeField, Expandable, Tooltip("The list of current and upcoming Bubble_Colors to spawn, represented "
                                       + "by a list of bubble_ColorVars. The 0th index is the current color, and each "
                                       + "successive index is the color after that.\n\nThis array may be ANY size.")]
    private List<bubble_ColorVar> colors;
    [Tooltip("The center of the playable space.\n\nDefault: (0,0)")]
    public Vector2 center = new Vector2(0,0);

    [Tooltip("The radii (inner, outer) at which to spawn bubbles. When gravity isn't "
           + "inverted, the outer radius is used. When gravity IS inverted, the inner "
           + "radius is used.\n\nDefault: (1,4.4)")]
    public Vector2 radius = new Vector2(1f, 4.4f);
    [SerializeField, Tooltip("The strength of the initial force applied to bubbles when they spawn.\n\nDefault: 500")]
    private float initialForce = 500f;
    [SerializeField, Tooltip("The number of bubbles to spawn in mass rounds.\n\nDefault: 20")]
    private uint massRoundSize = 20;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The internal age variable that is passed along to bubbles. When a bubble is
    // spawned, we increment the age here.
    private uint age = 1;

    // ==============================================================
    // Default methods
    // ==============================================================

    private void Awake()
    {
        // Awake is called before Start. We use it to subscribe to beatReader.
        // ================

        beatReader.singleSpawn += SingleSpawnBubble;
        beatReader.massSpawn += MassSpawnBubble;
    }

    private void Start()
    {
        // Start is called before the first frame update. We use it to initialize the
        // state of bubbles.
        // ================

        RandomizeColors();
        MassSpawnBubble();
    }

    private void OnDestroy()
    {
        // Called when this object is destroyed. We use it to unsubscribe from beatReader.
        // ================

        beatReader.singleSpawn += SingleSpawnBubble;
        beatReader.massSpawn += MassSpawnBubble;
    }

    // ==============================================================
    // Instantiation/Destruction Methods
    // ==============================================================

    public void SingleSpawnBubble()
    {
        // Spawns a single bubble at the mouse orbital position. The radius at which to
        // spawn the bubble is determined in SpawnBubble depending if gravity is flipped.
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
        
        SpawnBubble(spawnPoint, colors[0].value);
        UpdateColors();
    }

    public void MassSpawnBubble()
    {
        // Spawns number Bubbles at an orbit around the screen at regular intervals, all
        // at once. The radius at which to spawn the bubble is determined in SpawnBubble
        // depending on if gravity is flipped.
        // ================

        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MassBubbleSpawn");
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //

        // Used to generate Bubble_Colors non-repetitiously.
        Bubble_Color currentColor;
        Bubble_Color lastColor = Bubble_Color.NONE;
        Bubble_Color colorBeforeThat = Bubble_Color.NONE;

        for (int i = 0; i < massRoundSize; i++) {
            // Each spawnPoint should be a unit vector, equally spaced out depending on
            // the size of i.
            Quaternion rotation = Quaternion.Euler(0,0,(360*i/massRoundSize));
            Vector2 spawnPoint = rotation * Vector2.up;

            // If the Color we generated was one of the last two Colors we generated,
            // regenerate it.
            do {
                currentColor = Bubble_Color_Methods.random();
            } while (currentColor == lastColor || currentColor == colorBeforeThat);

            SpawnBubble(spawnPoint, currentColor);

            // After spawning, pass back the Colors we've seen.
            colorBeforeThat = lastColor;
            lastColor = currentColor;
        }
    }

    private void SpawnBubble(Vector2 spawnPoint, Bubble_Color color)
    {
        // Spawns a Bubble at spawnPoint and initializes its Bubble_Color, age, and
        // sprite color.
        // ================

        // If gravity isn't flipped, multiply by outer radius.
        if (!gravityFlipped.value) 
        {
            spawnPoint *= radius.y;
        }
        // Otherwise, multiply by inner radius.
        else 
        {
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

        // Apply an initial force to our bubble.
        Vector2 direction = (center - spawnPoint).normalized;
        if (gravityFlipped.value)
        {
            direction *= -1f;
        }
        
        obj.GetComponent<Rigidbody2D>().AddForce((direction)*initialForce);
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