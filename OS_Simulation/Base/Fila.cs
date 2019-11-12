using System;
using System.Collections.Generic;
using System.Text;

namespace OS_Simulation
{
    public class Fila
    {
        public int MaximoTempoDeEspera { get; set; }
        public int AcumuladorDeTempoDeEspera { get; set; }
        public int TamanhoMaximoFila { get; set; }
        public int TamanhoFilaAtual { get; set; }
        public int InstanteUltimaModificacao { get; set; }
        public LinkedList<Evento> Eventos { get; set; }
        public int NumeroEventos { get; set; }
    }

    // Ordenação por Prioridade, Ordem de Chegada

    // Processos ficam na Fila aguardando autorização para uso de recurso
}

