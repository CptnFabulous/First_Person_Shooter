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
            yield return null;
            Assert.IsTrue(true);
        }

        // 1
        [UnityTest]
        public IEnumerator BulletsDisappearAfterExpiry()
        {
            GameObject g = Object.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, Vector3.zero, Quaternion.identity);

            Projectile p = g.GetComponent<Projectile>();
            p.projectileLifetime = 1;
            p.velocity = 1;
            p.gravityMultiplier = 1;
            p.diameter = 1;

            yield return new WaitForSeconds(2);

            Assert.IsTrue(g == null);
        }

        [UnityTest]
        public IEnumerator GravityWorks()
        {
            GameObject g = Object.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity);
            Vector3 position = g.transform.position;
            g.AddComponent<Rigidbody>();
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(position != g.transform.position);
        }
        
        [UnityTest]
        public IEnumerator HitboxTakesDamage()
        {
            GameObject o = Object.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), new Vector3(0, 0, -3), Quaternion.identity);
            Character c = o.AddComponent<Character>();
            c.faction = Resources.Load("Factions/Good Guys") as Faction;

            GameObject g = Object.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity);
            Health h = g.AddComponent<Health>();
            int currentHealth = h.health.current;
            DamageHitbox dh = g.AddComponent<DamageHitbox>();
            dh.healthScript = h;

            Debug.Log(h.health.current);
            Debug.Log(currentHealth);

            yield return new WaitForEndOfFrame();

            Debug.Log("schlep");

            Damage.PointDamage(o, c.faction, dh.gameObject, 5, DamageType.Shot, false);
            Debug.Log("frank");

            yield return new WaitForEndOfFrame();

            Debug.Log("hashbrown");

            Debug.Log(h.health.current);
            Debug.Log(currentHealth);

            Assert.IsTrue(h.health.current != currentHealth);
        }

        [UnityTest]
        public IEnumerator ProjectileShoots()
        {
            GameObject g = Object.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, Quaternion.identity);
            ProjectileData pd = Resources.Load("PlayerWeapons/M16A3 with M203/M16A3 Bullet") as ProjectileData;
            Damage.ShootProjectile(pd, 3, 50, null, null, g.transform, g.transform, g.transform.forward);

            yield return new WaitForEndOfFrame();

            Projectile p = Object.FindObjectOfType<Projectile>();

            Assert.IsNotNull(p.gameObject);
        }
    }
}
