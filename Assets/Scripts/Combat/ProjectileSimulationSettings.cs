namespace LV
{
    [System.Serializable]
	public class ProjectileSimulationSettings
	{
        public float pss_gravity = 9.81f;
        public int pss_sim_steps_per_frame= 15;
        public bool pss_debug_sim = true;
    }
}