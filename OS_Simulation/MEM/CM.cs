using System;
using System.Collections.Generic;
using System.Linq;

namespace OS_Simulation.MEM
{
    public class CM
    {
        Queue<Programa> Fila { get; set; }
        int MemoriaTotal { get; set; }
        LinkedList<Particao> Particoes { get; set; }

        public CM(int memoriaTotal)
        {
            Fila = new Queue<Programa>();
            MemoriaTotal = memoriaTotal;
            Particoes = new LinkedList<Particao>();
            Particoes.AddFirst(new Particao(0, memoriaTotal));
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
            LinkedListNode<Particao> maiorParticaoLivre = MaiorParticaoLivre();
            return String.Format("CM: {0}/{1} - Maior partição livre: {2} - Fila: {3} programa(s), proximo: {4}",
                MemoriaAlocada(),
                MemoriaTotal,
                maiorParticaoLivre.Value == null ? "-" : maiorParticaoLivre.Value.Tamanho.ToString(),
                programasEmFila,
                proximoPrograma != null ? proximoPrograma.Identificador : "-"
            ); ;
        }       

        // #### Maior Particao Disponivel ####
        public int MemoriaDisponivel()
        {
            LinkedListNode<Particao> maiorParticaoLivre = MaiorParticaoLivre();
            return maiorParticaoLivre.Value == null ? 0 : maiorParticaoLivre.Value.Tamanho;
        }

        public void Reservar(Programa programa)
        {
            LinkedListNode<Particao> maiorParticaoLivre = MaiorParticaoLivre();

            if (maiorParticaoLivre.Value == null)
                throw new Exception("Não há Partições Livres de Memória");

            Particao particaoPrograma = new Particao(maiorParticaoLivre.Value.PosicaoInicial, programa.MemoriaNecessaria, programa);
            Particao particaoLivre = new Particao(particaoPrograma.PosicaoFinal() + 1, maiorParticaoLivre.Value.Tamanho - particaoPrograma.Tamanho);

            Particoes.AddBefore(maiorParticaoLivre, particaoPrograma);
            Particoes.AddAfter(maiorParticaoLivre, particaoLivre);
            Particoes.Remove(maiorParticaoLivre);

            if (MemoriaAlocada() > MemoriaTotal)
                throw new Exception("Overflow de Memória!");
        }

        public void Liberar(Programa programa)
        {
            List<Particao> particoesAlocadas = Particoes.Where(p => p.Programa == programa).ToList();
            particoesAlocadas.ForEach(p => p.Liberar());

            // TODO: Desfragmentador de Memória
            UnirParticoesLivresAdjacentes();

            if (MemoriaAlocada() < 0)
                throw new Exception("Underflow de Memória!");
        }

        private void UnirParticoesLivresAdjacentes()
        {
            LinkedListNode<Particao> particao = Particoes.First;

            while(particao != null)
            {
                LinkedListNode<Particao> proximaParticao = particao.Next;
                if (particao.Value.Livre && proximaParticao != null && proximaParticao.Value.Livre)
                {
                    particao.Value.Tamanho += proximaParticao.Value.Tamanho;
                    Particoes.Remove(proximaParticao);
                    continue;
                }
                particao = particao.Next;
            }
        }

        // TODO: Tempo de Relocacao
        public int TempoDeRelocacao(Programa programa)
        {
            return (int)Math.Ceiling(0.1 * programa.MemoriaNecessaria);
        }

        int MemoriaAlocada()
        {
            return Particoes.Where(p => !p.Livre).Sum(p => p.Tamanho);
        }

        LinkedListNode<Particao> MaiorParticaoLivre()
        {
            LinkedListNode<Particao> maiorParticaoLivre = new LinkedListNode<Particao>(null);
            LinkedListNode<Particao> particao = Particoes.First;

            while (particao != null)
            {
                if (particao.Value.Livre)
                {
                    if (maiorParticaoLivre.Value == null)
                        maiorParticaoLivre = particao;
                    else if (particao.Value.Tamanho > maiorParticaoLivre.Value.Tamanho)
                        maiorParticaoLivre = particao;
                }
                particao = particao.Next;
            }
            return maiorParticaoLivre;
        }
    }
}
