using System.Collections;
using HFramework.Handlers;
using HFramework.Scenes;
using UnityEngine;

namespace Gallery.Handlers
{
	/// <summary>
	/// Sets animation "name" to loop indefinetely in "animation"
	/// </summary>
	public class GalleryPlayerGrappled : BaseHandler
	{
		private readonly CommonStates Player;

		public GalleryPlayerGrappled(IScene scene,CommonStates player) : base(scene)
		{
			this.Player = player;
		}

		protected override IEnumerator Run()
		{
			while (this.Scene.CanContinue() && !Input.GetMouseButtonDown(0))
			{
				yield return null;
			}

			this.Player.faint = 0.0f;
		}
	}
}
