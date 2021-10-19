using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * classe qui permet de gérer les déplacement et les controle du joeur dans le jeu
 * 
 * elle permet un déplacement en vu fps et une vu a 360 degré avec la caméra du joueur
 * 
 */
public class PlayerControl : MonoBehaviour
{
    
    
    [SerializeField] public Transform dudecam; //objet assigné dans unity
    [SerializeField] public Camera camtop;//objet assigné dans unity
    [SerializeField] public Camera playercam;//objet assigné dans unity


    CharacterController Player = null; //objet assigné dans unity
    Vector3 startPos;
    Quaternion startrota;
    float rotation = 1f; 

    [SerializeField] float walkspeed = 3f;

    // Start is called before the first frame update

    void Start()
    {
        Player = GetComponent<CharacterController>();
        camtop.enabled = false;
        playercam.enabled = true;
        startPos = this.transform.position;
        startrota = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Updatecam();
        Walk();
        upview();
        updatepos();

    }
    //permet de faire bouger la caméra sur l'axe de X
    void Updatecam()
    {
        Vector2 delta = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
      
        transform.Rotate(Vector3.up * delta*6f*Time.timeScale);

        
    }
    //fonction de déplacement du joueur
    void Walk()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();
        Vector3 speed = (transform.forward * input.y + transform.right * input.x)*walkspeed;
        Player.Move(speed * Time.deltaTime);
    }
    //fonction qui permet de faire revenir le joueur à la case départ en appuyant sur B
    void updatepos()
    {
        if (Input.GetKey(KeyCode.B))
        {
            
            this.transform.position = startPos;
            transform.rotation = Quaternion.Slerp(transform.rotation, startrota, Time.time * rotation);

        }
    }
  
    //fonction qui permet de switcher entre la caméra fps et vu de dessus
    void upview()
    {
        if (Input.GetKeyDown("space"))
        {
            camtop.enabled = true;
            playercam.enabled = false;
        }
        else if (Input.GetKeyUp("space"))
        {
            camtop.enabled = false;
            playercam.enabled = true;
        }
    }


}


