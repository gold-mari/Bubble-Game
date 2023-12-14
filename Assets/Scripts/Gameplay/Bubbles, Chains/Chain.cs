using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Chain : ScriptableObject
{
    // Used with DFS to indicated discovered-ness. White: undiscovered. Grey: discovered.
    enum DFS_Color {White, Grey};

    // ==============================================================
    // Parameters
    // ==============================================================

    [Tooltip("The numerical ID of this chain, randomly generated on Awake.")]
    [ReadOnly]
    public int ID = 0;
    [Tooltip("The Bubble_Flavor of the bubbles in this chain.")]
    [ReadOnly]
    public Bubble_Flavor chainFlavor = Bubble_Flavor.NONE;
    [Tooltip("The list of bubbles in this chain.")]
    [ReadOnly]
    public List<Bubble> members = new List<Bubble>();
    [Tooltip("The length of this chain.")]
    [ReadOnly]
    public uint length = 0;
    [Tooltip("The uintVar representing the maximum length of ALL chains.")]
    [Expandable]
    public uintVar maxLength;

    // ==============================================================
    // Default methods
    // ==============================================================

    void Awake()
    {
        // Awake runs on Scriptable Object startup, either on game start or on creation.
        // ================

        // Create a random ID.
        ID = Random.Range(int.MinValue,int.MaxValue);
    }

    // ================================================================
    // Instantiation/Destruction Methods
    // ================================================================

    void BreakChain(ChainBreakHandler handler)
    {
        // Called when a chain breaks. Passes a call up to the ChainBreakHandler and then
        // destroys all members.
        // ================

        // Otherwise, shout the chain break.
        handler.ShoutChainBreak();

        DestroyAllMembers();
    }

    void DestroyAllMembers()
    {
        // Destroys all members of the chain, usually called when a chain goes over its
        // maximum length.
        // ================

        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ChainDestroy");
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //
        // DEBUG DEBUG DEBUG //

        bool bombChain = false;
        foreach (Bubble bubble in members) {
            if (bubble && bubble.isBomb)
            {
                bombChain = true;

                // ..... ..... ..... .....
                // ..... ..... ..... .....
                // CURRENT TASK: CONFIRM THAT THE ANIMATION DOESN'T PLAY ON CHAINS WITH MULTIPLE BOMBS
                // ..... ..... ..... .....
                // ..... ..... ..... .....

                break;
            }
        }

        foreach (Bubble bubble in members) {
            // We check for bubble just in case it leaves our list while we're looping.
            if (bubble) 
            {
                bubble.DestroyBubble(bombChain);
            }
        }
        if (bombChain) Debug.Log("The chain we're destroying has a bomb.");
        else Debug.Log("The chain we're destroying IS INERT.");
        Destroy(this);
    }

    // ==============================================================
    // Data-manipulation methods
    // ==============================================================

    public void AddBubble(Bubble bubble, ChainBreakHandler handler)
    {
        // Adds a bubble to this Chain.
        // ================

        // If we have not decided a chainFlavor yet, set our chainFlavor to bubbleFlavor.
        if (chainFlavor == Bubble_Flavor.NONE) {
            chainFlavor = bubble.bubbleFlavor;
        }

        // Update our fields.
        members.Add(bubble);
        length++;

        // If incrementing length takes us over our max, destroy this chain.
        if (length >= maxLength.value) {
            BreakChain(handler);
        }

        // Update their field.
        bubble.chain = this;
    }

    public void Concatenate(Chain chain, ChainBreakHandler handler)
    {
        // Adds the contents of another Chain to this Chain.
        // ================

        // If we have not decided a chainFlavor yet, set our chainFlavor to theirs.
        if (chainFlavor == Bubble_Flavor.NONE) {
            chainFlavor = chain.chainFlavor;
        }

        // Update our fields.
        members.AddRange(chain.members);
        length += chain.length;

        // If adding to length takes us over our max, destroy this chain.
        if (length >= maxLength.value) {
            BreakChain(handler);
        }

        foreach (Bubble bubble in chain.members) {
            bubble.chain = this;
        }
    }

    public void Distribute(ChainBreakHandler handler)
    {
        // Distributes a chain into connected components. Is built off a DFS algorithm to
        // determine connected components. Each connected component is added to a new chain.
        // Takes a ChainBreakHandler as an argument so that it can be passed along to new chains.
        // ================
        
        // Run the DFS algorithm, visiting all bubbles to determine connected components.
        DFS_Main(handler);
        
        // After running DFS, all bullets have been distributed into new chains.
        // We are good to destroy this chain.
        Destroy(this);
    }

    private void DFS_Main(ChainBreakHandler handler)
    {
        // The main loop of the DFS algorithm. Each time a bubble is visited from
        // DFS_Main and not DFS_Visit, we can conclude it is a new connected component.
        // Thus, we create a new Chain and add it to there.
        // Takes a ChainBreakHandler as an argument so that it can be passed along to new chains.
        // ================

        // Step 1: Create and white-out the DFS dictionary.
        Dictionary<Bubble, DFS_Color> dict = new Dictionary<Bubble, DFS_Color>();
        foreach (Bubble bubble in members) {
            // In the rare case a neighbor was removed and readded while we were
            // calculating, then it will already be in our dictionary. If it's not
            // present, add it as white.
            if (!dict.ContainsKey(bubble)) {
                dict.Add(bubble, DFS_Color.White);
            }
        }

        // Step 2: Begin visiting each bubble.
        foreach (Bubble bubble in members) {
            // If the color is white, aka we haven't visited it yet,
            if (dict[bubble] == DFS_Color.White) {
                // Create a new chain, intializing it to the first bubble.
                Chain newChain = ScriptableObject.CreateInstance<Chain>();
                // Pass on the maxLength and handler variables.
                newChain.maxLength = maxLength;
                // Visit the bubble.
                DFS_Visit(bubble, dict, newChain, handler);
            }
        }
    }

    private void DFS_Visit(Bubble bubble, Dictionary<Bubble, DFS_Color> dict, Chain chain, ChainBreakHandler handler)
    {
        // The recursive portion of the DFS algorithm. Each time we visit a bubble, we
        // set its color to grey. Every bubble visited within a single recursive walk of
        // DFS_Visit will be added to the same chain.
        // ============

        // Mark the visited bubble as grey (partially explored).
        dict[bubble] = DFS_Color.Grey;
        // Add it to the new chain.
        chain.AddBubble(bubble, handler);

        // Visit each of its unexplored children.
        List<Bubble> adj = new List<Bubble>(bubble.adjacencies);
        foreach (Bubble neighbor in adj) {
            // In the rare case a neighbor was added while we were calculating, then it
            // will not be in our dictionary. If it's not present, add it as white.
            if (!dict.ContainsKey(neighbor)) {
                dict.Add(neighbor, DFS_Color.White);
            }

            // If the bubble color matches our color AND the DFS_Color is white, aka we
            // have't visited it yet,
            if (neighbor.bubbleFlavor == chainFlavor && dict[neighbor] == DFS_Color.White) {
                // Visit it and point it towards the new chain.
                DFS_Visit(neighbor, dict, chain, handler);
            }
        }
    }
}
