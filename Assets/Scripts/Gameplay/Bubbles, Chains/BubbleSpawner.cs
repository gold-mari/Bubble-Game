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
    [Tooltip("The transform that all bubbles will be parented under.")]
    public Transform bubbleParent;
    [SerializeField, Tooltip("The bubble prefab to spawn.\n\nIMPORTANT: This bubble should be initialized with the "
           + "NIL chain.")]
    private GameObject bubble;
    [SerializeField, Tooltip("The HYPERBUBBLE prefab to spawn.\n\nIMPORTANT: This bubble should be initialized with the "
           + "NIL chain.")]
    private GameObject hyperbubble;
    [SerializeField, Tooltip("The flavor bomb prefab to spawn.")]
    private GameObject flavorBomb;
    [SerializeField, Tooltip("A binder of bubble sprites, assigned to spawned bubbles depending on their flavor.")]
    private BubbleSpriteBinder binder;
    [SerializeField, Expandable, Tooltip("The list of current and upcoming Bubble_Flavors to spawn, represented "
                                       + "by a list of Bubble_FlavorVars. The 0th index is the current flavor, and each "
                                       + "successive index is the flavor after that.\n\nThis array may be ANY size.")]
    private List<Bubble_FlavorVar> flavors;
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
    // The chain break handler on this object. Passed onto bubbles onto chains.
    private ChainBreakHandler handler;

    // ==============================================================
    // Default methods
    // ==============================================================

    private void Awake()
    {
        // Awake is called before Start. We use it to subscribe to beatReader, and to find our
        // ChainBreakHandler.
        // ================

        beatReader.singleSpawn += SingleSpawnBubble;
        beatReader.massSpawn += MassSpawnBubble;
        beatReader.hyperSpawn += SpawnHyperbubble;
        beatReader.makeFlavorBomb += FlavorBombSpawnBubble;

        handler = GetComponent<ChainBreakHandler>();
    }

    private void Start()
    {
        // Start is called before the first frame update. We use it to initialize the
        // state of bubbles.
        // ================

        RandomizeFlavors();
        MassSpawnBubble();
    }

    private void OnDestroy()
    {
        // Called when this object is destroyed. We use it to unsubscribe from beatReader.
        // ================

        beatReader.singleSpawn -= SingleSpawnBubble;
        beatReader.massSpawn -= MassSpawnBubble;
        beatReader.hyperSpawn -= SpawnHyperbubble;
        beatReader.makeFlavorBomb -= FlavorBombSpawnBubble;
    }

    // ==============================================================
    // Instantiation/Destruction Methods
    // ==============================================================

    private void SingleSpawnBubble()
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
        Vector2 spawnPoint = (mousePosition - center).normalized * GetCurrentRadius();
        
        // Apply an initial force to our bubble.
        Vector2 direction = (center - spawnPoint).normalized;
        if (gravityFlipped.value)
        {
            direction *= -1f;
        }

        SpawnBubble(spawnPoint, flavors[0].value, direction);
        UpdateFlavors();
    }

    private void MassSpawnBubble()
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

        // Used to generate Bubble_Flavors non-repetitiously.
        Bubble_Flavor currentFlavor;
        Bubble_Flavor lastFlavor = Bubble_Flavor.NONE;
        Bubble_Flavor flavorBeforeThat = Bubble_Flavor.NONE;

        for (int i = 0; i < massRoundSize; i++) {
            // Each spawnPoint should be a unit vector, equally spaced out depending on
            // the size of i.
            Quaternion rotation = Quaternion.Euler(0,0,(360*i/massRoundSize));
            Vector2 spawnPoint = rotation * Vector2.up * GetCurrentRadius();

            // If the Flavor we generated was one of the last two Flavors we generated,
            // regenerate it.
            do {
                currentFlavor = Bubble_Flavor_Methods.random();
            } while (currentFlavor == lastFlavor || currentFlavor == flavorBeforeThat);

            SpawnBubble(spawnPoint, currentFlavor, Vector2.zero);

            // After spawning, pass back the Flavors we've seen.
            flavorBeforeThat = lastFlavor;
            lastFlavor = currentFlavor;
        }
    }

    private void FlavorBombSpawnBubble()
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
        Vector2 spawnPoint = (mousePosition - center).normalized * GetCurrentRadius();
        
        // Apply an initial force to our bubble.
        Vector2 direction = (center - spawnPoint).normalized;
        if (gravityFlipped.value)
        {
            direction *= -1f;
        }

        // Spawn a bubble and attach a flavor bomb to it.
        Bubble bubble = SpawnBubble(spawnPoint, flavors[0].value, direction);
        GameObject bomb = Instantiate(flavorBomb, bubble.transform);
        bomb.GetComponent<FlavorBomb>().Initialize(this, bubble.isHyperbubble);
        bubble.isBomb = true;

        // Update our flavors afterwards!
        UpdateFlavors();
    }


    private void SpawnHyperbubble()
    {
        // Spawns a single HYPERBUBBLE at the mouse orbital position. The radius at which to
        // spawn the bubble is determined in SpawnBubble depending if gravity is flipped.
        // ================

        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HyperbubbleSpawn");
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //

        // Get the mouse position on the screen.
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // The spawn point is the vector from the center to the mouse position, normalized and then multiplied by the radius.
        Vector2 spawnPoint = (mousePosition - center).normalized * GetCurrentRadius();
        
        // Apply an initial force to our bubble.
        Vector2 direction = (center - spawnPoint).normalized;
        if (gravityFlipped.value)
        {
            direction *= -1f;
        }

        SpawnBubble(spawnPoint, flavors[0].value, direction, true);
        UpdateFlavors();
    }

    public Bubble SpawnBubble(Vector2 spawnPoint, Bubble_Flavor flavor, Vector2 force, bool hyper=false)
    {
        // Spawns a Bubble at spawnPoint and initializes its Bubble_Flavor, age, and
        // sprite color. Applies initialForce if force is not the zero vector.
        // Spawns a HYPERBUBBLE if hyper is true. Otherwise, spawns a regular bubble.
        // ================

        GameObject toSpawn = bubble;
        if (hyper)
        {
            toSpawn = hyperbubble;
        }

        GameObject obj = Instantiate(toSpawn, spawnPoint, Quaternion.identity, bubbleParent);
        Bubble objBubble = obj.GetComponent<Bubble>();

        if (hyper)
        {
            objBubble.isHyperbubble = true;
        }

        // Initialize color and age.
        objBubble.bubbleFlavor = flavor;
        objBubble.age = age;
        objBubble.handler = handler;
        // Increment age after we spawn it.
        age++;

        // Initialize sprite.
        obj.GetComponentInChildren<SpriteRenderer>().sprite = Bubble_Flavor_Methods.getSprite(flavor, binder);

        // Intialize dangerManager reference in dangerTracker.
        obj.GetComponent<DangerTracker>().dangerManager = dangerManager;

        if (force != Vector2.zero)
        {            
            obj.GetComponent<Rigidbody2D>().AddForce((force)*initialForce);
        }

        // Return the object we spawned.
        return objBubble;
    }

    // ==============================================================
    // Data-manipulation methods
    // ==============================================================

    private float GetCurrentRadius()
    {
        // Returns the active radius, as determined by the value of gravityFlipped.
        // ================

        // If gravity isn't flipped, multiply by outer radius.
        if (!gravityFlipped.value) 
        {
            return radius.y;
        }
        // Otherwise, multiply by inner radius.
        else 
        {
            return radius.x;
        }
    }

    private void RandomizeFlavors()
    {
        // Updates the Bubble_Flavors in the array flavors. Each flavor is randomized.
        // Rerolls a color if it was seen in the last two generations.
        // ================

        Bubble_Flavor lastFlavor = Bubble_Flavor.NONE;
        Bubble_Flavor flavorBeforeThat = Bubble_Flavor.NONE;
        
        for (int i = 0; i < flavors.Count; i++)
        {
            // Generate flavors nonrepetitively.
            do {
                flavors[i].value = Bubble_Flavor_Methods.random();
            } while (flavors[i].value == lastFlavor || flavors[i].value == flavorBeforeThat);

            // After selecting, pass back the Flavors we've seen.
            flavorBeforeThat = lastFlavor;
            lastFlavor = flavors[i].value;
        }
    }

    private void UpdateFlavors()
    {
        // Updates the Bubble_Flavors in the array flavors. For all i > 0, the Bubble_Flavor
        // at flavors[i] is passed to flavors[i-1]. The flavor at flavors[Count-1] is then
        // randomly regenerated.
        // ================
        
        int count = flavors.Count;

        for (int i = 1; i < count; i++)
        {
            // Pass the flavors backwards.
            flavors[i-1].value = flavors[i].value;
        }

        // Regenerate the final flavor, ensuring it wasn't either of the last two flavors.
        do {
            flavors[count-1].value = Bubble_Flavor_Methods.random();  
        } while (flavors[count-1].value == flavors[count-2].value || flavors[count-1].value == flavors[count-3].value);
    }
}