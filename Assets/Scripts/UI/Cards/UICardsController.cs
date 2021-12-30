using System.Collections;
using Encounter;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace UI.Cards
{
	public class UICardsController : MonoBehaviour
	{
		[SerializeField] private UICardsHand cardsHand;
		[SerializeField] private UICardsDeck cardsDeck;
		[SerializeField] private UICardPreview cardPreview;


		public void Init()
		{
			cardsDeck.Init();
			cardPreview.Init();

			for (int i = 0; i < CombatManager.Player.cardsInHand; i++)
			{
				DrawCardToHand();
			}
		}

		[Button]
		public void DrawCardToHand()
		{
			StartCoroutine(TmpDrawCardToHandAnimation());
			
			IEnumerator TmpDrawCardToHandAnimation()
			{
				CardUI card = cardsDeck.DrawCard(CombatManager.Player.GetCard());
				yield return new WaitForSeconds(0.1f);
				cardsHand.ReceiveCard(card);
			}
		}

		[Button]
		public void ResetHand()
		{
			int discardedCards = cardsHand.DiscardHand();
			for (int i = 0; i < discardedCards; i++)
			{
				DrawCardToHand();
			}
		}
	}
}
