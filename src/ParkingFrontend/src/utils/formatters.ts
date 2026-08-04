import { SlotType } from '../models/types';

export const getSlotEmoji = (type: SlotType): string => {
  switch (type) {
    case SlotType.VIP: return '👑';
    case SlotType.Electric: return '⚡';
    case SlotType.Accessible: return '♿';
    default: return '';
  }
};

export const getUserEmojis = (authorizedTypes: SlotType[]): string => {
  const emojis = authorizedTypes
    .map(getSlotEmoji)
    .filter(e => e !== '')
    .join('');
    
  return emojis ? ` (${emojis})` : '';
};