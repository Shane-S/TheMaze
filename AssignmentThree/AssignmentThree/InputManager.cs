using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace Lab3
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
        public Keys ActionKey;

        /// <summary>
        /// The button corresponding to the action.
        /// </summary>
        public Buttons ActionButton;

        /// <summary>
        /// Constructs a new InputAction with the specified values.
        /// </summary>
        /// <param name="ActionKey">The key corresponding to this action.</param>
        /// <param name="ActionButton">The button corresponding to this action.</param>
        public InputAction(Keys ActionKey, Buttons ActionButton)
        {
            this.ActionKey = ActionKey;
            this.ActionButton = ActionButton;
        }
    }

    public class InputManager
    {
        public const int NOT_MOVING = 0;
        public const int MOVING_RIGHT = 1;
        public const int MOVING_LEFT = -1;

        public static readonly InputAction DEFAULT_ESCAPE = new InputAction(Keys.Escape, Buttons.Back);
        public static readonly InputAction DEFAULT_RIGHT = new InputAction(Keys.Right, Buttons.RightShoulder);
        public static readonly InputAction DEFAULT_LEFT = new InputAction(Keys.Left, Buttons.LeftShoulder);
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
        private InputAction _leftMove;

        /// <summary>
        /// The input that will cause the player to move right.
        /// </summary>
        private InputAction _rightMove;

        /// <summary>
        /// A mapping of named actions to InputActions.
        /// </summary>
        private Dictionary<string, InputAction> _actions;

        /// <summary>
        /// Gets or sets the input that will cause the player to move left.
        /// </summary>
        public InputAction LeftMove { get { return _leftMove; } set { _leftMove = value; } }
        
        /// <summary>
        /// Gets or sets the input that will cause the player to move right.
        /// </summary>
        public InputAction RightMove { get { return _rightMove; } set { _rightMove = value; } }

        /// <summary>
        /// Gets or sets the input that will cause the game to exit.
        /// </summary>
        public InputAction Exit { get { return _exit; } set { _exit = value; } }

        public InputManager()
            : this(DEFAULT_RIGHT, DEFAULT_LEFT, DEFAULT_ESCAPE)
        { }

        public InputManager(InputAction rightMove, InputAction leftMove)
            : this(rightMove, leftMove, DEFAULT_ESCAPE)
        {

        }

        /// <summary>
        /// Create an InputManager with the specified escape key and button.
        /// </summary>
        /// <param name="escapeKey"></param>
        /// <param name="escapeButton"></param>
        public InputManager(InputAction rightMove, InputAction leftMove, InputAction exit)
            : base()
        {
            _prevKState = new KeyboardState();
            _prevGState = new GamePadState();
            _curKState = new KeyboardState();
            _curGState = new GamePadState();

            _rightMove = rightMove;
            _leftMove = leftMove;
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
        /// Determines whether a key was pressed last frame.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>Whether the key k was pressed last frame.</returns>
        public bool KeyWasPressed(Keys k)
        {
            return _prevKState.IsKeyDown(k) && _curKState.IsKeyUp(k);
        }

        /// <summary>
        /// Determines whether the specified button on the gamepad was pressed.
        /// </summary>
        /// <param name="b">The button to check.</param>
        /// <returns>Whether button b was pressed last frame.</returns>
        public bool ButtonWasPressed(Buttons b)
        {
            return _prevGState.IsButtonDown(b) && _curGState.IsButtonUp(b);
        }

        /// <summary>
        /// Determines whether a key was held down between two updates.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>Whether the key was held.</returns>
        public bool KeyHeld(Keys k)
        {
            return _curKState.IsKeyDown(k) && _prevKState.IsKeyDown(k);
        }

        /// <summary>
        /// Determines whether a button was held down between updates.
        /// </summary>
        /// <param name="b">The button to check.</param>
        /// <returns>Whether the button was held down.</returns>
        public bool ButtonHeld(Buttons b)
        {
            return _curGState.IsButtonDown(b) && _prevGState.IsButtonDown(b);
        }

        /// <summary>
        /// Determines whether a key was up between two updates.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>Whether the key was up.</returns>
        public bool KeyStillUp(Keys k)
        {
            return _curKState.IsKeyUp(k) && _prevKState.IsKeyUp(k);
        }

        /// <summary>
        /// Determines whether a button was up between updates.
        /// </summary>
        /// <param name="b">The button to check.</param>
        /// <returns>Whether the button was up.</returns>
        public bool ButtonStillUp(Buttons b)
        {
            return _curGState.IsButtonUp(b) && _prevGState.IsButtonUp(b);
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
                    return KeyWasPressed(action.ActionKey) || ButtonWasPressed(action.ActionButton);
                case InputActionType.StillDown:
                    return KeyHeld(action.ActionKey) || ButtonHeld(action.ActionButton);
                case InputActionType.StillUp:
                    return KeyStillUp(action.ActionKey) || ButtonStillUp(action.ActionButton);
                case InputActionType.Down:
                    return _curKState.IsKeyDown(action.ActionKey) || _curGState.IsButtonDown(action.ActionButton);
                case InputActionType.Up:
                    return _curKState.IsKeyUp(action.ActionKey) || _curGState.IsButtonDown(action.ActionButton);
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
