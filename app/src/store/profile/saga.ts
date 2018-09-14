import { all, /* call, */ fork, put, takeEvery } from 'redux-saga/effects'
// import { callApi } from '../effects'
import { profileFetchError, profileFetchSuccess } from './actions'
import { IProfile, ProfileActionTypes } from './types'

// const API_ENDPOINT = process.env.REACT_APP_API_URL || ''

const stubProfile : IProfile = {
    department: "UITS",
    displayName: "John Hoerr",
    expertise: "Web dev, classic cocktails, underwater basket weaving",
    username: "jhoerr",    
}

function* handleFetch() {
  try {
    console.log("Handling profile fetch...")
    /*
    const res = yield call(callApi, 'get', API_ENDPOINT, `/profile/me`)
    if (res.errors) {
      yield put(profileFetchError(res.errors))
    } else {
      yield put(profileFetchSuccess(res.data))
    }
    */
    yield put(profileFetchSuccess(stubProfile))
  } catch (err) {
    if (err instanceof Error) {
      yield put(profileFetchError(err.stack!))
    } else {
      yield put(profileFetchError('An unknown error occured.'))
    }
  }
}

// This is our watcher function. We use `take*()` functions to watch Redux for a specific action
// type, and run our saga, for example the `handleFetch()` saga above.
function* watchProfileFetch() {
  yield takeEvery(ProfileActionTypes.PROFILE_FETCH, handleFetch)
}

// We can also use `fork()` here to split our saga into multiple watchers.
function* profileSaga() {
  yield all([fork(watchProfileFetch)])
}

export default profileSaga
