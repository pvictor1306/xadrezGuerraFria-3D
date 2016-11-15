﻿using UnityEngine;

namespace tabuleiro
{
   abstract class Peca
    {
        public GameObject obj { get; set; }

        public Posicao posicao { get; set; }
        public Tabuleiro tab { get; protected set; }
        public Cor cor { get; protected set; }
        public int qteMovimentos { get; protected set; }

        public Peca(Tabuleiro tab, Cor cor, GameObject obj)
        {
            this.posicao = null;
            this.tab = tab;
            this.cor = cor;
            this.qteMovimentos = 0;
            this.obj = obj;
        }
        public void incrementarQteMovimentos(){
            qteMovimentos++;      
        }

        public void decrementarQteMovimentos() {
            qteMovimentos--;
        }

        public bool existeMovimentosPossiveis() {
            bool[,] mat = movimentosPossiveis();
            for(int i=0; i< tab.linhas; i++) {
                for(int j=0; j<tab.colunas; j++) {
                    if (mat[i, j]) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool movimentoPossivel(Posicao pos) {
            return movimentosPossiveis()[pos.linha, pos.coluna];
        }

        public abstract bool[,] movimentosPossiveis();
    }
}
