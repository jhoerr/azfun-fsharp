import { all, call, fork, put, select, takeEvery } from 'redux-saga/effects'
import { NotAuthorizedError } from '../../components/errors';
import { AuthActionTypes  } from '../auth/types'
import { callApiWithAuth, clearAuthToken } from '../effects'
import { IApplicationState } from '../index';
import { profileFetchError, profileFetchSuccess } from './actions'
import { IProfile, IProfileRequest, ProfileActionTypes,  } from './types'


const API_ENDPOINT = process.env.REACT_APP_API_URL || ''

const stubProfile : IProfile = {
    department: "UITS",
    displayName: "John Hoerr",
    expertise: "Web dev, classic cocktails, underwater basket weaving",
    username: "jhoerr",    
}

function* handleFetch() {
  try {
    const request = (yield select<IApplicationState>((s) => s.profile.request)) as IProfileRequest
    const response = yield call(callApiWithAuth, 'get', API_ENDPOINT, `/profile/${request.username}`)
    console.log ("in try block", response)
    if (response.errors) {
      yield put(profileFetchError(response.errors))
    } else {
      yield put(profileFetchSuccess(stubProfile))
    }
  } catch (err) {
    console.log ("in catch block", err)
    if (err instanceof NotAuthorizedError){
      yield call(clearAuthToken)
      yield put({ type:AuthActionTypes.SIGN_OUT})
    }
    else if (err instanceof Error) {
      yield put(profileFetchError(err.stack!))
    } else {
      yield put(profileFetchError('An unknown error occured.'))
    }
  }
}

// This is our watcher function. We use `take*()` functions to watch Redux for a specific action
// type, and run our saga, for example the `handleFetch()` saga above.
function* watchProfileFetch() {
  yield takeEvery(ProfileActionTypes.PROFILE_FETCH_REQUEST, handleFetch)
}

// We can also use `fork()` here to split our saga into multiple watchers.
function* profileSaga() {
  yield all([fork(watchProfileFetch)])
}

export default profileSaga
