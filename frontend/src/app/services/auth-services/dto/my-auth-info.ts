// DTO to hold authentication information

export interface MyAuthInfo {
  isLoggedIn: boolean;
  userId: number;
  username: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  isAdmin: boolean;
  isPharmacist: boolean;
  isCustomer: boolean;
}
