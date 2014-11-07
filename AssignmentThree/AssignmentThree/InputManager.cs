using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace AssignmentThree
{
    /// <summary>
    /// A type of input action.
    /// </summary>
    public enum InputActionType
    {
        /// <summary>
        /// The button/key was down and is now up.
        /// </summary>
        Pressed,

        /// <summary>
        /// The button/key was down in the previous update and is still down.
        /// </summary>
        StillDown,

        /// <summary>
        /// The button/key was up in the previous update and is still up.
        /// </summary>
        StillUp,

        /// <summary>
        /// The button/key was down in this update (previous state is not considered).
        /// </summary>
        Down,

        /// <summary>
        /// The button/key was up in this update (previous state is not considered).
        /// </summary>
        Up
    }

    /// <summary>
    /// Represents a signifcant key/button combination.
    /// </summary>
    public struct InputAction
    {
        /// <summary>
        /// Denotes that an input action has no corresponding button.
        /// </summary>
        public const Buttons NO_ACTION_BUTTON = (Buttons)(-1);

        /// <summary>
        /// Denotes that an input action has no corresponding key.
        /// </summary>
        public const Keys NO_ACTION_KEY = (Keys)(-1);

        /// <summary>
        /// The key corresponding to the action.
        /// </summary>
        public Keys[] ActionKeys;

        /// <summary>
        /// The button corresponding to the action.
        /// </summary>
        public Buttons[] ActionButtons;

        /// <summary>
        /// Constructs a new InputAction with the specified values.
        /// </summary>
        /// <param name="ActionKey">The set of keys associated with this action. Use null if there are no associated keys.</param>
        /// <param name="ActionButton">The set of buttons associated with this action. Use null if there are no associated buttons.</param>
        public InputAction(Keys[] ActionKeys, Buttons[] ActionButtons)
        {
            this.ActionKeys = ActionKeys;
            this.ActionButtons = ActionButtons;
        }

        /// <summary>
        /// Constructs a new InputAction with the specified key and button.
        /// </summary>
        /// <param name="ActionKey">The key associated with this action or InputAction.NO_ACTION_KEY if there isn't one.</param>
        /// <param name="ActionButton">The button associated with this action or InputAction.NO_ACTION_BUTTON if there isn't one.</param>
        public InputAction(Keys ActionKey, Buttons ActionButton)
            : this(ActionKey == NO_ACTION_KEY ? null : new Keys[] { ActionKey }, 
                   ActionButton == NO_ACTION_BUTTON ? null : new Buttons[] { ActionButton })
        {}
    }

    public class InputManager
    {
        public static readonly InputAction DEFAULT_ESCAPE = new InputAction(Keys.Escape, Buttons.Back);
        public static readonly InputAction DEFAULT_PAUSE = new InputAction(Keys.Home, Buttons.Start);
        /// <summary>
        /// The keyboard state in the last update.
        /// </summary>
        private KeyboardState _prevKState;

        /// <summary>
        /// The gamepad state in the last update.
        /// </summary>
        private GamePadState _prevGState;

        /// <summary>
        /// The keyboard state in the current update.
        /// </summary>
        private KeyboardState _curKState;

        /// <summary>
        /// The gamepad state in the current update.
        /// </summary>
        private GamePadState _curGState;

        /// <summary>
        /// The mouse state in the previous update.
        /// </summary>
        private MouseState _prevMState;

        /// <summary>
        /// The mouse state in the current update.
        /// </summary>
        private MouseState _curMState;

        /// <summary>
        /// The input that will cause the game to exit.
        /// </summary>
        private InputAction _exit;

        /// <summary>
        /// The input that will cause the player to move left.
        /// </summary>
        private InputAction _pause;

        /// <summary>
        /// A mapping of named actions to InputActions.
        /// </summary>
        private Dictionary<string, InputAction> _actions;

        /// <summary>
        /// Gets or sets the exit InputAction.
        /// </summary>
        public InputAction Exit { get { return _exit; } set { _exit = value; } }

        /// <summary>
        /// Gets or sets the pause InputAction.
        /// </summary>
        public InputAction Pause { get { return _pause; } set { _pause = value; } }

        /// <summary>
        /// Gets the mouse's state during the second-most recent update.
        /// </summary>
        public MouseState PrevMouseState { get { return _prevMState; } }

        /// <summary>
        /// Gets the mouse's state as of the most recent update.
        /// </summary>
        public MouseState CurMouseState { get { return _curMState; } }

        /// <summary>
        /// Gets the keyboard's state during the second-most recent update..
        /// </summary>
        public KeyboardState PrevKeyboardState { get { return _prevKState; } }

        /// <summary>
        /// Gets the keyboard's state as of the most recent update.
        /// </summary>
        public KeyboardState CurKeyboardState { get { return _curKState; } }

        /// <summary>
        /// Gets the gamepad's state during the second-most recent update.
        /// </summary>
        public GamePadState PrevGamepadState { get { return _prevGState; } }
        
        /// <summary>
        /// Gets the gamepad's state as of the most recent update.
        /// </summary>
        public GamePadState CurGamepadState { get { return _curGState; } }

        public InputManager()
            : this(DEFAULT_PAUSE, DEFAULT_ESCAPE)
        { }

        /// <summary>
        /// Create an InputManager with the specified escape key and button.
        /// </summary>
        /// <param name="pause">The InputAction which should pause the game.</param>
        /// <param name="exit">The InputAction which should exit the game.</param>
        public InputManager(InputAction pause, InputAction exit)
            : base()
        {        
            _pause = pause;
            _exit = exit;
            _actions = new Dictionary<string, InputAction>();
        }

        /// <summary>
        /// Updates the input states wrapped by the InputManager.
        /// </summary>
        /// <param name="gState">The current GamePadState for a given player.</param>
        /// <param name="kState">The current KeyboardState.</param>
        public void Update()
        {
            _prevKState = _curKState;
            _prevGState = _curGState;
            _prevMState = _curMState;
            _curKState = Keyboard.GetState();
            _curGState = GamePad.GetState(PlayerIndex.One);
            _curMState = Mouse.GetState();
        }

        /// <summary>
        /// Update the gamepad's state.
        /// </summary>
        /// <param name="gState">The current gamepad state.</param>
        public void UpdateGamePad(GamePadState gState)
        {
            _prevGState = _curGState;
            _curGState = gState;
        }

        /// <summary>
        /// Update the keyboard's state.
        /// </summary>
        /// <param name="kState">The keyboard's current state.</param>
        public void UpdateKeyboard(KeyboardState kState)
        {
            _prevKState = _curKState;
            _curKState = kState;
        }

        /// <summary>
        /// Update the mouse's state.
        /// </summary>
        /// <param name="mState">The mouse's current state.</param>
        public void UpdateMouseState(MouseState mState)
        {
            _prevMState = _curMState;
            _curMState = mState;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was pressed between the
        /// last update and the current one.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was pressed.</returns>
        public bool KeyWasPressed(Keys[] k)
        {
            if (k == null)
                return false;

            int i;
            for (i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyDown(k[i]) && _curKState.IsKeyUp(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of gamepad buttons was pressed
        /// between the last update and the current one.
        /// </summary>
        /// <param name="b">The buttons to check.</param>
        /// <returns>Whether at least one of the buttons was pressed.</returns>
        public bool ButtonWasPressed(Buttons[] b)
        {
            if (b == null)
                return false;

            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_prevGState.IsButtonDown(b[i]) && _curGState.IsButtonUp(b[i]))
                    break;
            }
            return i != b.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was held down 
        /// during the last update and the current one.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was held.</returns>
        public bool KeyHeld(Keys[] k)
        {
            if (k == null)
                return false;

            int i;
            for (i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyDown(k[i]) && _curKState.IsKeyDown(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of buttons was held down 
        /// during the last update and the current one.
        /// </summary>
        /// <param name="b">The buttons to check.</param>
        /// <returns>Whether at least one of the buttons was held down.</returns>
        public bool ButtonHeld(Buttons[] b)
        {
            if (b == null)
                return false;

            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_prevGState.IsButtonDown(b[i]) && _curGState.IsButtonDown(b[i]))
                    break;
            }

            return i != b.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up during
        /// the last update and the current one.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was still up.</returns>
        public bool KeyStillUp(Keys[] k)
        {
            if (k == null)
                return false;

            int i;
            for (i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyUp(k[i]) && _curKState.IsKeyUp(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of buttons was up during 
        /// the last update and the current one.
        /// </summary>
        /// <param name="b">The buttons to check.</param>
        /// <returns>Whether the buttons was up.</returns>
        public bool ButtonStillUp(Buttons[] b)
        {
            if (b == null)
                return false;

            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_curGState.IsButtonUp(b[i]) && _prevGState.IsButtonUp(b[i]))
                    break;
            }

            return i != b.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up during the
        /// current update.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was up.</returns>
        public bool KeyIsDown(Keys[] k)
        {
            if (k == null)
                return false;

            int i;
            for(i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyDown(k[i]) || _curKState.IsKeyDown(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up during 
        /// the current update.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was up.</returns>
        public bool KeyIsUp(Keys[] k)
        {
            if (k == null)
                return false;

            int i;
            for (i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyUp(k[i]) || _curKState.IsKeyUp(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up during
        /// the the current update.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was up.</returns>
        public bool ButtonIsUp(Buttons[] b)
        {
            if (b == null)
                return false;

            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_prevGState.IsButtonUp(b[i]) || _curGState.IsButtonUp(b[i]))
                    break;
            }
            return i != b.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up during the current
        /// update.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was up.</returns>
        public bool ButtonIsDown(Buttons[] b)
        {
            if (b == null)
                return false;
            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_prevGState.IsButtonDown(b[i]) || _curGState.IsButtonDown(b[i]))
                    break;
            }
            return i != b.Length;
        }
        
        /// <summary>
        /// Gets the mouse's movement in the x and y axes since the last update.
        /// </summary>
        /// <param name="mouseDiff">A Vector2 to hold the differences.</param>
        public void GetMouseDiff(out Vector2 mouseDiff)
        {
            mouseDiff = new Vector2(_prevMState.X - _curMState.X, _prevMState.Y - _curMState.Y);
        }


        /// <summary>
        /// Determines whether the specified action occurred.
        /// </summary>
        /// <param name="action">The action buttons/keys to check.</param>
        /// <param name="type">The type of action for which to check.</param>
        /// <returns></returns>
        public bool ActionOccurred(ref InputAction action, InputActionType type)
        {
            switch(type)
            {
                case InputActionType.Pressed:
                    return KeyWasPressed(action.ActionKeys) || ButtonWasPressed(action.ActionButtons);
                case InputActionType.StillDown:
                    return KeyHeld(action.ActionKeys) || ButtonHeld(action.ActionButtons);
                case InputActionType.StillUp:
                    return KeyStillUp(action.ActionKeys) || ButtonStillUp(action.ActionButtons);
                case InputActionType.Down:
                    return KeyIsDown(action.ActionKeys) || ButtonIsDown(action.ActionButtons);
                case InputActionType.Up:
                    return KeyIsUp(action.ActionKeys) || ButtonIsUp(action.ActionButtons);
                default:
                    throw new ArgumentException("Unknown action type " + type);
            }
        }

        /// <summary>
        /// Determines if an action mapped to a string occurred.
        /// 
        /// Note that the function throws an exception if the action has not been added with
        /// AddNamedAction.
        /// </summary>
        /// <param name="action">The name of the action.</param>
        /// <param name="type">The type of action for which to check.</param>
        /// <returns>Whether the action occurred.</returns>
        public bool ActionOccurred(string action, InputActionType type)
        {
            InputAction actual;

            if (!_actions.TryGetValue(action, out actual))
                throw new ArgumentException("InputManager.ActionOccurred: No action with name " + action + " found.");

            return ActionOccurred(ref actual, type);
        }

        /// <summary>
        /// Adds a named action to the list.
        /// </summary>
        /// <param name="name">The name to associate with the action.</param>
        /// <param name="action">The action key and button.</param>
        public void AddNamedAction(string name, InputAction action)
        {
            _actions.Add(name, action);
        }

        /// <summary>
        /// Removes a named action from the list.
        /// </summary>
        /// <param name="name">The action's name.</param>
        public void RemoveNamedAction(string name)
        {
            _actions.Remove(name);
        }

        /// <summary>
        /// Determines whether the exit key or button (escape and back by default) was pressed.
        /// </summary>
        /// <returns>Whether the exit key or button was pressed.</returns>
        public bool ExitWasPressed()
        {
            return ActionOccurred(ref _exit, InputActionType.Pressed);
        }

        /// <summary>
        /// Determines whether the pause button/key was pressed.
        /// </summary>
        /// <returns>Whether the pause button/key was pressed.</returns>
        public bool PauseWasPressed()
        {
            return ActionOccurred(ref _pause, InputActionType.Pressed);
        }

        // Reset the input states
        public void ResetState()
        {
            _prevKState = new KeyboardState();
            _prevGState = new GamePadState();
            _prevMState = new MouseState();
            _curKState = new KeyboardState();
            _curGState = new GamePadState();
            _curMState = new MouseState();
        }
    }
}
