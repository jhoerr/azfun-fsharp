const clearAuthToken = () =>
    sessionStorage.removeItem('authToken')

const getAuthToken = () => 
    sessionStorage.getItem("authToken")

const setAuthToken = (token: string) => 
    sessionStorage.setItem('authToken', token)

const callApi = (method: string, url: string, path: string, auth?: string, data?: any) => {
    return fetch(url + '/api' + path, {
        body: JSON.stringify(data),
        headers: {
            Accept: 'application/json',
            Authorization: `Bearer ${auth}`,
            'Content-Type': 'application/json',
        },
        method,
    }).then(res => res.json())
    }

export { 
    callApi,
    clearAuthToken,
    getAuthToken,
    setAuthToken 
}
