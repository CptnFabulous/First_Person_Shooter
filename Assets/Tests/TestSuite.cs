using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.AI;

namespace Tests
{
    public class TestSuite
    {
        [UnityTest]
        public IEnumerator TestErmTest()
        {
            yield return null; // No yield required
            Assert.IsTrue(true); // Returns true
        }

        // 1
        [UnityTest]
        public IEnumerator BulletsDisappearAfterExpiry()
        {
            GameObject g = Object.Instantiate(Resources.Load("Prefabs/Projectiles/Bullet") as GameObject, Vector3.zero, Quaternion.identity); // Finds bullet prefab in code

            Projectile p = g.GetComponent<Projectile>(); // Searches for projectile monobehaviour in spawned projectile
            // Assigns appropriate variables to projectile
            p.projectileLifetime = 1;
            p.velocity = 1;
            p.gravityMultiplier = 1;
            p.diameter = 1;

            yield return new WaitForSeconds(p.projectileLifetime + 1); // Waits for duration of projectileLifetime plus small delay to be safe
            Debug.Log("Bullet should have despawned");
            yield return new WaitForEndOfFrame();

            Assert.IsTrue(g == null); // Checks if projectile still exists
        }

        [UnityTest]
        public IEnumerator GravityWorks()
        {
            GameObject g = Object.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity); // Instantiates cube
            g.AddComponent<Rigidbody>(); // Applies rigidbody to cube
            Vector3 position = g.transform.position; // Saves current position of cube
            yield return new WaitForFixedUpdate(); // Waits for next FixedUpdate so physics can take effect
            Assert.IsTrue(position != g.transform.position); // Checks if cube's current position is different to its original position
        }
        
        [UnityTest]
        public IEnumerator HitboxTakesDamage()
        {
            // Instantiates 'attacker', adds Character script and Faction scriptableObject
            GameObject o = Object.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), new Vector3(0, 0, -3), Quaternion.identity);
            Character c = o.AddComponent<Character>();
            c.faction = Resources.Load("Factions/Good Guys") as Faction;

            // Creates 'attacked' gameObject and adds health script
            GameObject g = Object.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity);
            Health h = g.AddComponent<Health>();
            int currentHealth = h.health.current; // Saves attacked gameObject's current health
            // Adds DamageHitbox script to object, and links it to health script
            DamageHitbox dh = g.AddComponent<DamageHitbox>();
            dh.healthScript = h;

            Debug.Log(h.health.current);
            Debug.Log(currentHealth);

            yield return new WaitForEndOfFrame();

            Debug.Log("schlep");

            // Deals damage to attacked gameObject's hitbox
            Damage.PointDamage(o, c.faction, dh.gameObject, 5, DamageType.Shot, false);

            Debug.Log("frank");

            yield return new WaitForEndOfFrame();

            Debug.Log("hashbrown");
            Debug.Log(h.health.current);
            Debug.Log(currentHealth);

            // Checks if attacked object's health has changed
            Assert.IsTrue(h.health.current != currentHealth);
        }

        [UnityTest]
        public IEnumerator ProjectileShoots()
        {
            GameObject g = Object.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity); // Creates a 'gun' to shoot the projectile from
            ProjectileData pd = Resources.Load("PlayerWeapons/M16A3 with M203/M16A3 Bullet") as ProjectileData; // Find ProjectileData ScriptableObject in Resources
            Damage.ShootProjectile(pd, 3, 50, null, null, g.transform, g.transform, g.transform.forward); // Declares static function ShootProjectile to launch projectile from ProjectileData, at the desired position and in the desired direction

            yield return new WaitForEndOfFrame(); // Waits until all previous functions have finished

            Projectile p = Object.FindObjectOfType<Projectile>(); // Searches for a projectile gameObject in the scene

            Assert.IsNotNull(p.gameObject); // Asserts if projectile was found
        }
    }
}
