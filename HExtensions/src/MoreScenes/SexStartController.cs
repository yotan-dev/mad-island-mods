using System.Collections;
using HFramework;
using HFramework.Scenes;
using UnityEngine;
using YotanModCore;

namespace HExtensions.MoreScenes
{
	public class SexStartController : MonoBehaviour
	{
		public static SexStartController Create()
		{
			return new GameObject().AddComponent<SexStartController>();
		}

		public IEnumerator ChooseLocation(CommonStates man, CommonStates girl, SexStartControllerState state)
		{
			LayerMask tmpLayer = LayerMask.GetMask(["Ground", "BG", "Water", "Platform"]);
			var positionable = false;
			var positioned = false;
			GameObject tmpPlace = null;
			Transform tmpPos = null;
			var sexPos = Vector3.zero;
			ItemInfo onProp = null;
			var bedGrade = 0;
			Managers.mn.uiMN.SetCircleRad(0, 0.2f);
			Managers.mn.uiMN.SetCircleRad(1, 0.2f);
			Managers.mn.gameMN.Controlable(false, true);
			Managers.mn.uiMN.ControlTextActive(true, Managers.mn.textMN.texts[28]);
			Managers.mn.uiMN.npcPanel.SetActive(false);

			while (Managers.mn.gameMN.menuNPC != null && !positioned)
			{
				Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0f, 0f, 5f)) - Camera.main.transform.position;
				RaycastHit selectionRaycastHit;
				if (Physics.Raycast(Camera.main.transform.position, direction, out selectionRaycastHit, 100f, tmpLayer))
				{
					GameObject targetedObject = selectionRaycastHit.collider.gameObject;
					if (Vector3.Distance(man.gameObject.transform.position, selectionRaycastHit.point) <= 5f && targetedObject.layer != 4)
					{
						// Checks if it is a new target
						if (tmpPlace != targetedObject)
						{
							tmpPlace = targetedObject;

							// Ensures it is in the same house or is following the player (otherwise it doesn't make sense)
							if (Managers.mn.npcMN.follower.Contains(girl) || girl.houseID == Managers.mn.gameMN.tmpHouse)
							{
								if (selectionRaycastHit.collider.gameObject.layer == 12)
								{
									// Check if targeting a object
									ItemInfo component = targetedObject.GetComponent<ItemInfo>();
									if (component != null && component.houseID == Managers.mn.gameMN.tmpHouse)
									{
										// If it is a bed, get its bed grade
										ItemData itemData = Managers.mn.itemMN.FindItem(component.itemKey);
										if (itemData != null && itemData.itemType == ItemData.ItemType.Prop && itemData.subType == ItemData.SubType.Bed)
										{
											tmpPos = targetedObject.transform.Find("pos").transform;
											onProp = component;
											positionable = true;
											SexPlace component2 = component.GetComponent<SexPlace>();
											if (component2 != null)
											{
												bedGrade = component2.grade;
											}
										}
										else
										{
											positionable = false;
										}
									}
									else
									{
										positionable = false;
									}
								}
								else
								{
									positionable = true;
									onProp = null;
								}
							}
							else
							{
								positionable = false;
							}
						}
					}
					else
					{
						positionable = false;
					}
				}
				else
				{
					positionable = false;
				}

				// Clear position object leftovers
				if (!positionable)
				{
					onProp = null;
				}

				// Fix the position
				if (onProp != null)
				{
					sexPos = tmpPos.position;
				}
				else
				{
					sexPos = selectionRaycastHit.point;
				}

				// Changes cursor between blue/red
				if (positionable)
				{
					Managers.mn.uiMN.SetCircle(0, true, sexPos);
					Managers.mn.uiMN.SetCircle(1, false, default(Vector3));
				}
				else
				{
					tmpPlace = null;
					Managers.mn.uiMN.SetCircle(1, true, selectionRaycastHit.point);
					Managers.mn.uiMN.SetCircle(0, false, default(Vector3));
				}

				// Confirm the slection
				if (Input.GetMouseButtonDown(0) && positionable && !Managers.mn.gameMN.OnUI)
				{
					positioned = true;
				}

				// Cancel
				if (Input.GetMouseButton(1))
				{
					positionable = false;
					positioned = true;
				}

				// Wait next tick
				yield return null;
			}

			// Finish up
			Managers.mn.uiMN.ControlTextActive(false, "");
			Managers.mn.uiMN.NPCMenuClose();

			state.Positionable = positionable;
			state.SexPos = sexPos;
			state.BedGrade = bedGrade;
			state.CanContinue = positionable;
		}

		private void ClearCircles()
		{
			Managers.mn.uiMN.SetCircle(0, false, default(Vector3));
			Managers.mn.uiMN.SetCircle(1, false, default(Vector3));
		}

		private IEnumerator Teardown()
		{
			this.ClearCircles();
			yield return Managers.mn.story.wait0p2;
			Managers.mn.gameMN.Controlable(true, true);
		}

		public IEnumerator StartSex(CommonStates man, CommonStates girl)
		{
			PLogger.LogError(">>> GOT HTHERER");
			var state = new SexStartControllerState();
			yield return this.ChooseLocation(man, girl, state);
			if (!state.CanContinue)
			{
				yield return this.Teardown();
				yield break;
			}

			float timeOut = 1f;
			if (girl.sex == CommonStates.SexState.Playing)
			{
				girl.nMove.actType = NPCMove.ActType.Idle;
				while (girl.sex != CommonStates.SexState.None && timeOut > 0f)
				{
					timeOut -= Time.deltaTime;
					yield return null;
				}
				
				girl.nMove.actType = NPCMove.ActType.Wait;
				if (timeOut <= 0f)
					state.CanContinue = false;
			}

			if (!state.CanContinue)
			{
				yield return this.Teardown();
				yield break;
			}

			girl.sex = CommonStates.SexState.Playing;
			bool go2 = false;
			timeOut = 8f;
			Managers.mn.npcMN.StartCoroutine(Managers.mn.story.MovePosition(girl.gameObject, state.SexPos, 2f, "A_walk", true, false, 0.1f, 40f));
			Managers.mn.npcMN.StartCoroutine(Managers.mn.story.MovePosition(man.gameObject, state.SexPos, 2f, "A_walk", true, false, 0.1f, 40f));
			while (!go2 && timeOut > 0f)
			{
				timeOut -= Time.deltaTime;
				if (Vector3.Distance(girl.gameObject.transform.position, state.SexPos) > 0.2f && girl.anim.state.GetCurrent(0).Animation.Name != "A_walk")
				{
					girl.anim.state.SetAnimation(0, "A_walk", true);
					if (girl.nMove.rb.isKinematic)
					{
						girl.nMove.RBState(true);
					}
				}
				if (Vector3.Distance(man.gameObject.transform.position, state.SexPos) <= 0.2f)
				{
					go2 = true;
				}
				else if (man.anim.state.GetCurrent(0).Animation.Name != "A_walk")
				{
					man.anim.state.SetAnimation(0, "A_walk", true);
				}
				yield return null;
			}
			
			this.ClearCircles();
			if (timeOut > 0f)
			{
				PLogger.LogError(">>> Starting");
				var playerPerformer = new CommonSexPlayerPerformer(
						ManRapes.Name, "HF_Man_FemaleNative_Rape_Fainted", man, girl, state.SexPos, state.BedGrade
				);
				
				yield return Managers.mn.npcMN.StartCoroutine(
					playerPerformer.Run()	
					// Managers.mn.sexMN.CommonSexPlayer(0, man, girl, state.SexPos, state.BedGrade)

					// (string sceneId, string performerId, CommonStates playerCommon, CommonStates npcCommon, Vector3 pos, int sexType)
				);
			}
			else
			{
				girl.sex = CommonStates.SexState.None;
				girl.nMove.actType = NPCMove.ActType.Idle;
				man.anim.state.SetAnimation(0, "A_idle", true);
				man.nMove.RBState(true);
				Managers.mn.npcMN.StartCoroutine(Managers.mn.eventMN.GoCaution(41));
			}

			yield return this.Teardown();
		}

	}
}
