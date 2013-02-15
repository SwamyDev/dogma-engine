using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pontification.Components
{
    public class LinkComponent : Component
    {
        #region Public properties
        public GameObject Slave { get; set; }
        #endregion

        #region Public methods
        public override void Update(GameTime gameTime)
        {
            Slave.Position = GameObject.Position;
        }
        #endregion
    }
}
