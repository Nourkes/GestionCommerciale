export interface Product {
  id: number;
  reference: string;
  name: string;
  description: string | null;
  unitPriceHT: number;
  stockQuantity: number;
  createdAt: string;
}

export interface SaveProduct {
  reference: string;
  name: string;
  description: string | null;
  unitPriceHT: number;
  stockQuantity: number;
}