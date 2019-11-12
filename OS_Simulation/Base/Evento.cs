using System;
using System.Collections.Generic;
using System.Text;

namespace OS_Simulation
{
    public class Evento
    {
        public int InstanteChegada { get; set; }
        public TipoEvento Tipo { get; set; }
        public int Prioridade { get; set; }
        public string Tarefa { get; set; }
        public object Conteudo { get; set; }

    }
}
