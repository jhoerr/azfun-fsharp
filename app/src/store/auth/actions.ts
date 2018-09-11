
import { action } from 'typesafe-actions'
import { AuthActionTypes } from './types'

// Here we use the `action` helper function provided by `typesafe-actions`.
// This library provides really useful helpers for writing Redux actions in a type-safe manner.
// For more info: https://github.com/piotrwitek/typesafe-actions
const signIn = (code: string) => action(AuthActionTypes.SIGN_IN, code)

// Remember, you can also pass parameters into an action creator. Make sure to
// type them properly as well.
const signInSuccess = (token: string) => action(AuthActionTypes.SIGN_IN_SUCCESS, token)
const signInError = (message: string) => action(AuthActionTypes.SIGN_IN_ERROR, message)
const signOutRequest = () => action(AuthActionTypes.SIGN_OUT)

export {
    signIn,
    signInSuccess,
    signInError,
    signOutRequest
}
