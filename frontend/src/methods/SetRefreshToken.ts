export const setRefreshToken = (token: string) => {
  return localStorage.setItem('refreshToken',token);
};