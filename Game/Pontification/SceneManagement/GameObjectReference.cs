using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Pontification.Components;

namespace Pontification.SceneManagement
{
    public class GameObjectReference : DataObject
    {
        public string Reference { get; set; }

        public override void Assign(ContentManager cm, Component comp, string binder)
        {
            var prop = comp.GetType().GetProperty(binder);

            // Find game object with the name stored in Referecne.
            GameObject go = comp.FindGameObject(Reference);

            if (go == null)
            {
                var info = new GameObjectReferenceInfo();
                info.Component = comp;
                info.Property = prop;
                info.Name = Reference;
                comp.GameObject.Scene.PendingGameObjectRefereces.Add(info);
            }

            prop.SetValue(comp, go, null);
        }
    }
}
