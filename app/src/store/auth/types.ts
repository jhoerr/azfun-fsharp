import { IApiState } from '../common'

// Use `const enum`s for better autocompletion of action type names. These will
// be compiled away leaving only the final value in your compiled code.
//
// Define however naming conventions you'd like for your action types, but
// personally, I use the `@@context/ACTION_TYPE` convention, to follow the convention
// of Redux's `@@INIT` action.
export const enum AuthActionTypes {
    SIGN_IN = '@@auth/SIGN_IN',
    SIGN_IN_SUCCESS = '@@auth/SIGN_IN_SUCCESS',
    SIGN_IN_ERROR = '@@auth/SIGN_IN_ERROR',
    SIGN_OUT = '@@auth/SIGN_OUT',
}

export interface IAuthUser {
    user_name: string,
    user_role: string
}

// The name of the authorized user
export interface IAuthState extends IApiState<IAuthUser> { 
    readonly code?: string
    readonly user?: IAuthUser
}
