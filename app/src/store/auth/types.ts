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

// Declare state types with `readonly` modifier to get compile time immutability.
// https://github.com/piotrwitek/react-redux-typescript-guide#state-with-type-level-immutability
interface IApiState<T> {
    readonly loading: boolean
    readonly data: T
    readonly errors?: string
}

// The User's UAA JWT 
export interface IAuthState extends IApiState<string> { }
