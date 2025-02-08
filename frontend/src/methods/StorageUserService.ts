export const StorageKeys = {
  userName: "userName",
  userFotoPerfil: "userFotoPerfil",
};

const userKeys = [StorageKeys.userName, StorageKeys.userFotoPerfil];

const StorageService = {
  set: (key: string, value: string) => localStorage.setItem(key, value),
  get: (key: string): string | null => localStorage.getItem(key),
  remove: (key: string) => localStorage.removeItem(key),
};

export const clearUserData = () => {
  userKeys.forEach((key) => localStorage.removeItem(key));
};

export const setUserName = (email: string) =>
  StorageService.set(StorageKeys.userName, email);
export const getUserName = () =>
  StorageService.get(StorageKeys.userName);
export const removeUserName = () =>
  StorageService.remove(StorageKeys.userName);

export const setUserFotoPerfil = (foto: string) =>
  StorageService.set(StorageKeys.userFotoPerfil, foto);
export const getUserFotoPerfil = () =>
  StorageService.get(StorageKeys.userFotoPerfil);
export const removeUserFotoPerfil = () =>
  StorageService.remove(StorageKeys.userFotoPerfil);