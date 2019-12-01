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
        public Dictionary<TipoEvento, Action<Evento>> RotinasTratamento { get; set; };
        public int NivelLog { get; set; }

        public SistemaOperacional(List<ChegadaPrograma> chegadaProgramas, int inicioSimulacao = 0, int fimSimulacao = -1, int nivelLog = 0)
        {
            FilaProgramas = new Queue<ChegadaPrograma>(chegadaProgramas.OrderBy(c => c.InstanteChegada));
            InicioSimulacao = inicioSimulacao;
            FimSimulacao = fimSimulacao;
            NivelLog = nivelLog;

            Eventos = new LinkedList<Evento>();
            InstanteDeSimulacao = 0;

            RotinasTratamento = new Dictionary<TipoEvento, Action<Evento>>
            {
                { TipoEvento.ARRIVAL, new Action<Evento>(Arrival) },
                { TipoEvento.REQUEST_CM, new Action<Evento>(RequestCM) },
                { TipoEvento.REQUEST_CPU, new Action<Evento>(RequestCPU) },
                { TipoEvento.RELEASE_CPU_REQUEST_DISK, new Action<Evento>(ReleaseCPURequestDisk) },
                { TipoEvento.REQUEST_DISK, new Action<Evento>(RequestDisk) },
                { TipoEvento.RELEASE_DISK, new Action<Evento>(ReleaseDisk) },
                { TipoEvento.RELEASE_CM_CPU, new Action<Evento>(ReleaseCMCPU) },
                { TipoEvento.COMPLETION, new Action<Evento>(Completion) }
            };
        }

        public void Simulacao()
        {
            // TODO: Informações dos Dispositivos
            InstanciarDispositivos();
            // TODO: Evento Primeiro Programa (1/4)
            ChegadaPrograma primeiroPrograma = ProximoPrograma();
            AgendarPrograma(primeiroPrograma.InstanteChegada, TipoEvento.ARRIVAL, primeiroPrograma.Programa);

            if (FimSimulacao > 0)
                AgendarPrograma(FimSimulacao, TipoEvento.COMPLETION, new Programa() { Identificador = "SO" });

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
                ev = ev.Next;
            }
        }


        bool Scheduler()
        {
            LinkedListNode<Evento> evento = Eventos.First;
            if (evento == null)
            {
                // TODO: Evento COMPLETION sem FimSimulação
                FinalizaSimulacao();
                return false;
            }
            else
            {
                Eventos.Remove(evento);

                InstanteDeSimulacao = evento.Value.InstanteChegada;

                RotinasTratamento[evento.Value.Tipo](evento.Value);

                return true;
            }
        }

        private void FinalizaSimulacao()
        {
            throw new NotImplementedException();
        }

        private void Log(Evento evento, string acao, string resultado)
        {
            string log = string.Format("{0};{1};{2};{3};{4}", InstanteDeSimulacao, evento.Tipo, evento.Programa.Identificador, acao, resultado);

            // TODO: Escrever Relatorio de Saida

            if (NivelLog > 0)
                Console.WriteLine(string.Format("Instante {0} | Evento {1} | Programa {2} | {3}", InstanteDeSimulacao, evento.Tipo, evento.Programa.Identificador, resultado));

            if (NivelLog > 1)
            {
                Console.WriteLine("\t" + CM.Status());
                Console.WriteLine("\t" + CPU.Status());
                Console.WriteLine("\t" + Disk.Status());
                Console.WriteLine("\n");
            }

        }

        #region Rotinas de Tratamento

        // Evento 1
        public void Arrival(Evento evento)
        {
            // Sample Job Mix Distributions
            string resultado = "";

            // Schedule Next Event
            Evento proximoEvento = new Evento(evento.InstanteChegada, TipoEvento.REQUEST_CM, evento.Programa);
            AdicionarEvento(proximoEvento);
            resultado += "Chegada do Job. ";

            // Schedule Next Job
            ChegadaPrograma proximoPrograma = ProximoPrograma();
            if (proximoPrograma != null)
            {
                AgendarPrograma(proximoPrograma.InstanteChegada, TipoEvento.ARRIVAL, proximoPrograma.Programa);
                resultado += "Agendada chegada do Próximo Job. ";
            }

            Log(evento, "Arrival", resultado);
            return;
        }

        // Evento 2
        private void RequestCM(Evento evento)
        {
            string resultado = "";

            Programa programa = evento.Programa;
            if (evento.Programa.MemoriaNecessaria > CM.MemoriaDisponivel())
            {
                resultado += "Memória Insuficiente. Job aguardará liberação. ";
                CM.Inserir(programa);
            }
            else
            {
                resultado += "Job Carregado na Memória. ";
                CM.Reservar(programa);
                int tempoRelocacao = CM.TempoDeRelocacao(programa);
                Evento proximoEvento = new Evento(evento.InstanteChegada + tempoRelocacao,
                    TipoEvento.REQUEST_CPU,
                    programa);

                AdicionarEvento(proximoEvento);
            }

            Log(evento, "RequestCM", resultado);
            return;
        }

        // Evento 3
        private void RequestCPU(Evento evento)
        {
            string resultado = "";

            Programa programa = evento.Programa;
            if (CPU.EstaOcupado())
            {
                resultado += "CPU ocupada. Job aguardará liberação. ";
                CPU.Inserir(programa);
            }
            else
            {
                resultado += "CPU livre para processar Job. ";
                CPU.Reservar(programa);
                if (programa.OperacoesIO == 0)
                {
                    resultado += "Job não demanda mais Operações I/O. ";
                    Evento proximoEvento = new Evento(InstanteDeSimulacao,
                        TipoEvento.RELEASE_CM_CPU,
                        programa);

                    AdicionarEvento(proximoEvento);
                }
                else
                {
                    resultado += "Job ainda demanda Operações I/O. ";
                    int tempoProcessamento = CPU.TempoDeProcessamento(programa);
                    programa.OperacoesIO -= 1;
                    Evento proximoEvento = new Evento(InstanteDeSimulacao + tempoProcessamento,
                        TipoEvento.RELEASE_CPU_REQUEST_DISK,
                        programa);

                    AdicionarEvento(proximoEvento);
                }
            }

            Log(evento, "RequestCPU", resultado);
            return;
        }

        // Evento 4
        private void ReleaseCPURequestDisk(Evento evento)
        {
            string resultado = "";
            resultado += "CPU liberada. Job solicita Disco. ";

            Programa programa = evento.Programa;
            LiberarCPU(programa);

            // TODO: Overhead Time
            Evento proximoEvento = new Evento(evento.InstanteChegada + 1,
                TipoEvento.REQUEST_DISK,
                programa);

            AdicionarEvento(proximoEvento);

            Log(evento, "ReleaseCPURequestDisk", resultado);
            return;
        }

        // Evento 5
        private void RequestDisk(Evento evento)
        {
            string resultado = "";

            Programa programa = evento.Programa;
            if (Disk.EstaOcupado())
            {
                resultado += "Disco Ocupado. Job aguardará liberação. ";
                Disk.Inserir(programa);
            }
            else
            {
                resultado += "Disco Livre para Job acessar. ";

                Disk.Reservar(programa);
                int tempoOperacao = Disk.TempoDeOperacao(programa);

                Evento proximoEvento = new Evento(InstanteDeSimulacao + tempoOperacao,
                    TipoEvento.RELEASE_DISK,
                    programa);
                AdicionarEvento(proximoEvento);
            }

            Log(evento, "RequestDisk", resultado);
            return;
        }

        // Evento 6
        private void ReleaseDisk (Evento evento)
        {
            string resultado = "";
            resultado += "Disco liberado. Job solcitará CPU. ";

            Programa programa = evento.Programa;
            LiberarDisco(programa);

            // TODO: Overhead Time
            Evento proximoEvento = new Evento(evento.InstanteChegada + 1,
                TipoEvento.REQUEST_CPU,
                programa);

            AdicionarEvento(proximoEvento);

            Log(evento, "ReleaseDisk", resultado);
        }

        // Evento 7
        private void ReleaseCMCPU(Evento evento)
        {
            string resultado = "";
            resultado += "Job finalizado, liberando CPU e Memória. ";

            Programa programa = evento.Programa;
            LiberarCPU(programa);
            LiberarMemoria(programa);

            // TODO: Estatisticas do Programa

            // TODO? Release JobTable Space

            Log(evento, "ReleaseCMCPU", resultado);
        }

        // Evento 8
        private void Completion (Evento evento)
        {
            string resultado = "";
            resultado += "Fim de Simulação atingido. ";

            Log(evento, "Completion", resultado);

            // TODO: Completion como novo evento (2/4)
            throw new NotImplementedException();
        }
        #endregion

        #region Funções Auxiliares
        private void LiberarCPU(Programa programa)
        {
            CPU.Liberar(programa);
            Programa proximoPrograma = CPU.Avancar();
            if (proximoPrograma != null)
            {
                AdicionarEvento(new Evento(InstanteDeSimulacao, TipoEvento.REQUEST_CPU, proximoPrograma));
            }
        }

        private void LiberarDisco(Programa programa)
        {
            Disk.Liberar(programa);
            Programa proximoPrograma = Disk.Avancar();
            if (proximoPrograma != null)
            {
                AdicionarEvento(new Evento(InstanteDeSimulacao, TipoEvento.REQUEST_DISK, proximoPrograma));
            }
        }

        private void LiberarMemoria(Programa programa)
        {
            CM.Liberar(programa);
            Programa proximoPrograma = CM.Avancar();
            if (proximoPrograma != null)
            {
                AdicionarEvento(new Evento(InstanteDeSimulacao, TipoEvento.REQUEST_CM, proximoPrograma));
            }
        }

        #endregion
    }
}
