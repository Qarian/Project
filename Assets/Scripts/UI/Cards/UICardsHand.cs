using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Cards
{
	public class UICardsHand : MonoBehaviour, ICardHolder
	{
		[Range(0f, 90f)] [SerializeField] private float maxCardRotation;
		[MinValue(1f)] [SerializeField] private int maxCardsForCentering = 6;

		[Space] [SerializeField] private Vector2 cardsAnchor;

		private readonly List<CardUI> cardsInHand = new List<CardUI>(10);

		private new RectTransform transform;

		private float centeringOffset;

		private void Awake()
		{
			transform = GetComponent<RectTransform>();
		}
		
		public bool ReceiveCard(CardUI card)
		{
			card.Parent = this;
			card.OnCardReparented += RemoveCard;
			if (cardsInHand.Contains(card))
			{
				MoveCardToPosition(card, cardsInHand.IndexOf(card));
				return true;
			}

			cardsInHand.Add(card);
			Refresh();
			return true;
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

		[Button]
		public void RemoveCard(int index)
		{
			cardsInHand.RemoveAt(index);
			Refresh();
		}

		public void RemoveCard(CardUI card)
		{
			card.OnCardReparented -= RemoveCard;
			cardsInHand.Remove(card);
			Refresh();
		}

		public void MoveCardToPosition(CardUI card, int position)
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
	}
}