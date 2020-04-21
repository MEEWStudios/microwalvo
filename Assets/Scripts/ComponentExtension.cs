using UnityEngine;
using System.Collections.Generic;

namespace UnityEngine {
	public static class Extension {
		public static T[] GetComponentsInDirectChildren<T>(this Component parent) where T : Component {
			List<T> tmpList = new List<T>();

			foreach (Transform transform in parent.transform) {
				T component;
				if ((component = transform.GetComponent<T>()) != null) {
					tmpList.Add(component);
				}
			}

			return tmpList.ToArray();
		}
	}
}
