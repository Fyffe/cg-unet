using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerRoot root;
        public Rigidbody rb;
        private CapsuleCollider worldCol;
        private Camera cam;

        public float moveSpeed = 3f;
        public float sprintSpeed = 5f;

        public float jumpPower = 5f;
        public float jumpCooldown = 0.1f;

        private float landTime;

        public PlayerStates states;
        public PlayerCombatStates combatStates;

        private Vector3 moveDir;

        [SerializeField]
        private LayerMask groundMask;

        private bool isInit = false;

        private float mouseX;
        private float mouseY;

        public float vertical;
        public float horizontal;

        public float mouseSens;

        private float rotationX;
        private float rotationY;

        public LayerMask itemsMask;

        private Vector3 recoil;
        private float recoilTime = 0;

        private bool disabled = false;

        public void Init(PlayerRoot r)
        {
            root = r;
            rb = GetComponent<Rigidbody>();
            worldCol = GetComponent<CapsuleCollider>();
            cam = GetComponentInChildren<Camera>();

            combatStates = root.weapons.combatStates;

            root.meta.onDeath += DisableController;

            root.inGameGUI.onOpen += OnMenuOpen;
            root.inGameGUI.onClose += OnMenuClose;

            isInit = true;
        }

        void OnDisable()
        {
            if(!isInit)
            {
                return;
            }

            root.inGameGUI.onOpen -= OnMenuOpen;
            root.inGameGUI.onClose -= OnMenuClose;
        }

        void Update()
        {
            if (!isInit || disabled)
            {
                return;
            }

            states.isGrounded = CheckGround();

            HandleInput();
            HandleFalling();
            HandleJumping();
            HandleItems();
        }

        void FixedUpdate()
        {
            if (!isInit || disabled)
            {
                return;
            }

            HandleMovement();
        }

        void LateUpdate()
        {
            if (!isInit || disabled)
            {
                return;
            }

            HandleCamera();
        }

        bool CheckGround()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position + Vector3.one * 0.025f, -transform.up, out hit, 0.1f, groundMask))
            {
                if (hit.collider)
                {
                    if (!states.isGrounded && states.isJumping)
                    {
                        states.isJumping = false;
                        landTime = Time.time;
                    }

                    return true;
                }
            }

            return false;
        }

        public void OnMenuOpen()
        {
            root.vicinity.DetectItemsInVicinity();
            states.inMenus = true;
        }

        public void OnMenuClose()
        {
            root.vicinity.ClearItems();
            states.inMenus = false;
        }

        void HandleInput()
        {
            if (!states.inMenus && states.canMove)
            {
                vertical = Input.GetAxis("Vertical");
                horizontal = Input.GetAxis("Horizontal");
            }
            else
            {
                vertical = Mathf.MoveTowards(vertical, 0, Time.deltaTime * 5);
                horizontal = Mathf.MoveTowards(horizontal, 0, Time.deltaTime * 5);
            }

            if (!states.inMenus && states.canMove)
            {
                mouseX = Input.GetAxis("Mouse X") * mouseSens;
                mouseY = Input.GetAxis("Mouse Y") * mouseSens;
            }
            else
            {
                mouseX = 0;
                mouseY = 0;
            }
            
            moveDir = ((transform.forward * vertical) + (transform.right * horizontal)).normalized;
            
            states.isSprinting = Input.GetButton("Sprint") && !combatStates.isAiming && (vertical > 0 || !(vertical <= 0.1f && Mathf.Abs(horizontal) > 0.1f));
            states.isMoving = moveDir.magnitude > 0.2f;
        }

        void HandleJumping()
        {
            if(Time.time - landTime > jumpCooldown)
            {
                if (states.isGrounded && Input.GetButtonDown("Jump"))
                {
                    rb.velocity += Vector3.up * jumpPower;
                    states.isJumping = true;
                }
            }
        }

        void HandleFalling()
        {
            if(rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (2.5f - 1f) * Time.deltaTime;
            }
            else if(rb.velocity.y > 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (2f - 1f) * Time.deltaTime;
            }
        }

        void HandleMovement()
        {
            float speed = moveSpeed;

            if(states.isSprinting)
            {
                speed = sprintSpeed;
            }

            Vector3 moveVector = moveDir * speed * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + moveVector);
        }

        void HandleCamera()
        {
            float multi = 1;

            if(recoilTime > 0)
            {
                multi = Time.deltaTime * 15;
            }

            if(states.canMove)
            {
                rotationX += mouseY + recoil.y;
                rotationY += mouseX + recoil.x;
            }

            recoil = Vector3.zero;
            
            rotationX = Mathf.Clamp(rotationX, -75, 60);

            Quaternion bodyRot = Quaternion.AngleAxis(rotationY, Vector3.up);
            Quaternion camRot = Quaternion.AngleAxis(rotationX, Vector3.left);

            if (states.canMove)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, bodyRot, multi);
                cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, camRot, multi);
            }

            recoilTime -= Time.deltaTime;
            recoilTime = Mathf.Clamp01(recoilTime);
        }

        void HandleItems()
        {
            if(!states.canMove || states.inMenus)
            {
                root.inGameGUI.picker.ToggleDisplay(false);
                return;
            }

            RaycastHit hit;

            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2.5f, itemsMask))
            {
                if(hit.collider)
                {
                    GameObject hitGO = hit.collider.gameObject;

                    if(hitGO.layer == 12)
                    {
                        ItemEntity ent = hitGO.GetComponent<ItemEntity>();

                        if (ent)
                        {
                            if (root.inGameGUI.picker.setEntity != ent)
                            {
                                root.inGameGUI.picker.SetItem(ent);
                            }

                            if (Input.GetButtonDown("Pick Up"))
                            {
                                root.itManager.RequestPickUpItem(ent.identity);
                            }
                        }
                        else
                        {
                            root.inGameGUI.picker.ToggleDisplay(false);
                        }
                    }
                    else
                    {
                        root.inGameGUI.picker.ToggleDisplay(false);
                    }
                }
                else
                {
                    root.inGameGUI.picker.ToggleDisplay(false);
                }
            }
            else
            {
                root.inGameGUI.picker.ToggleDisplay(false);
            }
        }

        public void AddRecoil(float x, float y)
        {
            recoil = new Vector3(x, y); 
            recoilTime += 0.09f;
        }

        public void DisableController()
        {
            disabled = true;

            moveDir = Vector3.zero;
        }
	}
}