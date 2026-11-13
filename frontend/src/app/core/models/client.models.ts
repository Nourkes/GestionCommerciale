export interface Client {
  id: number;
  lastName: string;
  firstNameOrCompany: string;
  email: string;
  phone: string;
  address: string;
  createdAt: string;
}

export interface SaveClient {
  lastName: string;
  firstNameOrCompany: string;
  email: string;
  phone: string;
  address: string;
}