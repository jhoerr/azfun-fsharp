import { IApiState } from '../common'

// Use `const enum`s for better autocompletion of action type names. These will
// be compiled away leaving only the final value in your compiled code.
//
// Define however naming conventions you'd like for your action types, but
// personally, I use the `@@context/ACTION_TYPE` convention, to follow the convention
// of Redux's `@@INIT` action.
export const enum AuthActionTypes {
    SIGN_IN_REQUEST = '@@auth/SIGN_IN',
    POST_SIGN_IN_REQUEST = '@@auth/POST_SIGN_IN',
    POST_SIGN_IN_SUCCESS = '@@auth/POST_SIGN_IN_SUCCESS',
    POST_SIGN_IN_ERROR = '@@auth/POST_SIGN_IN_ERROR',
    SIGN_OUT = '@@auth/SIGN_OUT',
}

export interface IAuthRequest {
    code: string
}

export interface IAuthUser {
    user_name: string,
    user_role: string
}

// The name of the authorized user
export interface IAuthState extends IApiState<IAuthRequest, IAuthUser> { }
