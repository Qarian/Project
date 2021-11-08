﻿using Cards;
using Sirenix.OdinInspector;
using UnityEngine;
using Entity;

namespace Entity
{
	[CreateAssetMenu(menuName = "Characters/Player")]
	public class PlayerData : Character
	{
		[Header("Player Specific")]
		[Required]
		[SerializeField] private DeckScriptable startingDeck = default;

		public Deck PermanentDeck
		{
			get
			{
				if (!initialized)
					Initialize();
				return permanentDeck;
			}
		}
		private Deck permanentDeck;

		public Deck TemporaryDeck
		{
			get
			{
				if (!initialized)
					Initialize();
				return temporaryDeck;
			}
		}
		private Deck temporaryDeck;

		private bool initialized;

		private void Initialize()
		{
			permanentDeck = new Deck(startingDeck.cards);
			temporaryDeck = new Deck(startingDeck.cards);
		}
	}
}