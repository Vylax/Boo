using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/** Unity OnMouseDown has never worked properly, except in trivial scenes with only 1/few objects

 c.f. */
public class FixUnityBrokenMouseDown : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            List<RaycastHit> orderedHits = new List<RaycastHit>(Physics.RaycastAll(mouseRay));
            orderedHits.Sort((h1, h2) => h1.distance.CompareTo(h2.distance));

            bool hasBeenConsumed = false;
            foreach (RaycastHit hit in orderedHits)
            {
                if (hasBeenConsumed)
                {
                    break;
                }

                Debug.Log($"hit.name={hit.transform.name}");

                ComponentWithMethod target = ComponentThatCanReceiveMethod(hit.collider.gameObject, "FixedOnMouseDown");

                if (target.component != null)
                {
                    object result = target.method.Invoke(target.component, null);
                    bool didConsume = (result == null) ? (true /** assume consumption if not specified */ ) : (bool)result;

                    if (!didConsume)
                    {
                        continue;
                    }
                    else
                    {
                        hasBeenConsumed = true;
                        break;
                    }
                }
            }
        }
    }

    struct ComponentWithMethod
    {
        public Component component;
        public MethodInfo method;
    }

    private ComponentWithMethod ComponentThatCanReceiveMethod(GameObject go, string methodName)
    {
        foreach (Component subComponent in go.GetComponents(typeof(Component)))
        {
            MethodInfo info = subComponent.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (info != null)
            {
                ComponentWithMethod result = new ComponentWithMethod();
                result.component = subComponent;
                result.method = info;
                return result;
            }
        }

        /** 
		didn't find aything on this object or its components.
		
		So ... check again on parent object. Keep going till you find a match or fail
		*/
        if (go.transform.parent != null)
        {
            return ComponentThatCanReceiveMethod(go.transform.parent.gameObject, methodName);
        }

        return new ComponentWithMethod();
    }
}