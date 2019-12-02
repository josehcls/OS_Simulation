using OS_Simulation.Base;
using System;
using System.Collections.Generic;

namespace OS_Simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            SistemaOperacional sistemaOperacional = new SistemaOperacional(
                new List<ChegadaPrograma>()
                { 
                    new ChegadaPrograma() {
                        InstanteChegada = 4, 
                        Programa = new Programa()
                        {
                            Identificador="1",
                            MemoriaNecessaria=300,
                            TempoProcessamento=20,
                            OperacoesIO=1                     
                        }
                    },
                    new ChegadaPrograma() {
                        InstanteChegada = 10, 
                        Programa = new Programa()
                        {
                            Identificador="2",
                            MemoriaNecessaria=800,
                            TempoProcessamento=20,
                            OperacoesIO=1
                        }
                    },
                    new ChegadaPrograma() {
                        InstanteChegada = 20, 
                        Programa = new Programa()
                        {
                            Identificador="3",
                            MemoriaNecessaria=100,
                            TempoProcessamento=5,
                            OperacoesIO=5
                        }
                    },
                    new ChegadaPrograma() {
                        InstanteChegada = 25, 
                        Programa = new Programa()
                        {
                            Identificador="4",
                            MemoriaNecessaria=500,
                            TempoProcessamento=5,
                            OperacoesIO=3
                        }
                    },
                }
                , 0, -1, 2
            );

            sistemaOperacional.Simulacao();
        }
    }
}
