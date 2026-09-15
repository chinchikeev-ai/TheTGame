using System;
using System.Linq;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class HectorAnimationContractTests
    {
        [Test]
        public void HectorCombatActions_CoverAllAuthoredPresentationStates()
        {
            string[] actual = Enum.GetNames(typeof(HectorCombatAction));
            string[] expected =
            {
                "None",
                "BasicAttack",
                "WarCry",
                "ShieldWall",
                "SpearThrow",
                "Ultimate",
                "Downed"
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void HectorPresentationBridge_ExposesAnimationPhaseGameplayHooks()
        {
            Type type = typeof(HectorPresentationBridge);

            AssertMethod(type, "PlayWarCryImpact", typeof(Action));
            AssertMethod(type, "PlayShieldWallImpact", typeof(Action));
            AssertMethod(type, "PlaySpearImpact", typeof(UnityEngine.Vector3), typeof(Action));
            AssertMethod(type, "PlayUltimateImpact", typeof(Action));
            AssertMethod(type, "PlayWarCryEffect", typeof(float));
            AssertMethod(type, "PlayShieldWallEffect", typeof(UnityEngine.Vector3), typeof(float));
            AssertMethod(type, "PlayUltimateEffect", typeof(float));
            AssertMethod(type, "SetDownedState", typeof(bool));
            AssertMethod(type, "PlayReviveEffect");
        }

        [Test]
        public void HectorFallback_ExposesHitAndReviveReactions()
        {
            Type type = typeof(HectorMotionFallbackAnimator);
            AssertMethod(type, "PlayHitReaction", typeof(bool));
            AssertMethod(type, "PlayRevive");
        }

        static void AssertMethod(Type type, string name, params Type[] parameters)
        {
            var method = type.GetMethod(name, parameters);
            Assert.That(method, Is.Not.Null,
                $"{type.Name} must expose {name}({string.Join(", ", parameters.Select(p => p.Name))}) for the Hector animation contract.");
        }
    }
}
