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
    callApi
}
