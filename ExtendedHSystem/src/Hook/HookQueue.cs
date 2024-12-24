using System.Collections.Generic;
using System.Linq;

namespace ExtendedHSystem.Hook
{
	public class HookQueue
	{
		private LinkedList<Hook> Queue = new LinkedList<Hook>();

		public void Add(Hook hook)
		{
			Queue.AddLast(hook);
		}

		public void AddBefore(string before, Hook hook)
		{
			var beforeHook = Queue.Find(new Hook(before));
			if (beforeHook != null)
				Queue.AddBefore(beforeHook, hook);
			else
				Queue.AddLast(hook);
		}

		public void AddAfter(string after, Hook hook)
		{
			var afterHook = Queue.Find(new Hook(after));
			if (afterHook != null)
				Queue.AddAfter(afterHook, hook);
			else
				Queue.AddLast(hook);
		}

		public void Remove(string uid)
		{
			var hook = Queue.Find(new Hook(uid));
			if (hook != null)
				Queue.Remove(hook);
		}

		public IEnumerable<Hook> GetEnumerable()
		{
			return Queue.AsEnumerable();
		}
	}
}