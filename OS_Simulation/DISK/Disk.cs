﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OS_Simulation.DISK
{
    public class Disk
    {
        Queue<Programa> Fila { get; set; }
        bool Ocupado { get; set; }

        public Disk()
        {
            Fila = new Queue<Programa>();
            Ocupado = false;
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
            Programa proximoPrograma = Fila.Peek();
            int programasEmFila = Fila.Count;
            return String.Format("Disk: {0} - Fila: {1} programa(s), proximo: {2}",
                Ocupado ? "Ocupado" : "Livre",
                programasEmFila,
                proximoPrograma != null ? proximoPrograma.Identificador : "-"
            );
        }       

        public bool EstaOcupado()
        {
            return Ocupado;
        }

        public void Reservar(Programa programa)
        {
            if (Ocupado)
                throw new Exception("Acesso a Disco Ocupado!");
            Ocupado = true;
        }

        public void Liberar(Programa programa)
        {
            Ocupado = false;
        }

        // TODO: Tempo de Operacao
        public int TempoDeOperacao(Programa programa)
        {
            return 10;
        }

    }
}
