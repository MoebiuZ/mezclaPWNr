using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MezcladitosPWNer {
    public partial class MainWindow : Form {

        private static InputHooks.LowLevelMouseProc _mCallback = mouseHookCallback;
        private static InputHooks.LowLevelKeyboardProc _kCallback = keyboardHookCallback;
        public static IntPtr mouseHookID = IntPtr.Zero;
        public static IntPtr keyboardHookID = IntPtr.Zero;


        public MainWindow() {
            InitializeComponent();
            Globales.frm = this;
            this.FormClosing += onCerrar;
        }



        private void onCerrar(Object sender, FormClosingEventArgs e) {
            InputHooks.UnhookWindowsHookEx(mouseHookID);
            InputHooks.UnhookWindowsHookEx(keyboardHookID);
        }



        private void iniciar() {

            Thread esperar;
            FuerzaBruta brutus;

            string palabra;
            List<string> ruta;

            Globales.doneEvents = new ManualResetEvent[4 * 4];

            Globales.cas = new string[4][];
            Globales.cas[0] = new string[] { "", "", "", "" };
            Globales.cas[1] = new string[] { "", "", "", "" };
            Globales.cas[2] = new string[] { "", "", "", "" };
            Globales.cas[3] = new string[] { "", "", "", "" };

            int k = 0;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    Globales.cas[i][j] = tbLetras.Text[k].ToString();
                    k++;
                }
            }


            actualizarCasilleroForm();

            Globales.palabrasEncontradas = new Stack<string>();
            Globales.rutasEncontradas = new Stack<List<string>>();

            cargarDiccionario(Globales.cas);


            MessageBox.Show("(CTRL+Click) en el centro de la primera casilla, luego segunda casilla, luego botón introducir");

            AsyncHook();

            while (!Globales.startPWN) {}

            AsyncUnHook();

            Globales.calibrado = Globales.coordCasilla2.X - Globales.coordCasilla1.X;

            k = 0;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {

                    Globales.doneEvents[k] = new ManualResetEvent(false);
                    brutus = new FuerzaBruta(i, j, new List<string>(), "", Globales.doneEvents[k]);
                    ThreadPool.QueueUserWorkItem(brutus.ThreadPoolCallback, k);
                    k++;
                }
            }

            esperar = new Thread(new ParameterizedThreadStart(esperarTodos));
            esperar.Start(Globales.doneEvents);
            esperar.Join();

            while (Globales.palabrasEncontradas.Count > 0) {
                palabra = Globales.palabrasEncontradas.Pop();
                ruta = Globales.rutasEncontradas.Pop();

                for (int i = 0; i < ruta.Count; i++) {
                    InputHooks.ClickLeftMouseButton(Globales.coordCasilla1.X + (Globales.calibrado * Convert.ToInt32(ruta[i][1].ToString())),
                                               Globales.coordCasilla1.Y + (Globales.calibrado * Convert.ToInt32(ruta[i][0].ToString())));
                    System.Threading.Thread.Sleep(50);
                }
                System.Threading.Thread.Sleep(50);
                InputHooks.ClickLeftMouseButton(Globales.coordBtnIntroducir.X, Globales.coordBtnIntroducir.Y);
                System.Threading.Thread.Sleep(50);
            }
        }


        public void esperarTodos(object eventos) {
            ManualResetEvent[] evs = eventos as ManualResetEvent[];
            WaitHandle.WaitAll(evs);
        }


        private void cargarDiccionario(string[][] casillero) {

            string cadLetras;
            string[] lista;

            Globales.cadenas = new Dictionary<string, string>();
            Globales.arrays = new Dictionary<string, string[]>();


            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {

                    if (!Globales.arrays.ContainsKey(casillero[i][j])) {

                        lista = File.ReadAllLines(@"Diccionario/" + casillero[i][j] + ".txt");
                        Globales.arrays.Add(casillero[i][j], lista);

                        cadLetras = String.Join("#", lista);
                        cadLetras = "#" + cadLetras + "#";

                        Globales.cadenas.Add(casillero[i][j], cadLetras);
                    }
                }
            }
            lista = null;
            GC.Collect();
        }


        private void butIniciar_Click(object sender, EventArgs e) {
            Globales.esperarThreads = new Thread(new ThreadStart(iniciar));
            Globales.esperarThreads.Start();
        }



        public void AsyncHook() {
            this.BeginInvoke(new MethodInvoker(delegate {
                mouseHookID = InputHooks.SetMouseHook(_mCallback);
                keyboardHookID = InputHooks.SetKeyboardHook(_kCallback);
            }));
        }


        public void AsyncUnHook() {
            this.BeginInvoke(new MethodInvoker(delegate {
                InputHooks.UnhookWindowsHookEx(mouseHookID);
                InputHooks.UnhookWindowsHookEx(keyboardHookID);
            }));
        }

        public void AsyncWriteLine(String Text) {
            this.BeginInvoke(new MethodInvoker(delegate {
                rTBEncontradas.AppendText(Text);
                rTBEncontradas.ScrollToCaret();
            }));
        }

        public void pwnWord(string palabra, List<string> ruta) {
            this.BeginInvoke(new MethodInvoker(delegate {
                if (!Globales.palabrasEncontradas.Contains(palabra)) {
                    Globales.palabrasEncontradas.Push(palabra);
                    Globales.rutasEncontradas.Push(ruta);
                }
            }));
        }

        public static IntPtr mouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0 && InputHooks.MouseMessages.WM_LBUTTONDOWN == (InputHooks.MouseMessages)wParam && Globales.ctrlDOWN == true) {
                InputHooks.MSLLHOOKSTRUCT hookStruct = (InputHooks.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(InputHooks.MSLLHOOKSTRUCT));
               if (Globales.siguiente == 0) {
                    Globales.coordCasilla1 = hookStruct.pt;
                } else if (Globales.siguiente == 1) {
                    Globales.coordCasilla2 = hookStruct.pt;
                } else if (Globales.siguiente == 2) {
                    Globales.coordBtnIntroducir = hookStruct.pt;
                    Globales.startPWN = true;
                }
                Globales.siguiente++;
                
            }
            
            return InputHooks.CallNextHookEx(mouseHookID, nCode, wParam, lParam);
        }


        public static IntPtr keyboardHookCallback(int nCode, IntPtr wParam, ref InputHooks.KEYBDINPUT lParam) {

            if (nCode >= 0 && InputHooks.KeyboardMessages.WM_KEYDOWN == (InputHooks.KeyboardMessages)wParam && lParam.wVk == 0xA2) {
                if (Globales.ctrlDOWN == false) {
                    Globales.ctrlDOWN = true;
               }
                
            } else if (nCode >= 0 && InputHooks.KeyboardMessages.WM_KEYUP == (InputHooks.KeyboardMessages)wParam && lParam.wVk == 0xA2) {
                if (Globales.ctrlDOWN == true) {
                    Globales.ctrlDOWN = false;
                }
            }

            return InputHooks.CallNextHookEx(keyboardHookID, nCode, wParam, ref lParam);
        }

        public void actualizarCasilleroForm() {
            this.BeginInvoke(new MethodInvoker(delegate {
                lCas00.Text = Globales.cas[0][0].ToUpper();
                lCas01.Text = Globales.cas[0][1].ToUpper();
                lCas02.Text = Globales.cas[0][2].ToUpper();
                lCas03.Text = Globales.cas[0][3].ToUpper();
                lCas10.Text = Globales.cas[1][0].ToUpper();
                lCas11.Text = Globales.cas[1][1].ToUpper();
                lCas12.Text = Globales.cas[1][2].ToUpper();
                lCas13.Text = Globales.cas[1][3].ToUpper();
                lCas20.Text = Globales.cas[2][0].ToUpper();
                lCas21.Text = Globales.cas[2][1].ToUpper();
                lCas22.Text = Globales.cas[2][2].ToUpper();
                lCas23.Text = Globales.cas[2][3].ToUpper();
                lCas30.Text = Globales.cas[3][0].ToUpper();
                lCas31.Text = Globales.cas[3][1].ToUpper();
                lCas32.Text = Globales.cas[3][2].ToUpper();
                lCas33.Text = Globales.cas[3][3].ToUpper();

            }));
        }

    }
}
