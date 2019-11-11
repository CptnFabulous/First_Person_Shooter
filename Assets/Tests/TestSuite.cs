using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
    }
}
