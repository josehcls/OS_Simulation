using OS_Simulation.DISK;
using OS_Simulation.MEM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OS_Simulation.Base
{
    public class SistemaOperacional
    {
        public List<Evento> Eventos { get; set; }
        public int InstanteDeSimulacao { get; set; }
        public CM CM { get; set; }
        public CPU CPU { get; set; }
        public Disk Disk { get; set; }


        public void Simulacao (List<ChegadaPrograma> chegadaProgramas, int inicioSimulacao = 0, int fimSimulacao = -1)
        {
            InstanciarDispositivos();
            ChegadaPrograma primeiroPrograma = chegadaProgramas.OrderBy(c => c.InstanteChegada).First();
            chegadaProgramas.Remove(primeiroPrograma);
            AgendarPrograma(primeiroPrograma.InstanteChegada, TipoEvento.ARRIVAL, primeiroPrograma.Programa);
            while (Scheduler()) ;
        }

        void InstanciarDispositivos()
        {
            CM = new CM(1000);
            CPU = new CPU();
            Disk = new Disk();
        }

        void AgendarPrograma(int instanteChegada, TipoEvento tipoEvento, Programa programa)
        {
            Evento evento = new Evento(instanteChegada, tipoEvento, programa);
            Eventos.Add(evento);
        }

        bool Scheduler()
        {
            
        }
    }
}
