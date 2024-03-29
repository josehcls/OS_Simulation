﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace OS_Simulation
{
    public class CPU
    {
        Queue<Programa> Fila { get; set; }
        bool Ocupado { get; set; }

        public CPU()
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
            if (Fila.Any())
                return Fila.Dequeue();
            else
                return null;
        }

        public string Status()
        {
            Programa proximoPrograma = Fila.Any() ? Fila.Peek() : null;
            int programasEmFila = Fila.Count;
            return String.Format("CPU: {0} - Fila: {1} programa(s), proximo: {2}",
                Ocupado ? "Ocupada" : "Livre",
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
                throw new Exception("Acesso a CPU Ocupada!");
            Ocupado = true;
        }

        public void Liberar(Programa programa)
        {
            Ocupado = false;
        }

        // TODO: Tempo de Processamento / Inter-request Time
        public int TempoDeProcessamento(Programa programa)
        {
            return programa.TempoProcessamento;
        }

        public int TempoDeOverhead(Programa programa)
        {
            return 1;
        }

    }
}

