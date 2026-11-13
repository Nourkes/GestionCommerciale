export type OrderStatus = 'Draft' | 'Validated' | 'Cancelled';

export interface OrderLine {
  id: number;
  productId: number;
  productReference: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface OrderSummary {
  id: number;
  orderNumber: string;
  clientId: number;
  clientName: string;
  orderDate: string;
  status: OrderStatus;
  lineCount: number;
  totalHT: number;
  totalTTC: number;
}

export interface OrderDetail {
  id: number;
  orderNumber: string;
  clientId: number;
  clientName: string;
  orderDate: string;
  status: OrderStatus;
  totalHT: number;
  totalVAT: number;
  totalTTC: number;
  lines: OrderLine[];
}

export interface SaveOrderLine {
  productId: number;
  quantity: number;
}

export interface SaveOrder {
  clientId: number;
  orderDate: string;
  lines: SaveOrderLine[];
}