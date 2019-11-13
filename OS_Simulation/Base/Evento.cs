using System;
using System.Collections.Generic;
using System.Text;

namespace OS_Simulation.Base
{
    public class Evento
    {
        public int InstanteChegada { get; set; }
        public TipoEvento Tipo { get; set; }
        public Programa Programa { get; set; }
        //public int Prioridade { get; set; }

        public Evento (int instanteChegada, TipoEvento tipoEvento, Programa programa)
        {
            InstanteChegada = instanteChegada;
            Tipo= tipoEvento;
            Programa = programa;
        }
    }
}
