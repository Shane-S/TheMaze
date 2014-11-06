using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

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
        /// <param name="ActionKey">The key corresponding to this action.</param>
        /// <param name="ActionButton">The button corresponding to this action.</param>
        public InputAction(Keys[] ActionKeys, Buttons[] ActionButtons)
        {
            this.ActionKeys = ActionKeys;
            this.ActionButtons = ActionButtons;
        }

        public InputAction(Keys ActionKey, Buttons ActionButton)
            : this(new Keys[] { ActionKey }, new Buttons[] { ActionButton })
        {}
    }

    public class InputManager
    {
        public const int NOT_MOVING = 0;
        public const int MOVING_RIGHT = 1;
        public const int MOVING_LEFT = -1;

        public static readonly InputAction DEFAULT_ESCAPE = new InputAction(Keys.Escape, Buttons.Back);
        public static readonly InputAction DEFAULT_PAUSE = new InputAction(Keys.Home, Buttons.Start);
        /// <summary>
        /// The keyboard state in the last frame.
        /// </summary>
        private KeyboardState _prevKState;

        /// <summary>
        /// The gamepad state in the last frame.
        /// </summary>
        private GamePadState _prevGState;

        /// <summary>
        /// The keyboard state in the current frame.
        /// </summary>
        private KeyboardState _curKState;

        /// <summary>
        /// The gamepad state in the current frame.
        /// </summary>
        private GamePadState _curGState;

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
        /// Gets or sets the input that will cause the game to exit.
        /// </summary>
        public InputAction Exit { get { return _exit; } set { _exit = value; } }

        public InputAction Pause { get { return _pause; } set { _pause = value; } }

        public InputManager()
            : this(DEFAULT_PAUSE, DEFAULT_ESCAPE)
        { }

        /// <summary>
        /// Create an InputManager with the specified escape key and button.
        /// </summary>
        /// <param name="escapeKey"></param>
        /// <param name="escapeButton"></param>
        public InputManager(InputAction pause, InputAction exit)
            : base()
        {
            _prevKState = new KeyboardState();
            _prevGState = new GamePadState();
            _curKState = new KeyboardState();
            _curGState = new GamePadState();

            _pause = pause;
            _exit = exit;
            _actions = new Dictionary<string, InputAction>();
        }

        /// <summary>
        /// Updates the input states wrapped by the InputManager.
        /// </summary>
        /// <param name="gState">The current GamePadState for a given player.</param>
        /// <param name="kState">The current KeyboardState.</param>
        public void Update(GamePadState gState, KeyboardState kState)
        {
            _prevKState = _curKState;
            _prevGState = _curGState;
            _curKState = kState;
            _curGState = gState;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was pressed last frame.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was pressed last frame.</returns>
        public bool KeyWasPressed(Keys[] k)
        {
            int i;
            for (i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyDown(k[i]) && _curKState.IsKeyUp(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of gamepad buttons was pressed.
        /// </summary>
        /// <param name="b">The buttons to check.</param>
        /// <returns>Whether at least one of the buttons was pressed last frame.</returns>
        public bool ButtonWasPressed(Buttons[] b)
        {
            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_prevGState.IsButtonDown(b[i]) && _curGState.IsButtonUp(b[i]))
                    break;
            }
            return i != b.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was held down between two updates.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was held.</returns>
        public bool KeyHeld(Keys[] k)
        {
            int i;
            for (i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyDown(k[i]) && _curKState.IsKeyDown(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of buttons was held down between updates.
        /// </summary>
        /// <param name="b">The buttons to check.</param>
        /// <returns>Whether at least one of the buttons was held down.</returns>
        public bool ButtonHeld(Buttons[] b)
        {
            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_prevGState.IsButtonDown(b[i]) && _curGState.IsButtonDown(b[i]))
                    break;
            }

            return i != b.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up between two updates.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was still up.</returns>
        public bool KeyStillUp(Keys[] k)
        {
            int i;
            for (i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyUp(k[i]) && _curKState.IsKeyUp(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether a button was up between updates.
        /// </summary>
        /// <param name="b">The button to check.</param>
        /// <returns>Whether the button was up.</returns>
        public bool ButtonStillUp(Buttons[] b)
        {
            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_curGState.IsButtonUp(b[i]) && _prevGState.IsButtonUp(b[i]))
                    break;
            }

            return i != b.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up between two updates.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was up.</returns>
        public bool KeyIsDown(Keys[] k)
        {
            int i;
            for(i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyDown(k[i]) || _curKState.IsKeyDown(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up between two updates.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was up.</returns>
        public bool KeyIsUp(Keys[] k)
        {
            int i;
            for (i = 0; i < k.Length; i++)
            {
                if (_prevKState.IsKeyUp(k[i]) || _curKState.IsKeyUp(k[i]))
                    break;
            }
            return i != k.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up between two updates.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was up.</returns>
        public bool ButtonIsUp(Buttons[] b)
        {
            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_prevGState.IsButtonUp(b[i]) || _curGState.IsButtonUp(b[i]))
                    break;
            }
            return i != b.Length;
        }

        /// <summary>
        /// Determines whether at least one of a set of keys was up between two updates.
        /// </summary>
        /// <param name="k">The keys to check.</param>
        /// <returns>Whether at least one of the keys was up.</returns>
        public bool ButtonIsDown(Buttons[] b)
        {
            int i;
            for (i = 0; i < b.Length; i++)
            {
                if (_prevGState.IsButtonDown(b[i]) || _curGState.IsButtonDown(b[i]))
                    break;
            }
            return i != b.Length;
        }

        /// <summary>
        /// Determines whether the specified action occurred.
        /// </summary>
        /// <param name="action">The action button/key to check.</param>
        /// <param name="type">The type of action for which to check.</param>
        /// <returns></returns>
        public bool ActionOccurred(InputAction action, InputActionType type)
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

            return ActionOccurred(actual, type);
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
            return ActionOccurred(_exit, InputActionType.Pressed);
        }

        /// <summary>
        /// Determines whether the pause button was pressed.
        /// </summary>
        /// <returns></returns>
        public bool PauseWasPressed()
        {
            return ActionOccurred(_pause, InputActionType.Pressed);
        }

        // Reset the input states
        public void ResetState()
        {
            _prevKState = new KeyboardState();
            _prevGState = new GamePadState();
            _curKState = new KeyboardState();
            _curGState = new GamePadState();
        }
    }
}
