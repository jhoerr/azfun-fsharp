import { IApiState } from '../common'

// Use `const enum`s for better autocompletion of action type names. These will
// be compiled away leaving only the final value in your compiled code.
//
// Define however naming conventions you'd like for your action types, but
// personally, I use the `@@context/ACTION_TYPE` convention, to follow the convention
// of Redux's `@@INIT` action.
export const enum ProfileActionTypes {
    PROFILE_FETCH = '@@profile/PROFILE_FETCH',
    PROFILE_FETCH_SUCCESS = '@@profile/PROFILE_FETCH_SUCCESS',
    PROFILE_FETCH_ERROR = '@@profile/PROFILE_FETCH_ERROR',
}

export interface IProfile {
    username: string,
    displayName: string,
    department: string,
    expertise: string
}

export interface IProfileState extends IApiState<IProfile> { 
}
