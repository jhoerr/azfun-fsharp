
import { action } from 'typesafe-actions'
import { AuthActionTypes, IAuthRequest, IAuthUser } from './types'

const signInRequest = () => action(AuthActionTypes.SIGN_IN_REQUEST)
const postSignInRequest = (request: IAuthRequest) => action(AuthActionTypes.POST_SIGN_IN_REQUEST, request)
const postSignInSuccess = (data: IAuthUser) => action(AuthActionTypes.POST_SIGN_IN_SUCCESS, data)
const postSignInError = (message: string) => action(AuthActionTypes.POST_SIGN_IN_ERROR, message)
const signOutRequest = () => action(AuthActionTypes.SIGN_OUT)

export {
    postSignInRequest,
    postSignInSuccess,
    postSignInError,
    signInRequest,
    signOutRequest
}
