export const setRefreshToken = (token: string) => {
    console.log(token);
    return localStorage.setItem('refreshToken',token);
  };
  