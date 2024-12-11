using UnityEngine;

namespace OutOfThisWorld {
    public class MaterialTrigger : Triggerable {
        public Renderer Renderer;
        public Material Material;
        public Material UndoMaterial;

        public override void Trigger() {
            if (Material is not null) {
                Renderer.material = Material;
            }
        }

        public override void Undo() {
            if (UndoMaterial is not null) {
                Renderer.material = UndoMaterial;
            }
        }
    }
}