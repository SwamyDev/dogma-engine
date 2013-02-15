using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pontification.Components
{
    public class PriestDoor : Component
    {
        private PhysicsComponent _physics;

        public override void Start()
        {
            _physics = GetComponent<PhysicsComponent>();
            _physics.IgnoreGravity = true;
            if (_physics == null)
                throw new ArgumentNullException("Priest door needs a physics component attached");
        }

        public void Trigger()
        {
            if (SceneManagement.SceneInfo.Player.GetComponent<CharacterStats>().Category == CharacterCategory.CC_PRIEST)
                _physics.MoveTo(GameObject.Position - Microsoft.Xna.Framework.Vector2.UnitY * 180);
        }
    }
}
