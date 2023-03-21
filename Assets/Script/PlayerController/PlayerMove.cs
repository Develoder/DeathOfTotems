using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed = 1.3f; //Скорость перемещения
    [SerializeField] private float impulsJump = 5; //Высота прыжка
    private float impulsJumpAdd; // Сила прыжка при попадании на прыжковую платформу
    [SerializeField] private float verticaleMove = 1.3f; // Вертикальная скорость перемещения

    private Vector3 moveForece = new Vector3(0,0,0);
    private new Rigidbody rigidbody;
    private bool jump = true; //Можно ли прыгать
    private bool startJump = false; //Началась ли анимация прыжка

    [SerializeField] Transform downPositionJump; // Разрешающая позиция прыжка


    private Animator animator;// Ссылка на аниматор
    private Transform transformdBody; // Ссылка на туловище персонажа


    float num = 0;


    int playerLayer, variableColaid; // Слои объектов
    
    void Start()
    {
        transformdBody = gameObject.transform.Find("Body").GetComponent<Transform>();
        animator = transformdBody.transform.Find("default").GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody>();

        playerLayer = LayerMask.NameToLayer("Player");
        variableColaid = LayerMask.NameToLayer("VariableCollide");
    }

    // Update is called once per frame
    void Update()
    {
        // Движение в право
        if (Input.GetKey(KeyCode.D))
        {
            moveForece.z = speed;
            transform.position += moveForece * Time.deltaTime;
            animator.SetBool("Move", true);
        }
        // Движение в лево
        else if (Input.GetKey(KeyCode.A))
        {
            moveForece.z = -speed;
            transform.position += moveForece * Time.deltaTime;
            animator.SetBool("Move", true);
        }
        // Прекращение движения
        else
        {
            moveForece.z = -1;
            animator.SetBool("Move", false);
        }

        
        // Заапуск прыжка
        if (Input.GetKeyDown(KeyCode.Space) && jump && !startJump)
        {
            startJump = true;
            jump = false;
            animator.SetBool("Jump", true);
            StartCoroutine(Jump());
        }
        // Запускается анимация падения
        else if (!startJump && !animator.GetBool("Drop"))
        {
            if (rigidbody.velocity.y <= -0.1f)
            {
                animator.SetBool("Drop", true);
            }
        }


        if (rigidbody.velocity.y > 3f)
        {
            Physics.IgnoreLayerCollision(playerLayer, variableColaid, true);
        }
        else
        {
            Physics.IgnoreLayerCollision(playerLayer, variableColaid, false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Прекращение анимации прыжка
        if (!startJump && downPositionJump.position.y > collision.transform.position.y)
        {
            animator.SetBool("Drop", false);
            animator.SetBool("Jump", false);
            jump = true;
        }
    }


    public void SetImpulsJumpAdd (float impulsJump)
    {
        impulsJumpAdd = impulsJump;
    }


    /// <summary>
    /// Оталкивает игрока в противаоположное нарпавление от выстрела
    /// </summary>
    /// <param name="recoilWeapon"> Отдача оружия с заданным направлением </param>
    public void RecoilForse(Vector3 recoilWeapon)
    {
        rigidbody.AddForce(new Vector3(0, recoilWeapon.y, recoilWeapon.z), ForceMode.Impulse);
    }


    // Воспроизведение прыжка
    IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.01f);
        rigidbody.AddForce(new Vector3(0, (impulsJumpAdd == 0 ? impulsJump : impulsJumpAdd), 0), ForceMode.Impulse);
        startJump = false;
    }
}
