//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/Input/TetrisInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Tetris
{
    public partial class @TetrisInput : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @TetrisInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""TetrisInput"",
    ""maps"": [
        {
            ""name"": ""Game"",
            ""id"": ""c3cc1bd8-b16a-4e44-8d08-c62bc6a81a18"",
            ""actions"": [
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""ccfa196d-97b3-48d6-9e4d-e67ec8795621"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""1e2bc92b-3a84-4222-a245-43a974f38250"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Fall"",
                    ""type"": ""Button"",
                    ""id"": ""c87740bb-1279-4d05-91f7-14478e34d7f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hold"",
                    ""type"": ""Button"",
                    ""id"": ""0bf9f8aa-8ff0-4c0f-9cb4-1fd6eb6a5011"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Drop"",
                    ""type"": ""Button"",
                    ""id"": ""ebc285da-97a4-40b6-addd-c399424f75ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""6aee0a60-a0ea-4791-8b31-6a314d529bca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""RightHand"",
                    ""id"": ""93f0d420-b9df-4ea3-8269-efa46b4545bf"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""24dc89ed-b38f-4df7-b730-9dbeac13b77f"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""94d8bf10-0ee3-401d-839b-41ee90ecc606"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftHand"",
                    ""id"": ""517e9a3c-e490-47fa-9bd6-a079b6813a65"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""84da3fb8-f1b5-4684-bf46-871ad9f1071e"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""498d227f-f387-4f50-9716-22e595ef805a"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""de58a940-885b-4fd8-9165-fed762d68f2e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""ddf1cc3d-9f43-47d8-aefe-04a5efc858bd"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e43afb0b-cd94-4d0e-8e78-deb314635d7b"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3494c8b2-fb68-4a89-ae1b-7f5d9a496909"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fall"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""43ac578d-8d9e-4cfb-aa93-dd46ca641b53"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9f383094-08a6-4a2e-8266-c9b83bcd3bf5"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db072e95-565e-4bd4-98c0-eb948fc2d31b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""26f92a24-d29a-4251-8532-4d4769d87769"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d22991dd-b997-4257-b341-f5a1dcc0fa0c"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Game
            m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
            m_Game_Rotate = m_Game.FindAction("Rotate", throwIfNotFound: true);
            m_Game_Move = m_Game.FindAction("Move", throwIfNotFound: true);
            m_Game_Fall = m_Game.FindAction("Fall", throwIfNotFound: true);
            m_Game_Hold = m_Game.FindAction("Hold", throwIfNotFound: true);
            m_Game_Drop = m_Game.FindAction("Drop", throwIfNotFound: true);
            m_Game_Pause = m_Game.FindAction("Pause", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }
        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Game
        private readonly InputActionMap m_Game;
        private IGameActions m_GameActionsCallbackInterface;
        private readonly InputAction m_Game_Rotate;
        private readonly InputAction m_Game_Move;
        private readonly InputAction m_Game_Fall;
        private readonly InputAction m_Game_Hold;
        private readonly InputAction m_Game_Drop;
        private readonly InputAction m_Game_Pause;
        public struct GameActions
        {
            private @TetrisInput m_Wrapper;
            public GameActions(@TetrisInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Rotate => m_Wrapper.m_Game_Rotate;
            public InputAction @Move => m_Wrapper.m_Game_Move;
            public InputAction @Fall => m_Wrapper.m_Game_Fall;
            public InputAction @Hold => m_Wrapper.m_Game_Hold;
            public InputAction @Drop => m_Wrapper.m_Game_Drop;
            public InputAction @Pause => m_Wrapper.m_Game_Pause;
            public InputActionMap Get() { return m_Wrapper.m_Game; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
            public void SetCallbacks(IGameActions instance)
            {
                if (m_Wrapper.m_GameActionsCallbackInterface != null)
                {
                    @Rotate.started -= m_Wrapper.m_GameActionsCallbackInterface.OnRotate;
                    @Rotate.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnRotate;
                    @Rotate.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnRotate;
                    @Move.started -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                    @Fall.started -= m_Wrapper.m_GameActionsCallbackInterface.OnFall;
                    @Fall.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnFall;
                    @Fall.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnFall;
                    @Hold.started -= m_Wrapper.m_GameActionsCallbackInterface.OnHold;
                    @Hold.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnHold;
                    @Hold.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnHold;
                    @Drop.started -= m_Wrapper.m_GameActionsCallbackInterface.OnDrop;
                    @Drop.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnDrop;
                    @Drop.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnDrop;
                    @Pause.started -= m_Wrapper.m_GameActionsCallbackInterface.OnPause;
                    @Pause.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnPause;
                    @Pause.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnPause;
                }
                m_Wrapper.m_GameActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Rotate.started += instance.OnRotate;
                    @Rotate.performed += instance.OnRotate;
                    @Rotate.canceled += instance.OnRotate;
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Fall.started += instance.OnFall;
                    @Fall.performed += instance.OnFall;
                    @Fall.canceled += instance.OnFall;
                    @Hold.started += instance.OnHold;
                    @Hold.performed += instance.OnHold;
                    @Hold.canceled += instance.OnHold;
                    @Drop.started += instance.OnDrop;
                    @Drop.performed += instance.OnDrop;
                    @Drop.canceled += instance.OnDrop;
                    @Pause.started += instance.OnPause;
                    @Pause.performed += instance.OnPause;
                    @Pause.canceled += instance.OnPause;
                }
            }
        }
        public GameActions @Game => new GameActions(this);
        public interface IGameActions
        {
            void OnRotate(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnFall(InputAction.CallbackContext context);
            void OnHold(InputAction.CallbackContext context);
            void OnDrop(InputAction.CallbackContext context);
            void OnPause(InputAction.CallbackContext context);
        }
    }
}