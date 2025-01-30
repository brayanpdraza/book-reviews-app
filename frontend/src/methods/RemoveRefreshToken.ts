export const RemoveRefreshToken = async ()=>{
    localStorage.removeItem('refreshToken');
}