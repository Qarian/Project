using System.Collections.Generic;
using Managers;
using Sirenix.OdinInspector;
using UI.Cards;
using UI.Entities;
using UnityEngine;
using Utilities;
using Zenject;

namespace UI.CardsManagement
{
	public class UICardsHand : MonoBehaviour, ICardProvider
	{
		[Range(0f, 90f)] [SerializeField] private float maxCardRotation;
		[MinValue(1f)] [SerializeField] private int maxCardsForCentering = 6;

		[Space] [SerializeField] private Vector2 cardsAnchor;

		private readonly List<CardUI> cardsInHand = new (10);

		private new RectTransform transform;

		private float centeringOffset;

		private PlayerEntity player;
		private EncounterManager encounterManager;

		[Inject]
		private void Init(PlayerEntity playerEntity, EncounterManager encounterManager)
		{
			player = playerEntity;
			this.encounterManager = encounterManager;
			
			transform = GetComponent<RectTransform>();
		}

		public bool CanReceiveCard(CardUI card)
		{
			return true;
		}

		public void ReceiveCard(CardUI card)
		{
			card.SetParent(this, transform);
			card.ChangeCardActive(encounterManager.PlayerActionsEnabled);
			if (cardsInHand.Contains(card))
			{
				MoveCardToPosition(card, cardsInHand.IndexOf(card));
				return;
			}

			cardsInHand.Add(card);
			Refresh();
		}

		public void DragAwayCard(CardUI card)
		{
			card.RotateCard(0);
		}

		public void RemoveCard(CardUI card)
		{
			cardsInHand.Remove(card);
			Refresh();
		}

		public void ReturnCard(CardUI card)
		{
			if (cardsInHand.Contains(card))
			{
				MoveCardToPosition(card, cardsInHand.IndexOf(card));
			}
			else
			{
				Debug.LogError("Card wasn't in hand before");
				ReceiveCard(card);
			}
		}

		private void MoveCardToPosition(CardUI card, int position)
		{
			float xPos = (position + centeringOffset) / Mathf.Max(maxCardsForCentering - 1, cardsInHand.Count - 1);
			Vector2 pos = new Vector2(xPos - cardsAnchor.x, 1 - Mathf.Pow(1 - 2 * xPos, 2) - cardsAnchor.y);

			pos *= transform.sizeDelta;
			card.SetParent(transform);
			card.AnchoredPosition = pos;
			card.RotateCard(maxCardRotation * (0.5f - xPos) * 2);
		}

		[Button]
		public void Refresh()
		{
			centeringOffset = Mathf.Clamp((maxCardsForCentering - cardsInHand.Count) / 2f, 0f, Mathf.Infinity);
			for (int i = 0; i < cardsInHand.Count; i++)
			{
				MoveCardToPosition(cardsInHand[i], i);
			}
		}

		public int DiscardHand()
		{
			int cardsCount = cardsInHand.Count;
			foreach (CardUI card in cardsInHand)
			{
				player.DiscardCard(card.data);
				PoolsManager.Remove(card);
			}

			return cardsCount;
		}
	}
}
