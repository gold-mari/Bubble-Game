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
    [SerializeField, Tooltip("The vector2Var equalling the vector from the cursor position to the center.")]
    private vector2Var cursorPointVector;
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
    [SerializeField, Tooltip("Whether or not mass bubble spawns are composed of HYPERBUBBLES.\n\nDefault: false")]
    private bool HYPERMassBubbles = false;
    [SerializeField, Tooltip("The flavor bomb prefab to spawn.")]
    private GameObject flavorBomb;
    [SerializeField, Tooltip("A binder of bubble sprites, assigned to spawned bubbles depending on their flavor.")]
    private BubbleSpriteBinder binder;
    [SerializeField, Expandable, Tooltip("The list of current and upcoming Bubble_Flavors to spawn, represented "
                                       + "by a list of Bubble_FlavorVars. The 0th index is the current flavor, and each "
                                       + "successive index is the flavor after that.\n\nThis array may be ANY size.")]
    private List<Bubble_FlavorVar> flavors;
    [Tooltip("The center of the playable space.\n\nDefault: (0,0)")]
    public Vector2 center = new(0,0);

    [Tooltip("The radii (inner, outer) at which to spawn bubbles. When gravity isn't "
           + "inverted, the outer radius is used. When gravity IS inverted, the inner "
           + "radius is used.\n\nDefault: (1,4.4)")]
    public Vector2 radius = new(1f, 4.4f);
    [SerializeField, Tooltip("The strength of the initial force (inner gravity, outer gravity) applied to " +
                             "bubbles when they spawn.\n\nDefault: (500,1000)")]
    private Vector2 initialForce = new(500f,1000f);
    [SerializeField, Tooltip("The number of bubbles to spawn in mass rounds.\n\nDefault: 20")]
    private uint massRoundSize = 20;

    [SerializeField, Tooltip("The SFX played on spawning a single bubble.")]
    FMODUnity.EventReference singleBubbleSFX;
    [SerializeField, Tooltip("The SFX played on spawning a mass round of bubbles.")]
    FMODUnity.EventReference massBubbleSFX;
    [SerializeField, Tooltip("The SFX played before spawning a mass round of bubbles.")]
    FMODUnity.EventReference beforeMassBubbleSFX;
    [SerializeField, Tooltip("The SFX played on spawning a flavor bombed-bubble.")]
    FMODUnity.EventReference flavorBombSFX;
    [SerializeField, Tooltip("The SFX played before spawning a flavor bombed-bubble.")]
    FMODUnity.EventReference beforeFlavorBombSFX;
    [SerializeField, Tooltip("The SFX played on spawning a HYPERBUBBLE.")]
    FMODUnity.EventReference hyperbubbleSFX;
    [SerializeField, Tooltip("The SFX played before spawning a HYPERBUBBLE.")]
    FMODUnity.EventReference beforeHyperbubbleSFX;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The internal age variable that is passed along to bubbles. When a bubble is
    // spawned, we increment the age here.
    private uint age = 1;
    // The chain break handler on this object. Passed onto bubbles onto chains.
    private ChainBreakHandler handler;

    // Sound effect hell!
    private FMOD.Studio.EventInstance singleBubbleSFX_i,
                                      massBubbleSFX_i, beforeMassBubbleSFX_i, 
                                      flavorBombSFX_i, beforeFlavorBombSFX_i, 
                                      hyperbubbleSFX_i, beforeHyperbubbleSFX_i;
    // A factor we scale all spawned bubbles by. Usually 1, may change at key moments.     
    private float bubbleScaleFactor = 1;
    // The localScale of our bubble and hyperbubble prefabs when we enter the level.
    private Vector3 baseBubbleScale, baseHyperbubbleScale;

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
        beatReader.makeFlavorBomb += FlavorBombSpawnBubble;
        beatReader.hyperSpawn += SpawnHyperbubble;

        beatReader.beforeMassSpawn += OnBeforeMassSpawnBubble;
        beatReader.beforeMakeFlavorBomb += OnBeforeFlavorBombSpawnBubble;
        beatReader.beforeHyperSpawn += OnBeforeSpawnHyperbubble;

        handler = GetComponent<ChainBreakHandler>();

        baseBubbleScale = bubble.transform.localScale;
        baseHyperbubbleScale = hyperbubble.transform.localScale;
    }

    private void Start()
    {
        // Start is called before the first frame update. We use it to initialize the
        // state of bubbles.
        // ================

        singleBubbleSFX_i  = FMODUnity.RuntimeManager.CreateInstance(singleBubbleSFX);
        massBubbleSFX_i = FMODUnity.RuntimeManager.CreateInstance(massBubbleSFX);
        beforeMassBubbleSFX_i = FMODUnity.RuntimeManager.CreateInstance(beforeMassBubbleSFX);
        flavorBombSFX_i = FMODUnity.RuntimeManager.CreateInstance(flavorBombSFX);
        beforeFlavorBombSFX_i = FMODUnity.RuntimeManager.CreateInstance(beforeFlavorBombSFX);
        hyperbubbleSFX_i = FMODUnity.RuntimeManager.CreateInstance(hyperbubbleSFX);
        beforeHyperbubbleSFX_i = FMODUnity.RuntimeManager.CreateInstance(beforeHyperbubbleSFX);

        RandomizeFlavors();
        MassSpawnBubble();
    }

    private void OnDestroy()
    {
        // Called when this object is destroyed. We use it to unsubscribe from beatReader.
        // ================

        beatReader.singleSpawn -= SingleSpawnBubble;
        beatReader.massSpawn -= MassSpawnBubble;
        beatReader.makeFlavorBomb -= FlavorBombSpawnBubble;
        beatReader.hyperSpawn -= SpawnHyperbubble;

        singleBubbleSFX_i.release();
        massBubbleSFX_i.release();
        beforeMassBubbleSFX_i.release();
        flavorBombSFX_i.release();
        beforeFlavorBombSFX_i.release();
        hyperbubbleSFX_i.release();
        beforeHyperbubbleSFX_i.release();
    } 
   
    // ==============================================================
    // Bubble-making methods
    // ==============================================================

    private void SingleSpawnBubble()
    {
        // Spawns a single bubble at the mouse orbital position. The radius at which to
        // spawn the bubble is determined in SpawnBubble depending if gravity is flipped.
        // ================

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Flavor", (int)flavors[0].value-1);
        singleBubbleSFX_i.start();

        // The spawn point is the cursor point vector, normalized and then multiplied by the radius.
        Vector2 spawnPoint = cursorPointVector.value.normalized * GetCurrentRadius() + center;
        
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

        massBubbleSFX_i.start();

        // Used to generate Bubble_Flavors non-repetitiously.
        BubbleFlavor currentFlavor;
        BubbleFlavor lastFlavor = BubbleFlavor.NONE;
        BubbleFlavor flavorBeforeThat = BubbleFlavor.NONE;

        for (int i = 0; i < massRoundSize; i++) {
            // Each spawnPoint should be a unit vector, equally spaced out depending on
            // the size of i.
            Quaternion rotation = Quaternion.Euler(0,0,(360*i/massRoundSize));
            Vector2 spawnPoint = rotation * Vector2.up * GetCurrentRadius() + (Vector3)center;

            // If the Flavor we generated was one of the last two Flavors we generated,
            // regenerate it.
            do {
                currentFlavor = BubbleFlavorMethods.Random();
            } while (currentFlavor == lastFlavor || currentFlavor == flavorBeforeThat);

            SpawnBubble(spawnPoint, currentFlavor, Vector2.zero, HYPERMassBubbles);

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

        flavorBombSFX_i.start();

        // The spawn point is the cursor point vector, normalized and then multiplied by the radius.
        Vector2 spawnPoint = cursorPointVector.value.normalized * GetCurrentRadius() + center;
        
        // Apply an initial force to our bubble.
        Vector2 direction = (center - spawnPoint).normalized;
        if (gravityFlipped.value)
        {
            direction *= -1f;
        }

        // Spawn a bubble and attach a flavor bomb to it.
        Bubble bubble = SpawnBubble(spawnPoint, flavors[0].value, direction);
        GameObject bomb = Instantiate(flavorBomb, bubble.transform);
        bomb.GetComponent<FlavorBomb>().Initialize(this, bubble.bubbleFlavor, bubble.isHyperbubble);
        bubble.isBomb = true;

        // Update our flavors afterwards!
        UpdateFlavors();
    }


    private void SpawnHyperbubble()
    {
        // Spawns a single HYPERBUBBLE at the mouse orbital position. The radius at which to
        // spawn the bubble is determined in SpawnBubble depending if gravity is flipped.
        // ================

        hyperbubbleSFX_i.start();

        // The spawn point is the cursor point vector, normalized and then multiplied by the radius.
        Vector2 spawnPoint = cursorPointVector.value.normalized * GetCurrentRadius()  + center;
        
        // Apply an initial force to our bubble.
        Vector2 direction = (center - spawnPoint).normalized;
        if (gravityFlipped.value)
        {
            direction *= -1f;
        }

        SpawnBubble(spawnPoint, flavors[0].value, direction, true);
        UpdateFlavors();
    }

    public Bubble SpawnBubble(Vector2 spawnPoint, BubbleFlavor flavor, Vector2 direction, bool hyper=false)
    {
        // Spawns a Bubble at spawnPoint and initializes its Bubble_Flavor, age, and
        // sprite color. Applies initialForce if direction is not the zero vector.
        // Spawns a HYPERBUBBLE if hyper is true. Otherwise, spawns a regular bubble.
        // ================

        GameObject toSpawn = hyper ? hyperbubble : bubble;

        GameObject obj = Instantiate(toSpawn, spawnPoint, Quaternion.identity, bubbleParent);
        Bubble objBubble = obj.GetComponent<Bubble>();
        // Scale the bubble size! Usually only does anything in the finale.
        objBubble.transform.localScale *= bubbleScaleFactor;

        objBubble.isHyperbubble = hyper;

        // Initialize color and age.
        objBubble.bubbleFlavor = flavor;
        objBubble.age = age;
        objBubble.handler = handler;
        // Increment age after we spawn it.
        age++;

        // Initialize sprite.
        obj.GetComponentInChildren<SpriteRenderer>().sprite = BubbleFlavorMethods.GetSprite(flavor, binder);

        // Intialize dangerManager reference in dangerTracker.
        obj.GetComponent<DangerTracker>().dangerManager = dangerManager;

        if (direction != Vector2.zero) {            
            obj.GetComponent<Rigidbody2D>().AddForce(direction*GetCurrentSpawnForce());
        }

        // Return the object we spawned.
        return objBubble;
    }

    // ==============================================================
    // Preemptive feedback methods
    // ==============================================================

    private void OnBeforeMassSpawnBubble()
    {
        beforeMassBubbleSFX_i.start();
    }

    private void OnBeforeFlavorBombSpawnBubble()
    {
        beforeFlavorBombSFX_i.start();
    }

    private void OnBeforeSpawnHyperbubble()
    {
        beforeHyperbubbleSFX_i.start();
    }

    // ==============================================================
    // Data-manipulation methods
    // ==============================================================

    private float GetCurrentRadius()
    {
        // Returns the active radius, as determined by the value of gravityFlipped.
        // ================

        return (!gravityFlipped.value) ? radius.y : radius.x;
    }

    private float GetCurrentSpawnForce()
    {
        // Returns the initial force, as determined by the value of gravityFlipped.
        // ================

        return (!gravityFlipped.value) ? initialForce.x : initialForce.y;
    }

    private void RandomizeFlavors()
    {
        // Updates the Bubble_Flavors in the array flavors. Each flavor is randomized.
        // Rerolls a color if it was seen in the last two generations.
        // ================

        BubbleFlavor lastFlavor = BubbleFlavor.NONE;
        BubbleFlavor flavorBeforeThat = BubbleFlavor.NONE;
        
        for (int i = 0; i < flavors.Count; i++)
        {
            // Generate flavors nonrepetitively.
            do {
                flavors[i].value = BubbleFlavorMethods.Random();
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
            flavors[count-1].value = BubbleFlavorMethods.Random();  
        } while (flavors[count-1].value == flavors[count-2].value || flavors[count-1].value == flavors[count-3].value);
    }

    public void SetBubbleScale(float value)
    {
        // Sets our bubble scale variable, and then loops through every
        // extant bubble and changes their scale to match.
        // ================

        bubbleScaleFactor = value;

        Bubble[] bubbles = bubbleParent.GetComponentsInChildren<Bubble>();
        foreach (Bubble bubble in bubbles) {
            if (bubble.isHyperbubble) {
                bubble.transform.localScale = baseHyperbubbleScale * bubbleScaleFactor;
            } else {
                bubble.transform.localScale = baseBubbleScale * bubbleScaleFactor;
            }
        }
    }
}