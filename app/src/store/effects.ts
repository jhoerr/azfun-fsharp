const clearAuthToken = () =>
    sessionStorage.removeItem('authToken')

const getAuthToken = () => 
    sessionStorage.getItem("authToken")

const setAuthToken = (token: string) => 
    sessionStorage.setItem('authToken', token)

export { 
    clearAuthToken,
    getAuthToken,
    setAuthToken 
}
