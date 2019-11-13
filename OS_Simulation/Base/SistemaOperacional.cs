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
        public LinkedList<Evento> Eventos { get; set; }
        public int InstanteDeSimulacao { get; set; }
        public CM CM { get; set; }
        public CPU CPU { get; set; }
        public Disk Disk { get; set; }
        public Queue<ChegadaPrograma> FilaProgramas { get; set; }
        public int InicioSimulacao { get; set; }
        public int FimSimulacao { get; set; }
        public Dictionary<TipoEvento, Action<Evento>> RotinasTratamento;


        public SistemaOperacional(List<ChegadaPrograma> chegadaProgramas, int inicioSimulacao = 0, int fimSimulacao = -1)
        {
            FilaProgramas = new Queue<ChegadaPrograma>(chegadaProgramas.OrderBy(c => c.InstanteChegada));
            InicioSimulacao = inicioSimulacao;
            FimSimulacao = FimSimulacao;

            Eventos = new LinkedList<Evento>();
            InstanteDeSimulacao = 0;

            RotinasTratamento = new Dictionary<TipoEvento, Action<Evento>>();
            RotinasTratamento.Add(TipoEvento.ARRIVAL, new Action<Evento>(Arrival));
            RotinasTratamento.Add(TipoEvento.REQUEST_CM, new Action<Evento>(RequestCM));
            RotinasTratamento.Add(TipoEvento.REQUEST_CPU, new Action<Evento>(RequestCPU));
            RotinasTratamento.Add(TipoEvento.REQUEST_DISK, new Action<Evento>(RequestDisk));
            RotinasTratamento.Add(TipoEvento.RELEASE_DISK, new Action<Evento>(ReleaseDisk));
            RotinasTratamento.Add(TipoEvento.RELEASE_CM_CPU, new Action<Evento>(ReleaseCMCPU));
            RotinasTratamento.Add(TipoEvento.COMPLETION, new Action<Evento>(Completion));
        }


        public void Simulacao()
        {
            // TODO: Informações dos Dispositivos
            InstanciarDispositivos();
            // TODO: Evento Primeiro Programa (1/4)
            ChegadaPrograma primeiroPrograma = ProximoPrograma();
            AgendarPrograma(primeiroPrograma.InstanteChegada, TipoEvento.ARRIVAL, primeiroPrograma.Programa);
            while (Scheduler()) ;
        }

        void InstanciarDispositivos()
        {
            CM = new CM(1000);
            CPU = new CPU();
            Disk = new Disk();
        }

        ChegadaPrograma ProximoPrograma()
        {
            if (FilaProgramas.Any())
                return FilaProgramas.Dequeue();
            else return null;
        }

        void AgendarPrograma(int instanteChegada, TipoEvento tipoEvento, Programa programa)
        {
            Evento evento = new Evento(instanteChegada, tipoEvento, programa);
            AdicionarEvento(evento);
        }

        // Adiciona Evento na Fila por Ordem de InstanteChegada
        void AdicionarEvento(Evento evento)
        {
            LinkedListNode<Evento> ev = Eventos.First;
            // TODO: Melhorar Lógica para não ter risco de Loop Infinito
            while (true)
            {
                if (ev == null)
                {
                    Eventos.AddFirst(evento);
                    break;
                }
                else if (ev.Value.InstanteChegada > evento.InstanteChegada)
                {
                    Eventos.AddBefore(ev, evento);
                    break;
                }
                else if (ev.Next == null)
                {
                    Eventos.AddAfter(ev, evento);
                    break;
                }
            }
        }


        bool Scheduler()
        {
            LinkedListNode<Evento> evento = Eventos.First;
            if (evento == null)
            {
                FinalizaSimulacao();
                return false;
            }
            else
            {
                Eventos.Remove(evento);

                InstanteDeSimulacao = evento.Value.InstanteChegada;

                RotinasTratamento[evento.Value.Tipo](evento.Value);

                Console.WriteLine(String.Format("Instante {0} | Evento {1} | Programa {2}", InstanteDeSimulacao, evento.Value.Tipo, evento.Value.Programa.Identificador));
                Console.WriteLine("\t" + CM.Status());
                Console.WriteLine("\t" + CPU.Status());
                Console.WriteLine("\t" + Disk.Status());

                return true;
            }
        }

        private void FinalizaSimulacao()
        {
            throw new NotImplementedException();
        }

        #region Rotinas de Tratamento
        public void Arrival(Evento evento)
        {
            // Sample Job Mix Distributions

            // Schedule Next Event
            Evento proximoEvento = new Evento(evento.InstanteChegada, TipoEvento.REQUEST_CM, evento.Programa);
            AdicionarEvento(proximoEvento);

            // Schedule Next Job
            ChegadaPrograma proximoPrograma = ProximoPrograma();
            if (proximoPrograma != null)
                AgendarPrograma(proximoPrograma.InstanteChegada, TipoEvento.ARRIVAL, proximoPrograma.Programa);

            return;
        }

        private void RequestCM(Evento evento)
        {
            Programa programa = evento.Programa;
            if (evento.Programa.MemoriaNecessaria > CM.MemoriaDisponivel())
            {
                CM.Inserir(programa);

                return;
            }
            else
            {
                CM.Reservar(programa);
                int tempoRelocacao = CM.TempoDeRelocacao(programa);
                Evento proximoEvento = new Evento(evento.InstanteChegada + tempoRelocacao,
                    TipoEvento.REQUEST_CPU,
                    programa);

                AdicionarEvento(proximoEvento);

                return;
            }
        }

        private void RequestCPU(Evento evento)
        {
            Programa programa = evento.Programa;
            if (CPU.EstaOcupado())
            {
                CPU.Inserir(programa);

                return;
            }
            else
            {
                CPU.Reservar(programa);
                if (programa.OperacoesIO == 0)
                {
                    Evento proximoEvento = new Evento(InstanteDeSimulacao,
                        TipoEvento.COMPLETION,
                        programa);

                    AdicionarEvento(proximoEvento);
                }
                else
                {
                    int tempoProcessamento = CPU.TempoDeProcessamento(programa);
                    programa.OperacoesIO -= 1;
                    Evento proximoEvento = new Evento(InstanteDeSimulacao + tempoProcessamento,
                        TipoEvento.REQUEST_DISK,
                        programa);

                    AdicionarEvento(proximoEvento);
                }
                return;
            }
        }

        private void RequestDisk(Evento evento)
        {
            Programa programa = evento.Programa;
            CPU.Liberar(programa);
            CPU.Avancar();

            // TODO: Overhead Time
            Evento proximoEvento = new Evento(evento.InstanteChegada + 1,
                TipoEvento.RELEASE_DISK,
                programa);

            AdicionarEvento(proximoEvento);

            return;
        }

        private void ReleaseDisk(Evento obj)
        {
            throw new NotImplementedException();
        }

        private void ReleaseCMCPU(Evento obj)
        {
            throw new NotImplementedException();
        }

        private void Completion(Evento obj)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
