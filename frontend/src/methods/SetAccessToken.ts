export const setAccessToken = (token: string) => {
    document.cookie = `accessToken=${token}; path=/; Secure; HttpOnly; SameSite=Strict`;
  };