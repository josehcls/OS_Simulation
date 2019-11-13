using System;
using System.Collections.Generic;
using System.Linq;

namespace OS_Simulation.MEM
{
    public class CM
    {
        // Guardar Programas Alocados ao invés de Memória Alocada

        Queue<Programa> Fila { get; set; }
        int MemoriaTotal { get; set; }
        int MemoriaAlocada { get; set; }

        public CM(int memoriaTotal)
        {
            Fila = new Queue<Programa>();
            MemoriaTotal = memoriaTotal;
            MemoriaAlocada = 0;
        }

        public void Inserir(Programa programa)
        {
            Fila.Enqueue(programa);
        }

        public Programa Avancar()
        {
            return Fila.Dequeue();
        }

        public string Status()
        {
            Programa proximoPrograma = Fila.Any() ? Fila.Peek() : null;
            int programasEmFila = Fila.Count;
            return String.Format("CM: {0}/{1} - Fila: {2} programa(s), proximo: {3}",
                MemoriaAlocada,
                MemoriaTotal,
                programasEmFila,
                proximoPrograma != null ? proximoPrograma.Identificador : "-"
            );
        }       

        public int MemoriaDisponivel()
        {
            return MemoriaTotal - MemoriaAlocada;
        }

        public void Reservar(Programa programa)
        {
            MemoriaAlocada += programa.MemoriaNecessaria;
            if (MemoriaAlocada > MemoriaTotal)
                throw new Exception("Overflow de Memória!");
        }

        public void Liberar(Programa programa)
        {
            MemoriaAlocada -= programa.MemoriaNecessaria;
            if (MemoriaAlocada < 0)
                throw new Exception("Underflow de Memória!");
        }

        // TODO: Tempo de Relocacao
        public int TempoDeRelocacao(Programa programa)
        {
            return 1 * programa.MemoriaNecessaria;
        }
    }
}
