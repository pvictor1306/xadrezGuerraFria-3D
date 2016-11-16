﻿using UnityEngine;

public class Movimentacao : MonoBehaviour {

    GameController gameController;

    bool colidindo;
    GameObject casa;

    int chaoMask;

    void Awake() {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        colidindo = false;
        casa = null;
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Casas")) {
            colidindo = true;
            casa = other.gameObject;

            chaoMask = LayerMask.GetMask("Chao");
        }
    }

    void OnTriggerExit(Collider other) {
        if(colidindo && other.gameObject == casa) {
            colidindo = false;
            casa = null;
        }
    }

    void OnMouseDown() {
        gameController.processarMouseDown(transform.gameObject, casa);
    }

    void OnMouseUp() {
        gameController.processarMouseUp(transform.gameObject, casa);
    }

    void OnMouseDrag() {
        if(gameController.estado == Estado.Arrastando && gameController.pecaEscolhida == transform.gameObject) {
            Ray raio = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(raio, out hit, 300f, chaoMask)) {
                transform.position = hit.point;
            }
        }
    }

}
