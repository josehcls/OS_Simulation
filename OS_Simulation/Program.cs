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


            // 

            /* Entradas do Simulador
             *  
             *  - Instante Inicial de Simulação
             *  - Instante Final de Simulação
             *  - Programas:
             *      - Instante de Chegada
             *      - Tempo Total Estimado de Processamento
             *      - Quantidade Estimada de Memória Necessária
             *      - Número estimado de Operações de Entrada e Saída
             *      
             */


            /* Log de Simulação
             *          Lista classificando todos os Instantes Previstos e Executados
             *          para todos os Eventos que ocorreram na Simulação de cada um 
             *          dos Programas
             *
             * - Instante Corrente de Simulação
             * - Tipo do Evento
             * - Identificação do Programa
             * - Ação Executada (Rotina executada)
             * - Resultado (Efeitos dessa Reação)
             */

            /* Componentes
             * 
             * - Memória Principal (segmentada)
             * - Processador
             * - Disco (Sistema de Arquivos Simples)
             * - 2 Leitoras Físicas
             * - 2 Impressoras Físicas
             */



        }
    }
}
