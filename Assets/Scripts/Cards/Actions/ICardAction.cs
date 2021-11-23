﻿using UI.Entities;

namespace Card.Actions
{
    public interface ICardAction
    {
        public void CreateAttack(CardAttackData data, Character player);
    }
}