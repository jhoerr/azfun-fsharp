import * as JWT from 'jwt-decode'
import { push } from 'react-router-redux';
import { all, call, fork, put, select, takeEvery } from 'redux-saga/effects'
import { callApi, clearAuthToken, setAuthToken } from '../effects'
import { IApplicationState } from '../index';
import { signInError, signInSuccess } from './actions'
import { AuthActionTypes, IAuthRequest, IAuthUser } from './types'

const API_ENDPOINT = process.env.REACT_APP_API_URL || ''

function* handleSignIn() {
  try {
    const request = (yield select<IApplicationState>((s) => s.auth.request)) as IAuthRequest
    const response = yield call(callApi, 'get', API_ENDPOINT, `/auth?code=${request.code}`)

    if (response.errors) {
      yield call(clearAuthToken)
      yield put(signInError(response.errors))
    } else {
      yield call(setAuthToken, response.access_token)
      const decoded = JWT<IAuthUser>(response.access_token)
      yield put(signInSuccess(decoded))
      yield put(push(`/profile/${decoded.user_name}`))
    }
  } catch (err) {
    yield call(clearAuthToken)
    if (err instanceof Error) {
      yield put(signInError(err.stack!))
    } else {
      yield put(signInError('An unknown error occured.'))
    }
  }
}

function* handleSignOut() {
  yield call(clearAuthToken)
  yield put(push('/'))
}

// This is our watcher function. We use `take*()` functions to watch Redux for a specific action
// type, and run our saga, for example the `handleFetch()` saga above.
function* watchSignIn() {
  yield takeEvery(AuthActionTypes.SIGN_IN_REQUEST, handleSignIn)
}

function* watchSignOut() {
  yield takeEvery(AuthActionTypes.SIGN_OUT, handleSignOut)
}

// We can also use `fork()` here to split our saga into multiple watchers.
function* authSaga() {
  yield all([fork(watchSignIn), fork(watchSignOut)])
}

export default authSaga
