using System;
using System.Collections.Generic;
using System.Text;

namespace OS_Simulation
{
    public class MotorEventos
    {
        public Evento EventoCorrente { get; set; }
        public Fila Fila{ get; set; }
        public int InstanteExecucao { get; set; }
        public SeletorRotinas SeletorRotinas { get; set; }
        public List<Evento> FilaSaida { get; set; }
        // TODO: Saida mais generica, como uma fila
        // TODO: Relatorio de execucao

        public MotorEventos(SeletorRotinas seletorRotinas, int instanteExecucao = 0, List<Evento> filaSaida = null)
        {
            EventoCorrente = null;
            Eventos = new List<Evento>();
            EventosPrioritarios = new List<Evento>();
            InstanteExecucao = instanteExecucao;
            SeletorRotinas = seletorRotinas;
            FilaSaida = filaSaida;
        }

        public void Inicializar(List<Evento> eventosIniciais)
        {
            Eventos = new List<Evento>();
            Eventos.AddRange(eventosIniciais);
        }

        public Evento ExtrairProximoEvento()
        {
            Evento proximoEvento = null;
            if (EventosPrioritarios.Count > 0)
            {
                proximoEvento = EventosPrioritarios[0];
                EventosPrioritarios.RemoveAt(0);
            }
            else if (Eventos.Count > 0)
            {
                proximoEvento = Eventos[0];
                Eventos.RemoveAt(0);
            }
            return proximoEvento;
        }

        public string Finalizar()
        {
            // TODO: Relatório de Execução
            return "";
        }

        public void ProcessarEvento(Evento evento)
        {
            SaidaRotina saidaRotina = SeletorRotinas.ProcessarEvento(evento);
            Eventos.AddRange(saidaRotina.EventosInternos);
            EventosPrioritarios.AddRange(saidaRotina.EventosPrioritarios);
            if (FilaSaida != null) FilaSaida.AddRange(saidaRotina.EventosExternos);
        }

        public void ReceberEvento(Evento evento)
        {
            Eventos.Add(evento);
        }

        public void EnviarEvento(Evento evento)
        {
            FilaSaida.Add(evento);
        }

        public bool Rodar()
        {
            Evento evento = ExtrairProximoEvento();
            if (evento != null)
            {
                InstanteExecucao = evento.InstanteProgramado;
                ProcessarEvento(evento);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
