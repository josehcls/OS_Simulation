using System;
using System.Collections.Generic;
using System.Text;

namespace OS_Simulation.MEM
{
    public class Particao
    {
        public int PosicaoInicial { get; set; }
        public int Tamanho { get; set; }
        public Programa Programa { get; set; }
        public bool Livre { get; set; }

        public Particao(int posicaoInicial, int tamanho, Programa programa = null)
        {
            PosicaoInicial = posicaoInicial;
            Tamanho = tamanho;
            Programa = programa;
            Livre = programa == null;
        }

        public int PosicaoFinal()
        {
            return PosicaoInicial + Tamanho - 1;
        }

        public void Liberar()
        {
            Programa = null;
            Livre = true;
        }
    }
}
