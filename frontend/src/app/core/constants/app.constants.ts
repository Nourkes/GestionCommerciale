import { OrderStatus } from '../models/order.models';
export const CURRENCY_CODE = 'TND';
export const VAT_RATE = 0.19;          // TVA fixe de 19 % 
export const LOW_STOCK_THRESHOLD = 5;  // en dessous : « stock faible »


export const ORDER_STATUS_LABELS: Record<OrderStatus, string> = {
  Draft: 'Brouillon',
  Validated: 'Validée',
  Cancelled: 'Annulée',
};