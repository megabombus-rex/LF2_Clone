using LF2Clone.Base;
using LF2Clone.UI;
using System.Numerics;

namespace LF2Clone.Components.CustomScripts
{
    public class TestScript : CustomScript
    {
        private Image? _image;

        public TestScript(Node node, bool isDrawable, bool isActive, int id) : base(node, isDrawable, isActive, id)
        {
        }

        public override void Awake()
        {
            base.Awake();
            _image = _node.GetComponentByType<Image>();
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Update()
        {
            base.Update();
            _node.MoveNodeByVector(new Vector3(1.0f, 0.001f, 0.0f));
        }
    }
}
