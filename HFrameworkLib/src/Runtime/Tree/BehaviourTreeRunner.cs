using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFramework.Tree
{
	public class BehaviourTreeRunner : MonoBehaviour
	{
		public BehaviourTree tree;

		// Start is called before the first frame update
		void Start()
		{
			tree = tree.Clone();
			// tree = ScriptableObject.CreateInstance<BehaviourTree>();

			// var log1 = ScriptableObject.CreateInstance<DebugLogNode>();
			// log1.message = "HElloo 111";

			// var pause1 = ScriptableObject.CreateInstance<WaitNode>();


			// var log2 = ScriptableObject.CreateInstance<DebugLogNode>();
			// log2.message = "HElloo 222";

			// var pause2 = ScriptableObject.CreateInstance<WaitNode>();


			// var log3 = ScriptableObject.CreateInstance<DebugLogNode>();
			// log3.message = "HElloo 333";

			// var pause3 = ScriptableObject.CreateInstance<WaitNode>();

			// var seq = ScriptableObject.CreateInstance<SequencerNode>();
			// seq.children.Add(log1);
			// seq.children.Add(pause1);
			// seq.children.Add(log2);
			// seq.children.Add(pause2);
			// seq.children.Add(log3);
			// seq.children.Add(pause3);

			// var loop = ScriptableObject.CreateInstance<RepeatNode>();
			// loop.child = seq;

			// tree.rootNode = loop;
		}

		// Update is called once per frame
		void Update()
		{
			tree.Update();
		}
	}
}
