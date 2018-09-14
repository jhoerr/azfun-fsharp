
import { action } from 'typesafe-actions'
import { IProfile, ProfileActionTypes } from './types'

const profileFetchRequest = () => action(ProfileActionTypes.PROFILE_FETCH)
const profileFetchSuccess = (profile: IProfile) => action(ProfileActionTypes.PROFILE_FETCH_SUCCESS, profile)
const profileFetchError = (error: string) => action(ProfileActionTypes.PROFILE_FETCH_ERROR, error)

export {
    profileFetchRequest,
    profileFetchError,
    profileFetchSuccess
}
