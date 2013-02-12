using System.Collections.Generic;
using System.Threading;
using System.Drawing;

namespace MezcladitosPWNer {

    public static class Globales {

        public static MainWindow frm;
        public static int minLength = 2;
        public static int maxLength = 18;
        public static bool startPWN = false;
        public static int siguiente = 0;
        public static string[][] cas;
        public static Point coordCasilla1;
        public static Point coordCasilla2;
        public static Point coordBtnIntroducir;
        public static int calibrado = 0;
        public static Dictionary<string, string> cadenas;
        public static Dictionary<string, string[]> arrays;
        public static Stack<string> palabrasEncontradas;
        public static Stack<List<string>> rutasEncontradas;
        public static Thread proceso;
        public static Thread esperarThreads;
        public static ManualResetEvent[] doneEvents;
        public static bool ctrlDOWN = false;
    }

}