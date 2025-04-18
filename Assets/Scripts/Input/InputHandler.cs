using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class which manages inputs from the new input system.
/// Modded from the input handler from the Unity FPS Microgame.
/// </summary>
public class InputHandler : MonoBehaviour, InputActions.IMainActions
{
    [SerializeField, Tooltip("A floatVar holding the 0-1 value that we scale our rumble by.")]
    floatVar rumbleScaling;

    public static InputHandler Instance { get; private set; }

    [ReadOnly] public InputControlScheme LastUsedScheme;
    [ReadOnly] public Gamepad LastUsedGamepad = null;
    public InputControlScheme KeyboardScheme => _controls.KeyboardMouseScheme;
    public InputControlScheme GamepadScheme => _controls.GamepadScheme;
    public bool MouseLastUsed => _mouseLastUsed;

    // Misc Internal Variables ====================================================================

    // Object references
    InputActions _controls;

    // Input states: set by InputAction callbacks, read by accessors
    private Vector2 _pointerPos;
    private Vector2 _stickDir;
    private readonly Dictionary<string, bool> _getDown = new() {
        {"_affirm", false},
        {"_deny", false}
    };

    private readonly Dictionary<string, bool> _get = new() {};

    // Misc

    private bool _mouseLastUsed;
    private Vector2 _lastPointerPos;
   
    // Initializers and Finalizers ================================================================

    private void OnEnable() 
    {
        // Basic singleton behavior.
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }

        if (_controls == null) {
            _controls = new InputActions();
            // Tell the "MainControls" action map that we want to get told about
            // when actions get triggered.
            _controls.Main.SetCallbacks(this);
        }

        // Initialize the _get dict from the _getDown dict
        foreach (string key in _getDown.Keys) {
            _get[key] = false;
        }

        _controls.Main.Enable();
    }

    private void OnDisable() 
    {
        _controls?.Main.Disable();
    }

    // InputAction Callbacks and Methods ==========================================================

    private void LateUpdate() 
    {
        // LateUpdate is called at the END of every frame, after all Update() calls.
        
        // ==============================================================================
        // NOTE:
        // Not happy that we're calling ToList every frame, but that's the cleanest way
        // to make this work without having 7 boolean assignments in a row.
        //
        // The performance hit should be minimal for us. If it isn't, change this.
        // ==============================================================================

        _getDown["_affirm"] = false;
        _getDown["_deny"] = false;
    }

    private void SetDown(InputAction.CallbackContext context, string input)
    {
        if (context.started) _getDown[input] = _get[input] = true; 
        if (context.canceled) _getDown[input] = _get[input] = false;

        UpdateLastUsedScheme(context);
    }

    private void UpdateLastUsedScheme(InputAction.CallbackContext context, bool forceUpdate=false)
    {
        if (!context.started && !forceUpdate) return;

        _mouseLastUsed = context.control.device is Mouse;

        if (context.control.device is Gamepad) {
            LastUsedGamepad = context.control.device as Gamepad;
        } else {
            LastUsedGamepad = null;
        }

        foreach (InputControlScheme scheme in _controls.controlSchemes) {
            if (scheme.SupportsDevice(context.control.device)) {
                LastUsedScheme = scheme;
                return;
            }
        }
    }

    // Button events ============================
    public void OnAffirm(InputAction.CallbackContext context) { SetDown(context, "_affirm"); }
    public void OnDeny(InputAction.CallbackContext context) { SetDown(context, "_deny"); }

    // Value events =============================
    public void OnPointer(InputAction.CallbackContext context) 
    {
        _pointerPos = context.ReadValue<Vector2>();

        // If we've moved the pointer, force update the last used scheme.
        if (_lastPointerPos != _pointerPos) {
            _lastPointerPos = _pointerPos;
            UpdateLastUsedScheme(context, true);
        }
    }

    public void OnStick(InputAction.CallbackContext context) 
    {
        _stickDir = context.ReadValue<Vector2>();
        UpdateLastUsedScheme(context);
    }

    // UI events ================================
    // Functionality for these inputs is handled by the Event System.
    public void OnUIMove(InputAction.CallbackContext context) { UpdateLastUsedScheme(context); }
    public void OnUIClick(InputAction.CallbackContext context) { UpdateLastUsedScheme(context); }
    public void OnUISubmit(InputAction.CallbackContext context) { UpdateLastUsedScheme(context); }

    // Public Manipulator Methods ====================================================================

    /// <summary>
    /// Sets the motor frequencies of the LastUsedGamepad, if any.
    /// </summary>
    /// <param name="lowFrequency">float - The frequency [0,1] of the low-freq motor, usually on the left.</param>
    /// <param name="highFrequency">float - The frequency [0,1] of the high-freq motor, usually on the right.</param>
    public static void SetRumble(float lowFrequency, float highFrequency)
    {
        if (Instance != null) {
            float scale = Instance.rumbleScaling ? Instance.rumbleScaling.value : 1;
            Instance.LastUsedGamepad?.SetMotorSpeeds(lowFrequency*scale, highFrequency*scale);
        } else {
            NoHandlerError();
        }
    }

    // Public Accessor Methods ====================================================================

    /// <summary>
    /// Accessor for the last held values of the pointer position input.
    /// </summary>
    /// <returns>Vector2 - last known pointer input.</returns>
    public static Vector2 GetPointer()
    { 
        if (Instance != null) {
            return Instance._pointerPos;
        } else {
            NoHandlerError();
            return Vector2.zero;
        }
    }

    /// <summary>
    /// Accessor for the last held values of the stick vector input.
    /// </summary>
    /// <returns>Vector2 - last known stick input (max length: 1).</returns>
    public static Vector2 GetStick() 
    { 
        if (Instance != null) {
            return Vector2.ClampMagnitude(Instance._stickDir, 1); 
        } else {
            NoHandlerError();
            return Vector2.zero;
        }
    }

    /// <summary>
    /// Accessor for if Affirm was set down on the last frame.
    /// </summary>
    /// <returns>bool - if Affirm was just pressed down.</returns>
    public static bool GetAffirmDown()
    { 
        if (Instance != null) {
            return Instance._getDown["_affirm"]; 
        } else {
            NoHandlerError();
            return false;
        }
    }

    /// <summary>
    /// Accessor for if Deny was set down on the last frame.
    /// </summary>
    /// <returns>bool - if Deny was just pressed down.</returns>
    public static bool GetDenyDown()
    { 
        if (Instance != null) {
            return Instance._getDown["_deny"]; 
        } else {
            NoHandlerError();
            return false;
        }
    }

    // Helper Methods =============================================================================

    private static void NoHandlerError()
    {
        Debug.LogWarning("InputHandler Warning: No instance of InputHandler in scene.");
    }
}