using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace jp.lilxyzw.shadercore
{
    internal class SCRemap : Vector2Field, IMaterialPropertyElement
    {
        public MaterialProperty Property { get; set; }
        public string ModuleID { get; set; }
        public string LocalizedLabel { get; set; }

        public SCRemap(MaterialProperty property) : base()
        {
            ((IMaterialPropertyElement)this).InitializeVisualElement(this, UpdateUI, property);
            SCStyles.ApplyVectorStyle(this);

            var x = this.Q<FloatField>("unity-x-input");
            var y = this.Q<FloatField>("unity-y-input");
            x.label = "x";
            y.label = "+";

            var buttonNormal = new Button(() => value = new(1,0)){text = L10n.L("__Normal")};
            var buttonInvert = new Button(() => value = new(-1,1)){text = L10n.L("__Invert")};

            var parent = Children().First(c => c is not Label);
            parent.Clear();
            parent.Add(buttonNormal);
            parent.Add(buttonInvert);
            parent.Add(x);
            parent.Add(y);

            parent.RegisterCallback<SCLocalizeEvent>(e =>
            {
                L10n.Load();
                buttonNormal.text = L10n.L("__Normal");
                buttonInvert.text = L10n.L("__Invert");
            });
        }

        public override void SetValueWithoutNotify(Vector2 newValue)
        {
            if (Property != null)
            {
                var vec = Property.vectorValue;
                Property.vectorValue = new(newValue.x, newValue.y, vec.z, vec.w);
            }
            base.SetValueWithoutNotify(newValue);
        }

        public void UpdateUI()
        {
            if (!Property.hasMixedValue)
            {
                rawValue = Property.vectorValue;
                int index = 0;
                foreach (var f in this.Query<FloatField>().Build())
                    f.SetValueWithoutNotify(rawValue[index++]);
            }
        }
    }
}
