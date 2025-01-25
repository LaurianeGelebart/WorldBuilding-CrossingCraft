using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôle les mouvements et l'interaction de la caméra dans l'environnement de jeu
/// </summary>
public class CameraController : MonoBehaviour 
{
    private float _rotationX;     // Rotation sur l'axe X (verticale)
    private float _rotationY;     // Rotation sur l'axe Y (horizontale)
    
    private bool _cameraEnabled;  // État de contrôle de la caméra

    public float sensitivity = 5f;   // Sensibilité de rotation de la caméra
    public float moveSpeed = 12f;    // Vitesse de déplacement de la caméra

    private Rigidbody rb;            // Composant Rigidbody pour les mouvements physiques

    /// <summary>
    /// Initialisation du contrôleur de caméra
    /// </summary>
    void Start()
    {
        // Récupérer le composant Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // Initialiser l'état du curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _cameraEnabled = false;
    }

    /// <summary>
    /// Mise à jour frame par frame du contrôle de caméra
    /// </summary>
    void Update()
    {
        // Basculer le verrouillage de la caméra au clic gauche
        if (Input.GetMouseButtonDown(0))
        {
            _cameraEnabled = !_cameraEnabled;
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = _cameraEnabled ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        // Gérer les mouvements si la caméra est activée
        if (_cameraEnabled)
        {
            HandleMovement();
        }
        else
        {
            // Arrêter tout mouvement si la caméra est désactivée
            rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Gère les mouvements de rotation et de déplacement de la caméra
    /// </summary>
    private void HandleMovement()
    {
        // Gérer la rotation de la caméra
        _rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
        _rotationY += Input.GetAxis("Mouse X") * sensitivity;

        // Limiter la rotation verticale
        _rotationX = Mathf.Clamp(_rotationX, -90, 90);

        // Appliquer la rotation
        transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0);

        // Calculer la direction de déplacement
        Vector3 moveDirection = Vector3.zero;

        // Mouvement avant/arrière (touches Z/S)
        moveDirection += transform.forward * Input.GetAxis("Vertical") * moveSpeed;

        // Mouvement gauche/droite (touches Q/D)
        moveDirection += transform.right * Input.GetAxis("Horizontal") * moveSpeed;

        // Appliquer le mouvement via le Rigidbody
        rb.velocity = moveDirection;
    }
}