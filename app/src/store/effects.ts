const clearAuthToken = () =>
    sessionStorage.removeItem('authToken')

const getAuthToken = () => 
    sessionStorage.getItem("authToken")

const setAuthToken = (token: string) => 
    sessionStorage.setItem('authToken', token)

const callApi = (method: string, url: string, path: string, data?: any, headers?: any) => {
    const combinedHeaders = {
        Accept: 'application/json',
        'Content-Type': 'application/json',
        ...headers
    }
    return fetch(url + '/api' + path, {
        body: JSON.stringify(data),
        headers: combinedHeaders,
        method,
    }).then(res => res.json())
}

const callApiWithAuth = (method: string, url: string, path: string, data?: any) => {
    const authToken = getAuthToken()
    const authHeader = authToken ? { Authorization: `Bearer ${authToken}` } : {}
    return callApi(method, url, path, data, authHeader)
}
    
export { 
    callApi,
    callApiWithAuth,
    clearAuthToken,
    getAuthToken,
    setAuthToken 
}
