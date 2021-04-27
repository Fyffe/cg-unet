using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class Projectile : MonoBehaviour 
	{
        public string shooter;
        public string weapon;

        public int damage;

        public float launchSpeed;
        public float maxDistance;

        public float drag;

        public Material m_Sim;
        public Material m_Trajectory;

        public LayerMask hitMask;

        protected Vector3 startPos;
        protected Vector3 stopPos;
        protected Vector3 velocity;

        protected static int simSteps;
        protected static float gravity;
        protected static bool debug;

        protected bool stopped;
        protected float stopTime;

        protected LineRenderer trajectory;

        protected bool isInit;

        public void Init(float speed, float range, int dmg, string shooter, string weapon)
        {
            launchSpeed = speed;
            maxDistance = range;
            damage = dmg;

            velocity = transform.forward * launchSpeed;

            this.shooter = shooter;
            this.weapon = weapon;

            simSteps = GameManager.globalGameSettings.projectileSimulationSettings.pss_sim_steps_per_frame;
            gravity = GameManager.globalGameSettings.projectileSimulationSettings.pss_gravity;
            debug = GameManager.globalGameSettings.projectileSimulationSettings.pss_debug_sim;

            startPos = transform.position;

            if(debug)
            {
                SimulateTrajectory();
            }

            isInit = true;
        }

        void SimulateTrajectory()
        {
            GameObject trail = new GameObject("Simulation");
            GameObject traj = new GameObject("Trajectory");
            trail.transform.SetParent(transform);

            LineRenderer line = trail.AddComponent<LineRenderer>();
            trajectory = traj.AddComponent<LineRenderer>();
            trajectory.transform.SetParent(transform);

            line.material = m_Sim;
            line.widthMultiplier = 0.05f;
            line.receiveShadows = false;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            trajectory.material = m_Trajectory;
            trajectory.widthMultiplier = 0.05f;
            trajectory.receiveShadows = false;
            trajectory.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trajectory.positionCount = 0;

            Vector3 p1 = transform.position;
            Vector3 predVelocity = transform.forward * launchSpeed;

            float stepSize = 0.01f;
            int num = Mathf.CeilToInt(1f / stepSize);

            line.positionCount = num + 1;
            int index = 0;

            for (float i = 0; i < 1; i += 0.01f)
            {
                predVelocity += ((Vector3.up * -gravity) + (transform.forward * -drag)) * stepSize;

                line.SetPosition(index, p1);

                Vector3 p2 = p1 + predVelocity * stepSize;

                p1 = p2;
                index++;
            }

            Destroy(trail, 10f);
        }

        void FixedUpdate()
        {
            if(!isInit)
            {
                return;
            }

            if(stopped)
            {
                if(Time.time - stopTime >= 10f)
                {
                    Destroy(trajectory);
                    Destroy(gameObject);
                }

                return;
            }

            Vector3 p1 = transform.position;
            float stepSize = 1f / simSteps;
            
            for(float i = 0; i < 1; i += stepSize)
            {
                if(stopped)
                {
                    break;
                }

                velocity += ((Vector3.up * -gravity) + (transform.forward * -drag)) * stepSize * Time.deltaTime;

                Vector3 p2 = p1 + velocity * stepSize * Time.deltaTime;
                Vector3 dir = p2 - p1;

                Ray ray = new Ray(p1, dir);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, dir.magnitude, hitMask))
                {
                    if (hit.collider)
                    {
                        stopped = true;
                        stopPos = hit.point;

                        DecalManager.instance.SetDecalAt(EDecalType.BULLETHOLE, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

                        int layer = hit.collider.gameObject.layer;

                        if(layer == 10)
                        {
                            PlayerMetabolism meta = hit.transform.root.GetComponent<PlayerMetabolism>();

                            if (meta)
                            {
                                Debug.Log("HIT " + meta.name + " FOR DAMAGE " + damage);
                                meta.DecreaseHealth(damage);
                            }
                        }
                    }
                }

                if(Vector3.Distance(p2, startPos) > maxDistance)
                {
                    stopped = true;
                }

                if(stopped)
                {
                    stopTime = Time.time;
                }

                p1 = p2;

                if (debug)
                {
                    trajectory.positionCount += 1;
                    trajectory.SetPosition(trajectory.positionCount - 1, p1);
                }
            }

            transform.position = p1;
        }
	}
}