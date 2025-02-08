export const GetAccessToken = () =>
{
    const cookie = document.cookie
    .split('; ')
    .find(row => row.startsWith('access_token=')); // Usa el mismo nombre que en setAccessToken

  return cookie ? cookie.split('=')[1] : null;
};
    