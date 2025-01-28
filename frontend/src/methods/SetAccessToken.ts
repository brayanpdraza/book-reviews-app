export const setAccessToken = (token: string) => {
  const isLocalhost = window.location.hostname === 'localhost';
  document.cookie = `access_token=${encodeURIComponent(token)}; path=/; ${
    !isLocalhost ? 'Secure; ' : ''
  }SameSite=Strict`;
};
