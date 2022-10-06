using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : Singleton<InputHandler>
{
    [SerializeField] protected short bufferFrames = 5;
    [SerializeField] protected bool bufferEnabled = false;
    public float dir {
        get;
        private set;
    }
    private ButtonState m_move = new();
    public ButtonState move {
        get{return m_move;}
    }
    private ButtonState m_jump = new();
    public ButtonState jump {
        get{return m_jump;}
    }

    private ButtonState m_primary = new();
    public ButtonState primary {
        get {return m_primary;}
    }

    private ButtonState m_secondary = new();
    public ButtonState secondary {
        get {return m_secondary;}
    }

    private ButtonState m_dodge = new();
    public ButtonState dodge {
        get {return m_dodge;}
    }

    private ButtonState m_interact = new();
    public ButtonState interact {
        get {return m_interact;}
    }

    private ButtonState m_menu = new();
    public ButtonState menu {
        get {return m_menu;}
    }

    private ButtonState m_any = new();
    public ButtonState any {
        get {return m_any;}
    }
    private void FixedUpdate() {
        this.m_move.Reset();
        this.m_jump.Reset();
        this.m_primary.Reset();
        this.m_secondary.Reset();
        this.m_dodge.Reset();
        this.m_interact.Reset();
        this.m_menu.Reset();
        this.m_any.Reset();

        if (bufferEnabled) {
            ButtonState.UpdateBuffer(bufferFrames);
        }
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

    public void Any(InputAction.CallbackContext ctx) {
        this.m_any.Set(ctx);
    }

    public void FlushBuffer() {
        ButtonState.FlushBuffer();
    }

    public class ButtonState {
        private short id = IDSRC++;
        private static short IDSRC = 0;
        private static short    STATE_PRESSED = 0,
                                STATE_RELEASED = 1;
        private static Queue<Dictionary<short, short>> inputBuffer = new Queue<Dictionary<short, short>>();
        private static Dictionary<short, short> currentFrame;
        public static void UpdateBuffer(short bufferFrames) {
            //PrintBuffer();
            if (inputBuffer.Count >= bufferFrames)
                inputBuffer.Dequeue();
            currentFrame = new Dictionary<short, short>();
            inputBuffer.Enqueue(currentFrame);
        }

        public static void FlushBuffer() {
            inputBuffer.Clear();
        }

        public static void PrintBuffer() {
            string bufferData = $"InputBuffer: count-{inputBuffer.Count}";
            foreach (var frame in inputBuffer)
                if (frame.Count > 0)
                    bufferData += $"\n{frame.Count}";
            Debug.Log(bufferData);
        }

        private bool firstFrame;
        public bool down {
            get;
            private set;
        }
        public bool pressed {
            get {
                if (InputHandler.Instance.bufferEnabled && inputBuffer.Count > 0) {
                    foreach (var frame in inputBuffer) {
                        if (frame.ContainsKey(id) && frame[id] == STATE_PRESSED) {
                            Debug.Log(id);
                            return frame.Remove(id);
                        }
                    }
                    return false;
                }
                return down && firstFrame;
            }
        }

        public bool released {
            get {
                if (InputHandler.Instance.bufferEnabled && inputBuffer.Count > 0) {
                    foreach (var frame in inputBuffer) {
                        if (frame.ContainsKey(id) && frame[id] == STATE_RELEASED) {
                            Debug.Log(id);
                            return frame.Remove(id);
                        }
                    }
                    return false;
                }
                return !down && firstFrame;
            }
        }

        public void Set(InputAction.CallbackContext ctx) {
            down = !ctx.canceled;             
            firstFrame = true;

            if (InputHandler.Instance.bufferEnabled && inputBuffer.Count > 0) {
                currentFrame.TryAdd(id, down ? STATE_PRESSED : STATE_RELEASED);
            }
        }
        public void Reset() {
            firstFrame = false;
        }
    }
}
