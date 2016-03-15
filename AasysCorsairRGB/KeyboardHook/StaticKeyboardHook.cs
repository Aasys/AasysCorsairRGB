using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using AasysCorsairRGB.KeyboardHook;

namespace AasysCorsairRGB
{
    class StaticKeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static bool lastKeyWasLetter = false;

        private static KeyCombo _currentCombo = new KeyCombo();

        private static Thread _bgThread = new Thread(new ThreadStart(Initialize));
        private static Boolean? _running = null;
        private static List<IGlobalKeyboardListener> _keyPressListeners = new List<IGlobalKeyboardListener>();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [STAThread]
        private static void Initialize()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _hookID = SetHook(_proc);

            Application.Run();

            UnhookWindowsHookEx(_hookID);
        }

        public static void Start()
        {
            if (!_running.HasValue)
            {
                _bgThread.Start();
                _running = true;
            }
            else if (!_running.Value)
            {
                _bgThread.Resume();
                _running = true;
            }
        }

        public static void Stop()
        {
            if (!_running.HasValue)
                return;
            else if (_running.Value)
            {
                _bgThread.Suspend();
                _running = false;
            }
        }

        public static void AddListner(IGlobalKeyboardListener listener)
        {
            _keyPressListeners.Add(listener);
        }

        public static void RemoveListner(IGlobalKeyboardListener listener)
        {
            _keyPressListeners.Remove(listener);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            
            Keys key = (Keys) Marshal.ReadInt32(lParam);
            if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
            {
                _currentCombo.AddKey(key);
                Console.WriteLine(_currentCombo);
                foreach (var keyPressListener in _keyPressListeners)
                {
                    try
                    {
                        keyPressListener.OnKeyPressed(_currentCombo.Clone());
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
            }
            else
            {
                _currentCombo.RemoveKey(key);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

    }
}
