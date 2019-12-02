using OS_Simulation.DISK;
using OS_Simulation.MEM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

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
        public Dictionary<TipoEvento, Func<Evento, bool>> RotinasTratamento { get; set; }
        public int NivelLog { get; set; }
        public string ArquivoSaida { get; set; }

        public SistemaOperacional(List<ChegadaPrograma> chegadaProgramas, int inicioSimulacao = 0, int fimSimulacao = -1, int nivelLog = 0)
        {
            FilaProgramas = new Queue<ChegadaPrograma>(chegadaProgramas.OrderBy(c => c.InstanteChegada));
            InicioSimulacao = inicioSimulacao;
            FimSimulacao = fimSimulacao;
            NivelLog = nivelLog;
            ArquivoSaida = "saida.csv";
            if (File.Exists(ArquivoSaida))
                File.Delete(ArquivoSaida); 
            EscreverSaida("Instante de Simulação;Tipo do Evento;Programa;Ação;Resultado");

            Eventos = new LinkedList<Evento>();
            InstanteDeSimulacao = 0;

            RotinasTratamento = new Dictionary<TipoEvento, Func<Evento, bool>>
            {
                { TipoEvento.ARRIVAL, new Func<Evento, bool>(Arrival) },
                { TipoEvento.REQUEST_CM, new Func<Evento, bool>(RequestCM) },
                { TipoEvento.REQUEST_CPU, new Func<Evento, bool>(RequestCPU) },
                { TipoEvento.RELEASE_CPU_REQUEST_DISK, new Func<Evento, bool>(ReleaseCPURequestDisk) },
                { TipoEvento.REQUEST_DISK, new Func<Evento, bool>(RequestDisk) },
                { TipoEvento.RELEASE_DISK, new Func<Evento, bool>(ReleaseDisk) },
                { TipoEvento.RELEASE_CM_CPU, new Func<Evento, bool>(ReleaseCMCPU) },
                { TipoEvento.COMPLETION, new Func<Evento, bool>(Completion) }
            };
        }

        public void Simulacao()
        {
            // TODO: Informações dos Dispositivos
            InstanciarDispositivos();
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


        private bool Scheduler()
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

                return RotinasTratamento[evento.Value.Tipo](evento.Value);
            }
        }

        private void FinalizaSimulacao()
        {
            Console.WriteLine("#### Fim de Simulação. O Arquivo de Saída consta na Pasta do Programa, com nome " + ArquivoSaida);
            Console.WriteLine("Aperte qualquer tecla para continuar... ");
            Console.ReadKey();
        }

        private void Log(Evento evento, string acao, string resultado)
        {
            string log = string.Format("{0};{1};{2};{3};{4}", InstanteDeSimulacao, evento.Tipo, evento.Programa.Identificador, acao, resultado);
            EscreverSaida(log);

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

        private void EscreverSaida(string conteudo)
        {
            using (StreamWriter file = new StreamWriter(ArquivoSaida, true, Encoding.Unicode))
            {
                file.WriteLine(conteudo);
                file.Flush();
            }
        }

        #region Rotinas de Tratamento

        // Evento 1
        public bool Arrival(Evento evento)
        {
            // TODO: Sample Job Mix Distributions
            string resultado = "";

            Evento proximoEvento = new Evento(evento.InstanteChegada, TipoEvento.REQUEST_CM, evento.Programa);
            AdicionarEvento(proximoEvento);
            resultado += "Chegada do Job. ";

            ChegadaPrograma proximoPrograma = ProximoPrograma();
            if (proximoPrograma != null)
            {
                AgendarPrograma(proximoPrograma.InstanteChegada, TipoEvento.ARRIVAL, proximoPrograma.Programa);
                resultado += "Agendada chegada do Próximo Job. ";
            }

            Log(evento, "Arrival", resultado);
            return true;
        }

        // Evento 2
        private bool RequestCM(Evento evento)
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
            return true;
        }

        // Evento 3
        private bool RequestCPU(Evento evento)
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
                    int tempoProcessamento = CPU.TempoDeProcessamento(programa);
                    Evento proximoEvento = new Evento(InstanteDeSimulacao + tempoProcessamento,
                        TipoEvento.RELEASE_CM_CPU,
                        programa);

                    AdicionarEvento(proximoEvento);
                }
                else
                {
                    resultado += "Job ainda demanda Operações I/O. ";
                    int tempoOverhead = CPU.TempoDeOverhead(programa);
                    programa.OperacoesIO -= 1;
                    Evento proximoEvento = new Evento(InstanteDeSimulacao + tempoOverhead,
                        TipoEvento.RELEASE_CPU_REQUEST_DISK,
                        programa);

                    AdicionarEvento(proximoEvento);
                }
            }

            Log(evento, "RequestCPU", resultado);
            return true;
        }

        // Evento 4
        private bool ReleaseCPURequestDisk(Evento evento)
        {
            string resultado = "";
            resultado += "CPU liberada. Job irá solicitar Disco. ";

            Programa programa = evento.Programa;
            LiberarCPU(programa);

            int tempoOverhead = CPU.TempoDeOverhead(programa);
            Evento proximoEvento = new Evento(evento.InstanteChegada + tempoOverhead,
                TipoEvento.REQUEST_DISK,
                programa);

            AdicionarEvento(proximoEvento);

            Log(evento, "ReleaseCPURequestDisk", resultado);
            return true;
        }

        // Evento 5
        private bool RequestDisk(Evento evento)
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
            return true;
        }

        // Evento 6
        private bool ReleaseDisk (Evento evento)
        {
            string resultado = "";
            resultado += "Disco liberado. Job voltará a solicitar CPU. ";

            Programa programa = evento.Programa;
            LiberarDisco(programa);

            int tempoOverhead = CPU.TempoDeOverhead(programa);
            Evento proximoEvento = new Evento(evento.InstanteChegada + tempoOverhead,
                TipoEvento.REQUEST_CPU,
                programa);

            AdicionarEvento(proximoEvento);

            Log(evento, "ReleaseDisk", resultado);
            return true;
        }

        // Evento 7
        private bool ReleaseCMCPU(Evento evento)
        {
            string resultado = "";
            resultado += "Job finalizado, liberando CPU e Memória. ";

            Programa programa = evento.Programa;
            LiberarCPU(programa);
            LiberarMemoria(programa);

            // TODO: Estatisticas do Programa

            // TODO? Release JobTable Space

            Log(evento, "ReleaseCMCPU", resultado);
            return true;
        }

        // Evento 8
        private bool Completion (Evento evento)
        {
            string resultado = "";
            resultado += "Instante Final de Simulação atingido. ";

            Log(evento, "Completion", resultado);

            return false;
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
