using UnityEngine;
using tabuleiro;
using Xadrez;
using UnityEngine.UI;
using System.Collections.Generic;

class GameController : MonoBehaviour {

    public GameObject reiBranco = null;
    public GameObject reiPreto = null;
    public GameObject torreBranca = null;
    public GameObject torrePreta = null;
    public GameObject damaPreta = null;
    public GameObject damaBranca = null;
    public GameObject cavaloBranco = null;
    public GameObject cavaloPreto = null;
    public GameObject bispoBranco = null;
    public GameObject bispoPreto = null;
    public GameObject peaoBranco = null;
    public GameObject peaoPreto = null;



    public Text txtMsg = null;
    public Text txtXeque = null;

    public GameObject pecaEscolhida { get; private set; }

    public Estado estado { get; private set; }

    PartidaDeXadrez partida;
    PosicaoXadrez origem, destino;
    Color corOriginal;

    Vector3 posDescarteBrancas, posDescartePretas;

    public GameObject particulas;

    List<GameObject> listaParticulas;

    void Start() {
        estado = Estado.AguardandoJogada;
        pecaEscolhida = null;
        corOriginal = txtMsg.color;
        listaParticulas = new List<GameObject>();

        posDescarteBrancas = new Vector3(-1.3f, 0f, -1f);
        posDescartePretas = new Vector3(1.3f, 0f, 1f);


        partida = new PartidaDeXadrez();

        txtXeque.text = "";
        informarAguardando();

        Util.instanciarRei('e', 1, Cor.Branca, partida, reiBranco);
        Util.instanciarBispo('c', 1, Cor.Branca, partida, bispoBranco);
        Util.instanciarCavalo('b', 1, Cor.Branca, partida, cavaloBranco);
        Util.instanciarDama('d', 1, Cor.Branca, partida, damaBranca);
        Util.instanciarBispo('f', 1, Cor.Branca, partida, bispoBranco);
        Util.instanciarCavalo('g', 1, Cor.Branca, partida, cavaloBranco);
        Util.instanciarTorre('a', 1, Cor.Branca, partida, torreBranca);
        Util.instanciarTorre('h', 1, Cor.Branca, partida, torreBranca);
        Util.instanciarPeao('a', 2, Cor.Branca, partida, peaoBranco);
        Util.instanciarPeao('b', 2, Cor.Branca, partida, peaoBranco);
        Util.instanciarPeao('c', 2, Cor.Branca, partida, peaoBranco);
        Util.instanciarPeao('d', 2, Cor.Branca, partida, peaoBranco);
        Util.instanciarPeao('e', 2, Cor.Branca, partida, peaoBranco);
        Util.instanciarPeao('f', 2, Cor.Branca, partida, peaoBranco);
        Util.instanciarPeao('g', 2, Cor.Branca, partida, peaoBranco);
        Util.instanciarPeao('h', 2, Cor.Branca, partida, peaoBranco);


        Util.instanciarTorre('a', 8, Cor.Preta, partida, torrePreta);
        Util.instanciarTorre('h', 8, Cor.Preta, partida, torrePreta);
        Util.instanciarRei('e', 8, Cor.Preta, partida, reiPreto);
        Util.instanciarBispo('c', 8, Cor.Preta, partida, bispoPreto);
        Util.instanciarCavalo('b', 8, Cor.Preta, partida, cavaloPreto);
        Util.instanciarDama('d', 8, Cor.Preta, partida, damaPreta);
        Util.instanciarBispo('f', 8, Cor.Preta, partida, bispoPreto);
        Util.instanciarCavalo('g', 8, Cor.Preta, partida, cavaloPreto);
        Util.instanciarPeao('a', 7, Cor.Preta, partida, peaoPreto);
        Util.instanciarPeao('b', 7, Cor.Preta, partida, peaoPreto);
        Util.instanciarPeao('c', 7, Cor.Preta, partida, peaoPreto);
        Util.instanciarPeao('d', 7, Cor.Preta, partida, peaoPreto);
        Util.instanciarPeao('e', 7, Cor.Preta, partida, peaoPreto);
        Util.instanciarPeao('f', 7, Cor.Preta, partida, peaoPreto);
        Util.instanciarPeao('g', 7, Cor.Preta, partida, peaoPreto);
        Util.instanciarPeao('h', 7, Cor.Preta, partida, peaoPreto);

    }

    public void processarMouseDown(GameObject obj, GameObject casa) {
        if(estado == Estado.AguardandoJogada) {
            if(casa != null) {
                try {
                    char coluna = casa.name[0];
                    int linha = casa.name[1] - '0';
                    origem = new PosicaoXadrez(coluna, linha);
                    partida.validarPosicaoDeOrigem(origem.toPosicao());
                    pecaEscolhida = obj;
                    estado = Estado.Arrastando;
                    txtMsg.text = "Selecione a casa de destino";
                    instanciarParticulas();
                }
                catch (TabuleiroException e) {
                    informarAviso(e.Message);
                }
            }
        }
        else if (estado == Estado.Arrastando) {
            GameObject casaDestino = null;
            if (obj.layer == LayerMask.NameToLayer("Casas")) {
                casaDestino = obj;
            }
            else {
                casaDestino = casa;
            }


            if (casaDestino != null && pecaEscolhida != null) {
                    try {
                        char coluna = casaDestino.name[0];
                        int linha = casaDestino.name[1] - '0';
                        destino = new PosicaoXadrez(coluna, linha);

                        partida.validarPosicaoDeDestino(origem.toPosicao(), destino.toPosicao());
                        Peca pecaCapturada = partida.realizaJogada(origem.toPosicao(), destino.toPosicao());

                        if (pecaCapturada != null) {
                            removerObjetoCapturado(pecaCapturada);
                        }

                        pecaEscolhida.transform.position = Util.posicaoNaCena(coluna, linha);

                        tratarJogadasEspeciais();

                        pecaEscolhida = null;

                        if (partida.terminada) {
                            estado = Estado.GameOver;
                            txtMsg.text = "Vencedor: " + partida.jogadorAtual;
                            txtXeque.text = "XEQUEMATE";
                        }
                        else {
                            estado = Estado.AguardandoJogada;
                            informarAguardando();
                            Invoke("girarCamera", 0.5f);
                            txtXeque.text = (partida.xeque) ? "XEQUE" : "";
                        }
                    }
                    catch (TabuleiroException e) {
                        pecaEscolhida.transform.position = Util.posicaoNaCena(origem.coluna, origem.linha);
                        estado = Estado.AguardandoJogada;
                        informarAviso(e.Message);
                    }
                    finally {
                        destruirParticulas();
                    }

                }
        }
    }
        
        void informarAviso(string msg) {
        txtMsg.color = Color.red;
        txtMsg.text = msg;
        Invoke("informarAguardando", 1f) ;
    }

        void informarAguardando(){
            txtMsg.color = corOriginal;
            txtMsg.text = "Aguardando jogada: " + partida.jogadorAtual;
        }

        void removerObjetoCapturado (Peca peca) {
        GameObject obj = peca.obj;
        if(peca.cor == Cor.Branca) {
            obj.transform.position = posDescarteBrancas;
            posDescarteBrancas.z = posDescarteBrancas.z + 0.2f;
        }
        else {
            obj.transform.position = posDescartePretas;
            posDescartePretas.z = posDescartePretas.z - 0.2f;
        }
    }

	void tratarJogadasEspeciais() {

        Posicao pos = destino.toPosicao();
        Peca pecaMovida = partida.tab.peca(pos);

        //jogadaespecial roque pequeno
        if(pecaMovida is Rei && destino.coluna == origem.coluna + 2) {
            GameObject torre = partida.tab.peca(pos.linha, pos.coluna - 1).obj;
            torre.transform.position = Util.posicaoNaCena('f', origem.linha);
        }
        //jogadaespecial roque grande
        if (pecaMovida is Rei && destino.coluna == origem.coluna - 2) {
            GameObject torre = partida.tab.peca(pos.linha, pos.coluna + 1).obj;
            torre.transform.position = Util.posicaoNaCena('f', origem.linha);
        }
        //jogadaespecial promocao
        if(partida.promovida != null) {
            removerObjetoCapturado(partida.promovida);
            Vector3 posPromovida = Util.posicaoNaCena(destino.coluna, destino.linha);
            GameObject prefab = (pecaMovida.cor == Cor.Branca) ? damaBranca : damaPreta;
            GameObject dama = Instantiate(prefab, posPromovida, Quaternion.identity) as GameObject;
            pecaMovida.obj = dama;
        }
    }
    
    void girarCamera() {
        if (partida.jogadorAtual == Cor.Branca) {
            Camera.main.GetComponent<CameraRotacao>().irParaBranca();
        }
        else {
            Camera.main.GetComponent<CameraRotacao>().irParaPreta();
        }
    }

    void instanciarParticulas() {
        listaParticulas.Clear();
        float y = GameObject.Find("planoDasPecas").transform.position.y;

        bool[,] mat = partida.tab.peca(origem.toPosicao()).movimentosPossiveis();
        for (int i = 0; i < partida.tab.linhas; i++) {
            for (int j = 0; j < partida.tab.colunas; j++) {
                if (mat[i,j]) {
                    char coluna = (char)('a' + j);
                    int linha = 8 - i;
                    Vector3 posCasa = GameObject.Find("" + coluna + linha).transform.position;
                    Vector3 pos = new Vector3(posCasa.x, y, posCasa.z);
                    GameObject obj = Instantiate(particulas, pos, Quaternion.identity) as GameObject;
                    listaParticulas.Add(obj);
                }
            }
        }
    }

    void destruirParticulas() {
        foreach (GameObject obj in listaParticulas) {
            Destroy(obj);
        }
        listaParticulas.Clear();
    }
}
