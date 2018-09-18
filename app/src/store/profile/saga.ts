import { all, call, fork, put, takeEvery } from 'redux-saga/effects'
import { NotAuthorizedError } from '../../components/errors';
import { signInRequest  } from '../auth/actions'
import { callApiWithAuth } from '../effects'
import { profileFetchError, profileFetchSuccess } from './actions'
import { ProfileActionTypes,  } from './types'


const API_ENDPOINT = process.env.REACT_APP_API_URL || ''

function* handleFetch() {
  try {
    const response = yield call(callApiWithAuth, 'get', API_ENDPOINT, `/profile`)
    console.log ("in try block", response)
    if (response.errors) {
      yield put(profileFetchError(response.errors))
    } else {
      yield put(profileFetchSuccess(response))
    }
  } catch (err) {
    console.log ("in catch block", err)
    if (err instanceof NotAuthorizedError){
      yield put(signInRequest())
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
