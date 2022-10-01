using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : Singleton<InputHandler>
{
    public float dir {
        get;
        private set;
    }
    private ButtonState m_move;
    public ButtonState move {
        get{return m_move;}
    }
    private ButtonState m_jump;
    public ButtonState jump {
        get{return m_jump;}
    }

    private ButtonState m_primary;
    public ButtonState primary {
        get {return m_primary;}
    }

    private ButtonState m_secondary;
    public ButtonState secondary {
        get {return m_secondary;}
    }

    private ButtonState m_dodge;
    public ButtonState dodge {
        get {return m_dodge;}
    }

    private ButtonState m_interact;
    public ButtonState interact {
        get {return m_interact;}
    }

    private ButtonState m_menu;
    public ButtonState menu {
        get {return m_menu;}
    }

    private void FixedUpdate() {
        this.m_move.Reset();
        this.m_jump.Reset();
        this.m_primary.Reset();
        this.m_secondary.Reset();
        this.m_dodge.Reset();
        this.m_interact.Reset();
        this.m_menu.Reset();

    }

    public void Move(InputAction.CallbackContext ctx) {
        this.dir = ctx.ReadValue<float>();
        this.m_move.Set(ctx);
    }

    public void Jump(InputAction.CallbackContext ctx) {
        this.m_jump.Set(ctx);
    }

    public void Primary(InputAction.CallbackContext ctx) {
        this.m_primary.Set(ctx);
    }

    public void Secondary(InputAction.CallbackContext ctx) {
        this.m_secondary.Set(ctx);
    }

    public void Dodge(InputAction.CallbackContext ctx) {
        this.m_dodge.Set(ctx);
    }

    public void Interact(InputAction.CallbackContext ctx) {
        this.m_interact.Set(ctx);
    }

    public void Menu(InputAction.CallbackContext ctx) {
        this.m_menu.Set(ctx);
    }

    public struct ButtonState {
        private bool firstFrame;
        public bool down {
            get;
            private set;
        }
        public bool pressed {
            get {
                return down && firstFrame;
            }
        }
        public bool released {
            get {
                return !down && firstFrame;
            }
        }

        public void Set(InputAction.CallbackContext ctx) {
            down = !ctx.canceled;             
            firstFrame = true;
        }
        public void Reset() {
            firstFrame = false;
        }
    }
}
