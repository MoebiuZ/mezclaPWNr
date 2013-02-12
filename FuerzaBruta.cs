using MezcladitosPWNer;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;


namespace MezcladitosPWNer {

    public class FuerzaBruta {

        private ManualResetEvent _doneEvent;

        private string _letra;

        private int _x;
        private int _y;
        private List<string> _ruta;
        private string _palabra;

        private int[,] adyacentesA = { { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };
        private int[,] adyacentesB = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };
        private int[,] adyacentesC = { { 1, 0 }, { 1, 1 }, { 0, 1 } };
        private int[,] adyacentesD = { { 0, 1 }, { -1, 1 }, { -1, 0 } };
        private int[,] adyacentesE = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 } };
        private int[,] adyacentesF = { { 0, -1 }, { 1, -1 }, { 1, 0 } };
        private int[,] adyacentesG = { { -1, 0 }, { -1, -1 }, { 0, -1 } };
        private int[,] adyacentesH = { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 } };
        private int[,] adyacentesI = { { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 } };



        public FuerzaBruta(int x, int y, List<string> ruta, string palabra, ManualResetEvent doneEvent) {
            _doneEvent = doneEvent;
            _x = x;
            _y = y;
            _ruta = ruta;
            _palabra = palabra;

        }


        public void ThreadPoolCallback(Object threadContext) {
            int threadIndex = (int)threadContext;
            ady(_x, _y, _ruta, _palabra);
            _doneEvent.Set();
        }


        private void ady(int x, int y, List<string> pRuta, string palabra) {

            if (_doneEvent.WaitOne(0)) {
                return;
            }

            List<string> ruta;

            ruta = new List<string>(pRuta);


            Regex regPart;
            int palMaxTam;
            Match m;


            if (ruta.Capacity > Globales.maxLength - 1) {
                return;
            }

            ruta.Add(x.ToString() + y.ToString());
           // ruta.TrimExcess();

            palabra += Globales.cas[x][y];


            if (palabra.Length == 1) {
                _letra = palabra;
            }


            if (palabra.Length >= Globales.minLength) {

                palMaxTam = Globales.maxLength - palabra.Length;

                if (Globales.arrays[_letra].Contains(palabra)) {
                    Globales.frm.pwnWord(palabra, new List<string>(ruta));
                    Globales.frm.AsyncWriteLine(palabra + " ");
                }

                regPart = new Regex(@"#(" + palabra + "\\S{" + palMaxTam + "})#");
                m = regPart.Match(Globales.cadenas[_letra]);

                
                if (!m.Success) {
                    return;
                }
            }


            if (y > 0 && y < 4 - 1 && x > 0 && x < 4 - 1) {
                for (int i = 0; i < adyacentesA.GetLength(0); i++) {
                    profundizar(x + adyacentesA[i, 0], y + adyacentesA[i, 1], ruta, palabra);
                }

            }

            if (y == 0) {
                if (x > 0 && x < 4 - 1) {
                    for (int i = 0; i < adyacentesB.GetLength(0); i++) {
                        profundizar(x + adyacentesB[i, 0], y + adyacentesB[i, 1], ruta, palabra);
                    }

                } else if (x == 0) {
                    for (int i = 0; i < adyacentesC.GetLength(0); i++) {
                        profundizar(x + adyacentesC[i, 0], y + adyacentesC[i, 1], ruta, palabra);
                    }


                } else if (x == 4 - 1) {

                    for (int i = 0; i < adyacentesD.GetLength(0); i++) {

                        profundizar(x + adyacentesD[i, 0], y + adyacentesD[i, 1], ruta, palabra);
                    }
                }
            }

            if (y == 4 - 1) {
                if (x > 0 && x < 4 - 1) {

                    for (int i = 0; i < adyacentesE.GetLength(0); i++) {
                        profundizar(x + adyacentesE[i, 0], y + adyacentesE[i, 1], ruta, palabra);
                    }

                } else if (x == 0) {

                    for (int i = 0; i < adyacentesF.GetLength(0); i++) {
                        profundizar(x + adyacentesF[i, 0], y + adyacentesF[i, 1], ruta, palabra);
                    }


                } else if (x == 4 - 1) {

                    for (int i = 0; i < adyacentesG.GetLength(0); i++) {
                        profundizar(x + adyacentesG[i, 0], y + adyacentesG[i, 1], ruta, palabra);
                    }
                }
            }

            if (x == 0 && y != 0 && y != 4 - 1) {

                for (int i = 0; i < adyacentesH.GetLength(0); i++) {
                    profundizar(x + adyacentesH[i, 0], y + adyacentesH[i, 1], ruta, palabra);
                }
            }

            if (x == 4 - 1 && y != 0 && y != 4 - 1) {

                for (int i = 0; i < adyacentesI.GetLength(0); i++) {
                    profundizar(x + adyacentesI[i, 0], y + adyacentesI[i, 1], ruta, palabra);
                }
            }
        }



        private void profundizar(int x, int y, List<string> ruta, String palabra) {
            if (ruta.LastIndexOf(x.ToString() + y.ToString()) == -1) {
                ady(x, y, ruta, palabra);
            }
        }


    }
}