
import { action } from 'typesafe-actions'
import { AuthActionTypes, IAuthRequest, IAuthUser } from './types'

const signInRequest = (request: IAuthRequest) => action(AuthActionTypes.SIGN_IN_REQUEST, request)
const signInSuccess = (data: IAuthUser) => action(AuthActionTypes.SIGN_IN_SUCCESS, data)
const signInError = (message: string) => action(AuthActionTypes.SIGN_IN_ERROR, message)
const signOutRequest = () => action(AuthActionTypes.SIGN_OUT)

export {
    signInRequest,
    signInSuccess,
    signInError,
    signOutRequest
}
