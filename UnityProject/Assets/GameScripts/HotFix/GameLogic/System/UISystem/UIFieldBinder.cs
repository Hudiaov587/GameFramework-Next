using System.Reflection;
using UnityEngine;

namespace GameLogic
{
    public class UIFieldBinder : MonoBehaviour
    {
        public void ExportFields(UIBase layer)
        {
            var layerType = layer.GetType();
            var objType = layer.gameObject.GetType();
            var layerTrans = layer.transform;
            var transType = layerTrans.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            void visit(Transform obj)
            {
                var name = obj.name;
                if (name.Length > 2 && name.StartsWith("m_"))
                {
                    var toVarField = layerType.GetField(name, flags);
                    if (toVarField != null)
                    {
                        var fieldType = toVarField.FieldType;
                        if (fieldType == objType)
                        {
                            toVarField.SetValue(layer, obj.gameObject);
                        }
                        else if (fieldType == transType)
                        {
                            toVarField.SetValue(layer, obj);
                        }
                        else
                        {
                            toVarField.SetValue(layer, obj.GetComponent(fieldType));
                        }
                    }
                }

                if (obj.GetComponent<UIFieldBinder>())
                {
                    return;
                }

                for (var i = 0; i < obj.childCount; ++i)
                {
                    visit(obj.GetChild(i));
                }
            }

            for (var i = 0; i < layerTrans.childCount; ++i)
            {
                visit(layerTrans.GetChild(i));
            }
        }
    }
}