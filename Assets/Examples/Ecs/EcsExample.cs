/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-02-10
 *Description:    Description
 *History:        2020-02-10--
*********************************************************************************/
using IFramework;
using IFramework.Modules.ECS;
using UnityEngine;

namespace IFramework_Demo
{
    [RequireComponent(typeof(APP))]

    public class EcsExample:UnityEngine.MonoBehaviour
	{



        private class SimpleEnity : Enity { }
        private class PlayerComponent : IComponent { }
        private class PCComponent : IComponent { }

        private class RotaComponet:IComponent
        {
            public UnityEngine.GameObject go;
        }
        private struct SpeedComponent: IComponent
        {
            public float speed;
        }
        private class PlayerSystem : ExcuteSystem<SimpleEnity>
        {
            public PlayerSystem(ECSModule module) : base(module) { }
            protected override bool Fitter(SimpleEnity enity)
            {
                return enity.ContainsComponent<PlayerComponent>() && enity.ContainsComponent<RotaComponet>();
            }
            protected override void Excute(SimpleEnity enity)
            {
                RotaComponet rc = enity.GetComponent<RotaComponet>();
                rc.go.transform.Rotate(UnityEngine.Vector3.up, 1);
            }
        }
        private class PCSystem : ExcuteSystem<SimpleEnity>
        {
            public PCSystem(ECSModule module) : base(module) { }
            protected override bool Fitter(SimpleEnity enity)
            {
                return enity.ContainsComponent<PCComponent>() && enity.ContainsComponent<RotaComponet>()&& enity.ContainsComponent<SpeedComponent>();
            }
            protected override void Excute(SimpleEnity enity)
            {
                SpeedComponent sp = enity.GetComponent<SpeedComponent>();
                RotaComponet rc = enity.GetComponent<RotaComponet>();
                rc.go.transform.Rotate(UnityEngine.Vector3.forward,sp.speed);
                sp.speed += 0.01f;
                enity.ReFreshComponent(sp);
            }
        }

        private ECSModule module { get { return Framework.env1.modules.ECS; } }
        private void Start()
        {
            Framework.env1.modules.ECS = Framework.env1.modules.CreateModule<ECSModule>();
            module.AddSystem(new PlayerSystem(module));
            module.AddSystem(new PCSystem(module));

            var player = module.CreateEnity<SimpleEnity>();
            player.AddComponent<PlayerComponent>();
            var playerRO = player.AddComponent<RotaComponet>();
            playerRO.go = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
            playerRO.go.name = "Player";
            playerRO.go.transform.position = new UnityEngine.Vector3(0, -2,0);

            var pc = module.CreateEnity<SimpleEnity>();
            pc.AddComponent<SpeedComponent>();
            pc.AddComponent<PCComponent>();
            var pcRO = pc.AddComponent<RotaComponet>();
            pcRO.go = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
            pcRO.go.name = "Pc";
            pcRO.go.transform.position = new UnityEngine.Vector3(0, 2, 0);
        }

    }
}
