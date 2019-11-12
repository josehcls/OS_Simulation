using System;
using System.Collections.Generic;
using System.Text;

namespace OS_Simulation
{
    public class SaidaRotina
    {
        public List<Evento> EventosInternos { get; set; }
        public List<Evento> EventosPrioritarios { get; set; }
        public List<Evento> EventosExternos { get; set; }

        public SaidaRotina(List<Evento> eventosInternos, List<Evento> eventosPrioritarios, List<Evento> eventosExternos)
        {
            EventosInternos = eventosInternos;
            EventosPrioritarios = eventosPrioritarios;
            EventosExternos = eventosExternos;
        }
    }

}
