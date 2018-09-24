import { IApiState } from '../common'

// Use `const enum`s for better autocompletion of action type names. These will
// be compiled away leaving only the final value in your compiled code.
//
// Define however naming conventions you'd like for your action types, but
// personally, I use the `@@context/ACTION_TYPE` convention, to follow the convention
// of Redux's `@@INIT` action.
export const enum ProfileActionTypes {
    PROFILE_FETCH_REQUEST = '@@profile/PROFILE_FETCH_REQUEST',
    PROFILE_FETCH_SUCCESS = '@@profile/PROFILE_FETCH_SUCCESS',
    PROFILE_FETCH_ERROR = '@@profile/PROFILE_FETCH_ERROR',
    PROFILE_UPDATE_REQUEST = '@@profile/PROFILE_UPDATE_REQUEST',
    PROFILE_UPDATE_SUCCESS = '@@profile/PROFILE_UPDATE_SUCCESS',
    PROFILE_UPDATE_ERROR = '@@profile/PROFILE_UPDATE_ERROR',
}

export interface IProfileRequest {
    id: number,
}

export interface IProfileUpdateRequest {
    expertise: string
}

export interface IProfile extends IProfileUpdateRequest {
    id: number,
    username: string,
    preferredName: string,
    department: string,
}

export interface IProfileState extends IApiState<IProfileRequest, IProfile> { 
}
