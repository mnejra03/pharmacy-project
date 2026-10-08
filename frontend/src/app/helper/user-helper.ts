import {MyAppUser} from '../services/admin-user.service';

export function prepareFullUserObject(user: MyAppUser) {
  return {
    id: user.id,
    username: user.username,
    firstName: user.firstName,
    lastName: user.lastName,
    isAdmin: user.isAdmin,
    isPharmacist: user.isPharmacist,
    isCustomer: user.isCustomer
  };
}

