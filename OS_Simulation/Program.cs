﻿using System;
using System.Collections.Generic;

namespace OS_Simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            int instanteInicialSimulacao = 0;
            int instanteFinalSimulacao = 99;
            List<Programa> programas = new List<Programa>(){
                new Programa(){TempoProcessamento=10, MemoriaNecessaria=256, OperacoesIO=2 }
            };

            // Inicializa Dispositivos e Entradas

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
