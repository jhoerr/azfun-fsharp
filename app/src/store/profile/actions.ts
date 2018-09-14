
import { action } from 'typesafe-actions'
import { IProfile, IProfileRequest, ProfileActionTypes,  } from './types'

const profileFetchRequest = (request: IProfileRequest) => action(ProfileActionTypes.PROFILE_FETCH_REQUEST, request)
const profileFetchSuccess = (data: IProfile) => action(ProfileActionTypes.PROFILE_FETCH_SUCCESS, data)
const profileFetchError = (error: string) => action(ProfileActionTypes.PROFILE_FETCH_ERROR, error)

export {
    profileFetchRequest,
    profileFetchError,
    profileFetchSuccess
}
