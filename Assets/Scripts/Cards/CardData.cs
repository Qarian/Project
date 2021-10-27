using System;
using System.Collections.Generic;
using System.Linq;
using Card.Actions;
using Entity;

namespace Cards
{
	public struct CardData
	{
		public readonly CardStyle style;
		public readonly string title;
		public readonly int cost;
		public readonly string description;

		public List<ICardAction> subActions;

		public Character owner;

		public CardData(CardDataScriptable origin)
		{
			style = origin.style;
			title = origin.title;
			cost = origin.cost;
			description = origin.description;
			subActions = origin.actions.ToList();
			owner = null;
		}
		
		private CardData(CardStyle style, string title, int cost, string description, List<ICardAction> subActions)
		{
			this.style = style;
			this.title = title;
			this.cost = cost;
			this.description = description;
			this.subActions = subActions;
			owner = null;
		}

		public static CardData Empty => new CardData(null, string.Empty, -1, string.Empty, null);

		public bool IsEmpty => title == string.Empty;

		public static bool operator ==(CardData c1, CardData c2)
		{
			return c1.title == c2.title && c1.style == c2.style;
		}

		public static bool operator !=(CardData c1, CardData c2)
		{
			return !(c1 == c2);
		}
		
		public bool Equals(CardData other)
		{
			return Equals(style, other.style) && title == other.title && cost == other.cost && description == other.description;
		}

		public override bool Equals(object obj)
		{
			return obj is CardData other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (style != null ? style.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (title != null ? title.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ cost;
				hashCode = (hashCode * 397) ^ (description != null ? description.GetHashCode() : 0);
				return hashCode;
			}
		}

		public CardAttackData PrepareAttack()
		{
			CardAttackData attackData = new CardAttackData();
			
			foreach (ICardAction cardAction in subActions)
			{
				cardAction.CreateAttack(attackData, owner);
			}

			return attackData;
		}
	}
}
